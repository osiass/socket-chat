using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Shared
{
    public class Like
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        public Post? Post { get; set; }
        public User? User { get; set; }
    }
}
