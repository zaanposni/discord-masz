using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace masz.Models
{
    public class InternalConfig
    {
        public string DiscordBotToken { get; set; }
        public string Version { get; set; }
        public string DiscordClientId { get; set; }
        public string DiscordClientSecret { get; set; }
        public string AbsolutePathToFileUpload { get; set; }
        public string ServiceHostName { get; set; }
        public string ServiceBaseUrl { get; set; }
        public List<string> SiteAdminDiscordUserIds { get; set; }
    }
}
