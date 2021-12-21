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

            EmbedBuilder embed = new EmbedBuilder()
                .WithTitle(Translator.T().CmdGetAvatarURL())
                .WithFooter($"{Translator.T().CmdUserID()}: {user.Id}")
                .WithColor(Color.Magenta)
                .WithCurrentTimestamp();

            if (member != null && member.GuildAvatarId != null)
            {
                embed.WithUrl(member.GetGuildAvatarUrl(size: 1024))
                    .WithImageUrl(member.GetGuildAvatarUrl(size: 1024))
                    .WithAuthor(member);
            }
            else
            {
                embed.WithUrl(user.GetAvatarOrDefaultUrl(size: 1024))
                    .WithImageUrl(user.GetAvatarOrDefaultUrl(size: 1024))
                    .WithAuthor(user);
            }

            await Context.Interaction.RespondAsync(embed: embed.Build(), ephemeral: true);
        }
    }
}