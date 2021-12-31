using Discord;
using Discord.Interactions;
using Discord.Net;
using MASZ.Attributes;
using MASZ.Enums;
using System.Net;

namespace MASZ.Commands
{

    public class SayCommand : BaseCommand<SayCommand>
    {
        [Require(RequireCheckEnum.GuildModerator)]
        [SlashCommand("say", "Let the bot send a message.")]
        public async Task Say([Summary("message", "message content the bot shall write")] string message, [Summary("channel", "channel to write the message in, defaults to current")] ITextChannel channel = null)
        {
            if (channel is null && Context.Channel is not ITextChannel)
            {
                await Context.Interaction.RespondAsync(Translator.T().CmdOnlyTextChannel(), ephemeral: true);
                return;
            }

            if (channel is null)
                channel = Context.Channel as ITextChannel;

            try
            {
                await channel.SendMessageAsync(message);

                await Context.Interaction.RespondAsync(Translator.T().CmdSaySent(), ephemeral: true);
            }
            catch (HttpException e)
            {
                if (e.HttpCode == HttpStatusCode.Unauthorized)
                    await Context.Interaction.RespondAsync(Translator.T().CmdCannotViewOrDeleteInChannel(), ephemeral: true);
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"Error while writing message in channel {channel.Id}");
                await Context.Interaction.RespondAsync(Translator.T().CmdSayFailed(), ephemeral: true);
            }
        }
    }
}