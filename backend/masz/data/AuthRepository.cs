using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using masz.Dtos.DiscordAPIResponses;
using masz.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;

namespace masz.data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ILogger<AuthRepository> logger;
        private readonly RestClient client;
        private readonly IOptions<InternalConfig> config;
        private readonly DataContext dbContext;

        private readonly IDiscordRepository discordRepo;

        public AuthRepository(ILogger<AuthRepository> logger, IOptions<InternalConfig> config, DataContext context, IDiscordRepository discordRepo) {
            this.logger = logger;
            this.config = config;
            this.dbContext = context;
            this.discordRepo = discordRepo;
        }

        public async Task<string> GetDiscordUserToken(HttpContext context)
        {
            return await context.GetTokenAsync("access_token");
        }

        public async Task<string> GetDiscordUserId(HttpContext context)
        {
            var token = await context.GetTokenAsync("access_token");

            var user = await discordRepo.ValidateDiscordUserToken(token);

            if (user != null)
                return user.Id;
            return null;
        }

        public async Task<bool> DiscordUserIsAuthorized(HttpContext context)
        {
            var token = await context.GetTokenAsync("access_token");

            return await discordRepo.ValidateDiscordUserToken(token) != null;
        }

        public async Task<bool> DiscordUserIsOnServer(HttpContext context, string guildId)
        {
            string token = await context.GetTokenAsync("access_token");

            return await discordRepo.DiscordUserIsMemberOfGuild(guildId, token);
        }

        public async Task<bool> DiscordUserHasModRoleOrHigherOnGuild(HttpContext context, string guildId)
        {
            if (! await DiscordUserIsOnServer(context, guildId)) 
            {
                return false;
            }

            string userId = await GetDiscordUserId(context);

            // Get ModRole from database
            GuildConfig modGuild = await dbContext.GuildConfigs.FirstOrDefaultAsync(x => x.GuildId == guildId);

            if (modGuild == null)
             {
                return false;      
            }      

            // check mod role
            bool isMod = await discordRepo.DiscordUserHasRoleOnGuild(guildId, modGuild.ModRoleId, userId);
            if (isMod)
                return true;
            
            // check admin role
            bool isAdmin = await discordRepo.DiscordUserHasRoleOnGuild(guildId, modGuild.AdminRoleId, userId);
            if (isAdmin)
                return true;

            return false;
        }
    }
}