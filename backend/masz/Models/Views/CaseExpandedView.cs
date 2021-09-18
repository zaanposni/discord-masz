using System;
using System.Collections.Generic;
using System.Linq;
using DSharpPlus.Entities;
using masz.Models.Views;

namespace masz.Models
{
    public class CaseExpandedView
    {
        public CaseExpandedView(ModCase modCase, DiscordUser moderator, DiscordUser lastModerator, DiscordUser suspect, DiscordUser lockedBy, DiscordUser deletedBy)
        {
            ModCase = new CaseView(modCase);
            Moderator = DiscordUserView.CreateOrDefault(moderator);
            LastModerator = DiscordUserView.CreateOrDefault(lastModerator);
            Suspect = DiscordUserView.CreateOrDefault(suspect);
            LockedBy = DiscordUserView.CreateOrDefault(lockedBy);
            DeletedBy = DiscordUserView.CreateOrDefault(deletedBy);
            if (modCase.Comments != null) Comments = modCase.Comments.Select(x => new CommentsView(x)).ToList();

            if (modCase.PunishedUntil != null) {
                if (modCase.PunishedUntil > modCase.CreatedAt) {
                    if (modCase.PunishedUntil < DateTime.UtcNow) {
                        PunishmentProgress = 100;
                    } else {
                        double totalPunished = (modCase.PunishedUntil.Value - modCase.CreatedAt).TotalSeconds;
                        double alreadyPunished = (DateTime.UtcNow - modCase.CreatedAt).TotalSeconds;

                        PunishmentProgress = alreadyPunished / totalPunished * 100;
                    }
                }
            }
        }
        public CaseView ModCase { get; set; }
        public DiscordUserView Moderator { get; set; }
        public DiscordUserView LastModerator { get; set; }
        public DiscordUserView Suspect { get; set; }
        public DiscordUserView LockedBy { get; set; }
        public DiscordUserView DeletedBy { get; set; }
        public List<CommentsView> Comments { get; set; }
        public double? PunishmentProgress { get; set; }

        public void RemoveModeratorInfo()
        {
            this.Moderator = null;
            this.LastModerator = null;
            this.LockedBy = null;
            this.DeletedBy = null;
            this.ModCase.RemoveModeratorInfo();

            foreach (var comment in this.Comments)
            {
                comment.RemoveModeratorInfo(ModCase.UserId);
            }
        }
    }
}