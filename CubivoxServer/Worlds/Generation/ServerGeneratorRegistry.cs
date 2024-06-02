using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CubivoxCore;
using CubivoxCore.BaseGame;
using CubivoxCore.Mods;
using CubivoxCore.Worlds.Generation;

namespace CubivoxServer.Worlds.Generation
{
    public class ServerGeneratorRegistry : GeneratorRegistry
    {
        private Dictionary<ControllerKey, WorldGenerator> worldGeneratorDictionary;

        public ServerGeneratorRegistry()
        {
            worldGeneratorDictionary = new Dictionary<ControllerKey, WorldGenerator>();
        }

        public WorldGenerator GetDefaultWorldGenerator()
        {
            return worldGeneratorDictionary.Values.First();
        }

        public WorldGenerator GetWorldGenerator(ControllerKey controllerKey)
        {
            return worldGeneratorDictionary[controllerKey];
        }

        public void RegisterWorldGenerator(Mod mod, WorldGenerator worldGenerator)
        {
            worldGeneratorDictionary.Add(new ControllerKey(mod, worldGenerator.Key), worldGenerator);
        }
    }
}
