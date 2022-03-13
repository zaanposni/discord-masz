using System.ComponentModel.DataAnnotations;

namespace MASZ.Models
{
    public class AppealStructure
    {
        [Key]
        public int Id { get; set; }
        public ulong GuildId { get; set; }
        public int SortOrder { get; set; }
        public string Question { get; set; }
        public bool Deleted { get; set; }
    }
}