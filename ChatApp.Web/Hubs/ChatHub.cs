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
        // benim ıd 1 diğer kullanıcı 2 ise 1_2 grubu oluşturulur 
        // aynı şekil onun içinde 1_2 2_1 olmaz 1_2 olur böylece iki kullanıcı arasında tek bir grup olur
        public async Task JoinPrivateChat(int userId, int otherUserId)
        {
            string groupName = userId < otherUserId ? $"{userId}_{otherUserId}" : $"{otherUserId}_{userId}";

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task SendPrivateMessage(int senderId, int receiverId, string message)
        {
            string groupName = senderId < receiverId ? $"{senderId}_{receiverId}" : $"{receiverId}_{senderId}";
            
            await Clients.Group(groupName).SendAsync("ReceivePrivateMessage", senderId, message);
        }
    }
}
