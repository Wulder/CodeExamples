using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using VideoChatCore;

namespace VideoChatClient
{

    internal class Connection
    {

        public event Action<Packet> OnReceivePacket;

        private UdpClient _udpClient;
        private IPEndPoint _address;
        private int _sendPacketSize;
        public Connection(int sendPacketSize, IPEndPoint adress)
        {
            _address = adress;
            _udpClient = new UdpClient();
            _udpClient.Client.ReceiveBufferSize = 64000 * 10;
            SendPacket(Packet.Connect());
            _sendPacketSize = sendPacketSize;

            Listen();
            
        }

        public void Dispose()
        {
            SendPacket(Packet.Disconnect());
        }

        #region Sending
        public async void SendData(byte[] data)
        {

            IPEndPoint remotePoint = _address;
            var bytes = await _udpClient.SendAsync(data, remotePoint);
            
        }
        public void SendPacket(Packet pack)
        {
            SendData(pack.GetBytes());
        }
        public void SendFrame(byte[] data, PacketType type)
        {
            SendFrame(data, type, _sendPacketSize);
        }
        public void SendFrame(byte[] data, PacketType type, int packetSize)
        {
            if (data.Length + 2 <= packetSize)
            {
                SendPacket(new Packet(type, FrameState.Completed, data));
                    return;
            }

            int totalSize = data.Length;
            int sendedData = 0;
            
            int packetNum = 0;

            while (sendedData < totalSize)
            {
                int dataPointerBegin = packetNum * packetSize; 
                int dataPointerEnd = dataPointerBegin + packetSize;
                
                if (dataPointerEnd >= data.Length) dataPointerEnd = data.Length;
                Range range = new Range(dataPointerBegin, dataPointerEnd);
                sendedData += data[range].Length;


                FrameState state = sendedData < totalSize ? FrameState.Write : FrameState.Completed;

                Packet pack = new Packet(type, state, data[range]);
                SendPacket(pack);
                packetNum++;
                
            }

        }
        #endregion

        #region Receiving
        private async void Listen()
        {
            Console.WriteLine("Start listening...");
            while (true)
            {
                try
                {
                    
                        var result = await _udpClient.ReceiveAsync();
                        IPEndPoint from = result.RemoteEndPoint;
                        Packet pack = Packet.PacketFromBytes(result.Buffer);
                        pack.SetSender(from);
                        OnReceivePacket?.Invoke(pack);
                    
                    
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                
            }
        }
        #endregion
    }
}
