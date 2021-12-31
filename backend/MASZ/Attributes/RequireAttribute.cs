using Discord;
using Discord.Interactions;
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

        public override async Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
        {
            var identity = await services.GetRequiredService<IdentityManager>().GetIdentity(context.User);

            foreach (RequireCheckEnum check in _checks)
            {
                switch (check)
                {
                    case RequireCheckEnum.GuildRegistered:
                        await RequireRegisteredGuild(services, context);
                        continue;
                    case RequireCheckEnum.GuildMember:
                        await RequireDiscordPermission(DiscordPermission.Member, services, context, identity);
                        continue;
                    case RequireCheckEnum.GuildModerator:
                        await RequireDiscordPermission(DiscordPermission.Moderator, services, context, identity);
                        continue;
                    case RequireCheckEnum.GuildAdmin:
                        await RequireDiscordPermission(DiscordPermission.Admin, services, context, identity);
                        continue;
                    case RequireCheckEnum.GuildMuteRole:
                        try
                        {
                            GuildConfig guildConfig = await GuildConfigRepository.CreateDefault(services).GetGuildConfig(context.Guild.Id);
                            if (guildConfig.MutedRoles.Length == 0)
                            {
                                throw new GuildWithoutMutedRoleException(context.Guild.Id);
                            }
                        }
                        catch (ResourceNotFoundException)
                        {
                            throw new UnregisteredGuildException(context.Guild.Id);
                        }
                        continue;
                    case RequireCheckEnum.SiteAdmin:
                        if (!identity.IsSiteAdmin())
                        {
                            throw new UnauthorizedException("Only site admins allowed.");
                        }
                        continue;
                    case RequireCheckEnum.GuildStrictModeMute:
                        await RequireStrictModeAccess(PunishmentType.Mute, services, context, identity);
                        continue;
                    case RequireCheckEnum.GuildStrictModeKick:
                        await RequireStrictModeAccess(PunishmentType.Kick, services, context, identity);
                        continue;
                    case RequireCheckEnum.GuildStrictModeBan:
                        await RequireStrictModeAccess(PunishmentType.Ban, services, context, identity);
                        continue;
                }
            }
            return PreconditionResult.FromSuccess();
        }

        private static async Task RequireRegisteredGuild(IServiceProvider services, IInteractionContext context)
        {
            try
            {
                await GuildConfigRepository.CreateDefault(services).GetGuildConfig(context.Guild.Id);
            }
            catch (ResourceNotFoundException)
            {
                throw new UnregisteredGuildException(context.Guild.Id);
            }
            catch (NullReferenceException)
            {
                throw new BaseAPIException("Only usable in a guild.", APIError.OnlyUsableInAGuild);
            }
        }

        private static async Task RequireDiscordPermission(DiscordPermission permission, IServiceProvider services, IInteractionContext context, Identity identity)
        {
            await RequireRegisteredGuild(services, context);

            if (identity.IsSiteAdmin())
            {
                return;
            }
            switch (permission)
            {
                case DiscordPermission.Member:
                    if (identity.IsOnGuild(context.Guild.Id)) return;
                    break;
                case DiscordPermission.Moderator:
                    if (await identity.HasModRoleOrHigherOnGuild(context.Guild.Id)) return;
                    break;
                case DiscordPermission.Admin:
                    if (await identity.HasAdminRoleOnGuild(context.Guild.Id)) return;
                    break;
            }
            throw new UnauthorizedException("You are not allowed to do that.");
        }

        private static async Task RequireStrictModeAccess(PunishmentType punishmentType, IServiceProvider services, IInteractionContext context, Identity identity)
        {
            await RequireRegisteredGuild(services, context);

            if (identity.IsSiteAdmin())
            {
                return;
            }

            if (await identity.HasPermissionToExecutePunishment(context.Guild.Id, punishmentType))
                return;

            throw new UnauthorizedException("You are not allowed to do that.");
        }
    }
}
