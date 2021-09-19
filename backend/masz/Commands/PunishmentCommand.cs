using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using masz.Enums;
using masz.Models;
using masz.Repositories;

namespace masz.Commands
{

    public class PunishmentCommand : BaseCommand<PunishmentCommand>
    {
        public PunishmentCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }

        [SlashCommand("punish", "Punish a user and create a modcase")]
        public async Task Punish(
            InteractionContext ctx,
            [Option("title", "The title of the modcase")] string title,
            [Option("user", "User to punish")] DiscordUser user,
            [Option("punishment", "Choose a punishment")]PunishmentType punishmentType,
                [Choice("None", 0)]
                [Choice("1 Hour", 1)]
                [Choice("1 Day", 24)]
                [Choice("1 Week", 168)]
                [Choice("1 Month", 672)]
            [Option("hours", "Number of days of message history to delete")] long punishedForHours = 0,
            [Option("description", "The description of the modcase")] string description = "",
            [Option("dm-notification", "Whether to send a dm notification")] bool sendDmNotification = true,
            [Option("public-notification", "Whether to send a public webhook notification")] bool sendPublicNotification = true,
            [Option("execute-punishment", "Whether to execute the punishment or just register it.")] bool executePunishment = true)
        {
            await Require(ctx, RequireCheckEnum.GuildModerator, RequireCheckEnum.GuildRegistered);

            ModCase modCase = new ModCase();
            modCase.Title = title;
            modCase.GuildId = ctx.Guild.Id;
            modCase.UserId = user.Id;
            modCase.ModId = _currentIdentity.GetCurrentUser().Id;
            if (String.IsNullOrEmpty(description))
            {
                modCase.Description = title;
            } else
            {
                modCase.Description = description;
            }
            modCase.PunishmentType = punishmentType;
            modCase.PunishmentActive = executePunishment;
            modCase.PunishedUntil = DateTime.UtcNow.AddHours(punishedForHours);
            if (punishmentType == PunishmentType.None || punishmentType == PunishmentType.Kick)
            {
                modCase.PunishedUntil = null;  // remove duration for warn and kick
            }
            modCase.CreationType = CaseCreationType.ByCommand;

            ModCase created = await ModCaseRepository.CreateDefault(_serviceProvider, _currentIdentity).CreateModCase(modCase, executePunishment, sendPublicNotification, sendDmNotification);

            DiscordInteractionResponseBuilder response =  new DiscordInteractionResponseBuilder();
            response.IsEphemeral = !sendPublicNotification;

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, response.WithContent($"Case `#{created.CaseId}` created: {_config.GetBaseUrl()}/guilds/{created.GuildId}/cases/{created.CaseId}"));
        }
    }
}