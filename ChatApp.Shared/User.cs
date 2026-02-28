using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ChatApp.Shared
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        [Required] //Data Annotations denir hem veritabanında kısıtlama oluşturur (notnull) hem de blazor formunda kullanıcı boş geçerse otomatik hata mesajı
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<Post> Posts { get; set; } = new();
        public List<Comment> Comments { get; set; } = new();
        public List<Like> Likes { get; set; } = new();
        public string? ProfilePictureUrl { get; set; }
        public List<Follow> Followers { get; set; } = new();
        public List<Follow> Following { get; set; } = new();
        public string? Bio { get; set; }
    }
}
