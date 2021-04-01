using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using masz.Models;
using masz.Dtos.Tokens;
using masz.Services;

namespace masz.Controllers
{
    [ApiController]
    [Route("api/v1/token")]
    [Authorize]
    public class TokenController : SimpleController
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<TokenController> logger;
        private readonly IIdentityManager identityManager;

        public TokenController(IServiceProvider serviceProvider, ILogger<TokenController> logger, IIdentityManager identityManager) : base(serviceProvider) {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
            this.identityManager = identityManager;
        }

        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] TokenForCreateDto tokenDto)
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            if (! await this.IsSiteAdmin()) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            if (await this.GetIdentity() is TokenIdentity) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Tokens cannot manage tokens.");
                return Unauthorized("Tokens cannot manage tokens");
            }

            if (await this.database.GetAPIToken() != null) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Bad Request.");
                return BadRequest("There already is a token.");
            }

            this.identityManager.ClearTokenIdentities();

            var token = this.generateToken(tokenDto.Name);
            APIToken apiToken = new APIToken();
            using var hmac = new HMACSHA512();
            apiToken.TokenHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(token));
            apiToken.TokenSalt = hmac.Key;
            apiToken.CreatedAt = DateTime.UtcNow;
            apiToken.ValidUntil = DateTime.UtcNow.AddYears(1);
            apiToken.Name = tokenDto.Name;

            await this.database.SaveToken(apiToken);
            await this.database.SaveChangesAsync();

            return Ok(new { id = apiToken.Id, generatedToken = token });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteToken()
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            if (! await this.IsSiteAdmin()) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            if (await this.GetIdentity() is TokenIdentity) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Tokens cannot manage tokens.");
                return Unauthorized("Tokens cannot manage tokens");
            }

            APIToken apiToken = await this.database.GetAPIToken();
            if (apiToken == null) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 404 Not found.");
                return NotFound();
            }

            this.database.DeleteToken(apiToken);
            await this.database.SaveChangesAsync();
            
            this.identityManager.ClearTokenIdentities();

            return Ok(new { id = apiToken.Id });
        }

        [HttpGet]
        public async Task<IActionResult> GetTokenInfo()
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            if (! await this.IsSiteAdmin()) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            if (await this.GetIdentity() is TokenIdentity) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Tokens cannot manage tokens.");
                return Unauthorized("Tokens cannot manage tokens");
            }

            APIToken apiToken = await this.database.GetAPIToken();
            if (apiToken == null) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 404 Not found.");
                return NotFound();
            }

            apiToken.TokenHash = null;
            apiToken.TokenSalt = null;

            return Ok(apiToken);
        }

        private string generateToken(string name) {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, name)
            };

            var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.config.Value.DiscordBotToken)), SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddYears(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}