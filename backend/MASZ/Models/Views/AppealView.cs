using Discord;
using MASZ.Enums;

namespace MASZ.Models.Views
{
    public class AppealView
    {
        public int Id { get; set; }
        public DiscordUserView User { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Discriminator { get; set; }
        public string Mail { get; set; }
        public string GuildId { get; set; }
        public AppealStatus Status { get; set; }
        public string ModeratorComment { get; set; }
        public DiscordUserView LastModerator { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Nullable<DateTime> UserCanCreateNewAppeals { get; set; }
        public Nullable<DateTime> InvalidDueToLaterRejoinAt { get; set; }
        public List<AppealAnswerView> Answers { get; set; } = new();
        public List<AppealStructureView> Structures { get; set; } = new();
        public List<ModCaseTableEntry> LatestCases { get; set; } = new();

        public AppealView(Appeal appeal, IUser user, IUser lastModerator, List<AppealAnswer> answers, List<AppealStructure> structures, List<ModCaseTableEntry> latestCases = null)
        {
            Id = appeal.Id;
            User = DiscordUserView.CreateOrDefault(user);
            UserId = appeal.UserId.ToString();
            Username = appeal.Username;
            Discriminator = appeal.Discriminator;
            Mail = appeal.Mail;
            GuildId = appeal.GuildId.ToString();
            Status = appeal.Status;
            ModeratorComment = appeal.ModeratorComment;
            LastModerator = DiscordUserView.CreateOrDefault(lastModerator);
            CreatedAt = appeal.CreatedAt;
            UpdatedAt = appeal.UpdatedAt;
            UserCanCreateNewAppeals = appeal.UserCanCreateNewAppeals;
            InvalidDueToLaterRejoinAt = appeal.InvalidDueToLaterRejoinAt;

            Answers = answers.Select(a => new AppealAnswerView(a)).ToList();
            Structures = structures.Select(s => new AppealStructureView(s)).ToList();

            LatestCases = latestCases ?? new List<ModCaseTableEntry>();
        }

        public void RemoveModeratorInfo()
        {
            LastModerator = null;
        }
    }
}
