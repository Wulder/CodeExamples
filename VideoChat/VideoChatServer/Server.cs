using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Configuration;
using System.Net;
using System.Diagnostics;


namespace VideoChatServer
{
    internal class Server
    {
        private UdpClient _udp;

        private Dictionary<IPEndPoint, Client> _clients = new Dictionary<IPEndPoint, Client>();

        public Server()
        {
            var port = 8080;
            if (int.TryParse(ConfigurationManager.AppSettings.Get("port"), out int parsedPort))
            {
                port = parsedPort;
            }
            while (true)
            {
                string inp = Console.ReadLine();
                if (int.TryParse(inp, out int resultPort))
                {
                    port = resultPort;
                    break;
                    
                }
                if (string.IsNullOrEmpty(inp))
                {
                    break;
                }
            }



            _udp = new UdpClient(port);
            _udp.Client.ReceiveBufferSize = 64000 * 10;

            Console.WriteLine($"Server started on {GetLocalIPAddress()}:{port}");

            Listen();
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
        private async void Listen()
        {
            Console.WriteLine("Start listening...");
            while (true)
            {
                try
                {
                    var result = await _udp.ReceiveAsync();
                    IPEndPoint from = result.RemoteEndPoint;
                    Packet pack = Packet.PacketFromBytes(result.Buffer);
                    pack.SetSender(from);

                    if (_clients.ContainsKey(from) || pack.Type == PacketType.Connect || pack.Type == PacketType.Disconnect)
                    {
                        HandlePacket(pack);
                        pack.ShowPacketData();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }



            }
        }


        private void AddClient(IPEndPoint ep)
        {
            if (_clients.ContainsKey(ep)) return;
            Client client = new Client(ep);
            _clients.Add(ep, client);
            Console.WriteLine($"Connected new client from {ep}");
        }

        private void RemoveClient(IPEndPoint ep)
        {
            if (!_clients.ContainsKey(ep)) return;
            _clients.Remove(ep);
            Console.WriteLine($"Disonnected client from {ep}");
        }

        private async void Send(Client client, Packet pack)
        {
            var bytes = await _udp.SendAsync(pack.GetBytes(), client.RemotePoint);
            Console.WriteLine($"sended: {bytes}");
        }

        private void HandlePacket(Packet pack)
        {
            switch (pack.Type)
            {
                case PacketType.Connect:
                    {
                        AddClient(pack.From);
                        break;
                    }
                case PacketType.Disconnect:
                    {
                        RemoveClient(pack.From);
                        break;
                    }
                case PacketType.Video:
                    {
                        var keys = _clients.Keys;

                        var key = keys.ToList().Find(i => i.Port != pack.From.Port);
                        if (key != null)
                            Send(_clients[key], pack);
                        // Send(_clients[pack.From], pack);
                        break;
                    }
                case PacketType.Audio:
                    {
                        var keys = _clients.Keys;

                        var key = keys.ToList().Find(i => i.Port != pack.From.Port);
                        if (key != null)
                            Send(_clients[key], pack);
                        //Send(_clients[pack.From], pack);
                        break;
                    }
                default:
                    {
                        Console.WriteLine("Received unknown type of package:");
                        pack.ShowPacketData();
                        break;
                    }

            }
        }

        private async void PeriodicSend()
        {

            while (true)
            {
                if (_clients.Count == 0) continue;
                var keys = _clients.Keys;
                var client = _clients.First();
                Send(client.Value, new Packet(PacketType.Video, VideoChatCore.FrameState.Uninitialized, new byte[1] { 88 }));
                Thread.Sleep(1000);
            }
        }
    }
}
