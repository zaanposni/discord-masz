using Discord;
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Extensions;
using MASZ.Models;
using MASZ.Models.Views;
using System.Text.RegularExpressions;
using System.Web;

namespace MASZ.Repositories
{

    public class ZalgoRepository : BaseRepository<ZalgoRepository>
    {
        private readonly IUser _currentUser;
        private ZalgoRepository(IServiceProvider serviceProvider, IUser currentUser) : base(serviceProvider)
        {
            _currentUser = currentUser;
        }
        private ZalgoRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _currentUser = DiscordAPI.GetCurrentBotInfo();
        }
        public static ZalgoRepository CreateDefault(IServiceProvider serviceProvider, Identity identity) => new(serviceProvider, identity.GetCurrentUser());
        public static ZalgoRepository CreateWithBotIdentity(IServiceProvider serviceProvider) => new(serviceProvider);
        public async Task<ZalgoConfig> GetZalgo(ulong guildId)
        {
            ZalgoConfig zalgo = await Database.GetZalgoConfigForGuild(guildId);
            if (zalgo == null)
            {
                throw new ResourceNotFoundException();
            }
            return zalgo;
        }
        public async Task<ZalgoConfig> CreateOrUpdateZalgo(ulong guildId, ZalgoConfig newConfig)
        {
            RestAction action = RestAction.Updated;
            ZalgoConfig zalgo;
            try
            {
                zalgo = await GetZalgo(guildId);
            }
            catch (ResourceNotFoundException)
            {
                zalgo = new ZalgoConfig
                {
                    GuildId = guildId
                };
                action = RestAction.Created;
            }

            zalgo.Enabled = newConfig.Enabled;
            zalgo.Percentage = newConfig.Percentage;
            zalgo.renameNormal = newConfig.renameNormal;
            zalgo.renameFallback = newConfig.renameFallback;
            zalgo.logToModChannel = newConfig.logToModChannel;

            Database.SaveZalgoConfig(zalgo);
            await Database.SaveChangesAsync();

            if (action == RestAction.Created)
            {
                _eventHandler.OnZalgoConfigCreatedEvent.InvokeAsync(zalgo, _currentUser);
            }
            else
            {
                _eventHandler.OnZalgoConfigUpdatedEvent.InvokeAsync(zalgo, _currentUser);
            }

            Task task = new(async () =>
            {
                await CheckZalgoForAllMembers(guildId, zalgo, true);
            });
            task.Start();

            return zalgo;
        }

        public bool ContainsZalgo(string content, int percentage)
        {
            Regex regex = new(@"[%CC%]|[%CD%]", RegexOptions.IgnoreCase);

            return regex.Matches(HttpUtility.UrlEncode(content)).Count / content.Length > percentage / 100;
        }

        public string CalculateZalgo(string content, int percentage, string fallback, bool renameNormal)
        {
            if (ContainsZalgo(content, percentage))
            {
                string newName = fallback;
                if (renameNormal)
                {
                    Regex replaceEndOfString = new(@"%C(C|D)(%[A-Z0-9]{2})+$", RegexOptions.IgnoreCase);
                    Regex replaceSpaces = new(@"%C(C|D)(%[A-Z0-9]{2})+(%20|\+)", RegexOptions.IgnoreCase);
                    Regex replaceChars = new(@"%C(C|D)(%[A-Z0-9]{2})+(\w)", RegexOptions.IgnoreCase);

                    newName = HttpUtility.UrlDecode(
                        replaceEndOfString.Replace(
                            replaceChars.Replace(
                                replaceSpaces.Replace(
                                    HttpUtility.UrlEncode(content),
                                    " "
                                ),
                                "$3"
                            ),
                            ""
                        )
                    );

                    if (newName.Trim().Length == 0 || ContainsZalgo(newName, percentage))
                    {
                        newName = fallback;
                    }
                }

                return newName;
            }

            return null;
        }

        public async Task<ZalgoSimulation> CheckZalgoForMember(ulong guildId, ZalgoConfig zalgoConfig, IGuildUser member, bool rename = false)
        {
            if (zalgoConfig.Enabled)
            {
                string current = member.DisplayName;
                string newName = CalculateZalgo(current, zalgoConfig.Percentage, zalgoConfig.renameFallback, zalgoConfig.renameNormal);
                if (newName != null)
                {
                    if (rename)
                    {
                        try
                        {
                            await DiscordAPI.RenameUser(member, newName);
                            if (zalgoConfig.logToModChannel)
                            {
                                _eventHandler.OnZalgoNicknameRenameEvent.InvokeAsync(zalgoConfig, member.Id, current, newName);
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.LogError(e, $"Failed to rename zalgo user {member.Id} in guild {guildId}.");
                        }
                    }

                    return new ZalgoSimulation
                    {
                        oldName = current,
                        newName = newName,
                        user = DiscordUserView.CreateOrDefault(member)
                    };
                }
            }

            return null;
        }

        public async Task<List<ZalgoSimulation>> CheckZalgoForAllMembers(ulong guildId, ZalgoConfig zalgoConfig, bool rename = false)
        {
            List<ZalgoSimulation> zalgoSimulations = new List<ZalgoSimulation>();

            if (zalgoConfig.Enabled)
            {
                List<IGuildUser> members = await DiscordAPI.FetchGuildMembers(guildId, CacheBehavior.Default);

                foreach (IGuildUser member in members)
                {
                    ZalgoSimulation res = await CheckZalgoForMember(guildId, zalgoConfig, member, rename);
                    if (res != null)
                    {
                        zalgoSimulations.Add(res);
                    }
                }
            }


            return zalgoSimulations;
        }
    }
}