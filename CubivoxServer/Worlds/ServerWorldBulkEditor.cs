using CubivoxCore;
using CubivoxCore.Voxels;
using CubivoxCore.Worlds;

namespace CubivoxServer.Worlds
{
    public class ServerWorldBulkEditor : WorldBulkEditor
    {
        private Cuboid editCuboid;
        private ServerWorld parentWorld;
        private Dictionary<ChunkLocation, ChunkBulkEditor> chunkBulkEdits;

        public ServerWorldBulkEditor(Cuboid editCuboid, ServerWorld world)
        {
            this.editCuboid = editCuboid;
            this.parentWorld = world;
            chunkBulkEdits = new Dictionary<ChunkLocation, ChunkBulkEditor>();

            ObtainChunkBulkEdits(editCuboid);
        }

        public Cuboid EditCuboid => editCuboid;

        public void Fill(VoxelDef voxelDef)
        {
            foreach (var editorEntry in chunkBulkEdits)
            {
                var chunkCuboid = editorEntry.Key.AsCuboid();
                if( EditCuboid.Contains(chunkCuboid) )
                {
                    // Chunk is 100% within the boundaries, we can just fill the whole chunk.
                    editorEntry.Value.Fill(voxelDef);
                }
                else
                {
                    // Chunk is not 100% in the boundaries, clamp and fill instead.
                    var fillCube = chunkCuboid.Clamp(EditCuboid);
                    editorEntry.Value.FillCube(fillCube, voxelDef);
                }
            }
        }

        public void FillCube(int x1, int y1, int z1, int x2, int y2, int z2, VoxelDef voxelDef)
        {
            var loc1 = new Location(parentWorld, x1, y1, z1);
            var loc2 = new Location(parentWorld, x2, y2, z2);
            FillCube(new Cuboid(loc1, loc2), voxelDef);
        }

        public void FillCube(Cuboid cuboid, VoxelDef voxelDef)
        {
            foreach (var editorEntry in chunkBulkEdits)
            {
                if( editorEntry.Key.AsCuboid().Overlaps(cuboid) )
                {
                    editorEntry.Value.FillCube(cuboid, voxelDef);
                }
            }
        }

        public void SetVoxel(int x, int y, int z, VoxelDef voxelDef)
        {
            ChunkLocation loc = new Location(parentWorld, x, y, z).ToChunkLocation();
            ChunkBulkEditor? chunkEditor;

            if( chunkBulkEdits.TryGetValue(loc, out chunkEditor) )
            {
                chunkEditor.SetVoxel(x, y, z, voxelDef);
            }
            else
            {
                throw new ArgumentException("Cannot find voxel within bulk edit region.");
            }
        }

        public void Submit()
        {
            foreach (var editorEntry in chunkBulkEdits)
            {
                parentWorld.GetChunk(editorEntry.Key).SubmitBulkEdit(editorEntry.Value);
            }

            chunkBulkEdits.Clear();
        }

        private void ObtainChunkBulkEdits(Cuboid cuboid)
        {
            // Convert Min and Max corners of the Cuboid to ChunkLocations
            ChunkLocation chunkMin = cuboid.MinCorner.ToChunkLocation();
            ChunkLocation chunkMax = cuboid.MaxCorner.ToChunkLocation();

            // Loop through the ChunkLocation space
            // The max is exclusive.
            for (int x = chunkMin.X; x <= chunkMax.X; x++)
            {
                for (int y = chunkMin.Y; y <= chunkMax.Y; y++)
                {
                    for (int z = chunkMin.Z; z <= chunkMax.Z; z++)
                    {
                        var chunkLocation = new ChunkLocation(parentWorld, x, y, z);
                        var chunk = parentWorld.GetChunk(chunkLocation);

                        if( chunk == null )
                        {
                            throw new ArgumentException("Cuboid provided to WorldBulkEditor extends past the boundary for the world.");
                        }

                        chunkBulkEdits.Add(chunkLocation, chunk.StartBulkEdit());
                    }
                }
            }
        }
    }
}
