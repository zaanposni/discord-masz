namespace MASZ.Models
{
    public class ZalgoConfig
    {
        public int Id { get; set; }
        public ulong GuildId { get; set; }
        public bool Enabled { get; set; }
        public int Percentage { get; set; }
        public bool renameNormal { get; set; }
        public string renameFallback { get; set; } = "zalgo user";
        public bool logToModChannel { get; set; }
    }
}
