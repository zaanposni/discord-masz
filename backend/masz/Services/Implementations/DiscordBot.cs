using DSharpPlus.SlashCommands;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using masz.Models;
using Microsoft.Extensions.Logging;
using DSharpPlus.SlashCommands.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using System;
using masz.Commands;
using masz.Exceptions;
using System.Net.WebSockets;
using masz.InviteTracking;
using System.Collections.Generic;
using System.Linq;
using masz.Repositories;
using masz.Logger;
using masz.AutoModerations;
using System.Text;

namespace masz.Services
{
    public class DiscordBot : IDiscordBot
    {
        private readonly ILogger<DiscordBot> _logger;
        private readonly IInternalConfiguration _config;
        private readonly DiscordClient _client;
        private DiscordConfiguration _discordConfiguration;
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private bool _isRunning = false;
        private DateTime? _lastDisconnect = null;

        public DiscordBot(ILogger<DiscordBot> logger, IInternalConfiguration config, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _config = config;
            _serviceProvider = serviceProvider;
            _serviceScopeFactory = serviceScopeFactory;

            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new CustomLoggerProvider());
            _discordConfiguration = new DiscordConfiguration()
            {
                Token = _config.GetBotToken(),
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged | DiscordIntents.GuildMembers,
                LoggerFactory = loggerFactory
            };

            _client = new DiscordClient(_discordConfiguration);
            _client.MessageCreated += this.MessageCreatedHandler;
            _client.MessageUpdated += this.MessageUpdatedHandler;
            _client.GuildCreated += this.GuildCreatedHandler;
            _client.GuildMemberAdded += this.GuildMemberAddedHandler;
            _client.InviteCreated += this.InviteCreatedHandler;
            _client.InviteDeleted += this.InviteDeletedHandler;
            _client.GuildUpdated += this.GuildUpdatedHandler;
            _client.GuildAvailable += this.GuildAvailableHandler;

            _client.SocketErrored += this.SocketErroredHandler;
            _client.Resumed += this.ResumedHandler;
            _client.Ready += this.ReadyHandler;

            var slash = _client.UseSlashCommands(new SlashCommandsConfiguration
            {
                Services = serviceProvider
            });

            ulong? debugGuild = null;  // set your guild id here to enable fast syncing debug commands

            slash.RegisterCommands<ReportCommand>(debugGuild);
            slash.RegisterCommands<PunishmentCommand>(debugGuild);
            slash.RegisterCommands<GitHubCommand>(debugGuild);
            slash.RegisterCommands<InviteCommand>(debugGuild);
            slash.RegisterCommands<RegisterCommand>(debugGuild);
            slash.RegisterCommands<UrlCommand>(debugGuild);
            slash.RegisterCommands<WhoisCommand>(debugGuild);
            slash.RegisterCommands<ViewCommand>(debugGuild);
            slash.RegisterCommands<TrackCommand>(debugGuild);
            slash.RegisterCommands<SayCommand>(debugGuild);
            slash.RegisterCommands<FeatureCommand>(debugGuild);
            slash.RegisterCommands<CleanupCommand>(debugGuild);

            slash.SlashCommandErrored += CmdErroredHandler;
        }

        public async Task Start()
        {
            DiscordActivity activity = new DiscordActivity(_config.GetBaseUrl(), ActivityType.Watching);
            await _client.ConnectAsync(activity);
        }

        public bool IsRunning()
        {
            return _isRunning;
        }

        public DateTime? GetLastDisconnectTime()
        {
            return _lastDisconnect;
        }

        private Task ResumedHandler(DiscordClient sender, ReadyEventArgs e)
        {
            _logger.LogWarning("Client reconnected.");
            _isRunning = true;
            return Task.CompletedTask;
        }

        private Task SocketErroredHandler(DiscordClient sender, SocketErrorEventArgs e)
        {
            if (e.Exception is WebSocketException)
            {
                _logger.LogCritical("Client disconnected.");
                _isRunning = false;
                _lastDisconnect = DateTime.UtcNow;
            }
            return Task.CompletedTask;
        }

        private Task ReadyHandler(DiscordClient sender, ReadyEventArgs e)
        {
            _logger.LogInformation("Client connected.");
            _isRunning = true;

            return Task.CompletedTask;
        }

        private Task MessageCreatedHandler(DiscordClient client, MessageCreateEventArgs e)
        {
            if (e.Message.MessageType != MessageType.Default && e.Message.MessageType != MessageType.Reply)
            {
                return Task.CompletedTask;
            }
            if (e.Message.Author.IsBot)
            {
                return Task.CompletedTask;
            }
            if (e.Message.Channel.Guild == null)
            {
                return Task.CompletedTask;
            }

            Task.Run(async () => {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    AutoModerator autoModerator = null;
                    try
                    {
                        autoModerator = await AutoModerator.CreateDefault(client, e.Guild.Id, scope.ServiceProvider);
                    } catch (ResourceNotFoundException)
                    {
                        return;
                    }
                    await autoModerator.HandleAutomoderation(e.Message);
                }
            });
            return Task.CompletedTask;
        }

        private Task MessageUpdatedHandler(DiscordClient client, MessageUpdateEventArgs e)
        {
            if (e.Message.MessageType != MessageType.Default && e.Message.MessageType != MessageType.Reply)
            {
                return Task.CompletedTask;
            }
            if (e.Message.Author.IsBot)
            {
                return Task.CompletedTask;
            }
            if (e.Message.Channel.Guild == null)
            {
                return Task.CompletedTask;
            }

            Task.Run(async () => {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    AutoModerator autoModerator = null;
                    try
                    {
                        autoModerator = await AutoModerator.CreateDefault(client, e.Guild.Id, scope.ServiceProvider);
                    } catch (ResourceNotFoundException)
                    {
                        return;
                    }
                    await autoModerator.HandleAutomoderation(e.Message, true);
                }
            });
            return Task.CompletedTask;
        }

        private async Task<List<TrackedInvite>> FetchInvites(DiscordGuild guild)
        {
            List<TrackedInvite> invites = new List<TrackedInvite>();
            try
            {
                IReadOnlyList<DiscordInvite> i = await guild.GetInvitesAsync();
                invites.AddRange(i.Select(x => new TrackedInvite(x, guild.Id)));
            } catch (DSharpPlus.Exceptions.UnauthorizedException) { }
              catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get invites from guild {guild.Id}.");
            }
            try
            {
                DiscordInvite vanityInvite = await guild.GetVanityInviteAsync();
                invites.Add(new TrackedInvite(guild.Id, vanityInvite.Code, vanityInvite.Uses));
            } catch (DSharpPlus.Exceptions.UnauthorizedException) { }
            return invites;
        }

        private async Task GuildAvailableHandler(DiscordClient sender, GuildCreateEventArgs e)
        {
            InviteTracker.AddInvites(e.Guild.Id, await FetchInvites(e.Guild));
        }

        private Task GuildCreatedHandler(DiscordClient client, GuildCreateEventArgs e)
        {
            _logger.LogInformation($"I joined guild '{e.Guild.Name}' with ID: '{e.Guild.Id}'");
            return Task.CompletedTask;
        }

        private async Task GuildMemberAddedHandler(DiscordClient client, GuildMemberAddEventArgs e)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                try
                {
                    IPunishmentHandler handler = scope.ServiceProvider.GetService<IPunishmentHandler>();
                    await handler.HandleMemberJoin(client, e);
                } catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to handle punishment on member join.");
                }

                GuildConfig guildConfig;
                try
                {
                    guildConfig = await GuildConfigRepository.CreateDefault(scope.ServiceProvider).GetGuildConfig(e.Guild.Id);
                } catch (ResourceNotFoundException)
                {
                    return;
                }

                List<TrackedInvite> newInvites = await FetchInvites(e.Guild);
                TrackedInvite usedInvite = null;
                try
                {
                    usedInvite = InviteTracker.GetUsedInvite(e.Guild.Id, newInvites);
                } catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to get used invite.");
                }
                InviteTracker.AddInvites(e.Guild.Id, newInvites);

                if (usedInvite != null)
                {
                    UserInvite invite = new UserInvite();
                    invite.GuildId = e.Guild.Id;
                    invite.JoinedUserId = e.Member.Id;
                    invite.JoinedAt = DateTime.UtcNow;
                    invite.InviteIssuerId = usedInvite.CreatorId;
                    invite.InviteCreatedAt = usedInvite.CreatedAt;
                    invite.TargetChannelId = usedInvite.TargetChannelId;
                    invite.UsedInvite = $"https://discord.gg/{usedInvite.Code}";

                    _logger.LogInformation($"User {e.Member.Username}#{e.Member.Discriminator} joined guild {e.Guild.Name} with ID: {e.Guild.Id} using invite {usedInvite.Code}");

                    if (guildConfig.ExecuteWhoisOnJoin && ! String.IsNullOrEmpty(guildConfig.ModInternalNotificationWebhook))
                    {
                        string message;
                        string registeredTime = e.Member.CreationTimestamp.ToString("yyyy-MM-dd HH:mm:ss");
                        if (invite.InviteIssuerId != 0 && invite.InviteCreatedAt != null)
                        {
                            string createdTime = invite.InviteCreatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss");
                            message = $"{e.Member.Mention} (registered `{registeredTime}`) joined with invite <{invite.UsedInvite}> (created `{createdTime}`) by <@{invite.InviteIssuerId}>.";
                        } else
                        {
                            message = $"{e.Member.Mention} (registered `{registeredTime}`) joined with invite <{invite.UsedInvite}>.";
                        }

                        IDiscordAPIInterface discordAPI = scope.ServiceProvider.GetService<IDiscordAPIInterface>();
                        await discordAPI.ExecuteWebhook(guildConfig.ModInternalNotificationWebhook, null, message);
                    }

                    await InviteRepository.CreateDefault(scope.ServiceProvider).CreateInvite(invite);
                }
            }
        }

        private async Task GuildUpdatedHandler(DiscordClient sender, GuildUpdateEventArgs e)
        {
            DiscordInvite invite = null;
            try
            {
                invite = await e.GuildAfter.GetVanityInviteAsync();
            } catch (DSharpPlus.Exceptions.UnauthorizedException) { }
            if (invite != null)
            {
                InviteTracker.AddInvite(invite.Guild.Id, new TrackedInvite(invite.Guild.Id, invite.Code, invite.Uses));
            }
        }

        private Task InviteCreatedHandler(DiscordClient sender, InviteCreateEventArgs e)
        {
            InviteTracker.AddInvite(e.Guild.Id, new TrackedInvite(e.Invite, e.Guild.Id));
            return Task.CompletedTask;
        }

        private Task InviteDeletedHandler(DiscordClient sender, InviteDeleteEventArgs e)
        {
            InviteTracker.RemoveInvite(e.Guild.Id, e.Invite.Code);
            return Task.CompletedTask;
        }

        private async Task CmdErroredHandler(SlashCommandsExtension _, SlashCommandErrorEventArgs e)
        {
            if (e.Exception is BaseAPIException)
            {
                _logger.LogError($"Command '{e.Context.CommandName}' invoked by '{e.Context.User.Username}#{e.Context.User.Discriminator}' failed: {(e.Exception as BaseAPIException).error}");

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    ITranslator translator = scope.ServiceProvider.GetService<ITranslator>();
                    if (e.Context.Guild != null)
                    {
                        await translator.SetContext(e.Context.Guild.Id);
                    }

                    string errorCode = "0#" + ((int) ((e.Exception as BaseAPIException).error)).ToString("D7");
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(translator.T().SomethingWentWrong());
                    sb.AppendLine($"`{translator.T().Enum((e.Exception as BaseAPIException).error)}`");
                    sb.Append($"**{translator.T().Code()}** ");
                    sb.Append($"`{errorCode}`");
                    try
                    {
                        await e.Context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent(sb.ToString()));
                    } catch (DSharpPlus.Exceptions.NotFoundException)
                    {
                        await e.Context.EditResponseAsync(new DiscordWebhookBuilder().WithContent(sb.ToString()));
                    }
                }
            } else
            {
                _logger.LogError($"Command '{e.Context.CommandName}' invoked by '{e.Context.User.Username}#{e.Context.User.Discriminator}' failed: " + e.Exception.Message + "\n" + e.Exception.StackTrace);
            }
        }

        public DiscordClient GetClient()
        {
            return _client;
        }
    }
}