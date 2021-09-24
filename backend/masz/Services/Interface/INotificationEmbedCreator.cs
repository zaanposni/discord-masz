using System.Threading.Tasks;
using DSharpPlus.Entities;
using masz.Enums;
using masz.Models;

namespace masz.Services
{
    public interface INotificationEmbedCreator
    {
        Task<DiscordEmbedBuilder> CreateModcaseEmbed(ModCase modCase, RestAction action, DiscordUser actor, DiscordUser suspect = null, bool isInternal = true);
        Task<DiscordEmbedBuilder> CreateFileEmbed(string filename, ModCase modCase, RestAction action, DiscordUser actor);
        Task<DiscordEmbedBuilder> CreateCommentEmbed(ModCaseComment comment, RestAction action, DiscordUser actor);
        Task<DiscordEmbedBuilder> CreateUserNoteEmbed(UserNote userNote, RestAction action, DiscordUser actor, DiscordUser target);
        Task<DiscordEmbedBuilder> CreateUserMapEmbed(UserMapping userMapping, RestAction action, DiscordUser actor);
        DiscordEmbedBuilder CreateTipsEmbedForNewGuilds(GuildConfig guildConfig);
    }
}
