using MASZ.Enums;

namespace MASZ.Models
{
    public class CaseCount
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int WarnCount { get; set; }
        public int MuteCount { get; set; }
        public int KickCount { get; set; }
        public int BanCount { get; set; }
    }
}
