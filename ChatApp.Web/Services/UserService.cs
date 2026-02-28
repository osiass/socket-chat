using ChatApp.Web.Data;
using ChatApp.Shared; 
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Web.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        public UserService(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        public async Task UpdateProfileAsync(int userId, string username, string bio, byte[]? imageBytes, string? fileName)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user != null)
            {
                user.Username = username;
                user.Bio = bio;

                if (imageBytes != null && !string.IsNullOrEmpty(fileName))
                {
                    var rootPath = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    var folderPath = Path.Combine(rootPath, "profile-images");

                    if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                    var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
                    var fullPath = Path.Combine(folderPath, uniqueFileName);

                    await File.WriteAllBytesAsync(fullPath, imageBytes);

                    user.ProfilePictureUrl = uniqueFileName;
                }

                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<User>> SearchUsersAsync(string query, int currentUserId)
        {
            if(string.IsNullOrWhiteSpace(query)) return new List<User>();

            return await _context.Users
                .Where(u => u.Id != currentUserId && u.Username.Contains(query))
                .Take(10) //ilk 10 sonucu getir
                .ToListAsync();

        }
    }
}