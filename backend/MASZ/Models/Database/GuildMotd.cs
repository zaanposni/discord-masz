namespace MASZ.Models
{
    public class GuildMotd
    {
        public int Id { get; set; }
        public ulong GuildId { get; set; }
        public ulong UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Message { get; set; }
        public bool ShowMotd { get; set; }
    }
}
