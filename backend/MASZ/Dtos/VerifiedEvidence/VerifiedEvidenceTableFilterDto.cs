namespace MASZ.Dtos.VerifiedEvidence
{
    public class VerifiedEvidenceTableFilterDto
    {
        public string CustomTextFilter { get; set; }
        public List<string> ReporterIds { get; set; }
        public List<string> ReportedIds { get; set; }
    }
}
