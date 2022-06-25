namespace MASZ.Models
{
    public class ZalgoConfigView
    {
        public int Id { get; set; }
        public string GuildId { get; set; }
        public bool Enabled { get; set; }
        public int Percentage { get; set; }
        public bool renameNormal { get; set; }
        public string renameFallback { get; set; }
        public bool logToModChannel { get; set; }

        public ZalgoConfigView() { }
        public ZalgoConfigView(ZalgoConfig zalgo)
        {
            Id = zalgo.Id;
            GuildId = zalgo.GuildId.ToString();
            Enabled = zalgo.Enabled;
            Percentage = zalgo.Percentage;
            renameNormal = zalgo.renameNormal;
            renameFallback = zalgo.renameFallback;
            logToModChannel = zalgo.logToModChannel;
        }
    }
}
