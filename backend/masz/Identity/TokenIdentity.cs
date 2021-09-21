using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using masz.Enums;

namespace masz.Models
{
    public class TokenIdentity : Identity
    {
        private bool isValid = false;
        public TokenIdentity (string token, IServiceProvider serviceProvider, APIToken dbToken, IServiceScopeFactory serviceScopeFactory) : base(token, serviceProvider, serviceScopeFactory)
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
                isValid = valid;
                currentUser = _discordAPI.GetCurrentBotInfo(CacheBehavior.Default);
                currentUserGuilds = new List<DiscordGuild>();
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

        public override Task<DiscordMember> GetGuildMembership(ulong guildId)
        {
            return Task.FromResult<DiscordMember>(null);
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
