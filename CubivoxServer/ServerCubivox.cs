using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using CubivoxCore;
using CubivoxCore.Chat;

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
using CubivoxServer.BaseGame.Generation;
using CubivoxServer.BaseGame.Generation.Generators;
using CubivoxCore.Worlds.Generation;
using CubivoxCore.Mods;
using System.Reflection;
using System.Text.Json;
using CubivoxServer.Loggers;
using System.Diagnostics;
using CubivoxCore.Console;
using CubivoxServer.Events;

namespace CubivoxServer
{
    public class ServerCubivox : Cubivox
    {
        public static readonly ushort PROTOCOL_VERSION = 0;

        private TcpListener server;
        private ClientManager clientManager;
        private ServerBoundPacketManager packetManager;
        private bool enabled = false;

        private List<Mod> mods;
        private ServerLogger logger;

        private List<ServerPlayer> players;
        private List<ServerWorld> worlds;

        public ServerCubivox()
        {
            instance = this;
            itemRegistry = new ServerItemRegistry();
            generatorRegistry = new ServerGeneratorRegistry();
            eventManager = new ServerEventManager();
            clientManager = new ClientManager();
            packetManager = new ServerBoundPacketManager();
            mods = new List<Mod>();
            players = new List<ServerPlayer>();
            worlds = new List<ServerWorld>();

            logger = new ServerLogger("Cubivox");

            packetManager.RegisterPacket(new ConnectPacket());
            packetManager.RegisterPacket(new UpdatePlayerPosition());
            packetManager.RegisterPacket(new SBBreakVoxelPacket());
            packetManager.RegisterPacket(new SBPlaceVoxelPacket());
        }

        public override EnvType GetEnvType()
        {
            return EnvType.SERVER;
        }

        public override Logger GetLogger()
        {
            return logger;
        }

        public override void LoadItemsStage(ItemRegistry itemRegistry)
        {
            itemRegistry.RegisterItem(new AirVoxel());
            itemRegistry.RegisterItem(new TestVoxel(this));
        }

        public override void LoadGeneratorsStage(GeneratorRegistry generatorRegistry)
        {
            generatorRegistry.RegisterWorldGenerator(this, new FlatHillsGenerator());
        }

        public override void OnEnable()
        {
            if (enabled)
            {
                throw new Exception("Server Cubivox cannot be enabled twice.");
            }
            enabled = true;

            LoadItemsStage(itemRegistry);
            LoadGeneratorsStage(generatorRegistry);

            // Load Mods
            LoadMods();

            // Load basic stuff here for now:
            ServerWorld world = new ServerWorld();
            worlds.Add(world);

            // TODO: Don't hard code this in the future.
            logger.Info("Generating World...");

            List<Task> generationTasks = new List<Task>();
            for (int x = -10; x < 10; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    for (int z = -10; z < 10; z++)
                    {
                        ServerChunk serverChunk = new ServerChunk(new Location(world, x, y, z));
                        generationTasks.Add(Task.Factory.StartNew(o => {
                            ServerChunk sChunk = (ServerChunk) o;
                            ServerChunkData sData = new ServerChunkData();
                            generatorRegistry.GetDefaultWorldGenerator().GenerateChunk(sChunk.GetLocation().GetVoxelX(), sChunk.GetLocation().GetVoxelY(), sChunk.GetLocation().GetVoxelZ(), sData);
                            sChunk.PopulateChunk(sData.Voxels, sData.VoxelMap, sData.CurrentVoxelIndex);
                            world.AddLoadedChunk(sChunk);
                        }, serverChunk));
                    }
                }
            }
            Task.WaitAll(generationTasks.ToArray());
            logger.Info("Finished Generating World!");
        }

        public async void StartServer(int port)
        {
            try
            {
                logger.Info($"Starting Cubivox Server on port {port}.");
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

                            logger.Warn("[Warning] A socket exception has occured.");
                            logger.Warn(ex.Message);
                            // Do nothing.
                        }
                        pollDataTask = clientManager.pollData();
                    }
                    Thread.Sleep(1);
                }
                logger.Info("Test3");
            }
            catch (SocketException exception)
            {
                logger.Error("Test4");
                logger.Error(exception.ToString());
            }
            finally
            {
                logger.Info("Test5");
                if (server != null)
                    server.Stop();
            }
            logger.Info("Test6");
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
            logger.Info($"{player.Username} has joined the game!");


            // Inform all online players about this user.
            foreach (ServerPlayer p in players)
            {
                p.SendMessage($"{player.Username} has joined the game!".Color("yellow"));
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

        public override void AssertServer()
        {
        }

        public override void AssertClient()
        {
            throw new Exception();
        }

        private void LoadMods()
        {
            DirectoryInfo dir = new DirectoryInfo("." + Path.DirectorySeparatorChar + "mods");
            if (!dir.Exists)
            {
                dir.Create();
            }

            foreach(FileInfo file in dir.GetFiles())
            {
                var dll = Assembly.LoadFile(file.FullName);
                var resourceName = file.Name.Replace(".dll", "") + ".mod.json";

                string resource = null;
                using (Stream stream = dll.GetManifestResourceStream(resourceName))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        resource = reader.ReadToEnd();
                    }
                }

                if(resource == null)
                {
                    logger.Error($"Failed to load mod {file.Name}! Is the mod.json file in the right namespace?");
                    continue;
                }

                ModDescriptionFile descriptionFile = JsonSerializer.Deserialize<ModDescriptionFile>(resource);

                var mainModClass = dll.GetType(descriptionFile.MainClass);

                if(mainModClass == null)
                {
                    logger.Error($"Failed to load mod {file.Name}! Cannot find the mod's main class {descriptionFile.MainClass}!");
                    continue;
                }

                ServerLogger modLogger = new ServerLogger(descriptionFile.ModName);

                CubivoxMod mod = (CubivoxMod) Activator.CreateInstance(mainModClass, descriptionFile, modLogger);

                if (mod == null)
                {
                    logger.Error($"Failed to load mod {file.Name}! Unable to construct main mod class instance.");
                    continue;
                }    

                mods.Add(mod);
                logger.Info($"Found and loaded mod {descriptionFile.ModName}.");
            }

            foreach(Mod mod in mods)
            {
                try
                {
                    mod.LoadItemsStage(itemRegistry);
                }
                catch (Exception ex)
                {
                    logger.Error($"An internal error has occur for {mod.GetName()}:");
                    logger.Error(ex.ToString());
                }
            }

            foreach(Mod mod in mods)
            {
                try
                {
                    mod.LoadGeneratorsStage(generatorRegistry);
                }
                catch (Exception ex)
                {
                    logger.Error($"An internal error has occur for {mod.GetName()}:");
                    logger.Error(ex.ToString());
                }
            }

            foreach(Mod mod in mods)
            {
                try
                {
                    mod.OnEnable();
                }
                catch (Exception ex)
                {
                    logger.Error($"An internal error has occur for {mod.GetName()}:");
                    logger.Error(ex.ToString());
                }
            }
        }
    }
}