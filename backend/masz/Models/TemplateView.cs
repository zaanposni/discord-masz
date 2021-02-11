using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using masz.Dtos.DiscordAPIResponses;

namespace masz.Models
{
    public class TemplateView
    {
        public CaseTemplate CaseTemplate { get; set; }
        public User Creator { get; set; }
        public Guild Guild { get; set; }
    }
}
