using Discord;
using Discord.Interactions;
using Discord.Net;
using MASZ.Attributes;
using MASZ.Enums;
using MASZ.Models;
using MASZ.Repositories;
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
                IUserMessage createdMessage = await channel.SendMessageAsync(message);

                await Context.Interaction.RespondAsync(Translator.T().CmdSaySent(), ephemeral: true);

                try
                {
                    GuildConfig guildConfig = await GuildConfigRepository.CreateDefault(ServiceProvider).GetGuildConfig(Context.Guild.Id);
                    if (! string.IsNullOrEmpty(guildConfig.ModInternalNotificationWebhook))
                    {
                        await DiscordAPI.ExecuteWebhook(
                            guildConfig.ModInternalNotificationWebhook,
                            null,
                            Translator.T().CmdSaySentMod(
                                Context.User,
                                createdMessage,
                                channel
                            ),
                            AllowedMentions.None
                        );
                    }
                } catch (Exception ex)
                {
                    Logger.LogError(ex, $"Something went wrong while sending the internal notification for the say command by {Context.User.Id} in {Context.Guild.Id}/{Context.Channel.Id}.");
                }
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