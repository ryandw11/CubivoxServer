using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using CubivoxCore;

using CubivoxServer.BaseGame;
using CubivoxServer.Networking;
using CubivoxServer.Players;
using CubivoxServer.Protocol.ServerBound;
using CubivoxServer.Protocol.ClientBound;
using CubivoxCore.BaseGame.VoxelDefs;
using CubivoxServer.Items;
using CubivoxServer.Worlds;
using CubivoxServer.Utils;
using CubivoxCore.BaseGame;

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
        private List<ServerWorld> worlds;

        public ServerCubivox()
        {
            instance = this;
            itemRegistry = new ServerItemRegistry();
            clientManager = new ClientManager();
            packetManager = new ServerBoundPacketManager();
            players = new List<ServerPlayer>();
            worlds = new List<ServerWorld>();

            packetManager.RegisterPacket(new ConnectPacket());
            packetManager.RegisterPacket(new UpdatePlayerPosition());
            packetManager.RegisterPacket(new SBBreakVoxelPacket());
            packetManager.RegisterPacket(new SBPlaceVoxelPacket());
        }

        public override EnvType GetEnvType()
        {
            return EnvType.SERVER;
        }

        public override void OnEnable()
        {
            itemRegistry.RegisterItem(new AirVoxel());
            itemRegistry.RegisterItem(new TestVoxel(this));

            // Load basic stuff here for now:
            ServerWorld world = new ServerWorld();
            worlds.Add(world);

            // TODO: Don't hard code this in the future.
            Console.WriteLine("Generating World...");

            List<Task> generationTasks = new List<Task>();
            FastNoiseLite noise = new FastNoiseLite();
            noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            for (int x = -10; x < 10; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    for (int z = -10; z < 10; z++)
                    {
                        ServerChunk serverChunk = new ServerChunk(new Location(x, y, z));
                        generationTasks.Add(Task.Factory.StartNew(o => {
                            ServerChunk sChunk = (ServerChunk) o;
                            byte[,,] voxels = new byte[ServerChunk.CHUNK_SIZE, ServerChunk.CHUNK_SIZE, ServerChunk.CHUNK_SIZE];
                            if (sChunk.GetLocation().y != 2)
                            {
                                MemoryUtils.Fill3DArray(ref voxels, (byte)0, ServerChunk.CHUNK_SIZE * ServerChunk.CHUNK_SIZE * ServerChunk.CHUNK_SIZE);
                                Dictionary<byte, short> voxelMap = new Dictionary<byte, short>();
                                if (sChunk.GetLocation().y < 2)
                                {
                                    voxelMap.Add(0, 1);
                                }
                                else
                                {
                                    voxelMap.Add(0, 0);
                                }
                                //voxelMap.Add(0, 1);
                                sChunk.PopulateChunk(voxels, voxelMap, 1);
                            }
                            else
                            {

                                Dictionary<byte, short> voxelMap = new Dictionary<byte, short> { { 0, 0 }, { 1, 1 } };
                                for (int x = 0; x < ServerChunk.CHUNK_SIZE; x++)
                                {
                                    for (int z = 0; z < ServerChunk.CHUNK_SIZE; z++)
                                    {
                                        double height = noise.GetNoise(x + (float) (sChunk.GetLocation().x * ServerChunk.CHUNK_SIZE), z + (float)(sChunk.GetLocation().z * ServerChunk.CHUNK_SIZE)) * 16;
                                        for (int y = 0; y < ServerChunk.CHUNK_SIZE; y++)
                                        {
                                            if (y < height)
                                            {
                                                voxels[x, y, z] = 1;
                                            }
                                            else
                                            {
                                                voxels[x, y, z] = 0;
                                            }
                                        }
                                    }
                                }
                                sChunk.PopulateChunk(voxels, voxelMap, 2);
                            }
                            world.AddLoadedChunk(sChunk);
                        }, serverChunk));
                    }
                }
            }
            Task.WaitAll(generationTasks.ToArray());
            Console.WriteLine("Finished Generating World!");
        }

        public async void StartServer(int port)
        {
            try
            {
                Console.WriteLine($"Starting Cubivox Server on port {port}.");
                IPAddress localAddress = IPAddress.Parse("0.0.0.0");

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
                        } catch (SocketException ex)
                        {

                            Console.WriteLine("[Warning] A socket exception has occured.");
                            Console.WriteLine(ex.Message);
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

        public List<ServerWorld> GetWorlds()
        {
            return worlds;
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