using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using masz.Dtos.DiscordAPIResponses;

namespace masz.Models
{
    public class CommentsView
    {
        public int Id { get; set; }
        public String Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public String UserId { get; set; }
        public User User { get; set; }

        public void RemoveModeratorInfo(String suspectId)
        {
            if (this.UserId != suspectId) {
                this.UserId = null;
                this.User = null;
            }
        }
    }
}
