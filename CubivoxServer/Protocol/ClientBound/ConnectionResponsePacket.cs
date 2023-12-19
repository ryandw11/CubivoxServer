using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CubivoxServer.Protocol.ClientBound
{
    public class ConnectionResponsePacket : ClientBoundPacket
    {
        public string ServerName { get; set; }
        public JsonElement[] VoxelMap { get; set; }
        public JsonElement[] Players { get; set; }
        
        public void WritePacket(NetworkStream stream)
        {
            Console.WriteLine(ServerName);
            Console.WriteLine(JsonSerializer.Serialize(this));
            byte[] data = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(this));
            stream.Write(BitConverter.GetBytes(data.Length), 0, 4);
            stream.Write(data, 0, data.Length);
        }

        byte Packet.GetType()
        {
            return 0x01;
        }
    }
}
