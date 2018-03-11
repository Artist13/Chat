using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace ClientChat
{
    class Program
    {
        static string UserName;
        static string host;
        private const int port = 8080;
        static TcpClient Client;
        static NetworkStream Stream;

        static void Main(string[] args)
        {
            host = "127.0.0.1";
            Console.WriteLine("Введите имя: ");
            UserName = Console.ReadLine();
            Console.WriteLine("Введите адрес сервера: ");
            host = Console.ReadLine();
            Client = new TcpClient();
            try
            {
                Client.Connect(IPAddress.Parse(host), port);
                Stream = Client.GetStream();

                string Message = UserName;
                byte[] buffer = Encoding.Unicode.GetBytes(Message);
                Stream.Write(buffer, 0, buffer.Length);

                Thread RecieveThread = new Thread(new ThreadStart(RecieveMessage));
                RecieveThread.Start();

                Console.WriteLine("Добро пожаловать, {0}", UserName);
                SendMessage();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect();
            }
        }

        static void SendMessage()
        {
            Console.WriteLine("Введите сообщение: ");
            while(true)
            {
                string Message = Console.ReadLine();
                byte[] buffer = Encoding.Unicode.GetBytes(Message);
                Stream.Write(buffer, 0, buffer.Length);
            }
        }

        static void RecieveMessage()
        {
            while(true)
            {
                try
                {
                    byte[] buffer = new byte[64];
                    StringBuilder Builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = Stream.Read(buffer, 0, buffer.Length);
                        Builder.Append(Encoding.Unicode.GetString(buffer, 0, bytes));
                    } while (Stream.DataAvailable);

                    string Message = Builder.ToString();
                    Console.WriteLine(Message);
                }
                catch
                {
                    Console.WriteLine("Подключение прервано");
                    Console.ReadLine();
                    Disconnect();
                }
            }
        }
        
        static void Disconnect()
        {
            if(Stream != null)
            {
                Stream.Close();
            }
            if(Client != null)
            {
                Client.Close();
            }
            Environment.Exit(0);
        }

    }
}
