using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Data.SqlClient;

Int32 port = 13000;
IPAddress localAddr = IPAddress.Parse("127.0.0.1");
TcpListener server = new TcpListener(localAddr, port);

server.Start();
Console.WriteLine("Server Başlatıldı");

using TcpClient client = await server.AcceptTcpClientAsync();
NetworkStream stream = client.GetStream();

// Thread yerine Task
_ = ReceiveMessagesAsync(stream);

while (true)
{
    string message = Console.ReadLine();
    byte[] data = Encoding.UTF8.GetBytes(message);
    await stream.WriteAsync(data, 0, data.Length);
}

async Task ReceiveMessagesAsync(NetworkStream stream)
{
    byte[] buffer = new byte[1024];
    int bytesRead;

    while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
    {
        string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        Console.WriteLine("Gelen mesaj " + data);
        
        string[] parts = data.Split('|'); //login|ahmetk|1234 şeklinde geliyo


        if (parts[0]== "REGISTER")
        {
            bool result = await RegisterUser(parts[1], parts[2]);

            string response = result ? "OK REGISTER" : "ERROR REGISTER";
            byte[] responseData = Encoding.UTF8.GetBytes(response);
            await stream.WriteAsync(responseData, 0, responseData.Length);

        }
        else if(parts[0] == "LOGIN")
        {
            bool result = await LoginUser(parts[1], parts[2]);
            string response = result ? "OK LOGIN" : "ERROR LOGIN";
            byte[] responseData = Encoding.UTF8.GetBytes(response);
            await stream.WriteAsync(responseData, 0, responseData.Length);
        }
        
    }
}

async Task<bool> RegisterUser(string username, string password)
{

    string dbConnectionString = "Server=localhost;Database=TcpChatDb;Trusted_Connection=True;TrustServerCertificate=True;";

    using SqlConnection connection = new SqlConnection(dbConnectionString);
    await connection.OpenAsync();

    //buraya aynı kullanıcı adı var mı kontrolü eklenebilir

    string insertQuery = "INSERT INTO dbo.Users (Username, Password) VALUES (@Username, @Password)";
    using SqlCommand command = new SqlCommand(insertQuery, connection);
    command.Parameters.AddWithValue("@Username", username);
    command.Parameters.AddWithValue("@Password", password);

    await command.ExecuteNonQueryAsync();
    return true;
}

async Task<bool> LoginUser(string username, string password)
{
    string dbConnectionString = "Server=localhost;Database=TcpChatDb;Trusted_Connection=True;TrustServerCertificate=True;";
    using SqlConnection connection = new SqlConnection(dbConnectionString);
    await connection.OpenAsync();
    string selectQuery = "SELECT COUNT(*) FROM dbo.Users WHERE Username = @Username AND Password = @Password";
    using SqlCommand command = new SqlCommand(selectQuery, connection);
    command.Parameters.AddWithValue("@Username", username);
    command.Parameters.AddWithValue("@Password", password);
    int userCount = (int)await command.ExecuteScalarAsync();
    return userCount > 0;
}