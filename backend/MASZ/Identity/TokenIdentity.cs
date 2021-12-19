using Discord;
using System.Security.Cryptography;
using System.Text;

namespace MASZ.Models
{
    public class TokenIdentity : Identity
    {
        private readonly bool isValid = false;

        public TokenIdentity(string token, IServiceProvider serviceProvider, APIToken dbToken) : base(token, serviceProvider)
        {
            if (dbToken != null)
            {
                using var hmac = new HMACSHA512(dbToken.TokenSalt);
                var generatedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(token));
                bool valid = generatedHash.Length != 0;
                for (int i = 0; i < generatedHash.Length; i++)
                {
                    if (generatedHash[i] != dbToken.TokenHash[i]) valid = false;
                }
                if (dbToken.ValidUntil <= DateTime.UtcNow)
                {
                    valid = false;
                }
                isValid = valid;
                currentUser = _discordAPI.GetCurrentBotInfo();
                currentUserGuilds = new List<UserGuild>();
            }
        }

        public override bool IsAuthorized()
        {
            return isValid;
        }

        public override bool IsOnGuild(ulong guildId)
        {
            return IsAuthorized();
        }

        public override Task<IGuildUser> GetGuildMembership(ulong guildId)
        {
            return Task.FromResult<IGuildUser>(null);
        }

        public override Task<bool> HasAdminRoleOnGuild(ulong guildId)
        {
            return Task.FromResult(IsAuthorized());
        }

        public override Task<bool> HasModRoleOrHigherOnGuild(ulong guildId)
        {
            return Task.FromResult(IsAuthorized());
        }

        public override bool IsSiteAdmin()
        {
            return IsAuthorized();
        }
    }
}
