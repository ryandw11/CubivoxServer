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
        public async Task<bool> ProcessPacket(Client client, NetworkStream stream)
        {
            byte[] rawProtocolVersion = new byte[2];

            await NetworkUtil.FillBufferFromNetwork(rawProtocolVersion, stream);

            // TODO :: Handle this.
            ushort protocolVersion = BitConverter.ToUInt16(rawProtocolVersion);

            byte[] rawUsername = new byte[25];
            await NetworkUtil.FillBufferFromNetwork(rawUsername, stream);
            string username = Encoding.ASCII.GetString(rawUsername).Trim();

            byte[] rawUuid = new byte[16];
            await NetworkUtil.FillBufferFromNetwork(rawUuid, stream);
            Guid uuid = new Guid(rawUuid);

            ServerPlayer serverPlayer = new ServerPlayer(client, uuid, username, new Location(0, 54, 0));

            client.ServerPlayer = serverPlayer;
            client.CompletedHandshake = true;

            ServerCubivox server = ServerCubivox.GetServer();

            server.HandlePlayerConnection(serverPlayer);

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

            client.SendPacket(response);
            return true;
        }

        byte Packet.GetType()
        {
            return 0x00;
        }
    }
}
