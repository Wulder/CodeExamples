using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace VideoChatServer
{
    internal class Client
    {
        public IPEndPoint RemotePoint => _remotePoint;
        private IPEndPoint _remotePoint;

        public Client(IPEndPoint rp)
        {
            _remotePoint = rp;
        }
    }
}
