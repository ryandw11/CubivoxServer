using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace CubivoxServer.Networking
{
    public class PacketProcessor
    {
        private Packet currentPacket;

        public PacketProcessor()
        {
            currentPacket = new Packet();
        }

        public async Task<bool> ProcessCommand(NetworkStream networkStream)
        {
            byte[] type = new byte[1];
            if (await networkStream.ReadAsync(type, 0, 1) != 1) return false;
            byte[] size = new byte[4];
            if (await networkStream.ReadAsync(size, 0, 4) != 4) return false;
            int iSize = BitConverter.ToInt32(size, 0);
            // TODO :: Maybe limit packet size.
            byte[] data = new byte[iSize];
            if (await networkStream.ReadAsync(data, 0, iSize) != iSize) return false;

            Console.WriteLine($"Read Packet, Type: {type[0]}, Size: {iSize}");

            return true;
        }
    }

    internal struct Packet
    {
        byte type;
        int size;
        byte[] data;
    }
}
