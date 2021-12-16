namespace MASZ.Models
{
    public class UserMappingView
    {
        public int Id { get; set; }
        public string GuildId { get; set; }
        public string UserA { get; set; }
        public string UserB { get; set; }
        public string Reason { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatorUserId { get; set; }

        public UserMappingView(UserMapping userMapping)
        {
            Id = userMapping.Id;
            GuildId = userMapping.GuildId.ToString();
            UserA = userMapping.UserA.ToString();
            UserB = userMapping.UserB.ToString();
            Reason = userMapping.Reason;
            CreatedAt = userMapping.CreatedAt;
            CreatorUserId = userMapping.CreatorUserId.ToString();
        }
    }
}
