using Discord;
using Discord.Interactions;
using MASZ.Attributes;
using MASZ.Enums;
using MASZ.Extensions;
using MASZ.Models;
using MASZ.Models.Database;
using MASZ.Repositories;
using MASZ.Services;

namespace MASZ.Commands
{
    public class CreateEvidenceCommand : BaseCommand<CreateEvidenceCommand>
    {
        [Require(RequireCheckEnum.GuildModerator)]
        [MessageCommand("Create evidence")]
        public async Task CreateEvidence(IMessage message)
        { 
            try 
            {
                IGuildUser reportedUser = await DiscordAPI.FetchMemberInfo(Context.Guild.Id, message.Author.Id, CacheBehavior.IgnoreButCacheOnError);
                string content = message.Content.Truncate(1024);

                VerifiedEvidence evidence = new()
                { 
                    GuildId = Context.Guild.Id,
                    ChannelId = message.Channel.Id,
                    MessageId = message.Id,
                    ReportedAt = DateTime.UtcNow,
                    SentAt = message.Timestamp.DateTime,
                    ReportedContent = content,
                    UserId = reportedUser.Id,
                    Username = reportedUser.Username,
                    Nickname = reportedUser.Nickname,
                    Discriminator = reportedUser.Discriminator,
                    ModId = Context.User.Id
                };

                await VerifiedEvidenceRepository.CreateDefault(ServiceProvider, Context.User).CreateEvidence(evidence);
            }
            catch (Exception e) 
            {
                Logger.LogError(e, "Failed to save evidence in database");
                await Context.Interaction.RespondAsync(Translator.T().CmdEvidenceFailed(), ephemeral: true);
                return;
            }

            GuildConfig config = await GuildConfigRepository.CreateDefault(ServiceProvider).GetGuildConfig(Context.Guild.Id);

            if(!string.IsNullOrEmpty(config.ModInternalNotificationWebhook))
            {
                try
                {
                    // TODO: proper notification
                    await DiscordAPI.ExecuteWebhook(config.ModInternalNotificationWebhook, null, "created evidence TODO: change this", AllowedMentions.None);
                } catch (Exception e) 
                {
                    Logger.LogError(e, "Failed to send internal notification on evidence creation");
                }
            }

            await Context.Interaction.RespondAsync(Translator.T().CmdEvidenceCreated(), ephemeral: true);
        }
    }
}