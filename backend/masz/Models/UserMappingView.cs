using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using masz.Dtos.DiscordAPIResponses;

namespace masz.Models
{
    public class UserMappingView
    {
        public UserMapping UserMapping { get; set; }
        public User UserA { get; set; }
        public User UserB { get; set; }
        public User Moderator { get; set; }
    }
}
