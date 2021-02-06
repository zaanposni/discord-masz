using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestSharp;

namespace masz.Models
{
    public class DiscordApiRatelimit
    {
        public int Remaining { get; set; }
        public DateTime ResetsAt { get; set; }

        public DiscordApiRatelimit(IRestResponse response) {
            var limit = response.Headers.Where(x => x.Name == "x-ratelimit-remaining").Select(x => x.Value).FirstOrDefault();
            if (limit != null) {
                this.Remaining = Int32.Parse(limit.ToString());
            } else {
                this.Remaining = 1;
            }
            var reset = response.Headers.Where(x => x.Name == "x-ratelimit-reset-after").Select(x => x.Value).FirstOrDefault();
            var now = DateTime.UtcNow;
            if (reset != null) {
                this.ResetsAt = now.AddSeconds(Int32.Parse(reset.ToString()));
            } else {
                this.ResetsAt = now;
            }
        }
    }
}
