using Discord;
using Discord.Interactions;

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
                embed.WithUrl(member.GetGuildAvatarUrl());
                embed.WithImageUrl(member.GetGuildAvatarUrl());
            }
            else
            {
                embed.WithUrl(user.GetAvatarUrl());
                embed.WithImageUrl(user.GetAvatarUrl());
            }

            await Context.Interaction.RespondAsync(embed: embed.Build(), ephemeral: true);
        }
    }
}