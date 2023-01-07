using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;

namespace CubivoxServer.Networking
{
    public class ClientManager
    {
        private List<ClientPool> clientPools;

        public ClientManager()
        {
            clientPools = new List<ClientPool>();
        }

        public void HandleTCPConnection(TcpClient tcpClient)
        {
            Console.WriteLine("A client was connected!");
            Client client = new Client(tcpClient);
            foreach (var pool in clientPools)
            {
                if(pool.HasSpace())
                {
                    pool.AddClient(client);
                    return;
                }
            }
            clientPools.Add(new ClientPool());
            clientPools.Last().AddClient(client);
        }

        public async Task pollData()
        {
            Task[] tasks = new Task[clientPools.Count];
            for (int i = 0; i < clientPools.Count; i++)
            {
                tasks[i] = clientPools[i].PollClients();
            }
            await Task.WhenAll(tasks);
        }
    }
}
