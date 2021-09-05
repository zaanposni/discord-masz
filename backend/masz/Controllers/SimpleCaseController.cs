using System;
using System.Threading.Tasks;
using masz.Models;
using masz.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace masz.Controllers
{
    public class SimpleCaseController : SimpleController
    {
        protected readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SimpleCaseController> _logger;

        public SimpleCaseController(IServiceProvider serviceProvider, ILogger<SimpleCaseController> logger) : base(serviceProvider) {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task<IActionResult> HandleRequest(ulong guildId, DiscordPermission permission)
        {
            GuildConfig guild = await GetRegisteredGuild(guildId);
            Identity currentIdentity = await GetIdentity();
            if(! await currentIdentity.HasPermissionOnGuild(permission, guildId)) {
                _logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            return null;
        }

        public async Task<IActionResult> HandleRequest(ulong guildId, int caseId, APIActionPermission permission)
        {
            GuildConfig guild = await GetRegisteredGuild(guildId);
            Identity currentIdentity = await GetIdentity();
            ModCase modCase = await ModCaseRepository.CreateDefault(_serviceProvider).GetModCase(guildId, caseId);
            if (modCase == null)
            {
                _logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 404 Not Found.");
                return NotFound();
            }
            if (!await currentIdentity.IsAllowedTo(permission, modCase))
            {
                _logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            if (modCase.MarkedToDeleteAt != null && permission == APIActionPermission.Edit)
            {
                _logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Case is marked to be deleted.");
                return BadRequest("Case is marked to be deleted.");
            }

            return null;
        }

        public async Task<bool> HasPermissionToExecutePunishment(ulong guildId, PunishmentType punishment)
        {
            Identity currentIdentity = await GetIdentity();
            if (currentIdentity.IsSiteAdmin())
            {
                return true;
            }
            GuildConfig guildConfig = await GetRegisteredGuild(guildId);
            if (! await currentIdentity.HasPermissionOnGuild(DiscordPermission.Moderator, guildId))
            {
                return false;
            }
            if (guildConfig.StrictModPermissionCheck)
            {
                switch (punishment)
                {
                    case PunishmentType.Kick:
                        return await currentIdentity.HasRolePermissionInGuild(guildId, DSharpPlus.Permissions.KickMembers);
                    case PunishmentType.Ban:
                        return await currentIdentity.HasRolePermissionInGuild(guildId, DSharpPlus.Permissions.BanMembers);
                }
            }
            return true;
        }
    }
}