using System.ComponentModel.DataAnnotations;

namespace MASZ.Models
{
    public class AppealAnswer
    {
        [Key]
        public int Id { get; set; }
        public Appeal Appeal { get; set; }
        public AppealStructure AppealQuestion { get; set; }
        public string Answer { get; set; }
    }
}