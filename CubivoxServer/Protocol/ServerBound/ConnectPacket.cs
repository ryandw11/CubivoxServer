using CubivoxServer.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using CubivoxCore;

using CubivoxServer.Players;
using CubivoxServer.Protocol.ClientBound;

namespace CubivoxServer.Protocol.ServerBound
{
    public class ConnectPacket : ServerBoundPacket
    {
        public bool ProcessPacket(Client client, NetworkStream stream)
        {
            Console.WriteLine("Start Connect Packet");
            byte[] rawProtocolVersion = new byte[2];
            if (stream.Read(rawProtocolVersion, 0, 2) != 2) return false;

            // TODO :: Handle this.
            ushort protocolVersion = BitConverter.ToUInt16(rawProtocolVersion);
            byte[] rawUsername = new byte[25];
            if (stream.Read(rawUsername, 0, 25) != 25) return false;
            string username = Encoding.ASCII.GetString(rawUsername);

            Console.WriteLine("Username: " + username);
            Console.WriteLine("Start Connect Packet 2");

            byte[] rawUuid = new byte[16];
            int rd = stream.Read(rawUuid, 0, 16);
            Console.WriteLine(rd);
            if ( rd != 16) return false;
            Guid uuid = new Guid(rawUuid);

            ServerPlayer serverPlayer = new ServerPlayer(client, uuid, username, new Location(0, 54, 0));

            client.ServerPlayer = serverPlayer;

            ServerCubivox server = ServerCubivox.GetServer();

            Console.WriteLine("Start Connect Packet 3");

            server.HandlePlayerConnection(serverPlayer);

            Console.WriteLine("Start Connect Packet 4");

            var response = new ConnectionResponsePacket();
            response.ServerName = "Test Server";
            response.Voxels = new string[0];
            response.Players = new System.Text.Json.JsonElement[server.GetPlayers().Count - 1];
            int i = 0;
            foreach (var player in server.GetPlayers())
            {
                if (player.Uuid == uuid) continue;
                response.Players[i] = player.ToJsonObject();
                i++;
            }
            Console.WriteLine("Sent Response Packet");
            client.SendPacket(response);
            return true;
        }

        byte Packet.GetType()
        {
            return 0x00;
        }
    }
}
