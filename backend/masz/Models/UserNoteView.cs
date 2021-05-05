using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using masz.Dtos.DiscordAPIResponses;

namespace masz.Models
{
    public class UserNoteView
    {
        public UserNote UserNote { get; set; }
        public User User { get; set; }
        public User Moderator { get; set; }
    }
}
