using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using CubivoxCore;
using CubivoxCore.Events.Global;
using CubivoxCore.Events.Local;
using CubivoxCore.Voxels;

using CubivoxServer.Networking;
using CubivoxServer.Utils;
using CubivoxServer.Worlds;

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
            // Trigger Events
            VoxelDefPlaceEvent placeEvent = new VoxelDefPlaceEvent(client.ServerPlayer, new CubivoxCore.Location(x, y, z));
            Isolater.Isolate(() => voxelType._PlaceEvent?.Invoke(placeEvent));
            if (!placeEvent.IsCancelled)
            {
                VoxelPlaceEvent placeEvt = new VoxelPlaceEvent(client.ServerPlayer, new ServerVoxel(new CubivoxCore.Location(x, y, z), voxelType, null));
                if (Cubivox.GetEventManager().TriggerEvent(placeEvt))
                {
                    ServerCubivox.GetServer().GetWorlds()[0].SetVoxel(x, y, z, voxelType);
                }
            }    

            return true;
        }

        byte Packet.GetType()
        {
            return 0x04;
        }
    }
}
