using CubivoxServer.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using CubivoxServer;
using CubivoxServer.Players;
using CubivoxServer.Protocol.ClientBound;

namespace CubivoxServer.Protocol.ServerBound
{
    public class UpdatePlayerPosition : ServerBoundPacket
    {
        public bool ProcessPacket(Client client, NetworkStream stream)
        {
            if (client.ServerPlayer == null) return false;

            byte[] rawDouble = new byte[8];
            if(stream.Read(rawDouble, 0, 8) != 8) return false;
            double x = BitConverter.ToDouble(rawDouble);

            if (stream.Read(rawDouble, 0, 8) != 8) return false;
            double y = BitConverter.ToDouble(rawDouble);

            if (stream.Read(rawDouble, 0, 8) != 8) return false;
            double z = BitConverter.ToDouble(rawDouble);

            client.ServerPlayer.Location.Set(x, y, z);

            lock(ServerCubivox.GetServer().GetPlayers())
            {
                foreach(ServerPlayer player in ServerCubivox.GetServer().GetPlayers())
                {
                    if (player.Uuid == client.ServerPlayer.Uuid) continue;
                    player.SendPacket(new PlayerPositionUpdatePacket(client.ServerPlayer));
                }
            }

            return true;
        }

        byte Packet.GetType()
        {
            return 0x02;
        }
    }
}
