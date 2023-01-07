using MASZ.Enums;
using MASZ.Models;
using Microsoft.AspNetCore.Mvc;
using MASZ.Exceptions;
using MASZ.Repositories;
using Discord;
using System.Text.RegularExpressions;
using MASZ.Models.Database;

namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/embed")]
    public class EmbedController : SimpleController
    {
        public EmbedController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        private static readonly Regex _modCaseRegex = new(@"/guilds/([0-9]+)/cases/([0-9]+)");
        private static readonly Regex _evidenceRegex = new(@"/guilds/([0-9]+)/evidence/([0-9]+)");

        [HttpGet]
        public async Task<IActionResult> GetOEmbedInfo([FromQuery] string url = "")
        {
            if (! string.IsNullOrEmpty(url))
            {
                Match modCaseMatch = _modCaseRegex.Match(url);
                if (modCaseMatch.Success)
                {
                    try
                    {
                        ulong guildId = ulong.Parse(modCaseMatch.Groups[1].Value);
                        int caseId = int.Parse(modCaseMatch.Groups[2].Value);
                        return await GetModCaseEmbed(guildId, caseId);
                    }
                    catch (Exception)
                    {
                        return await GetDefaultEmbed();
                    }
                }

                Match evidenceMatch = _evidenceRegex.Match(url);
                if (evidenceMatch.Success)
                {
                    try
                    {
                        ulong guildId = ulong.Parse(evidenceMatch.Groups[1].Value);
                        int evidenceId = int.Parse(evidenceMatch.Groups[2].Value);
                        return await GetEvidenceEmbed(guildId, evidenceId);
                    }
                    catch (Exception)
                    {
                        return await GetDefaultEmbed();
                    }
                }
            }
            return await GetDefaultEmbed();
        }

        private async Task<ContentResult> GetDefaultEmbed()
        {
            AppSettings appSettings = await AppSettingsRepository.CreateDefault(_serviceProvider).GetAppSettings();
            IApplication application = await _discordAPI.GetCurrentApplicationInfo();

            return new ContentResult()
            {
                Content = appSettings.GetEmbedData(_config.GetBaseUrl(), application.IconUrl),
                ContentType = "text/html; charset=utf-8"
            };
        }

        private async Task<ContentResult> GetModCaseEmbed(ulong guildId, int caseId)
        {
            GuildConfig guildConfig;
            ModCase modCase;
            try
            {
                guildConfig = await GuildConfigRepository.CreateDefault(_serviceProvider).GetGuildConfig(guildId);
                modCase = await ModCaseRepository.CreateWithBotIdentity(_serviceProvider).GetModCase(guildId, caseId);
            }
            catch (ResourceNotFoundException)
            {
                return await GetDefaultEmbed();
            }

            if (guildConfig.PublicEmbedMode)
            {
                _translator.SetContext(guildConfig);
                IUser user = await _discordAPI.FetchUserInfo(modCase.UserId, CacheBehavior.OnlyCache);
                return new ContentResult()
                {
                    Content = modCase.GetEmbedData(_config.GetBaseUrl(), user, _translator),
                    ContentType = "text/html; charset=utf-8"
                };
            }
            return await GetDefaultEmbed();
        }

        private async Task<ContentResult> GetEvidenceEmbed(ulong guildId, int evidenceId)
        {
            GuildConfig guildConfig;
            VerifiedEvidence evidence;
            try
            {
                guildConfig = await GuildConfigRepository.CreateDefault(_serviceProvider).GetGuildConfig(guildId);
                evidence = await VerifiedEvidenceRepository.CreateWithBotIdentity(_serviceProvider).GetEvidence(guildId, evidenceId);
            }
            catch (ResourceNotFoundException)
            {
                return await GetDefaultEmbed();
            }

            if (guildConfig.PublicEmbedMode)
            {
                IUser user = await _discordAPI.FetchUserInfo(evidence.UserId, CacheBehavior.OnlyCache);
                return new ContentResult()
                {
                    Content = evidence.GetEmbedData(_config.GetBaseUrl(), user),
                    ContentType = "text/html; charset=utf-8"
                };
            }
            return await GetDefaultEmbed();
        }
    }
}