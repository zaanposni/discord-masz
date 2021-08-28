using System.Threading.Tasks;
using Discord;
using masz.Dtos.DiscordAPIResponses;
using masz.Models;

namespace masz.Services
{
    public interface INotificationEmbedCreator
    {
        Task<EmbedBuilder> CreateModcaseEmbed(ModCase modCase, RestAction action, User actor, User suspect = null, bool isInternal = true);
        Task<EmbedBuilder> CreateFileEmbed(string filename, ModCase modCase, RestAction action, User actor);
        Task<EmbedBuilder> CreateCommentEmbed(ModCaseComment comment, RestAction action, User actor);
        Task<EmbedBuilder> CreateUserNoteEmbed(UserNote userNote, RestAction action, User actor, User target);
        Task<EmbedBuilder> CreateUserMapEmbed(UserMapping userMapping, RestAction action, User actor);
    }
}
