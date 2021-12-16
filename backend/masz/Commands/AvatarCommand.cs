using Discord;
using Discord.Interactions;

namespace MASZ.Commands
{

    public class AvatarCommand : BaseCommand<AvatarCommand>
    {
        public AvatarCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }

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
            if (member != null && member.GuildAvatarId != user.AvatarId)
            {
                embed.WithUrl(member.GuildAvatarId);
                embed.WithImageUrl(member.GuildAvatarId);
            }
            else
            {
                embed.WithUrl(user.AvatarId);
                embed.WithImageUrl(user.AvatarId);
            }

            await Context.Interaction.RespondAsync(embed: embed.Build(), ephemeral: true);
        }
    }
}