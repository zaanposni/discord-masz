using Discord;
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Extensions;
using MASZ.Models;

namespace MASZ.Repositories
{

    public class GuildMotdRepository : BaseRepository<GuildMotdRepository>
    {
        private readonly IUser _currentUser;
        private GuildMotdRepository(IServiceProvider serviceProvider, IUser currentUser) : base(serviceProvider)
        {
            _currentUser = currentUser;
        }
        private GuildMotdRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _currentUser = DiscordAPI.GetCurrentBotInfo();
        }
        public static GuildMotdRepository CreateDefault(IServiceProvider serviceProvider, Identity identity) => new(serviceProvider, identity.GetCurrentUser());
        public static GuildMotdRepository CreateWithBotIdentity(IServiceProvider serviceProvider) => new(serviceProvider);
        public async Task<GuildMotd> GetMotd(ulong guildId)
        {
            GuildMotd motd = await Database.GetMotdForGuild(guildId);
            if (motd == null)
            {
                throw new ResourceNotFoundException();
            }
            return motd;
        }
        public async Task<GuildMotd> CreateOrUpdateMotd(ulong guildId, string content, bool visible)
        {
            RestAction action = RestAction.Updated;
            GuildMotd motd;
            try
            {
                motd = await GetMotd(guildId);
            }
            catch (ResourceNotFoundException)
            {
                motd = new GuildMotd
                {
                    GuildId = guildId
                };
                action = RestAction.Created;
            }
            motd.CreatedAt = DateTime.UtcNow;
            motd.UserId = _currentUser.Id;

            motd.Message = content;
            motd.ShowMotd = visible;

            Database.SaveMotd(motd);
            await Database.SaveChangesAsync();

            if (action == RestAction.Created)
            {
                _eventHandler.OnGuildMotdCreatedEvent.InvokeAsync(motd, _currentUser);
            }
            else
            {
                _eventHandler.OnGuildMotdUpdatedEvent.InvokeAsync(motd, _currentUser);
            }

            return motd;
        }
        public async Task DeleteForGuild(ulong guildId)
        {
            await Database.DeleteMotdForGuild(guildId);
            await Database.SaveChangesAsync();
        }
    }
}