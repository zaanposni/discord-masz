namespace MASZ.Models.Views
{
    public class GuildMotdView
    {
        public int Id { get; set; }
        public string GuildId { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Message { get; set; }
        public bool ShowMotd { get; set; }

        public GuildMotdView() { }
        public GuildMotdView(GuildMotd motd)
        {
            Id = motd.Id;
            GuildId = motd.GuildId.ToString();
            UserId = motd.UserId.ToString();
            CreatedAt = motd.CreatedAt;
            Message = motd.Message;
            ShowMotd = motd.ShowMotd;
        }
    }
}
