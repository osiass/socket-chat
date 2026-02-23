using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ChatApp.Shared
{
    public class Message
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.UtcNow; //ne zamana atıldı
        public int UserId { get; set; } //gönderen kullanıcı idsi
        [ForeignKey("UserId")]// kod yazarken kullanıcının adına ulaşırırz
        public User? User { get; set; }//sqlde join yapma 
    }
}
