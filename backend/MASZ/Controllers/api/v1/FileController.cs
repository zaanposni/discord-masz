using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Models;
using MASZ.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/guilds/{guildId}/cases/{caseId}/files")]
    public class FileController : SimpleCaseController
    {

        public FileController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpDelete("{filename}")]
        [Authorize]
        public async Task<IActionResult> DeleteSpecificItem([FromRoute] ulong guildId, [FromRoute] int caseId, [FromRoute] string filename)
        {
            await RequirePermission(guildId, caseId, APIActionPermission.Edit);
            Identity identity = await GetIdentity();

            await FileRepository.CreateDefault(_serviceProvider, identity).DeleteFile(guildId, caseId, filename);

            return Ok();
        }

        [HttpGet("{filename}")]
        public async Task<IActionResult> GetSpecificItem([FromRoute] ulong guildId, [FromRoute] int caseId, [FromRoute] string filename)
        {
            if (!_config.IsPublicFileEnabled())
            {
                await RequirePermission(guildId, caseId, APIActionPermission.View);
            }

            UploadedFile fileInfo = FileRepository.CreateWithBotIdentity(_serviceProvider).GetCaseFile(guildId, caseId, filename);

            HttpContext.Response.Headers.Add("Content-Disposition", fileInfo.ContentDisposition.ToString());
            HttpContext.Response.Headers.Add("Content-Type", fileInfo.ContentType);

            return File(fileInfo.FileContent, fileInfo.ContentType);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllItems([FromRoute] ulong guildId, [FromRoute] int caseId)
        {
            await RequirePermission(guildId, caseId, APIActionPermission.View);
            Identity identity = await GetIdentity();

            List<string> files = new();
            try
            {
                files = FileRepository.CreateDefault(_serviceProvider, identity).GetCaseFiles(guildId, caseId);
            }
            catch (ResourceNotFoundException) { }

            return Ok(new { names = files });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostItem([FromRoute] ulong guildId, [FromRoute] int caseId, [FromForm] Dtos.ModCase.UploadedFile uploadedFile)
        {
            await RequirePermission(guildId, caseId, APIActionPermission.Edit);
            Identity identity = await GetIdentity();

            return Ok(new { path = await FileRepository.CreateDefault(_serviceProvider, identity).UploadFile(uploadedFile.File, guildId, caseId) });
        }
    }
}