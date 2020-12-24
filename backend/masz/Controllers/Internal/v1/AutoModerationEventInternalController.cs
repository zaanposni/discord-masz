
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using masz.Dtos.AutoModerationEvent;
using masz.Dtos.DiscordAPIResponses;
using masz.Models;
using masz.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace masz.Controllers
{
    [ApiController]
    [Route("internalapi/v1/guilds/{guildid}/modcases")]
    public class AutoModerationEventInternalController : ControllerBase
    {
        private readonly ILogger<AutoModerationEventController> logger;
        private readonly IDatabase database;
        private readonly IOptions<InternalConfig> config;
        private readonly IIdentityManager identityManager;
        private readonly IDiscordAPIInterface discord;
        private readonly IPunishmentHandler punishmentHandler;
        private readonly IDiscordAnnouncer discordAnnouncer;

        public AutoModerationEventInternalController(ILogger<AutoModerationEventController> logger, IDatabase database, IOptions<InternalConfig> config, IIdentityManager identityManager, IDiscordAPIInterface discordInterface, IPunishmentHandler punishmentHandler, IDiscordAnnouncer discordAnnouncer)
        {
            this.logger = logger;
            this.database = database;
            this.config = config;
            this.identityManager = identityManager;
            this.discord = discordInterface;
            this.discordAnnouncer = discordAnnouncer;
            this.punishmentHandler = punishmentHandler;
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem([FromRoute] string guildid, [FromBody] AutoModerationEventForCreateDto dto) 
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            if (string.IsNullOrEmpty(this.config.Value.DiscordBotToken)) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Authorization header not defined.");
                return Unauthorized();
            }

            string auth = String.Empty;
            try {
                auth = Request.Headers["Authorization"];
            } catch(Exception e) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Authorization header not defined.", e);
                return Unauthorized();
            }
            if (this.config.Value.DiscordBotToken == auth) {

                AutoModerationConfig modConfig = await this.database.SelectModerationConfigForGuildAndType(guildid, dto.AutoModerationType);
                if (modConfig == null) {
                    logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 No config found for this type.");
                    return BadRequest("No config found for this type");
                }

                AutoModerationEvent modEvent = new AutoModerationEvent();
                if (modConfig.AutoModerationAction == AutoModerationAction.CaseCreated || modConfig.AutoModerationAction == AutoModerationAction.ContentDeletedAndCaseCreated)
                {
                    ModCase newModCase = new ModCase();

                    User discordBot = await discord.FetchCurrentBotInfo();

                    newModCase.CaseId = await database.GetHighestCaseIdForGuild(guildid) + 1;
                    newModCase.GuildId = guildid;
                    newModCase.UserId = dto.UserId;
                    newModCase.Username = dto.Username;
                    newModCase.Nickname = dto.Nickname;
                    newModCase.ModId = discordBot.Id;
                    newModCase.CreatedAt = DateTime.UtcNow;
                    newModCase.LastEditedAt = newModCase.CreatedAt;
                    newModCase.LastEditedByModId = discordBot.Id;
                    newModCase.OccuredAt = newModCase.CreatedAt;
                    newModCase.Title = $"AutoModeration: {dto.AutoModerationType.ToString()}";
                    newModCase.Description = $"User triggered AutoModeration.\nEvent: {dto.AutoModerationType.ToString()}.\nAction: {modConfig.AutoModerationAction.ToString()}.";
                    newModCase.Labels = new List<string>() { "automoderation", dto.AutoModerationType.ToString() }.ToArray();
                    newModCase.Valid = true;
                    newModCase.Severity = 1;
                    
                    if (modConfig.PunishmentType != null && modConfig.PunishmentType != PunishmentType.None)
                    {
                        newModCase.PunishmentType = modConfig.PunishmentType.Value;
                        newModCase.PunishmentActive = true;

                        if (modConfig.PunishmentDurationMinutes == null)
                        {
                            newModCase.Punishment = newModCase.PunishmentType.ToString();
                            newModCase.PunishedUntil = null;
                        } else {
                            newModCase.Punishment = "Temp" + newModCase.PunishmentType.ToString();
                            newModCase.PunishedUntil = DateTime.UtcNow.AddMinutes(modConfig.PunishmentDurationMinutes.Value) ;
                        }

                        try {
                            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Handling punishment.");
                            await punishmentHandler.ExecutePunishment(newModCase, database);
                        }
                        catch(Exception e){
                            logger.LogError(e, "Failed to handle punishment for modcase.");
                        }
                    } else {
                        newModCase.Punishment = "Warn";
                        newModCase.PunishmentType = PunishmentType.None;
                        newModCase.PunishedUntil = null;
                        newModCase.PunishmentActive = false;
                    }

                    await database.SaveModCase(newModCase);
                    await database.SaveChangesAsync();

                    modEvent.AssociatedCaseId = newModCase.Id;

                    logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Sending notification.");
                    try {
                        await discordAnnouncer.AnnounceModCase(newModCase, RestAction.Created, discordBot, modConfig.SendPublicNotification);
                    }
                    catch(Exception e){
                        logger.LogError(e, "Failed to announce modcase.");
                    }
                }

                modEvent.GuildId = guildid;
                modEvent.CreatedAt = DateTime.UtcNow;
                modEvent.UserId = dto.UserId;
                modEvent.Username = dto.Username;
                modEvent.Nickname = dto.Nickname;
                modEvent.Discriminator = dto.Discriminator;
                modEvent.MessageContent = dto.MessageContent;
                modEvent.MessageId = dto.MessageId;
                modEvent.AutoModerationType = dto.AutoModerationType;
                modEvent.AutoModerationAction = modConfig.AutoModerationAction;

                await database.SaveModerationEvent(modEvent);
                await database.SaveChangesAsync();

                return Ok(new { Id = modEvent.Id });
            } else {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
        }
    }
}