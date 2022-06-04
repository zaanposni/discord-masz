using Discord;
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Models;

namespace MASZ.Repositories
{
    public class ScheduledMessageRepository : BaseRepository<ScheduledMessageRepository>
    {
        private readonly IUser _currentUser;
        private ScheduledMessageRepository(IServiceProvider serviceProvider, IUser currentUser) : base(serviceProvider)
        {
            _currentUser = currentUser;
        }
        private ScheduledMessageRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _currentUser = DiscordAPI.GetCurrentBotInfo();
        }
        public static ScheduledMessageRepository CreateDefault(IServiceProvider serviceProvider, Identity identity) => new(serviceProvider, identity.GetCurrentUser());
        public static ScheduledMessageRepository CreateWithBotIdentity(IServiceProvider serviceProvider) => new(serviceProvider);

        public async Task<List<ScheduledMessage>> GetDueMessages()
        {
            return await Database.GetDueMessages();
        }

        public async Task<List<ScheduledMessage>> GetPendingMessages(ulong guildId)
        {
            return await Database.GetPendingMessages(guildId);
        }

        public async Task<List<ScheduledMessage>> GetAllMessages(ulong guildId, int page = 0)
        {
            return await Database.GetScheduledMessages(guildId, page);
        }

        public async Task<ScheduledMessage> GetMessage(int id)
        {
            ScheduledMessage message = await Database.GetMessage(id);
            if (message == null)
            {
                throw new ResourceNotFoundException($"ScheduledMessage with id {id} not found.");
            }
            return message;
        }

        public async Task<ScheduledMessage> CreateMessage(ScheduledMessage message)
        {
            message.CreatedAt = DateTime.UtcNow;
            message.LastEditedAt = message.CreatedAt;
            message.CreatorId = _currentUser.Id;
            message.LastEditedById = _currentUser.Id;
            message.Status = ScheduledMessageStatus.Pending;

            Database.SaveMessage(message);
            await Database.SaveChangesAsync();

            return message;
        }

        public async Task<ScheduledMessage> UpdateMessage(ScheduledMessage message)
        {
            message.LastEditedAt = DateTime.UtcNow;
            message.LastEditedById = _currentUser.Id;

            Database.UpdateMessage(message);
            await Database.SaveChangesAsync();

            return message;
        }

        public async Task<int> CountMessages()
        {
            return await Database.CountMessages();
        }

        public async Task<int> CountMessages(ulong guildId)
        {
            return await Database.CountMessages(guildId);
        }

        public async Task<ScheduledMessage> SetMessageAsSent(int id)
        {
            ScheduledMessage message = await GetMessage(id);
            message.Status = ScheduledMessageStatus.Sent;

            Database.UpdateMessage(message);
            await Database.SaveChangesAsync();

            return message;
        }

        public async Task<ScheduledMessage> SetMessageAsFailed(int id, ScheduledMessageFailureReason reason)
        {
            ScheduledMessage message = await GetMessage(id);
            message.Status = ScheduledMessageStatus.Failed;
            message.FailureReason = reason;

            Database.UpdateMessage(message);
            await Database.SaveChangesAsync();

            return message;
        }

        public async Task<ScheduledMessage> DeleteMessage(int id)
        {
            ScheduledMessage message = await GetMessage(id);

            Database.DeleteMessage(message);
            await Database.SaveChangesAsync();

            return message;
        }

        public async Task DeleteMessagesForGuild(ulong guildId)
        {
            await Database.DeleteMessagesForGuild(guildId);
        }
    }
}
