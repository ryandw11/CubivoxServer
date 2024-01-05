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
using CubivoxServer.Worlds;
using CubivoxServer.BaseGame;

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
            string username = Encoding.ASCII.GetString(rawUsername).Trim().Replace("\0", "");

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
            response.VoxelMap = server.GetServerItemRegistry().GetVoxelDict();
            response.Players = new System.Text.Json.JsonElement[server.GetPlayers().Count - 1];

            int i = 0;
            foreach (var player in server.GetPlayers())
            {
                if (player.Uuid == uuid) continue;
                response.Players[i] = player.ToJsonObject();
                i++;
            }

            client.SendPacket(response);

            // Now the player has connected, send the required chunks to the player.
            _ = Task.Run(() => SendChunks(client));

            return true;
        }

        public void SendChunks(Client client)
        {
            Console.WriteLine("[DEBUG] Starting to send chunk data to player: " + client.GetServerPlayer().GetName());
            ServerWorld world = ServerCubivox.GetServer().GetWorlds()[0];
            Dictionary<Location, ServerChunk> chunks;
            lock (world)
            {
                 chunks = new Dictionary<Location, ServerChunk>(world.GetLoadedChunks());
            }
            foreach(var chunk in chunks)
            {
                CBLoadChunkPacket loadChunkPacket = new CBLoadChunkPacket(chunk.Value);
                client.SendPacket(loadChunkPacket);
            }
            Console.WriteLine("[DEBUG] Finished sending chunk data to player: " + client.GetServerPlayer().GetName() + $" ({chunks.Count} chunks)");
        }

        byte Packet.GetType()
        {
            return 0x00;
        }
    }
}
