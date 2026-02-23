using System.Net.Sockets;
using System.Text;

string serverIP = "127.0.0.1";
int port = 13000;

TcpClient client = new TcpClient();
await client.ConnectAsync(serverIP, port);

Console.WriteLine("Servera bağlanıldı.");

NetworkStream stream = client.GetStream();

Console.WriteLine("1-Giriş Yap");
Console.WriteLine("2-Kayıt Ol");

string secim = Console.ReadLine();

Console.Write("Kullanıcı adınızı girin: ");
string username = Console.ReadLine();

Console.Write("Şifrenizi girin: ");
string password = Console.ReadLine();

if (secim == "1")
{
    string loginMessage = $"LOGIN|{username}|{password}";
    byte[] data = Encoding.UTF8.GetBytes(loginMessage);

    await stream.WriteAsync(data, 0, data.Length);
}
else if (secim == "2")
{
    string registerMessage = $"REGISTER|{username}|{password}";
    byte[] data = Encoding.UTF8.GetBytes(registerMessage);

    await stream.WriteAsync(data, 0, data.Length);
}
else
{
    Console.WriteLine("Geçersiz seçim. Program sonlandırılıyor.");
    return;
}

//buraya bi buffer ekleyip serverdan gelen cevaba göre giriş başarılı mı değil mi 
byte[] buffer = new byte[1024];
int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
Console.WriteLine("Server: " + response);

if (!response.StartsWith("OK"))
{
    Console.WriteLine("Giriş başarısız.");
    return;
}

// Thread yerine Task 
_ = ReceiveMessagesAsync(stream);

while (true)
{
    string message = Console.ReadLine();

    byte[] data = Encoding.UTF8.GetBytes(message);
    await stream.WriteAsync(data, 0, data.Length);
}

static async Task ReceiveMessagesAsync(NetworkStream stream)
{
    byte[] buffer = new byte[1024];
    int bytesRead;

    while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
    {
        string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        Console.WriteLine("Server: " + data);
    }
}

