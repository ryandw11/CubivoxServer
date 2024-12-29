using CubivoxCore;
using CubivoxCore.Utils;
using CubivoxCore.Voxels;
using CubivoxCore.Worlds;
using CubivoxServer.Utils;

namespace CubivoxServer.Worlds
{
    public class ServerChunkBulkEditor : ChunkBulkEditor
    {
        private static readonly int TOTAL_CHUNK_SIZE = ChunkLocation.CHUNK_SIZE * ChunkLocation.CHUNK_SIZE * ChunkLocation.CHUNK_SIZE;

        internal byte[,,] Voxels = new byte[ChunkLocation.CHUNK_SIZE, ChunkLocation.CHUNK_SIZE, ChunkLocation.CHUNK_SIZE];
        internal Dictionary<byte, short> VoxelMap = new Dictionary<byte, short>();
        internal byte CurrentVoxelIndex = 0;

        private ChunkLocation parentChunkLocation;

        public ServerChunkBulkEditor(byte[,,] voxels, Dictionary<byte, short> voxelMap, byte currentVoxelIndex, ChunkLocation chunkLocation)
        {
            MemoryUtils.CopyArray(ref voxels, ref this.Voxels, TOTAL_CHUNK_SIZE);
            this.VoxelMap = voxelMap.ToDictionary(entry => entry.Key, entry => entry.Value);
            this.CurrentVoxelIndex = currentVoxelIndex;
            this.parentChunkLocation = chunkLocation;
        }

        public void Fill(VoxelDef voxelDef)
        {
            MemoryUtils.Fill3DArray<byte>(ref Voxels, 0, TOTAL_CHUNK_SIZE);
            this.VoxelMap = new Dictionary<byte, short>() { { 0, VoxelShort( voxelDef ) }, { 1, 0 /*AIR*/ } };
            CurrentVoxelIndex = 1;
        }

        public void FillCube(int x1, int y1, int z1, int x2, int y2, int z2, VoxelDef voxelDef)
        {
            // [0, 15]
            x1 = CMath.mod(x1, ChunkLocation.CHUNK_SIZE);
            x2 = CMath.mod(x2, ChunkLocation.CHUNK_SIZE);
            y1 = CMath.mod(y1, ChunkLocation.CHUNK_SIZE);
            y2 = CMath.mod(y2, ChunkLocation.CHUNK_SIZE);
            z1 = CMath.mod(z1, ChunkLocation.CHUNK_SIZE);
            z2 = CMath.mod(z2, ChunkLocation.CHUNK_SIZE);

            int minX = Math.Min(x1, x2);
            int maxX = Math.Max(x1, x2);
            int minY = Math.Min(y1, y2);
            int maxY = Math.Max(y1, y2);
            int minZ = Math.Min(z1, z2);
            int maxZ = Math.Max(z1, z2);

            short voxelId = VoxelShort( voxelDef );
            byte voxelByte = 0;
            if( VoxelMap.ContainsValue( voxelId ) )
            {
                voxelByte = VoxelMap.First( entry => entry.Value == voxelId ).Key;
            }
            else
            {
                VoxelMap[CurrentVoxelIndex] = voxelId;
                voxelByte = CurrentVoxelIndex++;
            }

            for (int x = minX; x <= maxX; ++x)
            {
                for (int y = minY; y <= maxY; ++y)
                {
                    for (int z = minZ; z <= maxZ; ++z)
                    {
                        Voxels[x, y, z] = voxelByte;
                    }
                }
            }
        }

        public void FillCube(Cuboid cuboid, VoxelDef voxelDef)
        {
            if( parentChunkLocation.AsCuboid().Contains(cuboid) )
            {
                // Chunk if 100% inside the cuboid, just use the min and max corners.
                FillCube(cuboid.MinCorner.X, cuboid.MinCorner.Y, cuboid.MinCorner.Z, cuboid.MaxCorner.X, cuboid.MaxCorner.Y, cuboid.MaxCorner.Z, voxelDef);
            }
            else if( parentChunkLocation.AsCuboid().Overlaps(cuboid) )
            {
                var clampedCuboid = parentChunkLocation.AsCuboid().Clamp(cuboid);
                FillCube(clampedCuboid.MinCorner.X, clampedCuboid.MinCorner.Y, clampedCuboid.MinCorner.Z, clampedCuboid.MaxCorner.X, clampedCuboid.MaxCorner.Y, clampedCuboid.MaxCorner.Z, voxelDef);
            }
        }

        public void SetVoxel(int x, int y, int z, VoxelDef voxelDef)
        {
            x = CMath.mod(x, ChunkLocation.CHUNK_SIZE);
            y = CMath.mod(y, ChunkLocation.CHUNK_SIZE);
            z = CMath.mod(z, ChunkLocation.CHUNK_SIZE);

            short voxelId = VoxelShort(voxelDef);
            byte voxelByte = 0;
            if (VoxelMap.ContainsValue(voxelId))
            {
                voxelByte = VoxelMap.First(entry => entry.Value == voxelId).Key;
            }
            else
            {
                VoxelMap[CurrentVoxelIndex++] = voxelId;
            }

            Voxels[x, y, z] = voxelByte;
        }

        private short VoxelShort(VoxelDef voxelDef)
        {
            return ServerCubivox.GetServer().GetServerItemRegistry().GetVoxelDefId(voxelDef);
        }
    }
}
