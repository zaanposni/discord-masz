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
    public class FileController : ControllerBase
    {
        private readonly ILogger<FileController> logger;
        private readonly IDatabase database;
        private readonly IOptions<InternalConfig> config;
        private readonly IIdentityManager identityManager;
        private readonly IFilesHandler filesHandler;
        private readonly IDiscordAnnouncer discordAnnouncer;

        public FileController(ILogger<FileController> logger, IDatabase database, IOptions<InternalConfig> config, IIdentityManager identityManager, IFilesHandler filesHandler, IDiscordAnnouncer discordAnnouncer)
        {
            this.logger = logger;
            this.database = database;
            this.config = config;
            this.identityManager = identityManager;
            this.filesHandler = filesHandler;
            this.discordAnnouncer = discordAnnouncer;
        }

        [HttpDelete("{filename}")]
        public async Task<IActionResult> DeleteSpecificItem([FromRoute] string guildid, [FromRoute] string caseid, [FromRoute] string filename) 
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

            ModCase modCase = await database.SelectSpecificModCase(guildid, caseid);
            if (modCase == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Modcase not found.");
                return BadRequest("Modcase not found.");
            }

            if (modCase.MarkedToDeleteAt != null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Case is marked to be deleted.");
                return BadRequest("Case is marked to be deleted.");
            }

            string filePath = Path.Combine(config.Value.AbsolutePathToFileUpload, guildid, caseid, filename);
            if (! filesHandler.FileExists(filePath)) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 404 File not found.");
                return NotFound("File not found.");
            }

            filesHandler.DeleteFile(filePath);

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Sending notification.");
            try {
                await discordAnnouncer.AnnounceFile(filename, modCase, currentUser, RestAction.Deleted);
            }
            catch(Exception e){
                logger.LogError(e, "Failed to announce file.");
            }

            return Ok();
        }

        private string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                if ((str[i] >= '0' && str[i] <= '9')
                    || (str[i] >= 'A' && str[i] <= 'z'
                        || (str[i] == '.' || str[i] == '_')))
                    {
                        sb.Append(str[i]);
                    }
            }

            return sb.ToString();
        }

        [HttpGet("{filename}")]
        public async Task<IActionResult> GetSpecificItem([FromRoute] string guildid, [FromRoute] string caseid, [FromRoute] string filename) 
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            Identity currentIdentity = await identityManager.GetIdentity(HttpContext);
            User currentUser = await currentIdentity.GetCurrentDiscordUser();
            if (currentUser == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            ModCase modCase = await database.SelectSpecificModCase(guildid, caseid);
            if (!await currentIdentity.HasModRoleOrHigherOnGuild(guildid, this.database) && !config.Value.SiteAdminDiscordUserIds.Contains(currentUser.Id))
            {
                if (modCase == null) {
                    logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                    return Unauthorized();                    
                } else {
                    if (modCase.UserId != currentUser.Id) {
                        logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                        return Unauthorized();
                    }
                }
            }
            // ========================================================

            var filePath = Path.Combine(config.Value.AbsolutePathToFileUpload, guildid, caseid, RemoveSpecialCharacters(filename));
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
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            Identity currentIdentity = await identityManager.GetIdentity(HttpContext);
            User currentUser = await currentIdentity.GetCurrentDiscordUser();
            if (currentUser == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            ModCase modCase = await database.SelectSpecificModCase(guildid, caseid);
            if (!await currentIdentity.HasModRoleOrHigherOnGuild(guildid, this.database) && !config.Value.SiteAdminDiscordUserIds.Contains(currentUser.Id))
            {
                if (modCase == null) {
                    logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                    return Unauthorized();                    
                } else {
                    if (modCase.UserId != currentUser.Id) {
                        logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                        return Unauthorized();
                    }
                }
            }
            // ========================================================
            
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

            if (uploadedFile.File == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 No file provided.");
                return BadRequest();
            }

            ModCase modCase = await database.SelectSpecificModCase(guildid, caseid);
            if (modCase == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Modcase not found.");
                return BadRequest("Modcase not found.");
            }

            if (modCase.MarkedToDeleteAt != null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Case is marked to be deleted.");
                return BadRequest("Case is marked to be deleted.");
            }

            string uniqueFileName = await filesHandler.SaveFile(uploadedFile.File, Path.Combine(config.Value.AbsolutePathToFileUpload , guildid, caseid));

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