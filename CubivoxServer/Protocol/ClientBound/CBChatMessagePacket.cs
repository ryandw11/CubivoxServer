using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CubivoxServer.Protocol.ClientBound
{
    public class CBChatMessagePacket : ClientBoundPacket
    {
        private string message;

        public CBChatMessagePacket(string message)
        {
            this.message = message;
        }

        public void WritePacket(NetworkStream stream)
        {
            stream.WriteByte(0);
            stream.Write(BitConverter.GetBytes((short) message.Length));
            stream.Write(Encoding.ASCII.GetBytes(message));
        }

        byte Packet.GetType()
        {
            return 0x09;
        }
    }
}
