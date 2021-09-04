using System;
using System.ComponentModel.DataAnnotations;

namespace masz.Models
{
    public class ModCaseComment: ICloneable
    {
        [Key]
        public int Id { get; set; }
        public ModCase ModCase { get; set; }
        public string Message { get; set; }
        public ulong UserId { get; set; }
        public DateTime CreatedAt { get; set; }

        public object Clone()
        {
            return new ModCaseComment {
                Id = this.Id,
                ModCase = this.ModCase,
                Message = this.Message,
                UserId = this.UserId,
                CreatedAt = this.CreatedAt
            };
        }

        public void RemoveModeratorInfo(ulong suspectId)
        {
            if (this.UserId != suspectId) {
                this.UserId = 0;
            }
        }
    }
}