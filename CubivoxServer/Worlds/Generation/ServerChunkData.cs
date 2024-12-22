using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CubivoxCore.Voxels;
using CubivoxCore.Worlds.Generation;

namespace CubivoxServer.Worlds.Generation
{
    public class ServerChunkData : ChunkData
    {
        internal byte[,,] Voxels = new byte[ServerChunk.CHUNK_SIZE, ServerChunk.CHUNK_SIZE, ServerChunk.CHUNK_SIZE];
        internal Dictionary<byte, short> VoxelMap = new Dictionary<byte, short> { { 0, 0 } };
        internal byte CurrentVoxelIndex = 1;

        public void SetVoxel(int x, int y, int z, VoxelDef voxelDef)
        {
            short voxelId = ServerCubivox.GetServer().GetServerItemRegistry().GetVoxelDefId(voxelDef);
            if (VoxelMap.ContainsValue(voxelId))
            {
                byte key = VoxelMap.First(pair => pair.Value == voxelId).Key;
                Voxels[x, y, z] = key;
            }
            else
            {
                VoxelMap[CurrentVoxelIndex] = voxelId;
                Voxels[x, y, z] = CurrentVoxelIndex;
                CurrentVoxelIndex++;
            }
        }
    }
}
