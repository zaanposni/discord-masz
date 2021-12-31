using Discord;
using Discord.Interactions;
using Discord.Net;
using MASZ.Attributes;
using MASZ.Enums;
using System.Net;

namespace MASZ.Commands
{
    public class CleanupCommand : BaseCommand<CleanupCommand>
    {
        [Require(RequireCheckEnum.GuildModerator)]
        [SlashCommand("cleanup", "Cleanup specific data from the server and/or channel.")]
        public async Task Cleanup(
            [Summary("mode", "which data you want to delete")] CleanupMode cleanupMode,
            [Summary("channel", "where to delete, defaults to current.")] ITextChannel channel = null,
            [Summary("count", "how many messages to scan for your mode.")] long count = 100,
            [Summary("user", "additional filter on this user")] IUser filterUser = null)
        {
            if (channel == null && Context.Channel is ITextChannel txtChnl)
            {
                channel = txtChnl;
            }

            if (channel == null)
            {
                await Context.Interaction.RespondAsync(Translator.T().CmdOnlyTextChannel());
                return;
            }

            await Context.Interaction.RespondAsync("Cleaning channels...", ephemeral: true);

            if (count > 1000)
            {
                count = 1000;
            }

            var func = new Func<IMessage, bool>(m => true);
            switch (cleanupMode)
            {
                case CleanupMode.Bots:
                    func = IsFromBot;
                    break;
                case CleanupMode.Attachments:
                    func = HasAttachment;
                    break;
            }

            int deleted = 0;
            try
            {
                deleted = await IterateAndDeleteChannels(channel, (int)count, func, Context.User, filterUser);
            }
            catch (HttpException ex)
            {
                if (ex.HttpCode == HttpStatusCode.Forbidden)
                    await Context.Interaction.ModifyOriginalResponseAsync(msg => msg.Content = Translator.T().CmdCannotViewOrDeleteInChannel());
                else if (ex.HttpCode == HttpStatusCode.Forbidden)
                    await Context.Interaction.ModifyOriginalResponseAsync(msg => msg.Content = Translator.T().CmdCannotFindChannel());

                return;
            }
            await Context.Interaction.ModifyOriginalResponseAsync(msg => msg.Content = Translator.T().CmdCleanup(deleted, channel));
        }

        private bool HasAttachment(IMessage m)
        {
            return m.Attachments.Count > 0;
        }

        private bool IsFromBot(IMessage m)
        {
            return m.Author.IsBot;
        }

        private async Task<int> IterateAndDeleteChannels(ITextChannel channel, int limit, Func<IMessage, bool> predicate, IUser currentActor, IUser filterUser = null)
        {
            ulong lastId = 0;
            int deleted = 0;
            List<IMessage> toDelete = new ();
            var messages = channel.GetMessagesAsync(Math.Min(limit, 100));

            if (await messages.CountAsync() < Math.Min(limit, 100))
            {
                limit = 0;  // ignore while loop since channel will be empty soon
            }
            foreach (IMessage message in await messages.FlattenAsync())
            {
                lastId = message.Id;
                limit--;
                if (filterUser != null && message.Author.Id != filterUser.Id)
                {
                    continue;
                }
                if (predicate(message))
                {
                    deleted++;
                    if (message.CreatedAt.UtcDateTime.AddDays(14) > DateTime.UtcNow)
                    {
                        toDelete.Add(message);
                    }
                    else
                    {
                        await message.DeleteAsync();
                    }
                }
            }
            while (limit > 0)
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

                messages = channel.GetMessagesAsync(lastId, Direction.Before, Math.Min(limit, 100));
                bool breakAfterDeleteIteration = false;

                if (await messages.CountAsync() == 0)
                {
                    break;
                }
                if (await messages.CountAsync() < Math.Min(limit, 100))
                {
                    breakAfterDeleteIteration = true;
                }
                foreach (IMessage message in await messages.FlattenAsync())
                {
                    lastId = message.Id;
                    limit--;
                    if (filterUser != null && message.Author.Id != filterUser.Id)
                    {
                        continue;
                    }
                    if (predicate(message))
                    {
                        deleted++;
                        if (message.CreatedAt.UtcDateTime.AddDays(14) > DateTime.UtcNow)
                        {
                            toDelete.Add(message);
                        }
                        else
                        {
                            await message.DeleteAsync();
                        }
                    }
                }
                if (breakAfterDeleteIteration)
                {
                    break;
                }
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
            return deleted;
        }
    }
}