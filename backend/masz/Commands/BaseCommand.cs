using Discord;
using Discord.Interactions;
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Models;
using MASZ.Repositories;
using MASZ.Services;

namespace MASZ.Commands
{

    public class BaseCommand<T> : InteractionModuleBase<SocketInteractionContext>
    {

        protected ILogger<T> Logger { get; set; }
        protected IDatabase Database { get; set; }
        protected ITranslator Translator { get; set; }
        protected IIdentityManager IdentityManager { get; set; }
        protected Identity CurrentIdentity { get; set; }
        protected IInternalConfiguration Config { get; set; }
        protected IDiscordAPIInterface DiscordAPI { get; set; }
        protected IServiceProvider ServiceProvider { get; set; }

        public BaseCommand(IServiceProvider serviceProvider)
        {
            Logger = (ILogger<T>)serviceProvider.GetService(typeof(ILogger<T>));
            Database = (IDatabase)serviceProvider.GetService(typeof(IDatabase));
            Translator = (ITranslator)serviceProvider.GetService(typeof(ITranslator));
            IdentityManager = (IIdentityManager)serviceProvider.GetService(typeof(IIdentityManager));
            Config = (IInternalConfiguration)serviceProvider.GetService(typeof(IInternalConfiguration));
            DiscordAPI = (IDiscordAPIInterface)serviceProvider.GetService(typeof(IDiscordAPIInterface));
            ServiceProvider = serviceProvider;
        }

        public override async void BeforeExecute(ICommandInfo command)
        {
            if (Context.Channel is ITextChannel)
            {
                Logger.LogInformation($"{Context.User.Id} used {command.Name} in {Context.Channel.Id} | {Context.Guild.Id} {Context.Guild.Name}");
            }
            else
            {
                Logger.LogInformation($"{Context.User.Id} used {command.Name} in DM");
            }

            CurrentIdentity = await IdentityManager.GetIdentity(Context.User);

            if (CurrentIdentity == null)
            {
                Logger.LogError($"Failed to register command identity for '{Context.User.Id}'.");
                return;
            }

            if (Context.Guild != null)
            {
                await Translator.SetContext(Context.Guild.Id);
            }
        }

        protected async Task Require(params RequireCheckEnum[] checks)
        {
            foreach (RequireCheckEnum check in checks)
            {
                switch (check)
                {
                    case RequireCheckEnum.GuildRegistered:
                        await RequireRegisteredGuild();
                        continue;
                    case RequireCheckEnum.GuildMember:
                        await RequireDiscordPermission(DiscordPermission.Member);
                        continue;
                    case RequireCheckEnum.GuildModerator:
                        await RequireDiscordPermission(DiscordPermission.Moderator);
                        continue;
                    case RequireCheckEnum.GuildAdmin:
                        await RequireDiscordPermission(DiscordPermission.Admin);
                        continue;
                    case RequireCheckEnum.GuildMuteRole:
                        await RequireGuildWithMutedRole();
                        continue;
                    case RequireCheckEnum.SiteAdmin:
                        await RequireSiteAdmin();
                        continue;
                    case RequireCheckEnum.GuildStrictModeMute:
                        await RequireStrictModeAccess(PunishmentType.Mute);
                        continue;
                    case RequireCheckEnum.GuildStrictModeKick:
                        await RequireStrictModeAccess(PunishmentType.Kick);
                        continue;
                    case RequireCheckEnum.GuildStrictModeBan:
                        await RequireStrictModeAccess(PunishmentType.Ban);
                        continue;
                }
            }
        }

        private Task RequireSiteAdmin()
        {
            if (!CurrentIdentity.IsSiteAdmin())
            {
                throw new UnauthorizedException("Only site admins allowed.");
            }
            return Task.CompletedTask;
        }

        private async Task RequireRegisteredGuild()
        {
            try
            {
                await GuildConfigRepository.CreateDefault(ServiceProvider).GetGuildConfig(Context.Guild.Id);
            }
            catch (ResourceNotFoundException)
            {
                throw new UnregisteredGuildException(Context.Guild.Id);
            }
            catch (NullReferenceException)
            {
                throw new BaseAPIException("Only usable in a guild.", APIError.OnlyUsableInAGuild);
            }
        }

        private async Task RequireGuildWithMutedRole()
        {
            try
            {
                GuildConfig guildConfig = await GuildConfigRepository.CreateDefault(ServiceProvider).GetGuildConfig(Context.Guild.Id);
                if (guildConfig.MutedRoles.Length == 0)
                {
                    throw new GuildWithoutMutedRoleException(Context.Guild.Id);
                }
            }
            catch (ResourceNotFoundException)
            {
                throw new UnregisteredGuildException(Context.Guild.Id);
            }
        }

        private async Task RequireDiscordPermission(DiscordPermission permission)
        {
            await RequireRegisteredGuild();
            if (CurrentIdentity.IsSiteAdmin())
            {
                return;
            }
            switch (permission)
            {
                case DiscordPermission.Member:
                    if (CurrentIdentity.IsOnGuild(Context.Guild.Id)) return;
                    break;
                case DiscordPermission.Moderator:
                    if (await CurrentIdentity.HasModRoleOrHigherOnGuild(Context.Guild.Id)) return;
                    break;
                case DiscordPermission.Admin:
                    if (await CurrentIdentity.HasAdminRoleOnGuild(Context.Guild.Id)) return;
                    break;
            }
            throw new UnauthorizedException("You are not allowed to do that.");
        }

        private async Task RequireStrictModeAccess(PunishmentType punishmentType)
        {
            await RequireRegisteredGuild();
            if (CurrentIdentity.IsSiteAdmin())
            {
                return;
            }
            if (await CurrentIdentity.HasPermissionToExecutePunishment(Context.Guild.Id, punishmentType)) return;
            throw new UnauthorizedException("You are not allowed to do that.");
        }
    }
}