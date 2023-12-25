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
using CubivoxCore.BaseGame;

namespace CubivoxServer.Protocol.ServerBound
{
    public class SBPlaceVoxelPacket : ServerBoundPacket
    {
        public async Task<bool> ProcessPacket(Client client, NetworkStream stream)
        {
            if (client.ServerPlayer == null) return false;

            byte[] rawShort = new byte[2];
            await NetworkUtil.FillBufferFromNetwork(rawShort, stream);
            short type = BitConverter.ToInt16(rawShort);


            byte[] rawInt = new byte[4];
            await NetworkUtil.FillBufferFromNetwork(rawInt, stream);
            int x = BitConverter.ToInt32(rawInt);

            await NetworkUtil.FillBufferFromNetwork(rawInt, stream);
            int y = BitConverter.ToInt32(rawInt);

            await NetworkUtil.FillBufferFromNetwork(rawInt, stream);
            int z = BitConverter.ToInt32(rawInt);

            VoxelDef voxelType = ServerCubivox.GetServer().GetServerItemRegistry().GetVoxelDef(type);
            ServerCubivox.GetServer().GetWorlds()[0].SetVoxel(x, y, z, voxelType);

            return true;
        }

        byte Packet.GetType()
        {
            return 0x04;
        }
    }
}
