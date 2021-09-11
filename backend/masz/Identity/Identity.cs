using DSharpPlus;
using DSharpPlus.Entities;
using masz.Exceptions;
using masz.Repositories;
using masz.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace masz.Models
{
    public abstract class Identity
    {
        protected readonly IServiceProvider _serviceProvider;
        protected readonly IDiscordAPIInterface _discordAPI;
        protected readonly IInternalConfiguration _config;
        public DateTime ValidUntil { get; set; }
        protected string Token;
        protected DiscordUser currentUser;
        protected List<DiscordGuild> currentUserGuilds;
        public Identity (string token, IServiceProvider serviceProvider)
        {
            Token = token;
            ValidUntil = DateTime.UtcNow.AddMinutes(15);

            _serviceProvider = serviceProvider;
            _discordAPI = (IDiscordAPIInterface) serviceProvider.GetService(typeof(IDiscordAPIInterface));
            _config = (IInternalConfiguration) serviceProvider.GetService(typeof(IInternalConfiguration));
        }

        public IDatabase GetDatabase()
        {
            return (IDatabase) _serviceProvider.GetService(typeof(IDatabase));
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
        public abstract Task<DiscordMember> GetGuildMembership(ulong guildId);
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
        public async Task<bool> HasRolePermissionInGuild(ulong guildId, Permissions permission)
        {
            DiscordGuild guild = await _discordAPI.FetchGuildInfo(guildId, CacheBehavior.Default);
            if (currentUser == null || guild == null)
            {
                return false;
            }
            DiscordMember member = await _discordAPI.FetchMemberInfo(guildId, currentUser.Id, CacheBehavior.Default);
            if (member == null)
            {
                return false;
            }
            int memberPermission = (int) member.Permissions;
            return (memberPermission & 1 << (int) permission) >= 1;
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
            switch(permission) {
                case APIActionPermission.View:
                    if (currentUser == null) {
                        return false;
                    }
                    return modCase.UserId == currentUser.Id || await HasPermissionOnGuild(DiscordPermission.Moderator, modCase.GuildId);
                case APIActionPermission.Delete:
                    return await HasPermissionOnGuild(DiscordPermission.Moderator, modCase.GuildId);
                case APIActionPermission.ForceDelete:
                    return false;  // only siteadmin
                case APIActionPermission.Edit:
                    GuildConfig guildConfig;
                    try
                    {
                        guildConfig = await GuildConfigRepository.CreateDefault(_serviceProvider).GetGuildConfig(modCase.GuildId);
                    } catch (ResourceNotFoundException)
                    {
                        return false;
                    }
                    if (guildConfig.StrictModPermissionCheck) {
                        Permissions x = Permissions.None;
                        if (modCase.PunishmentType == PunishmentType.Ban) x = Permissions.BanMembers;
                        if (modCase.PunishmentType == PunishmentType.Mute) x = Permissions.BanMembers;
                        return await HasPermissionOnGuild(DiscordPermission.Moderator, modCase.GuildId) &&
                               await HasRolePermissionInGuild(modCase.GuildId, x);
                    } else {
                        return await HasPermissionOnGuild(DiscordPermission.Moderator, modCase.GuildId);
                    }
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
            if ( IsSiteAdmin())
            {
                return true;
            }
            switch(permission) {
                case APIActionPermission.View:
                    if (caseTemplate.UserId == currentUser.Id) {
                        return true;
                    }
                    if (caseTemplate.ViewPermission == ViewPermission.Self) {
                        return false;
                    }
                    if (caseTemplate.ViewPermission == ViewPermission.Global) {
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
            if ( IsSiteAdmin())
            {
                return true;
            }
            switch(permission)
            {
                case DiscordPermission.Member:
                    return IsOnGuild(guildId);
                case DiscordPermission.Moderator:
                    return await HasModRoleOrHigherOnGuild(guildId);
                case DiscordPermission.Admin:
                    return await HasAdminRoleOnGuild(guildId);
            }
            return false;
        }
        public DiscordUser GetCurrentUser()
        {
            if (currentUser == null)
            {
                throw new InvalidIdentityException(Token);
            }
            return currentUser;
        }
        public List<DiscordGuild> GetCurrentUserGuilds()
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
                guildConfig = await GuildConfigRepository.CreateDefault(_serviceProvider).GetGuildConfig(guildId);
            } catch (ResourceNotFoundException)
            {
                return false;
            }
            if (! await HasPermissionOnGuild(DiscordPermission.Moderator, guildId))
            {
                return false;
            }
            if (IsSiteAdmin())
            {
                return true;
            }
            if (guildConfig.StrictModPermissionCheck)
            {
                switch (punishment)
                {
                    case PunishmentType.Kick:
                        return await HasRolePermissionInGuild(guildId, DSharpPlus.Permissions.KickMembers);
                    case PunishmentType.Ban:
                        return await HasRolePermissionInGuild(guildId, DSharpPlus.Permissions.BanMembers);
                }
            }
            return true;
        }
    }
}