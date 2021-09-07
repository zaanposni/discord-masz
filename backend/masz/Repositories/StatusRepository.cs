
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using masz.Models;

namespace masz.Repositories
{

    public class StatusRepository : BaseRepository<StatusRepository>
    {
        private StatusRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public static StatusRepository CreateDefault(IServiceProvider serviceProvider) => new StatusRepository(serviceProvider);

        public async Task<StatusDetail> GetDbStatus()
        {
            StatusDetail dbStatus = new StatusDetail();
            try
            {
                Stopwatch timer = new Stopwatch();
                timer.Start();
                dbStatus.Online = await _database.CanConnectAsync();
                timer.Stop();
                dbStatus.ResponseTime = timer.Elapsed.TotalMilliseconds;
            } catch (Exception)
            {
                dbStatus.Online = false;
            }
            return dbStatus;
        }
        public StatusDetail GetBotStatus()
        {
            StatusDetail botStatus = new StatusDetail();
            try
            {
                if (! _discordBot.IsRunning())
                {
                    botStatus.Online = false;
                    botStatus.LastDisconnect = _discordBot.GetLastDisconnectTime();
                }
            } catch (Exception)
            {
                botStatus.Online = false;
                botStatus.LastDisconnect = _discordBot.GetLastDisconnectTime();
            }
            return botStatus;
        }
        public StatusDetail GetCacheStatus()
        {
            StatusDetail cacheStatus = new StatusDetail();
            try
            {
                Stopwatch timer = new Stopwatch();
                timer.Start();
                DiscordUser user = _discordAPI.GetCurrentBotInfo(CacheBehavior.OnlyCache);
                timer.Stop();
                cacheStatus.ResponseTime = timer.Elapsed.TotalMilliseconds;
                if (user == null)
                {
                    cacheStatus.Online = false;
                    cacheStatus.Message = "Cache is empty.";
                }
            } catch (Exception)
            {
                cacheStatus.Online = false;
            }
            return cacheStatus;
        }
        public async Task<StatusDetail> GetDiscordAPIStatus()
        {
            StatusDetail dbStatus = new StatusDetail();
            try
            {
                Stopwatch timer = new Stopwatch();
                timer.Start();
                DiscordUser user = await _discordAPI.FetchCurrentBotInfo();
                timer.Stop();
                dbStatus.ResponseTime = timer.Elapsed.TotalMilliseconds;
                if (user == null)
                {
                    dbStatus.Online = false;
                    dbStatus.Message = "Failed to fetch from discord API.";
                }
            } catch (Exception)
            {
                dbStatus.Online = false;
            }
            return dbStatus;
        }
    }
}