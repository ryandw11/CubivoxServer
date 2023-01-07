using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

using CubivoxServer.Players;
using CubivoxServer.Protocol;

namespace CubivoxServer.Networking
{
    public class Client
    {
        private TcpClient tcpClient;
        private NetworkStream stream;
        public ServerPlayer? ServerPlayer { get; internal set; }

        public Client(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
            stream = tcpClient.GetStream();
            ServerPlayer = null;
        }

        public bool HasData()
        {
            return stream.DataAvailable;
        }

        public async Task<bool> PollData()
        {
            bool result = await ProcessPacket(stream);
            if (!result)
                Console.WriteLine("Client: Failed to process packet!");
            return true;
        }

        public async Task<bool> ProcessPacket(NetworkStream networkStream)
        {
            if (!HasData())
                return true;

            byte[] id = new byte[1];
            if (await networkStream.ReadAsync(id, 0, 1) != 1) return false;
            Console.WriteLine($"Packet {(int)id[0]}");
            try
            {
                ServerCubivox.GetServer().GetPacketManager().GetPacket(id[0]).ProcessPacket(this, networkStream);
            } catch (KeyNotFoundException)
            {
                Console.WriteLine("Test");
                return false;
            }

            return true;
        }

        public ServerPlayer? GetServerPlayer()
        {
            return ServerPlayer;
        }

        public void SendPacket(ClientBoundPacket clientBoundPacket)
        {
            stream.WriteByte(clientBoundPacket.GetType());
            clientBoundPacket.WritePacket(stream);
            stream.Flush();
        }
    }
}
