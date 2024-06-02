using System;
using System.Collections.Concurrent;

using CubivoxCore;
using CubivoxCore.Voxels;
using CubivoxCore.Worlds;
using CubivoxCore.Worlds.Generation;

namespace CubivoxServer.Worlds
{
    public class ServerWorld : World
    {
        private Location spawnLocation;
        private ConcurrentDictionary<Location, ServerChunk> loadedChunks;
        private int renderDistance = 16;
        public ServerWorld()
        {
            spawnLocation = new Location(0, 0, 0);
            loadedChunks = new ConcurrentDictionary<Location, ServerChunk>();
        }

        public void AddLoadedChunk(ServerChunk chunk)
        {
            loadedChunks.TryAdd(chunk.GetLocation(), chunk);
        }

        public void RemoveLoadedChunk(ServerChunk chunk)
        {
            loadedChunks.Remove(chunk.GetLocation(), out _);
        }

        public Chunk GetChunk(Location location)
        {
            ServerChunk output;
            if (loadedChunks.TryGetValue(location, out output))
            {
                return output;
            }

            return null;
        }

        public Chunk GetChunk(int x, int y, int z)
        {
            return GetChunk(new Location(this, x, y, z));
        }

        public Location GetSpawnLocation()
        {
            return spawnLocation;
        }

        public Voxel GetVoxel(int x, int y, int z)
        {
            int chunkX = (int)Math.Floor((float)x / ServerChunk.CHUNK_SIZE);
            int chunkY = (int)Math.Floor((float)y / ServerChunk.CHUNK_SIZE);
            int chunkZ = (int)Math.Floor((float)z / ServerChunk.CHUNK_SIZE);

            Chunk chunk = GetChunk(chunkX, chunkY, chunkZ);

            if (chunk == null) return null;

            return chunk.GetVoxel(x, y, z);
        }

        public bool IsChunkLoaded(Location location)
        {
            return loadedChunks.TryGetValue(location, out _);
        }

        public bool IsChunkLoaded(int x, int y, int z)
        {
            return IsChunkLoaded(new Location(x, y, z));
        }

        public bool IsChunkLoaded(Chunk chunk)
        {
            return IsChunkLoaded(chunk.GetLocation());
        }

        public void LoadChunk(Location location)
        {
            throw new NotImplementedException();
        }

        public void LoadChunk(int x, int y, int z)
        {
            throw new NotImplementedException();
        }

        public void LoadChunk(Chunk chunk)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void SetVoxel(int x, int y, int z, VoxelDef voxel)
        {
            int chunkX = (int)Math.Floor((float)x / ServerChunk.CHUNK_SIZE);
            int chunkY = (int)Math.Floor((float)y / ServerChunk.CHUNK_SIZE);
            int chunkZ = (int)Math.Floor((float)z / ServerChunk.CHUNK_SIZE);

            ServerChunk chunk = (ServerChunk)GetChunk(chunkX, chunkY, chunkZ);

            if (chunk == null)
            {
                Console.WriteLine("[WARNING] Tried to set a Voxel when the chunk was not loaded!");
                return;
            }

            chunk.SetVoxel(x, y, z, voxel);
        }

        public void UnloadChunk(int x, int y, int z)
        {
            throw new NotImplementedException();
        }

        public void UnloadChunk(Chunk chunk)
        {
            throw new NotImplementedException();
        }

        public ConcurrentDictionary<Location, ServerChunk> GetLoadedChunks()
        {
            return loadedChunks;
        }

        public WorldGenerator GetGenerator()
        {
            throw new NotImplementedException();
        }
    }
}
