namespace MASZ.Models
{
    public class UserNoteView
    {
        public int Id { get; set; }
        public string GuildId { get; set; }
        public string UserId { get; set; }
        public string Description { get; set; }
        public string CreatorId { get; set; }
        public DateTime UpdatedAt { get; set; }

        public UserNoteView(UserNote userNote)
        {
            Id = userNote.Id;
            GuildId = userNote.GuildId.ToString();
            UserId = userNote.UserId.ToString();
            Description = userNote.Description;
            CreatorId = userNote.CreatorId.ToString();
            UpdatedAt = userNote.UpdatedAt;
        }
    }
}
