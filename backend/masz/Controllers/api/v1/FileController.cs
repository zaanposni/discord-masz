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
    [Route("api/v1/files/{guildid}/{modcaseid}")]
    [Authorize]
    public class FileController : ControllerBase
    {
        private readonly ILogger<FileController> logger;
        private readonly IDatabase database;
        private readonly IOptions<InternalConfig> config;
        private readonly IIdentityManager identityManager;

        public FileController(ILogger<FileController> logger, IDatabase database, IOptions<InternalConfig> config, IIdentityManager identityManager)
        {
            this.logger = logger;
            this.database = database;
            this.config = config;
            this.identityManager = identityManager;
        }

        [HttpGet("{filename}")]
        public async Task<IActionResult> GetSpecificItem([FromRoute] string guildid, [FromRoute] string modcaseid, [FromRoute] string filename) 
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

            var uploadDir = Path.Combine(config.Value.AbsolutePathToFileUpload, guildid, modcaseid);
            var filePath = Path.Combine(uploadDir, filename);
            if (!System.IO.File.Exists(filePath))
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 404 Not Found.");
                return NotFound();
            }

            byte[] filedata = System.IO.File.ReadAllBytes(filePath);
            string contentType;
            new FileExtensionContentTypeProvider().TryGetContentType(filePath, out contentType);
            contentType = contentType ?? "application/octet-stream";
            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = filename,
                Inline = true,
            };
            HttpContext.Response.Headers.Add("Content-Disposition", cd.ToString());
            HttpContext.Response.Headers.Add("Content-Type", contentType);

            return File(filedata, contentType);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllItems([FromRoute] string guildid, [FromRoute] string modcaseid) 
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
            
            var uploadDir = Path.Combine(config.Value.AbsolutePathToFileUpload , guildid, modcaseid);
            if (!System.IO.Directory.Exists(uploadDir))
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 404 Not Found.");
                return NotFound();
            }
            FileInfo[] files = new DirectoryInfo(uploadDir).GetFiles();
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Returning file list.");
            return Ok(new { names = files.Select(x => x.Name).ToList() } );
        }

        [HttpPost]
        [RequestSizeLimit(10485760)]
        public async Task<IActionResult> PostItem([FromRoute] string guildid, [FromRoute] string modcaseid, [FromForm] UploadedFile uploadedFile)
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

            if (await database.SelectSpecificModCase(guildid, modcaseid) == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Modcase not found.");
                return BadRequest("Modcase not found.");
            }

            var uniqueFileName = GetUniqueFileName(uploadedFile.File);
            var uploadDir = Path.Combine(config.Value.AbsolutePathToFileUpload , guildid, modcaseid);
            System.IO.Directory.CreateDirectory(uploadDir);
            var filePath = Path.Combine(uploadDir, uniqueFileName);
            await uploadedFile.File.CopyToAsync(new FileStream(filePath, FileMode.Create));

            return StatusCode(201, new { path = $"/{guildid}/{modcaseid}/{uniqueFileName}" });
        }

        private string GetUniqueFileName(IFormFile file)
        {
            // TODO: change to hasing algorithm
            string fileName = Path.GetFileName(file.FileName);
            return  GetSHA1Hash(file)
                    + "_"
                    + Guid.NewGuid().ToString().Substring(0, 8)
                    + "_"
                    + Path.GetFileNameWithoutExtension(fileName)
                    + Path.GetExtension(fileName);
        }

        private string GetSHA1Hash(IFormFile file)
        {
            // get stream from file then convert it to a MemoryStream
            MemoryStream stream = new MemoryStream();
            file.OpenReadStream().CopyTo(stream);
            // compute md5 hash of the file's byte array.
            byte[] bytes = SHA1.Create().ComputeHash(stream.ToArray());
            stream.Close();
            return BitConverter.ToString(bytes).Replace("-",string.Empty).ToLower();
        }
    }
}