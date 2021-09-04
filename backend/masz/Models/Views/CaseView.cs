using System;
using System.Collections.Generic;
using DSharpPlus.Entities;

namespace masz.Models
{
    public class CaseView
    {
        public CaseView(ModCase modCase, DiscordUser moderator, DiscordUser lastModerator, DiscordUser suspect, List<CommentsView> comments)
        {
            ModCase = modCase;
            Moderator = moderator;
            LastModerator = lastModerator;
            Suspect = suspect;
            Comments = comments;

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
        public ModCase ModCase { get; set; }
        public DiscordUser Moderator { get; set; }
        public DiscordUser LastModerator { get; set; }
        public DiscordUser Suspect { get; set; }
        public DiscordUser LockedBy { get; set; }
        public DiscordUser DeletedBy { get; set; }
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
