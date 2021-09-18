using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using masz.Dtos.ModCase;
using masz.Exceptions;
using masz.Models;
using masz.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace masz.Controllers
{
    [ApiController]
    [Route("api/v1/guilds/{guildId}/cases/{caseId}/files")]
    [Authorize]
    public class FileController : SimpleCaseController
    {
        private readonly ILogger<FileController> _logger;

        public FileController(ILogger<FileController> logger, IServiceProvider serviceProvider) : base(serviceProvider, logger)
        {
            _logger = logger;
        }

        [HttpDelete("{filename}")]
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
            await RequirePermission(guildId, caseId, APIActionPermission.View);
            Identity identity = await GetIdentity();

            Models.FileInfo fileInfo = FileRepository.CreateDefault(_serviceProvider, identity).GetCaseFile(guildId, caseId, filename);

            HttpContext.Response.Headers.Add("Content-Disposition", fileInfo.ContentDisposition.ToString());
            HttpContext.Response.Headers.Add("Content-Type", fileInfo.ContentType);

            return File(fileInfo.FileContent, fileInfo.ContentType);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllItems([FromRoute] ulong guildId, [FromRoute] int caseId)
        {
            await RequirePermission(guildId, caseId, APIActionPermission.View);
            Identity identity = await GetIdentity();

            List<string> files = new List<string>();
            try
            {
                files = FileRepository.CreateDefault(_serviceProvider, identity).GetCaseFiles(guildId, caseId);
            } catch (ResourceNotFoundException) { }

            return Ok(new { names = files});
        }

        [HttpPost]
        [RequestSizeLimit(10485760)]
        public async Task<IActionResult> PostItem([FromRoute] ulong guildId, [FromRoute] int caseId, [FromForm] UploadedFile uploadedFile)
        {
            await RequirePermission(guildId, caseId, APIActionPermission.Edit);
            Identity identity = await GetIdentity();

            return Ok(new { path = await FileRepository.CreateDefault(_serviceProvider, identity).UploadFile(uploadedFile.File, guildId, caseId)});
        }
    }
}