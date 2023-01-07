using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CubivoxServer.Protocol.ClientBound
{
    public class DisconnectPacket : ClientBoundPacket
    {
        private string reason;
        public DisconnectPacket(string reason)
        {
            this.reason = reason;
        }

        public void WritePacket(NetworkStream stream)
        {
            stream.Write(BitConverter.GetBytes((short)reason.Length), 0, 2);
            stream.Write(Encoding.ASCII.GetBytes(reason));
        }

        byte Packet.GetType()
        {
            return 0x05;
        }
    }
}
