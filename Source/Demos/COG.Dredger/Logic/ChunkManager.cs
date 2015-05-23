using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COG.Dredger.Rendering;
using COG.Framework;
using COG.Graphics;
using OpenTK;

namespace COG.Dredger.Logic
{
    public enum NoiseType { Perlin, Billow, RiggedMultifractal, Voronoi, Mix };

    public class ChunkManager : DisposableObject
    {
        public int resolution = 64;
        public NoiseType noise = NoiseType.Perlin;
        public float zoom = 1f;
        public float offset = 0f;
        public bool wireframe = false;

        public const int GRID_SIZE = 16; // MAX 64
        public const int CHUNK_HALFSIZE = 8;
        public const int CHUNK_SIZE = CHUNK_HALFSIZE + CHUNK_HALFSIZE; //MAX 64
        public const int MAP_DEPTH = 32; //MAX 256;

        private Volume[,] chunks;

        private Volume hover;
        private int sealevel = 8;

        public void Initialize()
        {
            chunks = new Volume[GRID_SIZE, GRID_SIZE];
            hover = SurfaceExtractor.makeVoxels(0, 0, 0,
                   new int[] { 0, 0, 0 },
                   new int[] { 1, 1, 1 },
                       (i, j, k) =>
                       {
                           return 0xff0000;
                       }
                   );

            hover.PrepareMesh();
            SurfaceExtractor.GenerateMesh(hover, centered: true);

            for (var x = 0; x < GRID_SIZE; ++x)
            {
                for (var z = 0; z < GRID_SIZE; ++z)
                {
                    var chunk = SurfaceExtractor.makeVoxels(x * CHUNK_SIZE, 0, z * CHUNK_SIZE,
                        new int[] { 0, 0, 0 },
                        new int[] { CHUNK_SIZE, CHUNK_SIZE, CHUNK_SIZE },
                           Generators.GenerateHeight(x, 0, z, CHUNK_SIZE, CHUNK_SIZE, noise, sealevel)
                        );

                    chunks[x, z] = chunk;
                }
            }

            //var opaque = Resources.Load<GameObject>("Prefabs/ChunkOpaque");
            //var water = Resources.Load<GameObject>("Prefabs/ChunkWater");
            for (var x = 0; x < GRID_SIZE; ++x)
            {
                for (var y = 0; y < GRID_SIZE; ++y)
                {
                    chunks[x, y].UpdateMesh();
                    //{

                    //    var r = Instantiate(opaque, new Vector3(chunks[x, y].X, chunks[x, y].Y, chunks[x, y].Z), Quaternion.identity);
                    //    r.name = string.Format("CHUNK_{0}_{1}_OPAQUE", x, y);
                    //    var rgo = (GameObject)r;
                    //    rgo.transform.parent = this.transform;
                    //    var filter = rgo.GetComponent<MeshFilter>();
                    //    filter.mesh = chunks[x, y].opaqueMesh;
                    //}
                    //{
                    //    var r = Instantiate(water, new Vector3(chunks[x, y].X, chunks[x, y].Y, chunks[x, y].Z), Quaternion.identity);
                    //    r.name = string.Format("CHUNK_{0}_{1}_WATER", x, y);
                    //    var rgo = (GameObject)r;
                    //    rgo.transform.parent = this.transform;
                    //    var filter = rgo.GetComponent<MeshFilter>();
                    //    filter.mesh = chunks[x, y].waterMesh;
                    //}
                }
            }

        }

        public void RenderOpaque(Program program)
        {
            if (chunks != null)
                for (var x = 0; x < GRID_SIZE; ++x)
                    for (var y = 0; y < GRID_SIZE; ++y)
                        chunks[x, y].RenderOpaque(program);
        }

        public void renderAlpha(Program program)
        {
            if (chunks != null)
                for (var x = 0; x < GRID_SIZE; ++x)
                    for (var y = 0; y < GRID_SIZE; ++y)
                        chunks[x, y].RenderAlpha(program);

        }

        protected override void DisposeManaged()
        {
            base.DisposeManaged();

            if (hover)
                hover.Dispose();

            if (chunks != null)
                for (var x = 0; x < GRID_SIZE; ++x)
                    for (var y = 0; y < GRID_SIZE; ++y)
                        chunks[x, y].Dispose();

            chunks = null;
        }
    }
}
