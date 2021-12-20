using Discord;
using Discord.Interactions;
using MASZ.Extensions;

namespace MASZ.Commands
{

    public class AvatarCommand : BaseCommand<AvatarCommand>
    {
        [SlashCommand("avatar", "Get the high resolution avatar of a user.")]
        public async Task Avatar([Summary("user", "User to get the avatar from")] IUser user)
        {
            IGuildUser member = null;
            try
            {
                member = Context.Guild.GetUser(user.Id);
            }
            catch (Exception) { }

            EmbedBuilder embed = new();
            embed.WithTitle("Avatar");
            embed.WithFooter($"UserId: {user.Id}");

            if (member != null && member.GuildAvatarId != null)
            {
                embed.WithUrl(member.GetGuildAvatarUrl(size: 1024));
                embed.WithImageUrl(member.GetGuildAvatarUrl(size: 1024));
            }
            else
            {
                embed.WithUrl(user.GetAvatarOrDefaultUrl(size: 1024));
                embed.WithImageUrl(user.GetAvatarOrDefaultUrl(size: 1024));
            }

            await Context.Interaction.RespondAsync(embed: embed.Build(), ephemeral: true);
        }
    }
}