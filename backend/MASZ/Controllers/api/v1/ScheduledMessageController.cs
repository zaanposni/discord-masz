using MASZ.Dtos;
using MASZ.Dtos.UserNote;
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Models;
using MASZ.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/guilds/{guildId}/scheduledmessages")]
    [Authorize]
    public class ScheduledMessageController : SimpleController
    {

        public ScheduledMessageController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetMessages([FromRoute] ulong guildId, [FromQuery][Range(0, int.MaxValue)] int page = 0)
        {
            await RequirePermission(guildId, DiscordPermission.Moderator);

            ScheduledMessageRepository repo = ScheduledMessageRepository.CreateDefault(_serviceProvider, await GetIdentity());
            List<ScheduledMessage> messages = await repo.GetAllMessages(guildId, page);
            int fullSize = await repo.CountMessages(guildId);

            List<ScheduledMessageView> results = new();
            foreach (ScheduledMessage message in messages)
            {
                results.Add(new ScheduledMessageView(message,
                                                     await _discordAPI.FetchUserInfo(message.CreatorId, CacheBehavior.OnlyCache),
                                                     await _discordAPI.FetchUserInfo(message.LastEditedById, CacheBehavior.OnlyCache),
                                                     _discordAPI.FetchGuildChannels(guildId, CacheBehavior.OnlyCache).FirstOrDefault(x => x.Id == message.ChannelId)));
            }

            return Ok(new {
                items = results,
                fullSize
            });
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetDueMessages([FromRoute] ulong guildId)
        {
            await RequirePermission(guildId, DiscordPermission.Moderator);

            List<ScheduledMessage> messages = await ScheduledMessageRepository.CreateDefault(_serviceProvider, await GetIdentity()).GetPendingMessages(guildId);

            List<ScheduledMessageView> results = new();

            foreach (ScheduledMessage message in messages)
            {
                if (results.Count >= 10)
                {
                    break;
                }

                results.Add(new ScheduledMessageView(message,
                                                     await _discordAPI.FetchUserInfo(message.CreatorId, CacheBehavior.OnlyCache),
                                                     await _discordAPI.FetchUserInfo(message.LastEditedById, CacheBehavior.OnlyCache),
                                                     _discordAPI.FetchGuildChannels(guildId, CacheBehavior.OnlyCache).FirstOrDefault(x => x.Id == message.ChannelId)));
            }

            return Ok(results);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage([FromRoute] ulong guildId, [FromBody] ScheduledMessageForCreateDto dto)
        {
            await RequirePermission(guildId, DiscordPermission.Moderator);
            await GetRegisteredGuild(guildId);

            ScheduledMessage message = new();

            message.Name = dto.Name;
            message.Content = dto.Content;
            message.ScheduledFor = dto.ScheduledFor;
            message.ChannelId = dto.ChannelId;
            message.GuildId = guildId;

            if (message.ScheduledFor < DateTime.UtcNow.AddMinutes(1))
            {
                throw new InvalidDateForScheduledMessageException();
            }

            message = await ScheduledMessageRepository.CreateDefault(_serviceProvider, await GetIdentity()).CreateMessage(message);

            return Ok(new ScheduledMessageView(
                message,
                await _discordAPI.FetchUserInfo(message.CreatorId, CacheBehavior.OnlyCache),
                await _discordAPI.FetchUserInfo(message.LastEditedById, CacheBehavior.OnlyCache),
                _discordAPI.FetchGuildChannels(guildId, CacheBehavior.OnlyCache).FirstOrDefault(x => x.Id == message.ChannelId)
            ));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditMessage([FromRoute] ulong guildId, [FromRoute] int id, [FromBody] ScheduledMessageForPutDto dto)
        {
            await RequirePermission(guildId, DiscordPermission.Moderator);
            await GetRegisteredGuild(guildId);

            ScheduledMessageRepository repo = ScheduledMessageRepository.CreateDefault(_serviceProvider, await GetIdentity());

            ScheduledMessage message = await repo.GetMessage(id);
            if (message.GuildId != guildId)
            {
                throw new UnauthorizedException();
            }
            if (message.Status != ScheduledMessageStatus.Pending)
            {
                throw new ProtectedScheduledMessageException();
            }
            if (dto.ScheduledFor < DateTime.UtcNow.AddMinutes(1))
            {
                throw new InvalidDateForScheduledMessageException();
            }

            message.Name = dto.Name;
            message.Content = dto.Content;
            message.ScheduledFor = dto.ScheduledFor;
            message.ChannelId = dto.ChannelId;

            message = await repo.UpdateMessage(message);

            return Ok(new ScheduledMessageView(
                message,
                await _discordAPI.FetchUserInfo(message.CreatorId, CacheBehavior.OnlyCache),
                await _discordAPI.FetchUserInfo(message.LastEditedById, CacheBehavior.OnlyCache),
                _discordAPI.FetchGuildChannels(guildId, CacheBehavior.OnlyCache).FirstOrDefault(x => x.Id == message.ChannelId)
            ));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage([FromRoute] ulong guildId, [FromRoute] int id)
        {
            await RequirePermission(guildId, DiscordPermission.Moderator);
            await GetRegisteredGuild(guildId);
            Identity identity = await GetIdentity();

            ScheduledMessageRepository repo = ScheduledMessageRepository.CreateDefault(_serviceProvider, identity);

            ScheduledMessage message = await repo.GetMessage(id);
            if (message.GuildId != guildId)
            {
                throw new UnauthorizedException();
            }

            // handled messages should only be deletable by an admin to prevent abuse
            if (message.Status != ScheduledMessageStatus.Pending && !(await identity.HasAdminRoleOnGuild(guildId)))
            {
                throw new ProtectedScheduledMessageException();
            }

            await repo.DeleteMessage(message.Id);
            return Ok();
        }
    }
}
