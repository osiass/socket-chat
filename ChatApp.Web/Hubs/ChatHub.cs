using ChatApp.Shared;
using ChatApp.Web.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
namespace ChatApp.Web.Hubs
{
    public class ChatHub : Hub 
    {

        private readonly AppDbContext _context;

        public ChatHub(AppDbContext context)
        {
            _context = context;
        }
        public async Task SendMessage(string username, string message)
        {
            // Mesajı herkese gönder 
            await Clients.All.SendAsync("ReceiveMessage", username, message);
            // Veritabanına kaydetmeyi dene
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user != null)
            {
                var newMessage = new Message
                {
                    Content = message,
                    UserId = user.Id,
                    SentAt = DateTime.UtcNow
                };
                _context.Messages.Add(newMessage);
                await _context.SaveChangesAsync();
                // bağlı olan herkese yayınla broadcast
                await Clients.All.SendAsync("ReceiveMessage", username, message);
            }
        }
    }
}
