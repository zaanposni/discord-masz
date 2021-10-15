using System;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using masz.Events;
using masz.Exceptions;
using masz.Models;
using masz.Enums;

namespace masz.Repositories
{

    public class GuildMotdRepository : BaseRepository<GuildMotdRepository>
    {
        private readonly DiscordUser _currentUser;
        private GuildMotdRepository(IServiceProvider serviceProvider, DiscordUser currentUser) : base(serviceProvider)
        {
            _currentUser = currentUser;
        }
        private GuildMotdRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _currentUser = _discordAPI.GetCurrentBotInfo(CacheBehavior.Default);
        }
        public static GuildMotdRepository CreateDefault(IServiceProvider serviceProvider, Identity identity) => new GuildMotdRepository(serviceProvider, identity.GetCurrentUser());
        public static GuildMotdRepository CreateWithBotIdentity(IServiceProvider serviceProvider) => new GuildMotdRepository(serviceProvider);
        public async Task<GuildMotd> GetMotd(ulong guildId)
        {
            GuildMotd motd = await _database.GetMotdForGuild(guildId);
            if (motd == null)
            {
                throw new ResourceNotFoundException();
            }
            return motd;
        }
        public async Task<GuildMotd> CreateOrUpdateMotd(ulong guildId, string content, bool visible)
        {
            GuildMotd motd;
            try {
                motd = await GetMotd(guildId);
            } catch (ResourceNotFoundException) {
                motd = new GuildMotd();
                motd.GuildId = guildId;
            }
            motd.CreatedAt = DateTime.UtcNow;
            motd.UserId = _currentUser.Id;

            motd.Message = content;
            motd.ShowMotd = visible;

            _database.SaveMotd(motd);
            await _database.SaveChangesAsync();

            await _eventHandler.InvokeGuildMotdUpdated(new GuildMotdUpdatedEventArgs(motd));

            return motd;
        }
        public async Task DeleteForGuild(ulong guildId)
        {
            await _database.DeleteMotdForGuild(guildId);
            await _database.SaveChangesAsync();
        }
    }
}