using System.Net;
using System.Net.Sockets;
using System.Text;

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

static async Task ReceiveMessagesAsync(NetworkStream stream)
{
    byte[] buffer = new byte[1024];
    int bytesRead;

    while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
    {
        string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        Console.WriteLine("Gelen mesaj " + data);
    }
}