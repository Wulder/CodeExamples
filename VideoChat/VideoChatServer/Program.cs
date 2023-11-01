using System;

namespace VideoChatServer 
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Server serv = new Server();
            Console.ReadKey(false);
        }
    }
}