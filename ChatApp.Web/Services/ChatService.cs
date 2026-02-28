using ChatApp.Web.Data;
using Microsoft.EntityFrameworkCore;
using ChatApp.Shared;

namespace ChatApp.Web.Services
{
    public class ChatService
    {
        private readonly AppDbContext _context;
        public ChatService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<DirectMessage>> GetDirectMessagesAsync(int currentUserId, int otherUserId)
        {
            return await _context.DirectMessages
        .Include(m => m.Sender)
        .Where(m => (m.SenderId == currentUserId && m.ReceiverId == otherUserId)
                 || (m.SenderId == otherUserId && m.ReceiverId == currentUserId))
        .OrderBy(m => m.SentAt)
        .ToListAsync();
        }

        public async Task<bool> SaveMessageAsync(DirectMessage message)
        {
            try
            {
                message.SentAt = DateTime.UtcNow;
                _context.DirectMessages.Add(message);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Mesaj kaydetme hatası: {ex.Message}");
                return false;

            }
        }
        public async Task<List<User>> GetRecentChatAsync(int userId)
        {
            //gönderdiğim mesajlardan ve aldığım mesajlardan karşı tarafın idlerini al

            var sentToIds = await _context.DirectMessages
                .Where(m => m.SenderId == userId)
                .Select(m => m.ReceiverId)
                .ToListAsync();
            var receivedFromIds = await _context.DirectMessages
                .Where(m => m.ReceiverId == userId)
                .Select(m => m.SenderId)
                .ToListAsync();

            var allChatUserIds = sentToIds.Concat(receivedFromIds).Distinct().ToList(); //idleri birleştir ve tekilleştir

            //bu idlere sahip kullanıcıları getir
            return await _context.Users
                .Where(u => allChatUserIds.Contains(u.Id))
                .ToListAsync();
        }
    }
}
