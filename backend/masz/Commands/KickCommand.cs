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

    public class KickCommand : BaseCommand<KickCommand>
    {
        public KickCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }

        [SlashCommand("kick", "Kick a user and create a modcase")]
        public async Task Kick(
            InteractionContext ctx,
            [Option("title", "The title of the modcase")] string title,
            [Option("user", "User to punish")] DiscordUser user,
            [Option("description", "The description of the modcase")] string description = "",
            [Option("dm-notification", "Whether to send a dm notification")] bool sendDmNotification = true,
            [Option("public-notification", "Whether to send a public webhook notification")] bool sendPublicNotification = true,
            [Option("execute-punishment", "Whether to execute the punishment or just register it.")] bool executePunishment = true)
        {
            await Require(ctx, RequireCheckEnum.GuildModerator, RequireCheckEnum.GuildRegistered, RequireCheckEnum.GuildStrictModeKick);

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
            modCase.PunishmentType = PunishmentType.Kick;
            modCase.PunishmentActive = executePunishment;
            modCase.PunishedUntil = null;
            modCase.CreationType = CaseCreationType.ByCommand;

            ModCase created = await ModCaseRepository.CreateDefault(_serviceProvider, _currentIdentity).CreateModCase(modCase, executePunishment, sendPublicNotification, sendDmNotification);

            DiscordInteractionResponseBuilder response =  new DiscordInteractionResponseBuilder();
            response.IsEphemeral = !sendPublicNotification;

            string url = $"{_config.GetBaseUrl()}/guilds/{created.GuildId}/cases/{created.CaseId}";
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, response.WithContent(_translator.T().CmdPunish(created.CaseId, url)));
        }
    }
}