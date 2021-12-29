using MASZ.Dtos.Tokens;
using MASZ.Exceptions;
using MASZ.Extensions;
using MASZ.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MASZ.Repositories
{

    public class TokenRepository : BaseRepository<TokenRepository>
    {
        private TokenRepository(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public static TokenRepository CreateDefault(IServiceProvider serviceProvider) => new(serviceProvider);
        public async Task<APIToken> GetToken()
        {
            APIToken apiToken = await Database.GetAPIToken();
            if (apiToken == null)
            {
                throw new ResourceNotFoundException($"Token does not exist.");
            }
            return apiToken;
        }

        public async Task<List<APIToken>> GetAllTokens()
        {
            return await Database.GetAllAPIToken();
        }

        public async Task<int> CountTokens()
        {
            return await Database.CountAllAPITokens();
        }

        public async Task<CreatedTokenDto> RegisterToken(string name)
        {
            string token = GenerateToken(name);

            APIToken apiToken = new();
            using var hmac = new HMACSHA512();
            apiToken.TokenHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(token));
            apiToken.TokenSalt = hmac.Key;
            apiToken.CreatedAt = DateTime.UtcNow;
            apiToken.ValidUntil = DateTime.UtcNow.AddYears(1);
            apiToken.Name = name;

            await Database.SaveToken(apiToken);
            await Database.SaveChangesAsync();

            _eventHandler.OnTokenCreatedEvent.InvokeAsync(apiToken);

            return new CreatedTokenDto()
            {
                Token = token,
                Id = apiToken.Id
            };
        }

        public async Task DeleteToken()
        {
            APIToken apiToken = await GetToken();
            Database.DeleteToken(apiToken);
            await Database.SaveChangesAsync();
            _eventHandler.OnTokenDeletedEvent.InvokeAsync(apiToken);
        }

        private string GenerateToken(string name)
        {
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