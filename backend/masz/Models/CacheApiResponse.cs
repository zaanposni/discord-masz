using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace masz.Models
{
    public class CacheApiResponse
    {
        public string Content { get; set; }
        public DateTime ExpiresAt { get; set; }

        public CacheApiResponse(string content) {
            this.Content = content;
            this.ExpiresAt = DateTime.Now.AddMinutes(15);
        }
    }
}
