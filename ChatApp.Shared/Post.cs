using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Shared
{
    public class Post
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? ImageUrl { get; set; } // Gönderiye ait resim URL
        public User? User { get; set; }
        public int UserId { get; set; } // Gönderiyi oluşturan kullanıcının ID'si
        public List<Comment> Comments { get; set; } = new List<Comment>(); // Gönderiye ait yorumlar
        public List<Like> Likes { get; set; } = new();
    }
}
