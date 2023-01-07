using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CubivoxCore;
using CubivoxCore.BaseGame;
using CubivoxCore.Worlds;

namespace CubivoxServer.BaseGame
{
    public class ServerChunk : Chunk
    {
        public static readonly int CHUNK_SIZE = 16;

        private Location location;
        private ServerVoxel[,,] voxels = new ServerVoxel[CHUNK_SIZE, CHUNK_SIZE, CHUNK_SIZE];

        public ServerChunk(Location location)
        {
            this.location = location;
        }

        public Location GetLocation()
        {
            return location;
        }

        public Voxel GetVoxel(int x, int y, int z)
        {
            if (!CMath.InChunk(location, x, y, z))
                throw new ArgumentOutOfRangeException("", "Provided location is outside of the chunk!");
            return voxels[CMath.FloorToInt(x % CHUNK_SIZE), CMath.FloorToInt(y % CHUNK_SIZE), CMath.FloorToInt(z % CHUNK_SIZE)];
        }

        public World GetWorld()
        {
            return location.GetWorld().Get();
        }

        public bool IsLoaded()
        {
            throw new NotImplementedException();
        }

        public bool Load()
        {
            throw new NotImplementedException();
        }

        public void SetVoxel(int x, int y, int z, VoxelDef voxelDef)
        {
            if (!CMath.InChunk(location, x, y, z))
                throw new ArgumentOutOfRangeException("", "Provided location is outside of the chunk!");

            voxels[CMath.FloorToInt(x % CHUNK_SIZE), CMath.FloorToInt(y % CHUNK_SIZE), CMath.FloorToInt(z % CHUNK_SIZE)].SetVoxelDef(voxelDef);
        }
    }
}
