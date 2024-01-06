using CubivoxCore;
using CubivoxCore.Attributes;
using CubivoxCore.Chat;
using CubivoxCore.Events;
using CubivoxCore.Mods;
using CubivoxCore.Players;

namespace CubivoxServer.Items
{
    [Name("TestBlock")]
    [Key("TESTBLOCK")]
    [Texture("")]
    public class TestVoxel : ModVoxel
    {
        public TestVoxel(Mod mod) : base(mod)
        {
            PlaceEvent += OnVoxelPlace;
            BreakEvent += OnVoxelBreak;
            Console.WriteLine("DONE!");
        }

        [ServerOnly]
        private void OnVoxelPlace(VoxelDefPlaceEvent evt)
        {
            Location loc = evt.Location;
            if (loc.x > 0)
            {
                evt.Player.SendMessage("Permission Denied. Cannot place a voxel in this area.".Color("red"));
                evt.Cancel();
                return;
            }
            Console.WriteLine($"{evt.Player.GetName()} has placed the test voxel!");
            evt.Player.SendMessage("Mmm, nice block you placed there at ".Color("red") + evt.Location.ToString().Color("green") + "!".Color("red"));
        }

        [ServerOnly]
        private void OnVoxelBreak(VoxelDefBreakEvent evt)
        {
            Location loc = evt.Location;
            if (loc.x > 0)
            {
                evt.Player.SendMessage("Permission Denied. Cannot break a voxel in this area.".Color("red"));
                evt.Cancel();
                return;
            }
        }
    }
}
