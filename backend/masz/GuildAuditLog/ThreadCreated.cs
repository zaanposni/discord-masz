using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using masz.Services;

namespace masz.GuildAuditLog
{
    public static class ThreadCreatedAuditLog
    {
        public static DiscordEmbedBuilder HandleThreadCreated(DiscordClient client, ThreadCreateEventArgs e, ITranslator translator)
        {
            DiscordEmbedBuilder embed = GuildAuditLogger.GenerateBaseEmbed(DiscordColor.Green);

            StringBuilder description = new StringBuilder();
            description.AppendLine($"> **{translator.T().GuildAuditLogChannel()}:** {e.Thread.Name} - {e.Thread.Mention}");
            description.AppendLine($"> **{translator.T().GuildAuditLogThreadCreatedParent()}:** {e.Parent.Name} - {e.Parent.Mention}");
            description.AppendLine($"> **{translator.T().GuildAuditLogThreadCreatedCreator()}:** <@{e.Thread.CreatorId}>");

            embed.WithTitle(translator.T().GuildAuditLogThreadCreatedTitle())
                 .WithDescription(description.ToString())
                 .WithFooter($"{translator.T().GuildAuditLogChannelId()}: {e.Thread.Id}");

            return embed;
        }
    }
}