using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using masz.Dtos.DiscordAPIResponses;

namespace masz.Models
{
    public class DbCount
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Count { get; set; }
    }
}
