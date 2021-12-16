using System.ComponentModel.DataAnnotations;

namespace MASZ.Models
{
    public class ModCaseComment : ICloneable
    {
        [Key]
        public int Id { get; set; }
        public ModCase ModCase { get; set; }
        public string Message { get; set; }
        public ulong UserId { get; set; }
        public DateTime CreatedAt { get; set; }

        public object Clone()
        {
            return new ModCaseComment
            {
                Id = Id,
                ModCase = ModCase,
                Message = Message,
                UserId = UserId,
                CreatedAt = CreatedAt
            };
        }

        public void RemoveModeratorInfo(ulong suspectId)
        {
            if (UserId != suspectId)
            {
                UserId = 0;
            }
        }
    }
}