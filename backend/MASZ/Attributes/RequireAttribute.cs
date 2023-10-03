using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Models;
using MASZ.Repositories;
using MASZ.Services;

namespace MASZ.Attributes
{
    public class RequireAttribute : PreconditionAttribute
    {
        private readonly RequireCheckEnum[] _checks;

        public RequireAttribute(params RequireCheckEnum[] checks)
        {
            _checks = checks;
        }

        public async Task<PreconditionResult> CheckRequirementsAsync(SocketModal modal, IServiceProvider services)
          => await InternalCheckRequirementsAsync(modal.User, modal.GuildId, null, services);

        public override async Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
          => await InternalCheckRequirementsAsync(context.User, context.Guild.Id, commandInfo, services);

        private async Task<PreconditionResult> InternalCheckRequirementsAsync(IUser user, ulong? guildId, ICommandInfo commandInfo, IServiceProvider services)
        {
            var identity = await services.GetRequiredService<IdentityManager>().GetIdentity(user);

            foreach (RequireCheckEnum check in _checks)
            {
                if (check != RequireCheckEnum.SiteAdmin && !guildId.HasValue)
                {
                    throw new BaseAPIException("Only usable in a guild.", APIError.OnlyUsableInAGuild);
                }

                switch (check)
                {
                    case RequireCheckEnum.SiteAdmin:
                        if (!identity.IsSiteAdmin())
                        {
                            throw new UnauthorizedException("Only site admins allowed.");
                        }
                        continue;
                    case RequireCheckEnum.GuildRegistered:
                        await RequireRegisteredGuild(services, guildId.Value);
                        continue;
                    case RequireCheckEnum.GuildMember:
                        await RequireDiscordPermission(DiscordPermission.Member, services, guildId.Value, identity);
                        continue;
                    case RequireCheckEnum.GuildModerator:
                        await RequireDiscordPermission(DiscordPermission.Moderator, services, guildId.Value, identity);
                        continue;
                    case RequireCheckEnum.GuildAdmin:
                        await RequireDiscordPermission(DiscordPermission.Admin, services, guildId.Value, identity);
                        continue;
                    case RequireCheckEnum.GuildMuteRole:
                        try
                        {
                            GuildConfig guildConfig = await GuildConfigRepository.CreateDefault(services).GetGuildConfig(guildId.Value);
                            if (guildConfig.MutedRoles.Length == 0)
                            {
                                throw new GuildWithoutMutedRoleException(guildId.Value);
                            }
                        }
                        catch (ResourceNotFoundException)
                        {
                            throw new UnregisteredGuildException(guildId.Value);
                        }
                        continue;
                    case RequireCheckEnum.GuildStrictModeMute:
                        await RequireStrictModeAccess(PunishmentType.Mute, services, guildId.Value, identity);
                        continue;
                    case RequireCheckEnum.GuildStrictModeKick:
                        await RequireStrictModeAccess(PunishmentType.Kick, services, guildId.Value, identity);
                        continue;
                    case RequireCheckEnum.GuildStrictModeBan:
                        await RequireStrictModeAccess(PunishmentType.Ban, services, guildId.Value, identity);
                        continue;
                }
            }
            return PreconditionResult.FromSuccess();
        }

        private static async Task RequireRegisteredGuild(IServiceProvider services, ulong guildId)
        {
            try
            {
                await GuildConfigRepository.CreateDefault(services).GetGuildConfig(guildId);
            }
            catch (ResourceNotFoundException)
            {
                throw new UnregisteredGuildException(guildId);
            }
            catch (NullReferenceException)
            {
                throw new BaseAPIException("Only usable in a guild.", APIError.OnlyUsableInAGuild);
            }
        }

        private static async Task RequireDiscordPermission(DiscordPermission permission, IServiceProvider services, ulong guildId, Identity identity)
        {
            await RequireRegisteredGuild(services, guildId);

            if (identity.IsSiteAdmin())
            {
                return;
            }
            switch (permission)
            {
                case DiscordPermission.Member:
                    if (identity.IsOnGuild(guildId)) return;
                    break;
                case DiscordPermission.Moderator:
                    if (await identity.HasModRoleOrHigherOnGuild(guildId)) return;
                    break;
                case DiscordPermission.Admin:
                    if (await identity.HasAdminRoleOnGuild(guildId)) return;
                    break;
            }
            throw new UnauthorizedException("You are not allowed to do that.");
        }

        private static async Task RequireStrictModeAccess(PunishmentType punishmentType, IServiceProvider services, ulong guildId, Identity identity)
        {
            await RequireRegisteredGuild(services, guildId);

            if (identity.IsSiteAdmin())
            {
                return;
            }

            if (await identity.HasPermissionToExecutePunishment(guildId, punishmentType))
                return;

            throw new UnauthorizedException("You are not allowed to do that.");
        }
    }
}
