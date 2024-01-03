using CubivoxCore;
using CubivoxCore.Attributes;
using CubivoxCore.BaseGame;
using CubivoxCore.Worlds.Generation;
using CubivoxServer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubivoxServer.BaseGame.Generation.Generators
{
    [Name("Flat Hills")]
    [Key("flat_hills")]
    public class FlatHillsGenerator : WorldGenerator
    {
        private FastNoiseLite noise;
        private VoxelDef air;
        private VoxelDef test;

        public FlatHillsGenerator() : base()
        { 
            noise = new FastNoiseLite();
            noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            air = Cubivox.GetVoxelDefinition(new ControllerKey("CUBIVOX", "AIR"));
            test = Cubivox.GetVoxelDefinition(new ControllerKey("CUBIVOX", "TESTBLOCK"));
        }

        public override void GenerateChunk(int chunkX, int chunkY, int chunkZ, ChunkData chunkData)
        {
            ServerChunkData serverChunkData = (ServerChunkData) chunkData;
            if (chunkY != 2)
            {
                MemoryUtils.Fill3DArray(ref serverChunkData.Voxels, (byte)0, ServerChunk.CHUNK_SIZE * ServerChunk.CHUNK_SIZE * ServerChunk.CHUNK_SIZE);
                
                if (chunkY < 2)
                {
                    serverChunkData.VoxelMap[0] = 1;
                }
            }
            else
            {
                for (int x = 0; x < ServerChunk.CHUNK_SIZE; x++)
                {
                    for (int z = 0; z < ServerChunk.CHUNK_SIZE; z++)
                    {
                        double height = noise.GetNoise(x + (float)(chunkX * ServerChunk.CHUNK_SIZE), z + (float)(chunkZ * ServerChunk.CHUNK_SIZE)) * 16;
                        for (int y = 0; y < ServerChunk.CHUNK_SIZE; y++)
                        {
                            if (y < height)
                            {
                                serverChunkData.SetVoxel(x, y, z, test);
                            }
                            else
                            {
                                serverChunkData.SetVoxel(x, y, z, air);
                            }
                        }
                    }
                }
            }
        }
    }
}
