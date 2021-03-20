using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using masz.Dtos.DiscordAPIResponses;

namespace masz.Models
{
    public class CommentListView
    {
        public CommentListView(ModCaseComment comment, User commentor)
        {
            Comment = comment;
            Commentor = commentor;
        }
        public ModCaseComment Comment { get; set; }
        public User Commentor { get; set; }
    }
}
