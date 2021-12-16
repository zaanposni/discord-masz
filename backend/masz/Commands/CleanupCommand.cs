using Discord;
using Discord.Interactions;
using Discord.Net;
using MASZ.Enums;
using System.Net;

namespace MASZ.Commands
{

    public class CleanupCommand : BaseCommand<CleanupCommand>
    {
        public CleanupCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }

        [SlashCommand("cleanup", "Cleanup specific data from the server and/or channel.")]
        public async Task Cleanup([Summary("mode", "which data you want to delete")] CleanupMode cleanupMode,
                                  [Summary("channel", "where to delete, defaults to current.")] IChannel channel = null,
                                  [Summary("count", "how many messages to scan for your mode.")] long count = 100,
                                  [Summary("user", "additional filter on this user")] IUser filterUser = null)
        {
            await Require(RequireCheckEnum.GuildModerator);

            if (channel == null)
            {
                channel = Context.Channel;
            }
            if (channel is not ITextChannel textChannel)
            {
                await Context.Interaction.RespondAsync(Translator.T().CmdOnlyTextChannel());
                return;
            }

            await Context.Interaction.RespondAsync("Deleting channels...");

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
                deleted = await IterateAndDeleteChannels(textChannel, (int)count, func, filterUser);
                await Context.Interaction.ModifyOriginalResponseAsync(message => message.Content = Translator.T().CmdCleanup(deleted, textChannel));
            }
            catch (HttpException e)
            {
                if (e.HttpCode == HttpStatusCode.NotFound)
                    await Context.Interaction.ModifyOriginalResponseAsync(message => message.Content = Translator.T().CmdCannotFindChannel());
                else if (e.HttpCode == HttpStatusCode.Unauthorized)
                    await Context.Interaction.ModifyOriginalResponseAsync(message => message.Content = Translator.T().CmdCannotViewOrDeleteInChannel());
                return;
            }
        }
        private bool HasAttachment(IMessage m)
        {
            return m.Attachments.Count > 0;
        }
        private bool IsFromBot(IMessage m)
        {
            return m.Author.IsBot;
        }

        private static async Task<int> IterateAndDeleteChannels(ITextChannel channel, int limit, Func<IMessage, bool> predicate, IUser filterUser = null)
        {
            int deleted = 0;
            List<IMessage> toDelete = new();

            while (limit > 0)
            {
                if (toDelete.Count >= 2)
                {
                    await channel.DeleteMessagesAsync(toDelete);
                    toDelete.Clear();
                }
                else if (toDelete.Count > 0)
                {
                    await toDelete[0].DeleteAsync();
                    toDelete.Clear();
                }

                var messages = channel.GetMessagesAsync(Math.Min(limit, 100));
                bool breakAfterDeleteIteration = false;
                if (await messages.CountAsync() == 0)
                {
                    break;
                }
                if (await messages.CountAsync() < Math.Min(limit, 100))
                {
                    breakAfterDeleteIteration = true;
                }
                await foreach (IMessage message in messages)
                {
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
                await channel.DeleteMessagesAsync(toDelete);
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