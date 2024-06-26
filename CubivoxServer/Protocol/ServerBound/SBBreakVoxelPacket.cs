﻿using CubivoxServer.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using CubivoxCore;
using CubivoxCore.Voxels;

using CubivoxServer.Utils;
using CubivoxServer.Worlds;
using CubivoxCore.Events.Local;
using CubivoxCore.Events.Global;

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

            VoxelDef voxelType = ServerCubivox.GetServer().GetServerItemRegistry().GetVoxelDef(0);

            ServerWorld world = ServerCubivox.GetServer().GetWorlds()[0];
            Voxel existingVoxel = world.GetVoxel(x, y, z);

            // Events
            VoxelDefBreakEvent voxelDefBreakEvent = new VoxelDefBreakEvent(client.ServerPlayer, new CubivoxCore.Location(x, y, z));
            Isolater.Isolate(() => existingVoxel.GetVoxelDef()._BreakEvent?.Invoke(voxelDefBreakEvent));

            if (!voxelDefBreakEvent.IsCancelled)
            {
                VoxelBreakEvent breakEvent = new VoxelBreakEvent(client.ServerPlayer, existingVoxel);
                if(Cubivox.GetEventManager().TriggerEvent(breakEvent))
                {
                    world.SetVoxel(x, y, z, voxelType);
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
