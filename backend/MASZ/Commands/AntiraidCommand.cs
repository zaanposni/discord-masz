using Discord;
using Discord.Interactions;
using Discord.Net;
using MASZ.Attributes;
using MASZ.Enums;
using System.Net;

namespace MASZ.Commands
{
    public class AntiraidCommand : BaseCommand<AntiraidCommand>
    {
        [Require(RequireCheckEnum.GuildModerator)]
        [SlashCommand("antiraid", "Timeout a specific user and delete all his messages in the last 2 hours.")]
        public async Task Antiraid(
            [Summary("user", "user to punish")] IUser user)
        {

            IGuildUser member = null;
            try
            {
                member = Context.Guild.GetUser(user.Id);
                await member.SetTimeOutAsync(TimeSpan.FromDays(1));
            }
            catch (Exception) { }

            await Context.Interaction.RespondAsync("Cleaning channels...", ephemeral: true);

            int deleted = 0;
            // delete messages in current channel first
            if (Context.Channel is ITextChannel ctxTxtChnl)
            {
                try
                {
                    deleted += await IterateAndDeleteChannel(ctxTxtChnl, Context.User, user.Id);
                }
                catch (HttpException) { }
            }

            // iterate over each channel
            foreach (var channel in Context.Guild.Channels)
            {
                if (channel is ITextChannel txtChnl && txtChnl.Id != Context.Channel.Id)
                {
                    try
                    {
                        deleted += await IterateAndDeleteChannel(txtChnl, Context.User, user.Id);
                    }
                    catch (HttpException) { }
                }
            }

            await Context.Interaction.ModifyOriginalResponseAsync(msg => msg.Content = Translator.T().CmdAntiraid(deleted));
        }

        private async Task<int> IterateAndDeleteChannel(ITextChannel channel, IUser currentActor, ulong userId)
        {
            ulong lastId = 0;
            int deleted = 0;
            List<IMessage> toDelete = new();
            var messages = channel.GetMessagesAsync();

            var flatten = await messages.FlattenAsync();
            if (flatten.Count() == 0)
            {
                return 0;
            }

            foreach (IMessage message in flatten)
            {
                if (message.CreatedAt.UtcDateTime.AddHours(2) < DateTime.UtcNow)
                {
                    if (toDelete.Count >= 2)
                    {
                        RequestOptions options = new();
                        options.AuditLogReason = $"Bulkdelete by {currentActor.Username}#{currentActor.Discriminator} ({currentActor.Id}).";

                        await channel.DeleteMessagesAsync(toDelete, options);
                        toDelete.Clear();
                    }
                    else if (toDelete.Count > 0)
                    {
                        await toDelete[0].DeleteAsync();
                        toDelete.Clear();
                    }
                    return deleted;
                }

                lastId = message.Id;
                if (message.Author.Id != userId)
                {
                    continue;
                }

                deleted++;
                toDelete.Add(message);
            }

            while (true)
            {
                messages = channel.GetMessagesAsync(lastId, Direction.Before, 100);

                flatten = await messages.FlattenAsync();
                bool shouldBreakWhile = flatten.Count() == 0;
                foreach (IMessage message in flatten)
                {
                    lastId = message.Id;
                    if (message.CreatedAt.UtcDateTime.AddHours(2) < DateTime.UtcNow)
                    {
                        shouldBreakWhile = true;
                        break;
                    }
                    if (message.Author.Id != userId)
                    {
                        continue;
                    }
                    deleted++;
                    toDelete.Add(message);
                }

                if (toDelete.Count >= 2)
                {
                    RequestOptions options = new();
                    options.AuditLogReason = $"Bulkdelete by {currentActor.Username}#{currentActor.Discriminator} ({currentActor.Id}).";

                    await channel.DeleteMessagesAsync(toDelete, options);
                    toDelete.Clear();
                }
                else if (toDelete.Count > 0)
                {
                    await toDelete[0].DeleteAsync();
                    toDelete.Clear();
                }

                if (shouldBreakWhile)
                {
                    break;
                }
            }

            return deleted;
        }
    }
}