namespace masz.Dtos
{
    public class TelemetryDataResourceDto
    {
        public int AllocatedMemory { get; set; }
        public int FreeMemory { get; set; }
        public int TotalMemory { get; set; }
        public int CPUUsage { get; set; }
        public int DiscordLatency { get; set; }
        public int DatabaseLatency { get; set; }
    }
}