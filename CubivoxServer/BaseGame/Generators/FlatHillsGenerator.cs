using CubivoxCore.Attributes;
using CubivoxCore.BaseGame;
using CubivoxCore.Voxels;
using CubivoxCore.Worlds.Generation;

using CubivoxServer.Worlds;
using CubivoxServer.Worlds.Generation;

namespace CubivoxServer.BaseGame.Generators
{
    [Name("Flat Hills")]
    [Key("flat_hills")]
    public class FlatHillsGenerator : WorldGenerator
    {
        private FastNoiseLite noise;
        private VoxelDef air;
        private VoxelDef grass;
        private VoxelDef dirt;

        public FlatHillsGenerator() : base()
        {
            noise = new FastNoiseLite();
            noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            air = Voxels.Voxel(Voxels.AIR);
            grass = Voxels.Voxel(Voxels.GRASS);
            dirt = Voxels.Voxel(Voxels.DIRT);
        }

        public override void GenerateChunk(int chunkX, int chunkY, int chunkZ, ChunkData chunkData)
        {
            ServerChunkData serverChunkData = (ServerChunkData)chunkData;
            if (chunkY != 2)
            {
                if (chunkY < 2)
                {
                    serverChunkData.VoxelMap[0] = 3; // Dirt
                    
                    if( chunkY == 1 )
                    {
                        for (int x = 0; x < ServerChunk.CHUNK_SIZE; x++)
                        {
                            for (int z = 0; z < ServerChunk.CHUNK_SIZE; z++)
                            {
                                double height = noise.GetNoise(x + (float)(chunkX * ServerChunk.CHUNK_SIZE), z + (float)(chunkZ * ServerChunk.CHUNK_SIZE)) * 16;
                                if( height <= 0 )
                                {
                                    serverChunkData.SetVoxel(x, 15, z, grass);
                                }
                            }
                        }
                    }
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
                            if( y == (int) height )
                            {
                                serverChunkData.SetVoxel(x, y, z, grass);
                            }
                            else if( y < height )
                            {
                                serverChunkData.SetVoxel(x, y, z, dirt);
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
