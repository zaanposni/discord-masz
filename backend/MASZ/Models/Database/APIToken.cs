using System.ComponentModel.DataAnnotations;

namespace MASZ.Models
{
    public class APIToken
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] TokenSalt { get; set; }
        public byte[] TokenHash { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ValidUntil { get; set; }
    }
}