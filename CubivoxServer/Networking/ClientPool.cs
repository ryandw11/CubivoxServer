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

        public ClientPool()
        {
            clients = new List<Client>();
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
            Task<bool>[] tasks = new Task<bool>[clients.Count];
            for (int i = 0; i < clients.Count; i++)
            {
                tasks[i] = clients[i].PollData();
            }
            await Task.WhenAll(tasks);
        }
    }
}
