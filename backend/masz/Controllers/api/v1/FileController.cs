using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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

        public FileController(ILogger<FileController> logger, IDatabase database, IOptions<InternalConfig> config, IIdentityManager identityManager, IFilesHandler filesHandler)
        {
            this.logger = logger;
            this.database = database;
            this.config = config;
            this.identityManager = identityManager;
            this.filesHandler = filesHandler;
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
            if (!await currentIdentity.HasModRoleOrHigherOnGuild(guildid) && !config.Value.SiteAdminDiscordUserIds.Contains(currentUser.Id))
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            // ========================================================

            var filePath = Path.Combine(config.Value.AbsolutePathToFileUpload, guildid, caseid, filename);
            filesHandler.DeleteFile(filePath);

            return Ok();
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
            if (!await currentIdentity.HasModRoleOrHigherOnGuild(guildid) && !config.Value.SiteAdminDiscordUserIds.Contains(currentUser.Id))
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

            var filePath = Path.Combine(config.Value.AbsolutePathToFileUpload, guildid, caseid, filename);
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
            if (!await currentIdentity.HasModRoleOrHigherOnGuild(guildid) && !config.Value.SiteAdminDiscordUserIds.Contains(currentUser.Id))
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
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 404 Not Found.");
                return NotFound();
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
            if (!await currentIdentity.HasModRoleOrHigherOnGuild(guildid) && !config.Value.SiteAdminDiscordUserIds.Contains(currentUser.Id))
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

            if (await database.SelectSpecificModCase(guildid, caseid) == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Modcase not found.");
                return BadRequest("Modcase not found.");
            }

            string uniqueFileName = await filesHandler.SaveFile(uploadedFile.File, Path.Combine(config.Value.AbsolutePathToFileUpload , guildid, caseid));

            return StatusCode(201, new { path = uniqueFileName });
        }
    }
}