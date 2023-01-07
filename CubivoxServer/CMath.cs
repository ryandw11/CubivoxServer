using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CubivoxCore;
using CubivoxServer.BaseGame;

namespace CubivoxServer
{
    public class CMath
    {
        public static int FloorToInt(double v)
        {
            return (int) Math.Floor(v);
        }

        public static bool InChunk(Location chunkLocation, int voxelX, int voxelY, int voxelZ)
        {
            Location trueChunkLoc = chunkLocation * ServerChunk.CHUNK_SIZE;
            double xDiff = voxelX - trueChunkLoc.x;
            double yDiff = voxelY - trueChunkLoc.y;
            double zDiff = voxelZ - trueChunkLoc.z;
            return  xDiff < 16 && yDiff < 16 &&  zDiff < 16 && xDiff >= 0 && yDiff >= 0 && zDiff >= 0;
        }
    }
}
