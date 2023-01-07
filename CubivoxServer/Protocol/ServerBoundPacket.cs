using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;

using CubivoxServer.Networking;
using CubivoxServer.Players;

namespace CubivoxServer.Protocol
{
    public interface ServerBoundPacket : Packet
    {
        bool ProcessPacket(Client client, NetworkStream stream);
    }
}
