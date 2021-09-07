
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
        private readonly Identity _identity;
        private StatusRepository(IServiceProvider serviceProvider, Identity identity) : base(serviceProvider)
        {
            _identity = identity;
        }

        public static StatusRepository CreateDefault(IServiceProvider serviceProvider, Identity identity) => new StatusRepository(serviceProvider, identity);

        public async Task<StatusDetail> GetDbStatus()
        {
            StatusDetail dbStatus = new StatusDetail();
            try
            {
                CancellationTokenSource source = new CancellationTokenSource();
                source.CancelAfter(TimeSpan.FromSeconds(3));
                Task<bool> task = Task.Run(() => _database.CanConnectAsync(source.Token), source.Token);
                Stopwatch timer = new Stopwatch();
                timer.Start();
                dbStatus.Online = await task;
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
    }
}