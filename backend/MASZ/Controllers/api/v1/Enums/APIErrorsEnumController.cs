using MASZ.Dtos.Enum;
using MASZ.Enums;
using Microsoft.AspNetCore.Mvc;

namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/enums/")]
    public class APIErrorsEnumController : SimpleController
    {
        public APIErrorsEnumController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet("apierror")]
        public IActionResult CreationType([FromQuery] Language? language = null)
        {
            _translator.SetContext(language);
            return Ok(new List<EnumDto>()
            {
                EnumDto.Create((int) APIError.Unknown, _translator.T().Enum(APIError.Unknown)),
                EnumDto.Create((int) APIError.InvalidDiscordUser, _translator.T().Enum(APIError.InvalidDiscordUser)),
                EnumDto.Create((int) APIError.ProtectedModCaseSuspect, _translator.T().Enum(APIError.ProtectedModCaseSuspect)),
                EnumDto.Create((int) APIError.ProtectedModCaseSuspectIsBot, _translator.T().Enum(APIError.ProtectedModCaseSuspectIsBot)),
                EnumDto.Create((int) APIError.ProtectedModCaseSuspectIsSiteAdmin, _translator.T().Enum(APIError.ProtectedModCaseSuspectIsSiteAdmin)),
                EnumDto.Create((int) APIError.ProtectedModCaseSuspectIsTeam, _translator.T().Enum(APIError.ProtectedModCaseSuspectIsTeam)),
                EnumDto.Create((int) APIError.ResourceNotFound, _translator.T().Enum(APIError.ResourceNotFound)),
                EnumDto.Create((int) APIError.InvalidIdentity, _translator.T().Enum(APIError.InvalidIdentity)),
                EnumDto.Create((int) APIError.GuildUnregistered, _translator.T().Enum(APIError.GuildUnregistered)),
                EnumDto.Create((int) APIError.Unauthorized, _translator.T().Enum(APIError.Unauthorized)),
                EnumDto.Create((int) APIError.GuildUndefinedMutedRoles, _translator.T().Enum(APIError.GuildUndefinedMutedRoles)),
                EnumDto.Create((int) APIError.ModCaseIsMarkedToBeDeleted, _translator.T().Enum(APIError.ModCaseIsMarkedToBeDeleted)),
                EnumDto.Create((int) APIError.ModCaseIsNotMarkedToBeDeleted, _translator.T().Enum(APIError.ModCaseIsNotMarkedToBeDeleted)),
                EnumDto.Create((int) APIError.GuildAlreadyRegistered, _translator.T().Enum(APIError.GuildAlreadyRegistered)),
                EnumDto.Create((int) APIError.NotAllowedInDemoMode, _translator.T().Enum(APIError.NotAllowedInDemoMode)),
                EnumDto.Create((int) APIError.RoleNotFound, _translator.T().Enum(APIError.RoleNotFound)),
                EnumDto.Create((int) APIError.TokenCannotManageThisResource, _translator.T().Enum(APIError.TokenCannotManageThisResource)),
                EnumDto.Create((int) APIError.TokenAlreadyRegistered, _translator.T().Enum(APIError.TokenAlreadyRegistered)),
                EnumDto.Create((int) APIError.CannotBeSameUser, _translator.T().Enum(APIError.CannotBeSameUser)),
                EnumDto.Create((int) APIError.ResourceAlreadyExists, _translator.T().Enum(APIError.ResourceAlreadyExists)),
                EnumDto.Create((int) APIError.ModCaseDoesNotAllowComments, _translator.T().Enum(APIError.ModCaseDoesNotAllowComments)),
                EnumDto.Create((int) APIError.LastCommentAlreadyFromSuspect, _translator.T().Enum(APIError.LastCommentAlreadyFromSuspect)),
                EnumDto.Create((int) APIError.InvalidAutomoderationAction, _translator.T().Enum(APIError.InvalidAutomoderationAction)),
                EnumDto.Create((int) APIError.InvalidAutomoderationType, _translator.T().Enum(APIError.InvalidAutomoderationType)),
                EnumDto.Create((int) APIError.TooManyTemplates, _translator.T().Enum(APIError.TooManyTemplates)),
                EnumDto.Create((int) APIError.InvalidFilePath, _translator.T().Enum(APIError.InvalidFilePath)),
                EnumDto.Create((int) APIError.NoGuildsRegistered, _translator.T().Enum(APIError.NoGuildsRegistered)),
                EnumDto.Create((int) APIError.OnlyUsableInAGuild, _translator.T().Enum(APIError.OnlyUsableInAGuild)),
                EnumDto.Create((int) APIError.InvalidAuditLogEvent, _translator.T().Enum(APIError.InvalidAuditLogEvent)),
                EnumDto.Create((int) APIError.ProtectedScheduledMessage, _translator.T().Enum(APIError.ProtectedScheduledMessage)),
                EnumDto.Create((int) APIError.InvalidDateForScheduledMessage, _translator.T().Enum(APIError.InvalidDateForScheduledMessage))
            });
        }
    }
}