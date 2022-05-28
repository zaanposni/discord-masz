using Discord;
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Models;
using MASZ.Models.Views;
using MASZ.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/guilds/{guildId}/cases/{caseId}/view")]
    [Authorize]
    public class CaseViewController : SimpleCaseController
    {
        public CaseViewController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetModCaseView([FromRoute] ulong guildId, [FromRoute] int caseId)
        {
            await RequirePermission(guildId, caseId, APIActionPermission.View);
            GuildConfig guildConfig = await GetRegisteredGuild(guildId);
            Identity identity = await GetIdentity();

            ModCase modCase = await ModCaseRepository.CreateDefault(_serviceProvider, identity).GetModCase(guildId, caseId);

            IUser suspect = await _discordAPI.FetchUserInfo(modCase.UserId, CacheBehavior.OnlyCache);

            List<CommentExpandedView> comments = new();
            foreach (ModCaseComment comment in modCase.Comments)
            {
                comments.Add(new CommentExpandedView(
                    comment,
                    await _discordAPI.FetchUserInfo(comment.UserId, CacheBehavior.OnlyCache)
                ));
            }

            UserNoteExpandedView userNote = null;
            if (await identity.HasPermissionOnGuild(DiscordPermission.Moderator, guildId))
            {
                try
                {
                    var note = await UserNoteRepository.CreateDefault(_serviceProvider, identity).GetUserNote(guildId, modCase.UserId);
                    userNote = new UserNoteExpandedView(
                        note,
                        suspect,
                        await _discordAPI.FetchUserInfo(note.CreatorId, CacheBehavior.OnlyCache)
                    );
                }
                catch (ResourceNotFoundException) { }
            }

            CaseExpandedView caseView = new(
                modCase,
                await _discordAPI.FetchUserInfo(modCase.ModId, CacheBehavior.OnlyCache),
                await _discordAPI.FetchUserInfo(modCase.LastEditedByModId, CacheBehavior.OnlyCache),
                suspect,
                comments,
                userNote
            );

            if (modCase.LockedByUserId != 0)
            {
                caseView.LockedBy = DiscordUserView.CreateOrDefault(await _discordAPI.FetchUserInfo(modCase.LockedByUserId, CacheBehavior.OnlyCache));
            }
            if (modCase.DeletedByUserId != 0)
            {
                caseView.DeletedBy = DiscordUserView.CreateOrDefault(await _discordAPI.FetchUserInfo(modCase.DeletedByUserId, CacheBehavior.OnlyCache));
            }

            bool isMod = await identity.HasPermissionOnGuild(DiscordPermission.Moderator, guildId);
            if (isMod) {
                caseView.LinkedCases = modCase.MappingsA.Select(mapping => new CaseView(mapping.CaseB)).ToList();
                caseView.LinkedCases.AddRange(modCase.MappingsB.Select(mapping =>new CaseView(mapping.CaseA)));
            }

            if (!(isMod || guildConfig.PublishModeratorInfo))
            {
                caseView.RemoveModeratorInfo();
            }

            return Ok(caseView);
        }
    }
}