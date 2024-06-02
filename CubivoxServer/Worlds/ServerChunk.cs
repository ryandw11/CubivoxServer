using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CubivoxCore;
using CubivoxCore.Utils;
using CubivoxCore.Voxels;
using CubivoxCore.Worlds;
using CubivoxServer.Players;
using CubivoxServer.Protocol.ClientBound;
using CubivoxServer.Voxels;

namespace CubivoxServer.Worlds
{
    public class ServerChunk : Chunk
    {
        public static readonly int CHUNK_SIZE = 16;

        private Location location;
        private byte[,,] voxels = new byte[CHUNK_SIZE, CHUNK_SIZE, CHUNK_SIZE];
        private Dictionary<byte, short> voxelMap = new Dictionary<byte, short>();
        private byte currentVoxelIndex = 0;

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
            if (!ServerCMath.InChunk(location, x, y, z))
                throw new ArgumentOutOfRangeException("", "Provided location is outside of the chunk!");
            byte id = voxels[CMath.mod(x, CHUNK_SIZE), CMath.mod(y, CHUNK_SIZE), CMath.mod(z, CHUNK_SIZE)];

            return ByteToVoxel(id, new Location(GetWorld(), x, y, z));
        }

        public World GetWorld()
        {
            return location.GetWorld().Get();
        }

        /// <summary>
        /// Set a voxel in this chunk.
        /// 
        /// This will send the Place Voxel Packet to each player in the server.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="voxelDef"></param>
        public void SetVoxel(int x, int y, int z, VoxelDef voxelDef)
        {
            SetLocalVoxel(CMath.mod(x, CHUNK_SIZE), CMath.mod(y, CHUNK_SIZE), CMath.mod(z, CHUNK_SIZE), voxelDef);

            List<ServerPlayer> players = ServerCubivox.GetServer().GetPlayers();
            lock (players)
            {
                foreach (ServerPlayer player in players)
                {
                    player.SendPacket(new CBPlaceVoxelPacket(VoxelDefToShort(voxelDef), x, y, z));
                }
            }
        }

        /// <summary>
        /// Set the voxel using local chunk coordinates.
        /// This will NOT send a packet to players in the server.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="voxelDef"></param>
        public void SetLocalVoxel(int x, int y, int z, VoxelDef voxelDef)
        {
            short voxelId = ServerCubivox.GetServer().GetServerItemRegistry().GetVoxelDefId(voxelDef);
            if (voxelMap.ContainsValue(voxelId))
            {
                byte key = voxelMap.First(pair => pair.Value == voxelId).Key;
                voxels[x, y, z] = key;
            }
            else
            {
                voxelMap[currentVoxelIndex] = voxelId;
                currentVoxelIndex++;
            }
        }

        internal byte[,,] Voxels()
        {
            return voxels;
        }

        internal Dictionary<byte, short> VoxelMap()
        {
            return voxelMap;
        }

        /// <summary>
        /// Populate the ServerChunk with its data after it is loaded/generated.
        /// </summary>
        /// <param name="voxels">The 3D array of voxels.</param>
        /// <param name="voxelMap">The voxel map.</param>
        /// <param name="currentVoxelIndex">The current voxel index.</param>
        internal void PopulateChunk(byte[,,] voxels, Dictionary<byte, short> voxelMap, byte currentVoxelIndex)
        {
            this.voxels = voxels;
            this.voxelMap = voxelMap;
            this.currentVoxelIndex = currentVoxelIndex;
        }

        private Voxel ByteToVoxel(byte b, Location location)
        {
            VoxelDef definition = ServerCubivox.GetServer().GetServerItemRegistry().GetVoxelDef(voxelMap[b]);
            ServerVoxel clientVoxel = new ServerVoxel(location, definition, this);

            return clientVoxel;
        }
        private short VoxelDefToShort(VoxelDef voxelDef)
        {
            return ServerCubivox.GetServer().GetServerItemRegistry().GetVoxelDefId(voxelDef);
        }
    }
}
