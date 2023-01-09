namespace MASZ.Models.Views
{
    public class VerifiedEvidenceTable
    {
        public List<VerifiedEvidenceTableEntry> Evidence { get; set; }
        public int FullSize { get; set; }

        public VerifiedEvidenceTable(List<VerifiedEvidenceTableEntry> evidence, int fullSize) 
        {
            Evidence = evidence;
            FullSize = fullSize;
        }
    }
}
