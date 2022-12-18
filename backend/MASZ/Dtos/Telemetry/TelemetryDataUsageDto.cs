using MASZ.Enums;
using Newtonsoft.Json.Linq;

namespace maszindex.Models.Dto
{
    public class TelemetryDataUsageDto
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string HashedServer { get; set; }
        public string HashedUserId { get; set; }
        public string HashedGuildId { get; set; }
        public bool UserIsSiteAdmin { get; set; }
        public bool UserIsToken { get; set; }
        public TelemetryDataUsageActionType ActionType { get; set; }
        public JObject AdditionalData { get; set; }
    }
}