using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using masz.Dtos.DiscordAPIResponses;

namespace masz.Models
{
    public class CaseView
    {
        public CaseView(ModCase modCase, User moderator, User lastModerator, User suspect, List<CommentsView> comments)
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
        public User Moderator { get; set; }
        public User LastModerator { get; set; }
        public User Suspect { get; set; }
        public User LockedBy { get; set; }
        public User DeletedBy { get; set; }
        public List<CommentsView> Comments { get; set; }
        public double? PunishmentProgress { get; set; }
    }
}
