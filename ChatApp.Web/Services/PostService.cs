using ChatApp.Shared;
using ChatApp.Web.Data;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;

namespace ChatApp.Web.Services
{
    public class PostService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly AppDbContext _context;
        public PostService(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<bool> CreatePostAsync(Post post, string? fileName, byte[]? fileBytes)
        {
            try
            {
                if (fileBytes != null && !string.IsNullOrEmpty(fileName))

                {
                    var rootPath = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    var folderPath = Path.Combine(rootPath, "uploads");

                    if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                    var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";//benzersiz 128 bitlik id oluşturur
                    var fullPath = Path.Combine(folderPath, uniqueFileName);

                    await File.WriteAllBytesAsync(fullPath, fileBytes);
                    // using kullanmımı dosyanın işi bitince oto kapanmasını sağlar oto bellekten atılır

                    post.ImageUrl = uniqueFileName;

                }

                _context.Posts.Add(post);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"!!! HATA: {ex.Message}");
                return false;
            }
        }
        public async Task<List<Post>> GetAllPostsAsync()
        {
            return await _context.Posts
                .Include(p => p.User)//Include kullanarak Post ile beraber User bilgisini de çekiyoruz join
                .Include(p => p.Likes)
                .Include(p => p.Comments) // Postun yorumları
                    .ThenInclude(c => c.User) //Yorumun sahibi JOIN içinde JOIN
                .AsSplitQuery()
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }
        public async Task<bool> ToggleLikeAsync(int postId, int userId)
        {
            try
            {
                //Veritabanında bu beğeninin olup olmadığını kontrol et
                var existingLike = await _context.Likes
                    .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);

                if (existingLike != null)
                {
                    // Varsa beğeniyi kaldır
                    _context.Likes.Remove(existingLike);
                }
                else
                {
                    // Yoksa yeni beğeni oluştur
                    var newLike = new Like
                    {
                        PostId = postId,
                        UserId = userId
                    };
                    _context.Likes.Add(newLike);
                }

                //  Değişiklikleri veritabanına işle
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Beğeni hatası: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> AddCommentAsync(int postId, int userId, string content)
        {
            try
            {
                var newComment = new Comment
                {
                    PostId = postId,
                    UserId = userId,
                    Content = content,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Comments.Add(newComment);
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<User?> GetUserProfileAsync(int userId)
        { 
            return await _context.Users
                .Include(u => u.Posts)
                .Include(u => u.Likes) // Beğendiği postların sayısını almak için
                .Include(u => u.Comments)
                .Include(u => u.Followers) // Beni takip edenler
                .Include(u => u.Following)
                .AsSplitQuery()//profil yükleniyorda kaldı performans için
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
        public async Task<bool> ToggleFollowAsync(int followerId, int followingId)
        {
            if (followerId == followingId) return false;

            try
            {
                var existingFollow = await _context.Follows
                    .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);

                if(existingFollow != null)
                {
                    _context.Follows.Remove(existingFollow);
                }
                else
                {
                    var newFollow = new Follow
                    {
                        FollowerId = followerId,
                        FollowingId = followingId
                    };
                    _context.Follows.Add(newFollow);
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Takip hatası: {ex.Message}");
                return false;
            }
        }

        public async Task<Post?> GetPostByIdAsync(int postId)
        {
            return await _context.Posts
                .Include(p => p.User)    // Postu kim atmış beğenileri yorumları
                .Include(p => p.Likes)   
                .Include(p => p.Comments) 
                    .ThenInclude(c => c.User) // Yorumları yapan kullanıcıların bilgilerini de getir 
                .FirstOrDefaultAsync(p => p.Id == postId);
        }
        public async Task<bool> DeletePostAsync(int postId, int userId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post==null || post.UserId != userId)
            {
                return false;
            }
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
