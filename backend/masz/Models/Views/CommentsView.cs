using System;
using masz.Models.Views;

namespace masz.Models
{
    public class CommentsView
    {
        public int Id { get; set; }
        public String Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; }

        public CommentsView(ModCaseComment comment)
        {
            Id = comment.Id;
            Message = comment.Message;
            CreatedAt = comment.CreatedAt;
            UserId = comment.UserId.ToString();
        }

        public void RemoveModeratorInfo(string suspectId)
        {
            if (UserId != suspectId) {
                UserId = null;
            }
        }
    }
}
