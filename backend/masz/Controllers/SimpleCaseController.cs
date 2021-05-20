using System;
using System.Threading.Tasks;
using masz.Dtos.DiscordAPIResponses;
using masz.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace masz.Controllers
{
    public class SimpleCaseController : SimpleController
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<SimpleCaseController> logger;

        public SimpleCaseController(IServiceProvider serviceProvider, ILogger<SimpleCaseController> logger) : base(serviceProvider) {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }

        private async Task<IActionResult> GuildIsValid(string guildId)
        {
            GuildConfig guildConfig = await this.database.SelectSpecificGuildConfig(guildId);
            if (guildConfig == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Guild not registered.");
                return BadRequest("Guild not registered.");
            }
            return null;
        }
        private async Task<IActionResult> UserIsValid()
        {
            User user = await this.IsValidUser();
            if (user == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            return null;
        }

        public async Task<IActionResult> HandleRequest(string guildId, DiscordPermission permission)
        {
            IActionResult guild = await GuildIsValid(guildId);
            if (guild != null) {
                return guild;
            }
            IActionResult user = await UserIsValid();
            if (user != null) {
                return user;
            }
            if(! await this.HasPermissionOnGuild(permission, guildId)) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            return null;
        }

        public async Task<IActionResult> HandleRequest(string guildId, string caseId, APIActionPermission permission)
        {
            IActionResult guild = await GuildIsValid(guildId);
            if (guild != null) {
                return guild;
            }
            IActionResult user = await UserIsValid();
            if (user != null) {
                return user;
            }
            ModCase modCase = await database.SelectSpecificModCase(guildId, caseId);
            if (modCase == null) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 404 Not Found.");
                return NotFound();
            }
            if (!await this.IsAllowedTo(permission, modCase)) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            if (modCase.MarkedToDeleteAt != null && permission == APIActionPermission.Edit)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Case is marked to be deleted.");
                return BadRequest("Case is marked to be deleted.");
            }

            return null;
        }

        public async Task<bool> HasPermissionExecutePunishment(string guildId, PunishmentType punishment)
        {
            switch (punishment)
            {
                case PunishmentType.Kick:
                    return await this.HasRolePermissionInGuild(guildId, DiscordBitPermissionFlags.KICK_MEMBERS);
                case PunishmentType.Ban:
                    return await this.HasRolePermissionInGuild(guildId, DiscordBitPermissionFlags.BAN_MEMBERS);
                default:
                    return true;
            }
        }
    }
}