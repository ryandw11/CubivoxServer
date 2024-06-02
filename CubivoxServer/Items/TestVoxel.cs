using CubivoxCore.Attributes;
using CubivoxCore.Mods;
using CubivoxCore.Voxels;

namespace CubivoxServer.Items
{
    [Name("TestBlock")]
    [Key("TESTBLOCK")]
    [Texture("")]
    public class TestVoxel : ModVoxel
    {
        public TestVoxel(Mod mod) : base(mod)
        {
        }
    }
}
