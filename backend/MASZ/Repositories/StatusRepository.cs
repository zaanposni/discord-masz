using Discord;
using MASZ.Models;
using System.Diagnostics;

namespace MASZ.Repositories
{

    public class StatusRepository : BaseRepository<StatusRepository>
    {
        private StatusRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public static StatusRepository CreateDefault(IServiceProvider serviceProvider) => new(serviceProvider);

        public async Task<StatusDetail> GetDbStatus()
        {
            StatusDetail dbStatus = new();
            try
            {
                Stopwatch timer = new();
                timer.Start();
                dbStatus.Online = await Database.CanConnectAsync();
                timer.Stop();
                dbStatus.ResponseTime = timer.Elapsed.TotalMilliseconds;
            }
            catch (Exception)
            {
                dbStatus.Online = false;
            }
            return dbStatus;
        }
        public StatusDetail GetBotStatus()
        {
            StatusDetail botStatus = new();
            try
            {
                botStatus.Online = _discordBot.IsRunning();
                botStatus.LastDisconnect = _discordBot.GetLastDisconnectTime();
                botStatus.ResponseTime = _client.Latency;
            }
            catch (Exception)
            {
                botStatus.Online = false;
                botStatus.LastDisconnect = _discordBot.GetLastDisconnectTime();
            }
            return botStatus;
        }
        public StatusDetail GetCacheStatus()
        {
            StatusDetail cacheStatus = new();
            try
            {
                Stopwatch timer = new();
                timer.Start();
                IUser user = DiscordAPI.GetCurrentBotInfo();
                timer.Stop();
                cacheStatus.ResponseTime = timer.Elapsed.TotalMilliseconds;
                if (user == null)
                {
                    cacheStatus.Online = false;
                    cacheStatus.Message = "Cache is empty.";
                }
            }
            catch (Exception)
            {
                cacheStatus.Online = false;
            }
            return cacheStatus;
        }
    }
}