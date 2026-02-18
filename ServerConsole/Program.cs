using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

Int32 port = 13000;
IPAddress localAddr = IPAddress.Parse("127.0.0.1");
TcpListener server = new TcpListener(localAddr, port);

server.Start();
Console.WriteLine("Server Başlatıldı ");
using TcpClient client = server.AcceptTcpClient();
NetworkStream stream = client.GetStream();

Thread receiveThread = new Thread(() => ReceiveMessages(stream));
receiveThread.Start();

//thread sınıfı metoda param gönderebilmek için tek tip kabul ediyor o yüzden object alıyoruz
//biz de metodun içinde (TcpClient)obj yazarak, aslında gönderdiğimiz nesnenin gerçek tipinin TcpClient olduğunu söyleyip tekrar kendi tipine çeviriyoruz
//yada lambda ifadesi kullanarak direkt olarak TcpClient nesnesini kullanabiliriz

while (true)
{
    string message = Console.ReadLine();
    byte[] data = Encoding.UTF8.GetBytes(message);
    stream.Write(data, 0, data.Length);
}
static void ReceiveMessages(NetworkStream stream)
{
    
    int bytesRead;
    byte[] buffer = new byte[1024];

    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
    {
        string data;
        data = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);
        Console.WriteLine("Gelen mesaj " + data);

     
    }
}
