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
        public int ResetsAt { get; set; }

        public DiscordApiRatelimit(IRestResponse response) {
            var limit = response.Headers.Where(x => x.Name == "x-ratelimit-remaining").Select(x => x.Value).FirstOrDefault();
            if (limit != null) {
                this.Remaining = Int32.Parse(limit.ToString());
            } else {
                this.Remaining = 1;
            }
            var reset = response.Headers.Where(x => x.Name == "x-ratelimit-reset").Select(x => x.Value).FirstOrDefault();
            if (reset != null) {
                this.ResetsAt = Int32.Parse(reset.ToString());
            } else {
                this.ResetsAt = 0;
            }
        }
    }
}
