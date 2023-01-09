namespace MASZ.Dtos.VerifiedEvidence
{
    public class VerifiedEvidenceTableFilterDto
    {
        public string CustomTextFilter { get; set; }
        public List<string> ReportedIds { get; set; }
        public List<string> ModIds { get; set; }
    }
}
