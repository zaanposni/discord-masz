using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using masz.Dtos.ModCase;

namespace masz.Models
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