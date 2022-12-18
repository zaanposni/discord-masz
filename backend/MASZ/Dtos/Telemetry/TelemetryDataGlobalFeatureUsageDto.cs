namespace MASZ.Dtos
{
    public class TelemetryDataGlobalFeatureUsageDto
    {
        public string HashedServer { get; set; }
        public int GuildCount { get; set; }
        public bool AuditLogEnabled { get; set; }
        public int APITokenCount { get; set; }
    }
}