using CubivoxCore.Chat;
using CubivoxServer.Protocol.ClientBound;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubivoxServer.Networking
{
    public class ClientPool
    {
        public static readonly int MAX_CLIENTS = 10;

        private List<Client> clients;
        private List<Client> clientsToRemove;

        public ClientPool()
        {
            clients = new List<Client>();
            clientsToRemove = new List<Client>();
        }

        public bool HasSpace()
        {
            return clients.Count < MAX_CLIENTS;
        }

        public void AddClient(Client client)
        {
            if (clients.Count == MAX_CLIENTS)
                throw new IndexOutOfRangeException("Max amount of clients already exist!");
            clients.Add(client);
        }

        public async Task PollClients()
        {
            List<Task<bool>> tasks = new List<Task<bool>>();
            foreach (var client in clients)
            {
                if (!client.GetClient().Connected) {
                    clientsToRemove.Add(client);
                } else if(client.GetClient().Client.Poll(0, System.Net.Sockets.SelectMode.SelectRead))
                {
                    if(client.GetClient().Client.Receive(new byte[1], System.Net.Sockets.SocketFlags.Peek) == 0)
                        clientsToRemove.Add(client);
                    else
                        tasks.Add(client.PollData());
                }
            }

            if(clientsToRemove.Count != 0)
            {
                foreach (Client client in clientsToRemove)
                {
                    HandleClientDisconnect(client);
                }
                clientsToRemove.Clear();
            }

            await Task.WhenAll(tasks);
        }

        public void HandleClientDisconnect(Client client)
        {
            clients.Remove(client);

            if (client.ServerPlayer != null)
            {
                client.ServerPlayer.Client = null;
                ServerCubivox.GetServer().HandlePlayerDisconnect(client.ServerPlayer);
            }
        }
    }
}
