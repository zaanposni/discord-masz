using Discord;
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Repositories;
using MASZ.Services;

namespace MASZ.Models
{
    public abstract class Identity
    {
        protected readonly IServiceProvider _serviceProvider;
        protected readonly DiscordAPIInterface _discordAPI;
        protected readonly InternalConfiguration _config;
        public DateTime ValidUntil { get; set; }
        protected string Token;
        protected IUser currentUser;
        protected List<UserGuild> currentUserGuilds;
        public Identity(string token, IServiceProvider serviceProvider)
        {
            Token = token;
            ValidUntil = DateTime.UtcNow.AddMinutes(15);

            _serviceProvider = serviceProvider;
            _discordAPI = serviceProvider.GetRequiredService<DiscordAPIInterface>();
            _config = serviceProvider.GetRequiredService<InternalConfiguration>();
        }

        /// <summary>
        /// This method checks if a HttpContext holds a valid Discord user token.
        /// https://discord.com/developers/docs/resources/user#get-current-user
        /// </summary>
        /// <returns>if a request with the users token could be authenticated against hte discord api</returns>
        public abstract bool IsAuthorized();
        /// <summary>
        /// This method checks if the discord user is member of a specified guild.
        /// https://discord.com/developers/docs/resources/user#get-current-user-guilds
        /// </summary>
        /// <param name="guildId">guild that the user should be member of</param>
        /// <returns>True the user is member of the specified guild.</returns>
        public abstract bool IsOnGuild(ulong guildId);
        /// <summary>
        /// This method checksif the discord user is member of a specified guild and has a mod role or higher as they are specified in the database.
        /// https://discord.com/developers/docs/resources/guild#get-guild-member
        /// </summary>
        /// <param name="guildId">guild that the user requestes for</param>
        /// <returns>the guildmember object if the current user could be found on that guild.</returns>
        public abstract Task<IGuildUser> GetGuildMembership(ulong guildId);
        /// <summary>
        /// Checks if the current user has the defined admin role on the defined guild.
        /// </summary>
        /// <param name="guildId">the guild to check on</param>
        /// <returns>True if the user is on this guild and is member of the admin role.</returns>
        public abstract Task<bool> HasAdminRoleOnGuild(ulong guildId);
        /// <summary>
        /// Checks if the current user has a defined team role on the defined guild.
        /// </summary>
        /// <param name="guildId">the guild to check on</param>
        /// <returns>True if the user is on this guild and has at least one of the configured roles.</returns>
        public abstract Task<bool> HasModRoleOrHigherOnGuild(ulong guildId);
        public async Task<bool> HasRolePermissionInGuild(ulong guildId, GuildPermission permission)
        {
            IGuild guild = _discordAPI.FetchGuildInfo(guildId, CacheBehavior.Default);
            if (currentUser == null || guild == null)
            {
                return false;
            }
            IGuildUser member = await _discordAPI.FetchMemberInfo(guildId, currentUser.Id, CacheBehavior.Default);
            if (member == null)
            {
                return false;
            }

            if (member.Guild.OwnerId == member.Id)
            {
                return true;
            }
            return member.GuildPermissions.Has(permission);
        }

        public async Task<bool> IsAllowedTo(APIActionPermission permission, ModCase modCase)
        {
            if (modCase == null)
            {
                return false;
            }
            if (IsSiteAdmin())
            {
                return true;
            }
            switch (permission)
            {
                case APIActionPermission.View:
                    if (currentUser == null)
                    {
                        return false;
                    }
                    return modCase.UserId == currentUser.Id || await HasPermissionOnGuild(DiscordPermission.Moderator, modCase.GuildId);
                case APIActionPermission.Delete:
                    return await HasPermissionOnGuild(DiscordPermission.Moderator, modCase.GuildId);
                case APIActionPermission.ForceDelete:
                    return await HasPermissionOnGuild(DiscordPermission.Admin, modCase.GuildId);
                case APIActionPermission.Edit:
                    GuildConfig guildConfig;
                    try
                    {
                        using var scope = _serviceProvider.CreateScope();
                        guildConfig = await GuildConfigRepository.CreateDefault(scope.ServiceProvider).GetGuildConfig(modCase.GuildId);
                    }
                    catch (ResourceNotFoundException)
                    {
                        return false;
                    }
                    if (guildConfig.StrictModPermissionCheck && modCase.PunishmentType != PunishmentType.Warn)
                    {
                        GuildPermission x = GuildPermission.CreateInstantInvite;
                        if (modCase.PunishmentType == PunishmentType.Kick) x = GuildPermission.KickMembers;
                        if (modCase.PunishmentType == PunishmentType.Ban) x = GuildPermission.BanMembers;
                        if (modCase.PunishmentType == PunishmentType.Mute) x = GuildPermission.ModerateMembers;
                        if (await HasPermissionOnGuild(DiscordPermission.Admin, modCase.GuildId))
                        {
                            return true;
                        }
                        return await HasPermissionOnGuild(DiscordPermission.Moderator, modCase.GuildId) &&
                            await HasRolePermissionInGuild(modCase.GuildId, x);
                    }
                    return await HasPermissionOnGuild(DiscordPermission.Moderator, modCase.GuildId);
            }
            return false;
        }
        public async Task<bool> IsAllowedTo(APIActionPermission permission, CaseTemplate caseTemplate)
        {
            if (currentUser == null)
            {
                return false;
            }
            if (caseTemplate == null)
            {
                return false;
            }
            if (IsSiteAdmin())
            {
                return true;
            }
            switch (permission)
            {
                case APIActionPermission.View:
                    if (caseTemplate.UserId == currentUser.Id)
                    {
                        return true;
                    }
                    if (caseTemplate.ViewPermission == ViewPermission.Self)
                    {
                        return false;
                    }
                    if (caseTemplate.ViewPermission == ViewPermission.Global)
                    {
                        return true;
                    }
                    return await HasPermissionOnGuild(DiscordPermission.Moderator, caseTemplate.CreatedForGuildId);
                case APIActionPermission.Edit:
                case APIActionPermission.Delete:
                    return await HasPermissionOnGuild(DiscordPermission.Moderator, caseTemplate.CreatedForGuildId) && caseTemplate.UserId == currentUser.Id;
                case APIActionPermission.ForceDelete:
                    return false;  // only siteadmin
                default:
                    return false;
            }
        }
        public abstract bool IsSiteAdmin();
        public async Task<bool> HasPermissionOnGuild(DiscordPermission permission, ulong guildId)
        {
            if (IsSiteAdmin())
            {
                return true;
            }
            return permission switch
            {
                DiscordPermission.Member => IsOnGuild(guildId),
                DiscordPermission.Moderator => await HasModRoleOrHigherOnGuild(guildId),
                DiscordPermission.Admin => await HasAdminRoleOnGuild(guildId),
                _ => false,
            };
        }
        public IUser GetCurrentUser()
        {
            if (currentUser == null)
            {
                throw new InvalidIdentityException(Token);
            }
            return currentUser;
        }
        public List<UserGuild> GetCurrentUserGuilds()
        {
            if (currentUserGuilds == null)
            {
                throw new InvalidIdentityException(Token);
            }
            return currentUserGuilds;
        }
        public async Task<bool> HasPermissionToExecutePunishment(ulong guildId, PunishmentType punishment)
        {
            GuildConfig guildConfig;
            try
            {
                using var scope = _serviceProvider.CreateScope();
                guildConfig = await GuildConfigRepository.CreateDefault(scope.ServiceProvider).GetGuildConfig(guildId);
            }
            catch (ResourceNotFoundException)
            {
                return false;
            }
            if (IsSiteAdmin())
            {
                return true;
            }
            if (!await HasPermissionOnGuild(DiscordPermission.Moderator, guildId))
            {
                return false;
            }
            if (await HasPermissionOnGuild(DiscordPermission.Admin, guildId))
            {
                return true;
            }
            if (guildConfig.StrictModPermissionCheck)
            {
                switch (punishment)
                {
                    case PunishmentType.Kick:
                        return await HasRolePermissionInGuild(guildId, GuildPermission.KickMembers);
                    case PunishmentType.Ban:
                        return await HasRolePermissionInGuild(guildId, GuildPermission.BanMembers);
                }
            }
            return true;
        }

        public virtual void RemoveGuildMembership(ulong guildId)
        {
            currentUserGuilds.RemoveAll(x => x.Id == guildId);
        }

        public virtual void AddGuildMembership(IGuildUser member)
        {
            if (!currentUserGuilds.Any(x => x.Id == member.Guild.Id))
            {
                currentUserGuilds.Add(new UserGuild(member));
            }
        }

        public virtual void UpdateGuildMembership(IGuildUser member)
        {
            if (!currentUserGuilds.Any(x => x.Id == member.Guild.Id))
            {
                currentUserGuilds.Add(new UserGuild(member));
            }
        }
    }
}