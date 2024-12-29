using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Net.Sockets;
using CubivoxCore;
using CubivoxCore.Attributes;
using CubivoxCore.BaseGame;
using CubivoxServer.Utils;
using CubivoxServer.Worlds;

namespace CubivoxServer.Protocol.ClientBound
{
    public class CBLoadChunkPacket : ClientBoundPacket
    {
        private ServerChunk serverChunk;

        public CBLoadChunkPacket(ServerChunk serverChunk)
        {
            this.serverChunk = serverChunk;
        }

        public void WritePacket(NetworkStream stream)
        {
            stream.Write(BitConverter.GetBytes((int)serverChunk.GetLocation().X));
            stream.Write(BitConverter.GetBytes((int)serverChunk.GetLocation().Y));
            stream.Write(BitConverter.GetBytes((int)serverChunk.GetLocation().Z));
            stream.Write(BitConverter.GetBytes((short)serverChunk.VoxelMap().Count));
            SortedDictionary <byte, short> shortedVoxelMap = new SortedDictionary<byte, short>(serverChunk.VoxelMap());
            foreach (var kv in shortedVoxelMap)
            {
                stream.Write(BitConverter.GetBytes(kv.Value));
            }
            byte[,,] voxelCopy = serverChunk.Voxels();
            byte[] voxels = MemoryUtils.ThreeDArrayTo1DArray(ref voxelCopy, ServerChunk.CHUNK_SIZE * ServerChunk.CHUNK_SIZE * ServerChunk.CHUNK_SIZE);
            voxels = Compress(voxels);
            stream.Write(BitConverter.GetBytes(voxels.Length));
            stream.Write(voxels);
        }

        private byte[] Compress(byte[] data)
        {
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(output, CompressionLevel.Optimal))
            {
                dstream.Write(data, 0, data.Length);
            }
            return output.ToArray();
        }

        private byte[] Decompress(byte[] data)
        {
            MemoryStream input = new MemoryStream(data);
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
            {
                dstream.CopyTo(output);
            }
            return output.ToArray();
        }

        byte Packet.GetType()
        {
            return 0x08;
        }
    }
}
