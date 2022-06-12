using Discord;
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Extensions;
using MASZ.Models;
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

            return zalgo;
        }

        public bool ContainsZalgo(string content, int percentage)
        {
            Regex regex = new(@"[%CC%]|[%CD%]", RegexOptions.IgnoreCase);

            return regex.Matches(HttpUtility.UrlEncode(content)).Count / content.Length > percentage / 100;
        }

        public string CalculateZalgo(string content, ZalgoConfig zalgoConfig)
        {
            if (ContainsZalgo(content, zalgoConfig.Percentage))
            {
                string newName = zalgoConfig.renameFallback;
                if (zalgoConfig.renameNormal)
                {
                    Regex replaceSpaces = new(@"%C(C|D)(%[A-Z0-9]{2})+%20", RegexOptions.IgnoreCase);
                    Regex replaceChars = new(@"%C(C|D)(%[A-Z0-9]{2})+(\w)", RegexOptions.IgnoreCase);

                    newName = HttpUtility.UrlDecode(
                        replaceChars.Replace(
                            replaceSpaces.Replace(
                                HttpUtility.UrlEncode(content),
                                " "
                            ),
                            "$3"
                        )
                    );

                    newName = newName.Trim().Length == 0 ? zalgoConfig.renameFallback : newName;
                }

                return newName;
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
                    string current = member.DisplayName;
                    string newName = CalculateZalgo(current, zalgoConfig);
                    if (newName != null)
                    {
                        zalgoSimulations.Add(new ZalgoSimulation
                        {
                            oldName = current,
                            newName = newName,
                            userImage = member.GetAvatarOrDefaultUrl(size: 512)
                        });

                        if (rename)
                        {
                            try
                            {
                                await DiscordAPI.RenameUser(member, newName);
                            }
                            catch (Exception e)
                            {
                                Logger.LogError(e, $"Failed to rename zalgo user {member.Id} in guild {guildId}.");
                            }
                        }
                    }
                }
            }


            return zalgoSimulations;
        }
    }
}