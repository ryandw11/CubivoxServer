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
    public class SBBreakVoxelPacket : ServerBoundPacket
    {
        public async Task<bool> ProcessPacket(Client client, NetworkStream stream)
        {
            if (client.ServerPlayer == null) return false;

            byte[] rawInt = new byte[4];
            await NetworkUtil.FillBufferFromNetwork(rawInt, stream);
            int x = BitConverter.ToInt32(rawInt);

            await NetworkUtil.FillBufferFromNetwork(rawInt, stream);
            int y = BitConverter.ToInt32(rawInt);

            await NetworkUtil.FillBufferFromNetwork(rawInt, stream);
            int z = BitConverter.ToInt32(rawInt);

            // TODO :: Modify Server Voxels

            lock(ServerCubivox.GetServer().GetPlayers())
            {
                foreach(ServerPlayer player in ServerCubivox.GetServer().GetPlayers())
                {
                    if (player.Uuid == client.ServerPlayer.Uuid) continue;
                    player.SendPacket(new CBBreakVoxelPacket(x, y, z));
                }
            }

            return true;
        }

        byte Packet.GetType()
        {
            return 0x03;
        }
    }
}
