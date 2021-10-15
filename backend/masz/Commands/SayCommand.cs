using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using masz.Enums;
using masz.Models;
using masz.Repositories;
using Microsoft.Extensions.Logging;

namespace masz.Commands
{

    public class SayCommand : BaseCommand<SayCommand>
    {
        public SayCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }

        [SlashCommand("say", "Let the bot send a message.")]
        public async Task Say(InteractionContext ctx,  [Option("message", "message content the bot shall write")] string message, [Option("channel", "channel to write the message in, defaults to current")] DiscordChannel channel = null)
        {
            await Require(ctx, RequireCheckEnum.GuildModerator);

            if (channel == null)
            {
                channel = ctx.Channel;
            }

            if (channel.Type != ChannelType.Text)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent(_translator.T().CmdOnlyTextChannel()).AsEphemeral(true));
                return;
            }

            try
            {
                await channel.SendMessageAsync(message);
            } catch (DSharpPlus.Exceptions.UnauthorizedException)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent(_translator.T().CmdCannotViewOrDeleteInChannel()).AsEphemeral(true));
                return;
            } catch (Exception e)
            {
                _logger.LogError(e, $"Error while writing message in channel {channel.Id}");
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent(_translator.T().CmdSayFailed()).AsEphemeral(true));
                return;
            }

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(_translator.T().CmdSaySent()).AsEphemeral(true));
        }
    }
}