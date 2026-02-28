using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Shared
{
    public class Comment
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public string Content { get; set; } = string.Empty;
        public Post? Post { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
