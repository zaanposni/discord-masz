using System.Threading.Tasks;
using DSharpPlus.Entities;
using masz.Enums;
using masz.Models;

namespace masz.Services
{
    public interface IDiscordAnnouncer
    {
        Task AnnounceTipsInNewGuild(GuildConfig guildConfig);
        Task AnnounceModCase(ModCase modCase, RestAction action, DiscordUser actor, bool announcePublic, bool announceDm);
        Task AnnounceComment(ModCaseComment comment, DiscordUser actor, RestAction action);
        Task AnnounceFile(string filename, ModCase modCase, DiscordUser actor, RestAction action);
        Task AnnounceUserNote(UserNote userNote, DiscordUser actor, RestAction action);
        Task AnnounceUserMapping(UserMapping userMapping, DiscordUser actor, RestAction action);
    }
}
