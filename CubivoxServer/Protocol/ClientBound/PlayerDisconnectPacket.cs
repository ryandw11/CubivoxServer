using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CubivoxServer.Protocol.ClientBound
{
    public class PlayerDisconnectPacket : ClientBoundPacket
    {
        private Guid uuid;
        public PlayerDisconnectPacket(Guid uuid)
        {
            this.uuid = uuid;
        }

        public void WritePacket(NetworkStream stream)
        {
            stream.Write(uuid.ToByteArray());
        }

        byte Packet.GetType()
        {
            return 0x04;
        }
    }
}
