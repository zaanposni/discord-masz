using Discord;
using Discord.Interactions;
using MASZ.Attributes;
using MASZ.Enums;
using MASZ.Extensions;
using MASZ.Models;
using MASZ.Repositories;
using System.Text;

namespace MASZ.Commands
{

    public class StatusCOmmand : BaseCommand<AvatarCommand>
    {
        private readonly string CHECK = "\u2705";
        private readonly string X_CHECK = "\u274C";

        private string GetCheckEmote(bool value)
        {
            return value ? CHECK : X_CHECK;
        }

        [Require(RequireCheckEnum.SiteAdmin)]
        [SlashCommand("status", "See the current status of your application.")]
        public async Task Status()
        {
            await Context.Interaction.DeferAsync(ephemeral: true);

            EmbedBuilder embed = new EmbedBuilder()
                .WithTitle(Translator.T().CmdStatusTitle())
                .WithColor(Color.Green)
                .WithCurrentTimestamp();

            StatusRepository repo = StatusRepository.CreateDefault(ServiceProvider);

            StatusDetail botDetails = repo.GetBotStatus();
            StatusDetail dbDetails = await repo.GetDbStatus();
            StatusDetail cacheDetails = repo.GetCacheStatus();

            string lastDisconnect = string.Empty;
            if (botDetails.LastDisconnect != null)
            {
                lastDisconnect = Translator.T().CmdStatusLastDisconnectAt(botDetails.LastDisconnect.Value.ToDiscordTS());
            }

            embed.AddField(
                GetCheckEmote(botDetails.Online) + " " + Translator.T().CmdStatusBot(),
                $"{botDetails.ResponseTime:0.0}ms\n{lastDisconnect}",
                false
            );
            embed.AddField(
                GetCheckEmote(dbDetails.Online) + " " + Translator.T().CmdStatusDatabase(),
                $"{dbDetails.ResponseTime:0.0}ms",
                false
            );
            embed.AddField(
                GetCheckEmote(cacheDetails.Online) + " " + Translator.T().CmdStatusInternalCache(),
                $"{cacheDetails.ResponseTime:0.0}ms",
                false
            );

            if (!(botDetails.Online && dbDetails.Online && cacheDetails.Online))
            {
                embed.WithColor(Color.Red);
            }

            StringBuilder loggedInString = new();
            int loggedInCount = 0;
            foreach (var item in IdentityManager.GetCurrentIdentities().Where(x => x is DiscordOAuthIdentity))
            {
                var user = item.GetCurrentUser();
                if (user != null)
                {
                    loggedInString.AppendLine($"{user.Username}#{user.Discriminator}");
                    loggedInCount++;
                }
            }
            if (loggedInCount != 0)
            {
                embed.AddField(
                    $"{Translator.T().CmdStatusCurrentlyLoggedIn()} [{loggedInCount}]",
                    loggedInString.ToString().Truncate(1024),
                    false
                );
            }

            await Context.Interaction.ModifyOriginalResponseAsync((msg) => { msg.Embed = embed.Build(); });
        }
    }
}