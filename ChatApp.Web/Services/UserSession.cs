namespace ChatApp.Web.Services
{
    public class UserSession
    {
        // giriş yapan kullanıcının adı
        public string? Username { get; set; }

        // kullanıcı giriş yaptı mı yapmadı mı kontrolü
        public bool IsLoggedIn => !string.IsNullOrEmpty(Username);
    }
}