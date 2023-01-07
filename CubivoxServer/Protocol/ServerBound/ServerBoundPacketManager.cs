using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubivoxServer.Protocol.ServerBound
{
    public class ServerBoundPacketManager
    {
        Dictionary<ushort, ServerBoundPacket> packets;

        public ServerBoundPacketManager()
        {
            packets = new Dictionary<ushort, ServerBoundPacket>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException">If the packet is not found.</exception>
        public ServerBoundPacket GetPacket(ushort id)
        {
            return packets[id];
        }

        public void RegisterPacket(ServerBoundPacket serverBoundPacket)
        {
            packets.Add(serverBoundPacket.GetType(), serverBoundPacket);
        }
    }
}
