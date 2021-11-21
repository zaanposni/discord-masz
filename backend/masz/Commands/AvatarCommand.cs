using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace masz.Commands
{

    public class AvatarCommand : BaseCommand<AvatarCommand>
    {
        public AvatarCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }

        [SlashCommand("avatar", "Get the high resolution avatar of a user.")]
        public async Task Avatar(InteractionContext ctx, [Option("user", "User to get the avatar from")] DiscordUser user)
        {
            DiscordMember member = null;
            try
            {
                member = await ctx.Guild.GetMemberAsync(user.Id);
            } catch (Exception) { }

            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
            embed.WithTitle("Avatar");
            embed.WithFooter($"UserId: {user.Id}");
            if (member != null && member.AvatarHash != user.AvatarHash)
            {
                embed.WithUrl(member.GuildAvatarUrl);
                embed.WithImageUrl(member.GuildAvatarUrl);
            } else
            {
                embed.WithUrl(user.AvatarUrl);
                embed.WithImageUrl(user.AvatarUrl);
            }

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(embed.Build()).AsEphemeral(true));
        }
    }
}