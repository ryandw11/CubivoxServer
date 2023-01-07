using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CubivoxCore;
using CubivoxCore.BaseGame;

namespace CubivoxServer.BaseGame
{
    public class ServerVoxel : Voxel
    {
        private Location location;
        private VoxelDef voxelDef;
        private ServerChunk serverChunk;

        public ServerVoxel(Location location, VoxelDef voxelDef, ServerChunk serverChunk)
        {
            this.location = location;
            this.voxelDef = voxelDef;
            this.serverChunk = serverChunk;
        }

        public Chunk GetChunk()
        {
            return serverChunk;
        }

        public Location GetLocation()
        {
            return location;
        }

        public VoxelDef GetVoxelDef()
        {
            return voxelDef;
        }

        public void SetVoxelDef(VoxelDef voxelDef)
        {
            this.voxelDef = voxelDef;
        }
    }
}
