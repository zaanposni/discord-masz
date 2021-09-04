using System;
using DSharpPlus.Entities;

namespace masz.Models
{
    public class CommentsView
    {
        public int Id { get; set; }
        public String Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public ulong UserId { get; set; }
        public DiscordUser User { get; set; }

        public void RemoveModeratorInfo(ulong suspectId)
        {
            if (UserId != suspectId) {
                UserId = 0;
                User = null;
            }
        }
    }
}
