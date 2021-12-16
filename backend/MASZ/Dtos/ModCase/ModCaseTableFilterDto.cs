using MASZ.Enums;

namespace MASZ.Dtos.ModCase
{
    public class ModCaseTableFilterDto
    {
        public string CustomTextFilter { get; set; }
        public List<string> UserIds { get; set; }
        public List<string> ModeratorIds { get; set; }
        public DateTime Since { get; set; }
        public DateTime Before { get; set; }
        public DateTime PunishedUntilMin { get; set; }
        public DateTime PunishedUntilMax { get; set; }
        public bool? Edited { get; set; }
        public List<CaseCreationType> CreationTypes { get; set; }
        public List<PunishmentType> PunishmentTypes { get; set; }
        public bool? PunishmentActive { get; set; }
        public bool? LockedComments { get; set; }
        public bool? MarkedToDelete { get; set; }
    }
}