namespace MASZ.Models
{
    public class UserInviteView
    {
        public int Id { get; set; }
        public string GuildId { get; set; }
        public string TargetChannelId { get; set; }
        public string JoinedUserId { get; set; }
        public string UsedInvite { get; set; }
        public string InviteIssuerId { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime? InviteCreatedAt { get; set; }
        public UserInviteView(UserInvite userInvite)
        {
            Id = userInvite.Id;
            GuildId = userInvite.GuildId.ToString();
            TargetChannelId = userInvite.TargetChannelId.ToString();
            JoinedUserId = userInvite.JoinedUserId.ToString();
            UsedInvite = userInvite.UsedInvite;
            InviteIssuerId = userInvite.InviteIssuerId.ToString();
            JoinedAt = userInvite.JoinedAt;
            InviteCreatedAt = userInvite.InviteCreatedAt;
        }
    }
}