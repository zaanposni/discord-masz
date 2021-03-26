using System;
using System.Threading.Tasks;
using masz.Dtos.DiscordAPIResponses;
using masz.Models;
using masz.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace masz.Controllers
{
    public class SimpleController : ControllerBase
    {
        public readonly IDatabase database;
        public readonly IOptions<InternalConfig> config;
        private readonly IIdentityManager identityManager;
        public readonly IDiscordAPIInterface discord;
        public readonly IDiscordAnnouncer discordAnnouncer;
        public readonly IFilesHandler filesHandler;
        public readonly IPunishmentHandler punishmentHandler;

        public SimpleController(IServiceProvider serviceProvider)
        {
            this.database = (IDatabase) serviceProvider.GetService(typeof(IDatabase));
            this.config = (IOptions<InternalConfig>) serviceProvider.GetService(typeof(IOptions<InternalConfig>));;
            this.identityManager = (IIdentityManager) serviceProvider.GetService(typeof(IIdentityManager));;
            this.discord = (IDiscordAPIInterface) serviceProvider.GetService(typeof(IDiscordAPIInterface));;
            this.discordAnnouncer = (IDiscordAnnouncer) serviceProvider.GetService(typeof(IDiscordAnnouncer));;
            this.filesHandler = (IFilesHandler) serviceProvider.GetService(typeof(IFilesHandler));;
            this.punishmentHandler = (IPunishmentHandler) serviceProvider.GetService(typeof(IPunishmentHandler));;
        }


        public async Task<Identity> GetIdentity() {
            return await this.identityManager.GetIdentity(HttpContext);
        }

        public async Task<User> IsValidUser() {
            Identity identity = await this.GetIdentity();
            if (identity == null) {
                return null;
            }
            return await identity.GetCurrentDiscordUser();            
        }

        public async Task<bool> HasPermissionOnGuild(DiscordPermission permission, string guildId) {
            Identity identity = await this.GetIdentity();
            if (identity == null) {
                return false;
            }
            if (await this.IsSiteAdmin()) {
                return true;
            }
            switch(permission) {
                case DiscordPermission.Member:
                    return await identity.IsOnGuild(guildId);
                case DiscordPermission.Moderator:
                    return await identity.HasModRoleOrHigherOnGuild(guildId, database);
                case DiscordPermission.Admin:
                    return await identity.HasAdminRoleOnGuild(guildId, database);
                default:
                    return false;
            }
        }

        public async Task<bool> IsSiteAdmin() {
            User currentUser = await this.IsValidUser();
            if (currentUser == null) {
                return false;
            }
            return config.Value.SiteAdminDiscordUserIds.Contains(currentUser.Id);
        }

        public async Task<bool> IsAllowedTo(APIActionPermission permission, ModCase modCase) {
            User currentUser = await this.IsValidUser();
            if (modCase == null) {
                return false;
            }
            if (await this.IsSiteAdmin()) {
                return true;
            }
            switch(permission) {
                case APIActionPermission.View:
                    if (currentUser == null) {
                        return false;
                    }
                    return modCase.UserId == currentUser.Id || await this.HasPermissionOnGuild(DiscordPermission.Moderator, modCase.GuildId);
                case APIActionPermission.Edit: case APIActionPermission.Delete:
                    return await this.HasPermissionOnGuild(DiscordPermission.Moderator, modCase.GuildId);
                case APIActionPermission.ForceDelete:
                    return false;  // only siteadmin
                default:
                    return false;
            }
        }

        public async Task<bool> IsAllowedTo(APIActionPermission permission, CaseTemplate template) {
            User currentUser = await this.IsValidUser();
            if (currentUser == null) {
                return false;
            }
            if (template == null) {
                return false;
            }
            if (await this.IsSiteAdmin()) {
                return true;
            }
            switch(permission) {
                case APIActionPermission.View:
                    if (template.UserId == currentUser.Id) {
                        return true;
                    }
                    if (template.ViewPermission == ViewPermission.Self) {
                        return false;
                    }
                    if (template.ViewPermission == ViewPermission.Global) {
                        return true;
                    }
                    return await this.HasPermissionOnGuild(DiscordPermission.Moderator, template.CreatedForGuildId);
                case APIActionPermission.Edit: case APIActionPermission.Delete:
                    return await this.HasPermissionOnGuild(DiscordPermission.Moderator, template.CreatedForGuildId) && template.UserId == currentUser.Id;
                case APIActionPermission.ForceDelete:
                    return false;  // only siteadmin
                default:
                    return false;
            }
        }

        public async Task<GuildConfig> GuildIsRegistered(string guildId) {
            return await database.SelectSpecificGuildConfig(guildId);
        }
    }
}