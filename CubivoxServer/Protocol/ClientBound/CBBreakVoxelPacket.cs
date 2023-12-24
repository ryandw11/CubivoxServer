using System;
using System.Net.Sockets;
using CubivoxCore;

namespace CubivoxServer.Protocol.ClientBound
{
    public class CBBreakVoxelPacket : ClientBoundPacket
    {
        private int x;
        private int y;
        private int z;

        public CBBreakVoxelPacket(Location location)
        {
            this.x = (int) location.x;
            this.y = (int) location.y;
            this.z = (int) location.z;
        }

        public CBBreakVoxelPacket(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public void WritePacket(NetworkStream stream)
        {
            stream.Write(BitConverter.GetBytes(x));
            stream.Write(BitConverter.GetBytes(y));
            stream.Write(BitConverter.GetBytes(z));
        }

        byte Packet.GetType()
        {
            return 0x06;
        }
    }
}
