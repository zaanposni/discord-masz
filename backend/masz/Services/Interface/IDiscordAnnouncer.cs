using Discord;
using MASZ.Enums;
using MASZ.Models;

namespace MASZ.Services
{
    public interface IDiscordAnnouncer
    {
        Task AnnounceTipsInNewGuild(GuildConfig guildConfig);
        Task AnnounceModCase(ModCase modCase, RestAction action, IUser actor, bool announcePublic, bool announceDm);
        Task AnnounceComment(ModCaseComment comment, IUser actor, RestAction action);
        Task AnnounceFile(string filename, ModCase modCase, IUser actor, RestAction action);
        Task AnnounceUserNote(UserNote userNote, IUser actor, RestAction action);
        Task AnnounceUserMapping(UserMapping userMapping, IUser actor, RestAction action);
        Task AnnounceAutomoderation(AutoModerationEvent modEvent, AutoModerationConfig moderationConfig, GuildConfig guildConfig, ITextChannel channel, IUser author);
    }
}
