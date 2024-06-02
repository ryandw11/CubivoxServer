using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using CubivoxCore;
using CubivoxCore.Items;
using CubivoxCore.Voxels;

namespace CubivoxServer.Items
{
    public class ServerItemRegistry : ItemRegistry
    {
        private Dictionary<ControllerKey, Item> itemDictionary;

        private Dictionary<short, VoxelDef> voxelMap;
        private Dictionary<VoxelDef, short> reverseVoxelMap;
        private short currentVoxelIndex;

        public ServerItemRegistry()
        {
            itemDictionary = new Dictionary<ControllerKey, Item>();

            voxelMap = new Dictionary<short, VoxelDef>();
            reverseVoxelMap = new Dictionary<VoxelDef, short>();
            currentVoxelIndex = 0;
        }

        public Item GetItem(ControllerKey key)
        {
            return itemDictionary[key];
        }

        public List<Item> GetItems()
        {
            return itemDictionary.Values.ToList();
        }

        public void RegisterItem(Item item)
        {
            itemDictionary.Add(item.GetControllerKey(), item);

            if (item is VoxelDef)
            {
                voxelMap[currentVoxelIndex] = (VoxelDef)item;
                reverseVoxelMap[(VoxelDef)item] = currentVoxelIndex;
                currentVoxelIndex++;
            }
        }

        public void UnregisterItem(Item item)
        {
            itemDictionary.Remove(item.GetControllerKey());
        }

        public VoxelDef GetVoxelDef(short id)
        {
            return voxelMap[id];
        }

        public short GetVoxelDefId(VoxelDef voxelDef)
        {
            return reverseVoxelMap[voxelDef];
        }

        public JsonElement[] GetVoxelDict()
        {
            JsonElement[] copyDict = new JsonElement[voxelMap.Count];
            int i = 0;
            foreach (KeyValuePair<short, VoxelDef> pair in voxelMap)
            {
                copyDict[i] = JsonSerializer.SerializeToElement(new
                {
                    Id = pair.Key,
                    ControllerKey = pair.Value.GetControllerKey().ToString(),
                });
                i++;
            }
            return copyDict;
        }

        public VoxelDef GetVoxelDefinition(ControllerKey key)
        {
            Item item = itemDictionary[key];

            if (item is VoxelDef)
            {
                return (VoxelDef)item;
            }

            return null;
        }
    }
}
