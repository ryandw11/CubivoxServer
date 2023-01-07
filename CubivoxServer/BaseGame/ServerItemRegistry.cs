using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CubivoxCore;
using CubivoxCore.BaseGame;

namespace CubivoxServer.BaseGame
{
    public class ServerItemRegistry : ItemRegistry
    {
        private Dictionary<ControllerKey, Item> itemDictionary;
        private List<ControllerKey> idList;

        public ServerItemRegistry()
        {
            itemDictionary = new Dictionary<ControllerKey, Item>();
            idList = new List<ControllerKey>();
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
            // TODO :: Load from save file.
            idList.Add(item.GetControllerKey());
        }

        public void UnregisterItem(Item item)
        {
            itemDictionary.Remove(item.GetControllerKey());
        }

        public ControllerKey GetKeyFromId(ushort id)
        {
            return idList[id];
        }

        /// <summary>
        /// Get the ControllerKey array in the order of its ID.
        /// </summary>
        /// 
        /// <returns>The ControllerKey array in the order of its ID.</returns>
        public ControllerKey[] GetKeyArray()
        {
            return idList.ToArray();
        }
    }
}
