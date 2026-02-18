using System.Net.Sockets;
using System.Text;

string serverIP = "127.0.0.1";
int port = 13000;

TcpClient client = new TcpClient();
await client.ConnectAsync(serverIP, port);

Console.WriteLine("Servera bağlanıldı.");

NetworkStream stream = client.GetStream();

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