using Discord;
using Discord.Interactions;
using MASZ.Attributes;
using MASZ.Enums;
using MASZ.Extensions;
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

                VerifiedEvidence evidence = new()
                { 
                    GuildId = Context.Guild.Id,
                    ChannelId = message.Channel.Id,
                    MessageId = message.Id,
                    ReportedAt = DateTime.UtcNow,
                    SentAt = message.Timestamp.DateTime,
                    ReportedContent = message.Content,
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

            await Context.Interaction.RespondAsync(Translator.T().CmdEvidenceCreated(), ephemeral: true);
        }
    }
}