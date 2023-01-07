namespace MASZ.Dtos
{
    public class TelemetryDataGuildSizeDto
    {
        public string HashedServer { get; set; }
        public string HashedUserId { get; set; }
        public string HashedGuildId { get; set; }
        public int MemberCount { get; set; }
        public int ChannelCount { get; set; }
        public int RoleCount { get; set; }
        public int InviteCount { get; set; }
    }
}