namespace MASZ.Dtos
{
    public class TelemetryDataResourceDto
    {
        public string HashedServer { get; set; }
        public long AllocatedMemory { get; set; }
        public long FreeMemory { get; set; }
        public long TotalMemory { get; set; }
        public double CPUUsage { get; set; }
        public double DiscordLatency { get; set; }
        public double DatabaseLatency { get; set; }
    }
}