using masz.Dtos.DiscordAPIResponses;
using masz.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace masz.Models
{
    public class TokenIdentity : Identity
    {
        private bool isValid = false;
        public TokenIdentity (string token, IDiscordAPIInterface discord, APIToken dbToken) : base(token, discord)
        {
            if (dbToken != null) {
                using var hmac = new HMACSHA512(dbToken.TokenSalt);
                var generatedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(token));
                bool valid = generatedHash.Length != 0;
                for (int i = 0; i < generatedHash.Length; i++) 
                {
                    if (generatedHash[i] != dbToken.TokenHash[i]) valid = false;
                }
                if (dbToken.ValidUntil <= DateTime.UtcNow) {
                    valid = false;
                }
                this.isValid = valid;
            }
        }

        public override async Task<bool> IsAuthorized() 
        {
            return this.isValid;
        }

        public override async Task<User> GetCurrentDiscordUser()
        {
            if (await this.IsAuthorized()) {
                return await discord.FetchCurrentBotInfo(CacheBehavior.Default);
            }
            return null;
        }

        public override async Task<List<Guild>> GetCurrentGuilds()
        {
            return new List<Guild>();
        }

        public override async Task<bool> IsOnGuild(string guildId)
        {
            return await this.IsAuthorized();
        }

        public override async Task<GuildMember> GetGuildMembership(string guildId)
        {
            return null;
        }

        public override async Task<bool> HasAdminRoleOnGuild(string guildId, IDatabase database)
        {
            return await this.IsAuthorized();
        }

        public override async Task<bool> HasModRoleOrHigherOnGuild(string guildId, IDatabase database)
        {
            return await this.IsAuthorized();
        }
    }
}
