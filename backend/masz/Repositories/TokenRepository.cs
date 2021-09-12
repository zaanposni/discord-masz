using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using masz.Dtos.Tokens;
using masz.Events;
using masz.Exceptions;
using masz.Models;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace masz.Repositories
{

    public class TokenRepository : BaseRepository<TokenRepository>
    {
        private TokenRepository(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public static TokenRepository CreateDefault(IServiceProvider serviceProvider) => new TokenRepository(serviceProvider);
        public async Task<APIToken> GetToken()
        {
            APIToken apiToken = await _database.GetAPIToken();
            if (apiToken == null)
            {
                _logger.LogWarning($"Token not found.");
                throw new ResourceNotFoundException($"Token does not exist.");
            }
            return apiToken;
        }

        public async Task<List<APIToken>> GetAllGuildConfig()
        {
            return await _database.GetAllAPIToken();
        }

        public async Task<CreatedTokenDto> RegisterToken(string name)
        {
            string token = generateToken(name);

            APIToken apiToken = new APIToken();
            using var hmac = new HMACSHA512();
            apiToken.TokenHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(token));
            apiToken.TokenSalt = hmac.Key;
            apiToken.CreatedAt = DateTime.UtcNow;
            apiToken.ValidUntil = DateTime.UtcNow.AddYears(1);
            apiToken.Name = name;

            await _database.SaveToken(apiToken);
            await _database.SaveChangesAsync();

            await _eventHandler.InvokeTokenCreated(new TokenCreatedEventArgs(apiToken));

            return new CreatedTokenDto() {
                Token = token,
                Id = apiToken.Id
            };
        }

        public async Task DeleteToken()
        {
            APIToken apiToken = await GetToken();
            _database.DeleteToken(apiToken);
            await _database.SaveChangesAsync();
            await _eventHandler.InvokeTokenDeleted(new TokenDeletedEventArgs(apiToken));
        }

        private string generateToken(string name) {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, name)
            };

            var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetBotToken())), SecurityAlgorithms.HmacSha512Signature);

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