using MASZ.Enums;
using Newtonsoft.Json.Linq;

namespace MASZ.Dtos
{
    public class TelemetryDataUsageDto
    {
        public string HashedServer { get; set; }
        public string HashedUserId { get; set; }
        public string HashedGuildId { get; set; }
        public bool UserIsSiteAdmin { get; set; }
        public bool UserIsToken { get; set; }
        public TelemetryDataUsageActionType ActionType { get; set; }
        public JObject AdditionalData { get; set; }
    }
}