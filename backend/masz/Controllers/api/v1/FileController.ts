using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using masz.data;
using masz.Dtos.DiscordAPIResponses;
using masz.Dtos.ModCase;
using masz.Models;
using masz.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace masz.Controllers
{
    [ApiController]
    [Route("api/v1/guilds/{guildid}/modcases/{caseid}/files")]
    [Authorize]
    public class FileController : SimpleCaseController
    {
        private readonly ILogger<FileController> logger;

        public FileController(ILogger<FileController> logger, IServiceProvider serviceProvider) : base(serviceProvider, logger)
        {
            this.logger = logger;
        }

        [HttpDelete("{filename}")]
        public async Task<IActionResult> DeleteSpecificItem([FromRoute] string guildid, [FromRoute] string caseid, [FromRoute] string filename) 
        {
            IActionResult result = await this.HandleRequest(guildid, caseid, APIActionPermission.Edit);
            if (result != null) {
                return result;
            }

            string filePath = Path.Combine(config.Value.AbsolutePathToFileUpload, guildid, caseid, filename);
            if (! filesHandler.FileExists(filePath)) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 404 File not found.");
                return NotFound("File not found.");
            }

            filesHandler.DeleteFile(filePath);

            ModCase modCase = await this.database.SelectSpecificModCase(guildid, caseid);
            User currentUser = await this.IsValidUser();
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Sending notification.");
            try {
                await discordAnnouncer.AnnounceFile(filename, modCase, currentUser, RestAction.Deleted);
            }
            catch(Exception e){
                logger.LogError(e, "Failed to announce file.");
            }

            return Ok();
        }

        [HttpGet("{filename}")]
        public async Task<IActionResult> GetSpecificItem([FromRoute] string guildid, [FromRoute] string caseid, [FromRoute] string filename) 
        {
            IActionResult result = await this.HandleRequest(guildid, caseid, APIActionPermission.View);
            if (result != null) {
                return result;
            }

            var filePath = Path.Combine(config.Value.AbsolutePathToFileUpload, guildid, caseid, filesHandler.RemoveSpecialCharacters(filename));
            // https://stackoverflow.com/a/1321535/9850709
            if (Path.GetFullPath(filePath) != filePath) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Invalid path.");
                return BadRequest();
            }
            byte[] fileData = filesHandler.ReadFile(filePath);
            if (fileData == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 404 Not Found.");
                return NotFound();
            }

            string contentType = filesHandler.GetContentType(filePath);
            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = filename,
                Inline = true,
            };
            HttpContext.Response.Headers.Add("Content-Disposition", cd.ToString());
            HttpContext.Response.Headers.Add("Content-Type", contentType);

            return File(fileData, contentType);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllItems([FromRoute] string guildid, [FromRoute] string caseid) 
        {
            IActionResult result = await this.HandleRequest(guildid, caseid, APIActionPermission.View);
            if (result != null) {
                return result;
            }
            
            var uploadDir = Path.Combine(config.Value.AbsolutePathToFileUpload , guildid, caseid);

            FileInfo[] files = filesHandler.GetFilesByDirectory(uploadDir);
            if (files == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Returning empty list.");
                return Ok(new { names = new List<string>() });
            }
            
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Returning file list.");
            return Ok(new { names = files.Select(x => x.Name).ToList() } );
        }

        [HttpPost]
        [RequestSizeLimit(10485760)]
        public async Task<IActionResult> PostItem([FromRoute] string guildid, [FromRoute] string caseid, [FromForm] UploadedFile uploadedFile)
        {
            IActionResult result = await this.HandleRequest(guildid, caseid, APIActionPermission.Edit);
            if (result != null) {
                return result;
            }

            if (uploadedFile.File == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 No file provided.");
                return BadRequest();
            }

            string uniqueFileName = await filesHandler.SaveFile(uploadedFile.File, Path.Combine(config.Value.AbsolutePathToFileUpload , guildid, caseid));

            ModCase modCase = await database.SelectSpecificModCase(guildid, caseid);
            User currentUser = await this.IsValidUser();
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Sending notification.");
            try {
                await discordAnnouncer.AnnounceFile(uniqueFileName, modCase, currentUser, RestAction.Created);
            }
            catch(Exception e){
                logger.LogError(e, "Failed to announce file.");
            }

            return StatusCode(201, new { path = uniqueFileName });
        }
    }
}