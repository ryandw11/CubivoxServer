using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CubivoxCore;
using CubivoxCore.BaseGame;
using CubivoxCore.Players;

using CubivoxServer.Networking;
using CubivoxServer.Protocol;

namespace CubivoxServer.Players
{
    public class ServerPlayer : Player
    {
        public Guid Uuid { get; private set; }
        public string Username { get; private set; }
        public Location Location { get; internal set; }
        internal Client Client { get; private set; }

        public ServerPlayer(Client client, Guid uuid, string name, Location location)
        {
            this.Uuid = uuid;
            this.Username = name;
            this.Location = location;
            this.Client = client;
        }

        public Entity GetEntityType()
        {
            throw new NotImplementedException();
        }

        public Location GetLocation()
        {
            return Location;
        }

        public string GetName()
        {
            return Username;
        }

        public Guid GetUUID()
        {
            return Uuid;
        }

        public bool IsLiving()
        {
            return true;
        }

        public void SendMessage(string message)
        {
            throw new NotImplementedException();
        }

        internal Client GetClient()
        {
            return Client;
        }

        public void SendPacket(ClientBoundPacket packet)
        {
            Client.SendPacket(packet);
        }

        public JsonElement ToJsonObject()
        {
            var playerObj = new
            {
                Username = Username,
                Uuid = Uuid.ToString(),
                X = Location.x,
                Y = Location.y,
                Z = Location.z
            };

            return JsonSerializer.SerializeToElement(playerObj);
        }
    }
}
