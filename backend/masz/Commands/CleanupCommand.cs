using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using masz.Enums;
using masz.Models;
using masz.Repositories;
using Microsoft.Extensions.Logging;

namespace masz.Commands
{

    public class CleanupCommand : BaseCommand<CleanupCommand>
    {
        private bool HasAttachment(DiscordMessage m)
        {
            return m.Attachments.Count > 0;
        }
        private bool IsFromBot(DiscordMessage m)
        {
            return m.Author.IsBot;
        }
        private async Task<int> IterateAndDeleteChannels(DiscordChannel channel, int limit, Func<DiscordMessage, bool> predicate, DiscordUser currentActor, DiscordUser fitlerUser = null)
        {
            ulong lastId = 0;
            int deleted = 0;
            List<DiscordMessage> toDelete = new List<DiscordMessage>();
            foreach (DiscordMessage message in await channel.GetMessagesAsync(Math.Min(limit, 100)))
            {
                lastId = message.Id;
                limit--;
                if (fitlerUser != null && message.Author.Id != fitlerUser.Id)
                {
                    continue;
                }
                if (predicate(message))
                {
                    deleted++;
                    if (message.CreationTimestamp.UtcDateTime.AddDays(14) > DateTime.UtcNow)
                    {
                        toDelete.Add(message);
                    } else
                    {
                        await message.DeleteAsync();
                    }
                }
            }
            while (limit > 0)
            {
                if (toDelete.Count >= 2)
                {
                    await channel.DeleteMessagesAsync(toDelete, $"Bulkdelete by {currentActor.Username}#{currentActor.Discriminator} ({currentActor.Id}).");
                    toDelete.Clear();
                } else if (toDelete.Count > 0)
                {
                    await toDelete[0].DeleteAsync();
                    toDelete.Clear();
                }
                foreach (DiscordMessage message in await channel.GetMessagesAfterAsync(lastId, Math.Min(limit, 100)))
                {
                    lastId = message.Id;
                    limit--;
                    if (fitlerUser != null && message.Author.Id != fitlerUser.Id)
                    {
                        continue;
                    }
                    if (predicate(message))
                    {
                        deleted++;
                        if (message.CreationTimestamp.UtcDateTime.AddDays(14) > DateTime.UtcNow)
                        {
                            toDelete.Add(message);
                        } else
                        {
                            await message.DeleteAsync();
                        }
                    }
                }
            }
            if (toDelete.Count >= 2)
            {
                await channel.DeleteMessagesAsync(toDelete, $"Bulkdelete by {currentActor.Username}#{currentActor.Discriminator} ({currentActor.Id}).");
                toDelete.Clear();
            } else if (toDelete.Count > 0)
            {
                await toDelete[0].DeleteAsync();
                toDelete.Clear();
            }
            return deleted;
        }
        public CleanupCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }

        [SlashCommand("cleanup", "Cleanup specific data from the server and/or channel.")]
        public async Task Cleanup(InteractionContext ctx,
                                    [Option("mode", "which data you want to delete")]CleanupMode cleanupMode,
                                    [Option("channel", "where to delete, defaults to current.")] DiscordChannel channel = null,
                                    [Option("count", "how many messages to scan for your mode.")] long count = 100,
                                    [Option("user", "additional filter on this user")] DiscordUser filterUser = null)
        {
            await Require(ctx, RequireCheckEnum.GuildModerator);

            if (channel == null)
            {
                channel = ctx.Channel;
            }
            if (channel.Type != ChannelType.Text)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent("I can only send messages in text channels."));
                return;
            }
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral(true));

            if (count > 1000)
            {
                count = 1000;
            }

            var func = new Func<DiscordMessage, bool>(m => true);
            switch (cleanupMode) {
                case CleanupMode.Bots:
                    func = IsFromBot;
                    break;
                case CleanupMode.Attachments:
                    func = HasAttachment;
                    break;
            }

            int deleted = 0;
            try
            {
                deleted = await IterateAndDeleteChannels(channel, (int)count, func, ctx.User, filterUser);
            } catch (DSharpPlus.Exceptions.UnauthorizedException)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("I'm not allowed to view or delete messages in this channel!"));
                return;
            } catch (DSharpPlus.Exceptions.NotFoundException)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Failed to find channel."));
                return;
            }
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Deleted {deleted} messages in {channel.Mention}."));
        }
    }
}