using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using masz.data;
using masz.Dtos.DiscordAPIResponses;
using masz.Dtos.ModCase;
using masz.Models;
using masz.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace masz.Controllers
{
    [ApiController]
    [Route("api/v1/guilds/{guildid}/dashboard")]
    [Authorize]
    public class GuildDashbordController : ControllerBase
    {
        private readonly ILogger<GuildDashbordController> logger;
        private readonly IDatabase database;
        private readonly IOptions<InternalConfig> config;
        private readonly IDiscordAPIInterface discord;
        private readonly IIdentityManager identityManager;
        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public GuildDashbordController(ILogger<GuildDashbordController> logger, IDatabase database, IOptions<InternalConfig> config, IDiscordAPIInterface discord, IIdentityManager identityManager)
        {
            this.logger = logger;
            this.database = database;
            this.config = config;
            this.discord = discord;
            this.identityManager = identityManager;
        }

        [HttpGet("chart")]
        public async Task<IActionResult> GetModCaseGrid([FromRoute] string guildid, [FromQuery] long? since = null)
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            Identity currentIdentity = await identityManager.GetIdentity(HttpContext);
            User currentUser = await currentIdentity.GetCurrentDiscordUser();
            if (currentUser == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            if (!await currentIdentity.HasModRoleOrHigherOnGuild(guildid, this.database) && !config.Value.SiteAdminDiscordUserIds.Contains(currentUser.Id))
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            // ========================================================

            DateTime sinceTime = DateTime.UtcNow.AddYears(-1);
            if (since != null) {
                sinceTime = epoch.AddSeconds(since.Value);
            }

            logger.LogInformation(HttpContext.Request.Method + " " + HttpContext.Request.Path + " | 200 Returning dashboard graphs.");
            return Ok( new {
                modCases = await this.database.GetCaseCountGraph(guildid, sinceTime),
                punishments = await this.database.GetPunishmentCountGraph(guildid, sinceTime),
                autoModerations = await this.database.GetModerationCountGraph(guildid, sinceTime)
            });
        }

        [HttpGet("stats")]
        public async Task<IActionResult> Stats([FromRoute] string guildid)
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            Identity currentIdentity = await identityManager.GetIdentity(HttpContext);
            User currentUser = await currentIdentity.GetCurrentDiscordUser();
            if (currentUser == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            if (!await currentIdentity.HasModRoleOrHigherOnGuild(guildid, this.database) && !config.Value.SiteAdminDiscordUserIds.Contains(currentUser.Id))
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            // ========================================================

            int modCases = await this.database.CountAllModCasesForGuild(guildid);
            int activePunishments = await this.database.CountAllActivePunishmentsForGuild(guildid);
            int activeBans = await this.database.CountAllActivePunishmentsForGuild(guildid, PunishmentType.Ban);
            int activeMutes = await this.database.CountAllActivePunishmentsForGuild(guildid, PunishmentType.Mute);
            int autoModerations = await this.database.CountAllModerationEventsForGuild(guildid);

            return Ok(new {
                caseCount = modCases,
                activeCount = activePunishments,
                activeBanCount = activeBans,
                activeMuteCount = activeMutes,
                moderationCount = autoModerations
            });
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromRoute] string guildid, [FromQuery] string search)
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            Identity currentIdentity = await identityManager.GetIdentity(HttpContext);
            User currentUser = await currentIdentity.GetCurrentDiscordUser();
            if (currentUser == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            if (!await currentIdentity.HasModRoleOrHigherOnGuild(guildid, this.database) && !config.Value.SiteAdminDiscordUserIds.Contains(currentUser.Id))
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            // ========================================================

            if (String.IsNullOrWhiteSpace(search)) {
                return Ok(new List<string>());
            }

            List<ModCase> modCases = await database.SelectAllModCasesForGuild(guildid);
            List<AutoModerationEvent> events = await database.SelectAllModerationEventsForGuild(guildid);

            List<QuickSearchEntry> entries = new List<QuickSearchEntry>();
            foreach (var c in modCases)
            {
                var entry = new ModCaseTableEntry() {
                    ModCase = c,
                    Suspect = await discord.FetchUserInfo(c.UserId),
                    Moderator = await discord.FetchUserInfo(c.ModId)
                };
                if (contains(entry, search)) {
                    entries.Add(new QuickSearchEntry<ModCaseTableEntry>() {
                        CreatedAt = entry.ModCase.CreatedAt,
                        Entry = entry,
                        QuickSearchEntryType = QuickSearchEntryType.ModCase
                    });
                }
            }

            foreach (var c in events)
            {
                var entry = new AutoModerationEventTableEntry() {
                    AutoModerationEvent = c,
                    Suspect = await discord.FetchUserInfo(c.UserId)
                };
                if (contains(entry, search)) {
                    entries.Add(new QuickSearchEntry<AutoModerationEventTableEntry>() {
                        CreatedAt = entry.AutoModerationEvent.CreatedAt,
                        Entry = entry,
                        QuickSearchEntryType = QuickSearchEntryType.AutoModeration
                    });
                }
            }

            return Ok(entries.OrderByDescending(x => x.CreatedAt).ToList());
        }

        private bool contains(ModCaseTableEntry obj, string search) {
            return contains(obj.ModCase, search) ||
                    contains(obj.Moderator, search) ||
                    contains(obj.Suspect, search);
        }

        private bool contains(AutoModerationEventTableEntry obj, string search) {
            return contains(obj.AutoModerationEvent, search) ||
                    contains(obj.Suspect, search);
        }

        private bool contains(ModCase obj, string search) {
            return contains(obj.Title, search) ||
                    contains(obj.Description, search) ||
                    contains(obj.Punishment, search) ||
                    contains(obj.Username, search) ||
                    contains(obj.Discriminator, search) ||
                    contains(obj.Nickname, search) ||
                    contains(obj.UserId, search) ||
                    contains(obj.ModId, search) ||
                    contains(obj.LastEditedByModId, search) ||
                    contains(obj.CreatedAt, search) ||
                    contains(obj.OccuredAt, search) ||
                    contains(obj.LastEditedAt, search) ||
                    contains(obj.Labels, search) ||
                    contains(obj.CaseId.ToString(), search) ||
                    contains("#" + obj.CaseId.ToString(), search);
        }

        private bool contains(AutoModerationEvent obj, string search) {
            return contains(obj.AutoModerationAction.ToString(), search) ||
                    contains(obj.AutoModerationType.ToString(), search) ||
                    contains(obj.CreatedAt, search) ||
                    contains(obj.Discriminator, search) ||
                    contains(obj.Username, search) ||
                    contains(obj.Nickname, search) ||
                    contains(obj.UserId, search) ||
                    contains(obj.MessageContent, search) ||
                    contains(obj.MessageId, search);
        }

        private bool contains(string obj, string search) {
            if (String.IsNullOrWhiteSpace(obj)) {
                return false;
            }
            return obj.Contains(search, StringComparison.CurrentCultureIgnoreCase);
        }

        private bool contains(DateTime obj, string search) {
            if (obj == null) {
                return false;
            }
            return obj.ToString().Contains(search, StringComparison.CurrentCultureIgnoreCase);
        }

        private bool contains(string[] obj, string search) {
            if (obj == null) {
                return false;
            }
            return obj.Contains(search);
        }

        private bool contains(User obj, string search) {
            if (obj == null) {
                return false;
            }
            return contains(obj.Username, search) || contains(obj.Discriminator, search);
        }
    }
}