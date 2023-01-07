using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;

namespace CubivoxServer.Protocol
{
    public interface ClientBoundPacket : Packet
    {
        void WritePacket(NetworkStream stream);
    }
}
