using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Shared
{
    public class DirectMessage
    {
        public int Id { get; set; }
        public string Content { get; set; } = null!;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public int SenderId { get; set; }
        public User? Sender { get; set; } 

        public int ReceiverId { get; set; }
        public User? Receiver { get; set; }
    }
}
