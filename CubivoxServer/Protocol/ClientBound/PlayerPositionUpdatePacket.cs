using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using CubivoxCore.Players;

namespace CubivoxServer.Protocol.ClientBound
{
    public class PlayerPositionUpdatePacket : ClientBoundPacket
    {
        private Player player;

        public PlayerPositionUpdatePacket(Player player)
        {
            this.player = player;
        }

        public void WritePacket(NetworkStream stream)
        {
            stream.Write(player.GetUUID().ToByteArray());
            stream.Write(BitConverter.GetBytes(player.GetLocation().x));
            stream.Write(BitConverter.GetBytes(player.GetLocation().y));
            stream.Write(BitConverter.GetBytes(player.GetLocation().z));
        }

        byte Packet.GetType()
        {
            return 0x03;
        }
    }
}
