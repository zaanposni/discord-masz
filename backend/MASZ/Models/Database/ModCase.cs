using MASZ.Enums;
using MASZ.Services;
using System.ComponentModel.DataAnnotations;
using Discord;
using MASZ.Extensions;

namespace MASZ.Models
{
    public class ModCase : ICloneable
    {
        [Key]
        public int Id { get; set; }
        public int CaseId { get; set; }
        public ulong GuildId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ulong UserId { get; set; }
        public string Username { get; set; }
        public string Discriminator { get; set; }
        public string Nickname { get; set; }
        public ulong ModId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime OccuredAt { get; set; }
        public DateTime LastEditedAt { get; set; }
        public ulong LastEditedByModId { get; set; }
        public string[] Labels { get; set; }
        public string Others { get; set; }
        public bool Valid { get; set; }
        public CaseCreationType CreationType { get; set; }
        public PunishmentType PunishmentType { get; set; }
        public DateTime? PunishedUntil { get; set; }
        public bool PunishmentActive { get; set; }
        public bool AllowComments { get; set; } = true;
        public ulong LockedByUserId { get; set; }
        public DateTime? LockedAt { get; set; }
        public DateTime? MarkedToDeleteAt { get; set; }
        public ulong DeletedByUserId { get; set; }
        public ICollection<ModCaseComment> Comments { get; set; }
        public ICollection<ModCaseMapping> MappingsA { get; set; }
        public ICollection<ModCaseMapping> MappingsB { get; set; }


        public object Clone()
        {
            return new ModCase
            {
                Id = Id,
                CaseId = CaseId,
                GuildId = GuildId,
                Title = Title,
                Description = Description,
                UserId = UserId,
                Username = Username,
                Nickname = Nickname,
                ModId = ModId,
                CreatedAt = CreatedAt,
                OccuredAt = OccuredAt,
                LastEditedAt = LastEditedAt,
                LastEditedByModId = LastEditedByModId,
                Labels = Labels,
                Others = Others,
                Valid = Valid,
                PunishmentType = PunishmentType,
                PunishedUntil = PunishedUntil,
                PunishmentActive = PunishmentActive,
                AllowComments = AllowComments,
                LockedByUserId = LockedByUserId,
                LockedAt = LockedAt,
                MarkedToDeleteAt = MarkedToDeleteAt,
                DeletedByUserId = DeletedByUserId,
                Comments = Comments,
            };
        }

        public string GetPunishment(Translator translator)
        {
            return translator.T().Enum(PunishmentType);
        }

        public string GetEmbedData(string baseUrl, IUser user, Translator translator)
        {
            return
                "<html>" +
                    "<head>" +
                        $"<meta name=\"theme-color\" content=\"#3498db\">" +
                        $"<meta property=\"og:site_name\" content=\"MASZ by zaanposni\" />" +
                        $"<meta property=\"og:title\" content=\"#{this.CaseId}: {this.Title}\" />" +
                        $"<meta property=\"og:url\" content=\"{baseUrl}/guilds/{this.GuildId}/cases/{this.CaseId}\" />" +
                        $"<meta property=\"og:description\" content=\"{this.GetPunishment(translator)}: {this.Description}\" />" +
                        ( user != null ? $"<meta property=\"og:image\" content=\"{user.GetAvatarOrDefaultUrl()}\" />" : "") +
                    "</head>" +
                "</html>";
        }
    }
}