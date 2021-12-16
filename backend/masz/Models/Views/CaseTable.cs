namespace MASZ.Models
{
    public class CaseTable
    {
        public List<ModCaseTableEntry> Cases { get; set; }
        public int FullSize { get; set; }
        public CaseTable(List<ModCaseTableEntry> modCase, int fullSize)
        {
            Cases = modCase;
            FullSize = fullSize;
        }
    }
}
