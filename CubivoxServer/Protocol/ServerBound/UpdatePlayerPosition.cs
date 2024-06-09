using CubivoxServer.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using CubivoxServer;
using CubivoxServer.Players;
using CubivoxServer.Protocol.ClientBound;

using CubivoxCore;
using CubivoxCore.Events.Global;

namespace CubivoxServer.Protocol.ServerBound
{
    public class UpdatePlayerPosition : ServerBoundPacket
    {
        public async Task<bool> ProcessPacket(Client client, NetworkStream stream)
        {
            if (client.ServerPlayer == null) return false;

            byte[] rawDouble = new byte[8];
            await NetworkUtil.FillBufferFromNetwork(rawDouble, stream);
            double x = BitConverter.ToDouble(rawDouble);

            await NetworkUtil.FillBufferFromNetwork(rawDouble, stream);
            double y = BitConverter.ToDouble(rawDouble);

            await NetworkUtil.FillBufferFromNetwork(rawDouble, stream);
            double z = BitConverter.ToDouble(rawDouble);

            var oldLocation = client.ServerPlayer.Location.Clone();
            client.ServerPlayer.Location.Set(x, y, z);
            var currentNewPosition = client.ServerPlayer.Location.Clone();
            PlayerMoveEvent playerMoveEvent = new PlayerMoveEvent(client.ServerPlayer, client.ServerPlayer.Location, oldLocation);
            Cubivox.GetEventManager().TriggerEvent(playerMoveEvent);

            if (playerMoveEvent.IsCanceled())
            {
                // Move the player back to where they were.
                client.ServerPlayer.Location = oldLocation;
                client.SendPacket(new PlayerPositionUpdatePacket(client.ServerPlayer));
            }
            else
            {
                // Ensure the event did not modify the player's location.
                if (client.ServerPlayer.Location == currentNewPosition)
                {
                    lock (ServerCubivox.GetServer().GetPlayers())
                    {
                        foreach (ServerPlayer player in ServerCubivox.GetServer().GetPlayers())
                        {
                            if (player.Uuid == client.ServerPlayer.Uuid) continue;
                            player.SendPacket(new PlayerPositionUpdatePacket(client.ServerPlayer));
                        }
                    }
                }
            }

            return true;
        }

        byte Packet.GetType()
        {
            return 0x02;
        }
    }
}
