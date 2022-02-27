using MASZ.Enums;

namespace MASZ.Dtos.Appeal
{
    public class AppealFilterDto
    {
        public List<string> UserIds { get; set; }
        public DateTime Since { get; set; }
        public DateTime Before { get; set; }
        public bool? Edited { get; set; }
        public List<AppealStatus> Status { get; set; }
    }
}