using System.Net.Sockets;
using System.Text;
using System.Threading;

string serverIP = "127.0.0.1";
int port = 13000;

TcpClient client = new TcpClient(serverIP, port);
Console.WriteLine("Servera bağlanıldı.");

NetworkStream stream = client.GetStream();

Thread receiveThread = new Thread(() => ReceiveMessages(stream));
receiveThread.Start();

while (true)
{
    string message = Console.ReadLine();
    byte[] data = Encoding.UTF8.GetBytes(message);
    stream.Write(data, 0, data.Length);
}

static void ReceiveMessages(NetworkStream stream)
{
    //byte olarak geliyo okumak için bufferliyoruz
    byte[] buffer = new byte[1024];
    int bytesRead;

    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
    {
        string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        Console.WriteLine("Server: " + data);
    }
}