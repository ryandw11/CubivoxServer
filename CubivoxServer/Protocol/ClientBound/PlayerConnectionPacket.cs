using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using CubivoxCore.Players;

namespace CubivoxServer.Protocol.ClientBound
{
    public class PlayerConnectionPacket : ClientBoundPacket
    {
        private Player player;

        public PlayerConnectionPacket(Player player)
        {
            this.player = player;
        }

        public void WritePacket(NetworkStream stream)
        {
            byte[] name = new byte[25];
            byte[] nameBuff = Encoding.ASCII.GetBytes(player.GetName());
            Array.Copy(nameBuff, name, nameBuff.Length < 26 ? nameBuff.Length : 25);
            stream.Write(name, 0, 25);
            // Pad out the name if it is less that 25 characters.
            if (name.Length < 25)
            {
                byte[] zeroArray = new byte[25 - name.Length];
                stream.Write(zeroArray, 0, zeroArray.Length);
            }

            stream.Write(player.GetUUID().ToByteArray(), 0, player.GetUUID().ToByteArray().Length);
            stream.Write(BitConverter.GetBytes(player.GetLocation().x), 0, 8);
            stream.Write(BitConverter.GetBytes(player.GetLocation().y), 0, 8);
            stream.Write(BitConverter.GetBytes(player.GetLocation().z), 0, 8);
        }

        byte Packet.GetType()
        {
            return 0x02;
        }
    }
}
