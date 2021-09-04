using System.Threading.Tasks;
using Discord;
using DSharpPlus.Entities;
using masz.Models;

namespace masz.Services
{
    public interface INotificationEmbedCreator
    {
        Task<EmbedBuilder> CreateModcaseEmbed(ModCase modCase, RestAction action, DiscordUser actor, DiscordUser suspect = null, bool isInternal = true);
        Task<EmbedBuilder> CreateFileEmbed(string filename, ModCase modCase, RestAction action, DiscordUser actor);
        Task<EmbedBuilder> CreateCommentEmbed(ModCaseComment comment, RestAction action, DiscordUser actor);
        Task<EmbedBuilder> CreateUserNoteEmbed(UserNote userNote, RestAction action, DiscordUser actor, DiscordUser target);
        Task<EmbedBuilder> CreateUserMapEmbed(UserMapping userMapping, RestAction action, DiscordUser actor);
    }
}
