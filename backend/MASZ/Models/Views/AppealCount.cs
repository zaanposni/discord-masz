namespace MASZ.Models
{
    public class AppealCount
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int PendingCount { get; set; }
        public int ApprovedCount { get; set; }
        public int DeclinedCount { get; set; }
    }
}
