using Discord;
using MASZ.Enums;
using MASZ.Models;

namespace MASZ.Services
{
    public interface INotificationEmbedCreator
    {
        Task<EmbedBuilder> CreateModcaseEmbed(ModCase modCase, RestAction action, IUser actor, IUser suspect = null, bool isInternal = true);
        Task<EmbedBuilder> CreateFileEmbed(string filename, ModCase modCase, RestAction action, IUser actor);
        Task<EmbedBuilder> CreateCommentEmbed(ModCaseComment comment, RestAction action, IUser actor);
        Task<EmbedBuilder> CreateUserNoteEmbed(UserNote userNote, RestAction action, IUser actor, IUser target);
        Task<EmbedBuilder> CreateUserMapEmbed(UserMapping userMapping, RestAction action, IUser actor);
        EmbedBuilder CreateTipsEmbedForNewGuilds(GuildConfig guildConfig);
        EmbedBuilder CreateInternalAutomodEmbed(AutoModerationEvent autoModerationEvent, GuildConfig guildConfig, IUser user, ITextChannel channel, PunishmentType? punishmentType = null);
    }
}
