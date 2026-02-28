using ChatApp.Shared;
using ChatApp.Web.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
namespace ChatApp.Web.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        //dependency injection ile db alıyoruz, 
        private readonly UserSession _userSession;
        public AuthService(AppDbContext context, UserSession userSession)
        {
            _context = context;
            _userSession = userSession;
        }
        public async Task<bool> RegisterUserAsync(string username, string password)
        {
            bool userExists = await _context.Users.AnyAsync(u => u.Username == username); //aynı kullanıcı adı var mı kontrolü
            if (userExists)
            {
                return false;
            }

            var newuser = new User
            {
                Username = username,
                PasswordHash = HashPassword(password),
                CreatedAt = DateTime.UtcNow
            };
            _context.Users.Add(newuser);//veritabanına ekleme işlemi
            await _context.SaveChangesAsync(); //sqldeki insert işlemi
            return true;

        }
        public async Task<bool> LoginUserAsync(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return false;
            }
            string enteredPasswordHash = HashPassword(password);
            if (user.PasswordHash == enteredPasswordHash)
            {
                _userSession.UserId = user.Id;
                _userSession.Username = user.Username;

                return true;
            }
            else
                return false;
        }
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();

            return Convert.ToBase64String(bytes);
        }
    }
}