using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

using CubivoxCore;

using CubivoxServer.BaseGame;
using CubivoxServer.Networking;
using CubivoxServer.Players;
using CubivoxServer.Protocol.ServerBound;
using CubivoxServer.Protocol.ClientBound;

namespace CubivoxServer
{
    public class ServerCubivox : Cubivox
    {
        public static readonly ushort PROTOCOL_VERSION = 0;

        private TcpListener server;
        private ClientManager clientManager;
        private ServerBoundPacketManager packetManager;
        private bool shouldStop = false;

        private List<ServerPlayer> players;

        public ServerCubivox()
        {
            instance = this;
            itemRegistry = new ServerItemRegistry();
            clientManager = new ClientManager();
            packetManager = new ServerBoundPacketManager();
            players = new List<ServerPlayer>();

            packetManager.RegisterPacket(new ConnectPacket());
            packetManager.RegisterPacket(new UpdatePlayerPosition());
        }

        public override EnvType GetEnvType()
        {
            return EnvType.SERVER;
        }

        public override void OnEnable()
        {
            
        }

        public async void StartServer(int port)
        {
            try
            {
                Console.WriteLine($"Starting Cubivox Server on port {port}.");
                IPAddress localAddress = IPAddress.Parse("localhost");

                server = new TcpListener(localAddress, port);

                server.Start();

                Task<TcpClient> client = server.AcceptTcpClientAsync();
                Task pollDataTask = clientManager.pollData();

                while (true)
                {
                    if (client.IsCompleted)
                    {
                        clientManager.HandleTCPConnection(await client);
                        client = server.AcceptTcpClientAsync();
                    }

                    if (pollDataTask.IsCompleted)
                    {
                        try
                        {
                            await pollDataTask;
                        } catch (IOException ex)
                        {
                            if ((ex.InnerException != null && !(ex.InnerException is SocketException)) || ex.InnerException == null)
                            {
                                throw new Exception("An error has occured processing data!", ex);
                            }
                        } catch (SocketException)
                        {
                            Console.WriteLine("[Warning] A socket exception has occured.");
                            // Do nothing.
                        }
                        pollDataTask = clientManager.pollData();
                    }
                    Thread.Sleep(1);
                }
                Console.WriteLine("Test3");
            }
            catch (SocketException exception)
            {
                Console.WriteLine("Test4");
                Console.WriteLine(exception.ToString());
            }
            finally
            {
                Console.WriteLine("Test5");
                if (server != null)
                    server.Stop();
            }
            Console.WriteLine("Test6");
        }

        public List<ServerPlayer> GetPlayers()
        {
            return players;
        }

        internal void HandlePlayerConnection(ServerPlayer player)
        {
            if(player.Client == null)
            {
                throw new ArgumentException("Player must have a TCPClient attached.");
            }

            players.Add(player);
            Console.WriteLine($"{player.Username} has joined the game!");


            // Inform all online players about this user.
            foreach (ServerPlayer p in players)
            {
                // Ignore the same player.
                if (p.Uuid == player.Uuid) continue;
                p.SendPacket(new PlayerConnectionPacket(player));
            }
        }

        public ServerItemRegistry GetServerItemRegistry()
        {
            return (ServerItemRegistry) itemRegistry;
        }

        public ServerBoundPacketManager GetPacketManager()
        {
            return packetManager;
        }

        public static ServerCubivox GetServer()
        {
            return (ServerCubivox) instance;
        }
    }
}