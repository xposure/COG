using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using COG.Dredger.Logic;
using COG.Dredger.Rendering;
using COG.Framework;
using COG.Graphics;
using OpenTK;

namespace COG.Dredger
{
    /*
     * Top occluded and unoccluded voxel mesh 
     * 
     * 
     */
    public class World
    {

    }

    public class VoxelMeta
    {

    }

    public class Map
    {
        private int m_cellCount;
        private int m_chunksHorizontal;
        private Voxel[] m_cells;
        private VoxelMeta[] m_cellsMeta;

        public Map(int chunksHorizontal)
        {
            m_chunksHorizontal = chunksHorizontal; ;
            m_cellCount = chunksHorizontal * chunksHorizontal * Config.MAP_HEIGHT * Config.MAP_CHUNK_SIZE;
            m_cells = new Voxel[m_cellCount];
            m_cellsMeta = new VoxelMeta[m_cellCount];
        }

        public Voxel GetCell(int index)
        {
            return m_cells[index];
        }

        public void SetCell(int index, Voxel cell)
        {
            m_cells[index] = cell;
        }

        public int ChunksHorizontal { get { return m_chunksHorizontal; } }
        public int CellsHorizontal { get { return m_chunksHorizontal * Config.MAP_CHUNK_SIZE; } }
        //public IEnumerable<MapChunk> Chunks
        //{
        //    get
        //    {
        //        for (var z = 0; z < m_chunksHorizontal; z++)
        //        {
        //            for (var x = 0; x < m_chunksHorizontal; x++)
        //            {
        //                return MapChunk.GetChunkFromCell
        //            }
        //        }
        //    }
        //}
        //public int IndexXZ(int x, int z)
        //{
        //    var cx = x / Config.MAP_CHUNK_SIZE;
        //    var cz = z / Config.MAP_CHUNK_SIZE;

        //    return ((cz * ChunksHorizontal * ChunksHorizontal) + (cx * ChunksHorizontal));
        //}

        public int IndexXZ(int x, int z)
        {
            return ((z * CellsHorizontal * CellsHorizontal) + (x * CellsHorizontal)) * Config.MAP_HEIGHT;
        }

        public void IterateZX(Action<int, int> action)
        {
            for (var z = 0; z < CellsHorizontal; z++)
            {
                for (var x = 0; x < CellsHorizontal; x++)
                {
                    action(x, z);
                }
            }
        }

        public void GenerateMap(IGenerator gen)
        {
            IterateZX((x, z) =>
            {
                var bi = IndexXZ(x, z);
                var height = gen.GetHeight(x, z, Config.MAP_HEIGHT / 2);
                for (var y = 0; y <= height; y++)
                {
                    if (y < height - 4)
                        m_cells[bi + y] = VoxelDescriptor.Stone.Block;
                    else if (y == height)
                        m_cells[bi + y] = VoxelDescriptor.Grass.Block;
                    else
                        m_cells[bi + y] = VoxelDescriptor.Dirt.Block;
                }
            });

            //m_mesh = new DynamicMesh(VertexPositionTextureColor.VertexDeclaration);

            //var amount = Surface.Extract(m_mesh, m_blocks, Config.MAP_COLUMN_SIZE, Config.MAP_COLUMN_SIZE, Config.MAP_COLUMN_HEIGHT);
            //Console.WriteLine("Extract {0}: verts - {1}", new Vector2(BlockMinX, BlockMinZ), amount);
        }
    }

    public struct MapChunk
    {
        private Map m_map;
        private int m_startCell;
        private int m_chunkX, m_chunkY, m_chunkZ;

        public bool IsValid { get { return m_startCell == -1 || m_map == null; } }
        public int ChunkX { get { return m_chunkX; } }
        public int ChunkY { get { return m_chunkY; } }
        public int ChunkZ { get { return m_chunkZ; } }
        public int CellOffset { get { return m_startCell; } }

        public int GetIndex(int x, int z)
        {
            return Config.MAP_CHUNK_SIZE * z + x + m_startCell;
        }

        public static MapChunk Invalid { get { return new MapChunk() { m_startCell = -1 }; } }

        public static MapChunk GetChunkFromCell(Map map, int x, int y, int z)
        {
            if (Utility.NotInRange(x, 0, map.ChunksHorizontal * Config.MAP_CHUNK_SIZE))
                return Invalid;

            if (Utility.NotInRange(z, 0, map.ChunksHorizontal * Config.MAP_CHUNK_SIZE))
                return Invalid;

            //is this a good idea, it would at least semi keep neighboring cells in memory close together
            //on the other hand its treating chunks as cubes rather than layers which is how
            //the game will be using them
            //if (Utility.NotInRange(y, 0, map.ChunksY * Config.MAP_COLUMN_SIZE))
            //    return Invalid;

            if (Utility.NotInRange(y, 0, Config.MAP_HEIGHT))
                return Invalid;

            var cx = x / Config.MAP_CHUNK_SIZE;
            var cz = z / Config.MAP_CHUNK_SIZE;
            var cy = y;

            var index = map.IndexXZ(cx * Config.MAP_CHUNK_SIZE, cz * Config.MAP_CHUNK_SIZE);// ((cz * map.ChunksHorizontal * map.ChunksHorizontal) + (cx * map.ChunksHorizontal) + cy);
            return new MapChunk()
            {
                m_map = map,
                m_startCell = index + y * Config.MAP_CHUNK_SIZE,
                m_chunkX = cx,
                m_chunkY = cy,
                m_chunkZ = cz
            };
        }

        //public static MapChunk GetChunk(Map map, int cx, int cy, int cz)
        //{

        //}
    }


    public class Map2 : DisposableObject
    {
        private int m_columnsXZ;
        private int m_columnsXZSqr;
        private MapColumn2[] m_columns;


        public Map2(int columnsXZ)
        {
            m_columnsXZ = columnsXZ;
            m_columnsXZSqr = m_columnsXZ * m_columnsXZ;

            m_columns = new MapColumn2[m_columnsXZSqr];
            m_columns.InitArray(i => { return new MapColumn2(i % m_columnsXZ, i / m_columnsXZ, this); });
        }

        public int BlocksX { get { return m_columnsXZ * Config.MAP_COLUMN_SIZE; } }
        public int BlocksZ { get { return m_columnsXZ * Config.MAP_COLUMN_SIZE; } }

        public MapColumn2 GetMapColumn(int columnX, int columnZ)
        {
            if (columnX < 0 || columnZ < 0 || columnX >= m_columnsXZ || columnZ >= m_columnsXZ)
                return null;

            return m_columns[columnX + (columnZ * m_columnsXZ)];
        }

        public void Generate(IGenerator gen)
        {
            foreach (var column in Columns)
                column.GenerateMap(gen);
        }

        public IEnumerable<MapColumn2> Columns
        {
            get
            {
                for (var i = 0; i < m_columns.Length; i++)
                    yield return m_columns[i];
            }
        }

        public Voxel GetBlock(int x, int y, int z)
        {
            if (x < 0 || y < 0 || z < 0 || x >= BlocksX || z >= BlocksZ || y >= Config.MAP_COLUMN_HEIGHT)
                return VoxelDescriptor.Air.Block;

            var cx = x / Config.MAP_COLUMN_SIZE;
            var cz = z / Config.MAP_COLUMN_SIZE;
            var index = cz * m_columnsXZ + cx;

            return m_columns[index].GetBlock(x - (cx * Config.MAP_COLUMN_SIZE), y, z - (cz * Config.MAP_COLUMN_SIZE));

        }

        public void Render(Program program)
        {
            foreach (var column in Columns)
                column.Render(program);
        }

        protected override void DisposeManaged()
        {
            base.DisposeManaged();

            foreach (var column in Columns)
                column.Dispose();
        }
    }

    public class MapColumn2 : DisposableObject
    {
        private Map2 m_map;
        private int m_x, m_z;
        private int m_maxHeight;
        private DynamicMesh m_mesh;

        //data is stored by Z, X, Y - (y + (x  
        private Voxel[] m_blocks;
        private MapLayer2[] m_layers;

        public MapColumn2(int x, int z, Map2 map)
        {
            m_x = x;
            m_z = z;
            m_map = map;


            m_blocks = new Voxel[Config.MAP_COLUMN_SIZE_SQR * Config.MAP_COLUMN_HEIGHT];

            m_layers = new MapLayer2[Config.MAP_COLUMN_HEIGHT];
            m_layers.InitArray(y => new MapLayer2(x, y, z, map));
        }

        public int MapX { get { return m_x; } }
        public int MapZ { get { return m_z; } }
        public int BlockMinX { get { return m_x * Config.MAP_COLUMN_SIZE; } }
        public int BlockMinZ { get { return m_z * Config.MAP_COLUMN_SIZE; } }
        public int BlockMaxX { get { return (m_x + 1) * Config.MAP_COLUMN_SIZE - 1; } }
        public int BlockMaxZ { get { return (m_z + 1) * Config.MAP_COLUMN_SIZE - 1; } }

        public void ComputeMaxHeight()
        {
            m_maxHeight = 0;

            IterateZX((bi) =>
            {
                for (var y = Config.MAP_COLUMN_HEIGHT - 1; y >= m_maxHeight; --y)
                {
                    var block = m_blocks[bi + y];
                    if (!block.IsEmpty)
                    {
                        Utility.Max(m_maxHeight, y);
                    }
                }
            });
        }

        //IterateZX shows almost no difference in speed compared to hand typing the loop out
        //In fact its so close that its 50/50 on which one performs faster, my guess is that each 
        //loop fills up the cache to do enough work and the small function call (COLUMN_SIZE*COLUMN_SIZE)
        //has no impact on the actual execution loop
        //On the other hand, IterateZXY is 2x slower than IterateZX
        public void IterateZX(Action<int, int> action)
        {
            for (var z = 0; z < Config.MAP_COLUMN_SIZE; z++)
            {
                for (var x = 0; x < Config.MAP_COLUMN_SIZE; x++)
                {
                    action(x, z);
                }
            }
        }

        public void IterateZX(Action<int> action)
        {
            for (var z = 0; z < Config.MAP_COLUMN_SIZE; z++)
            {
                for (var x = 0; x < Config.MAP_COLUMN_SIZE; x++)
                {
                    action(IndexXZ(x, z));
                }
            }
        }

        public int IndexXZ(int x, int z)
        {
            return (z * Config.MAP_COLUMN_SIZE * Config.MAP_COLUMN_HEIGHT) + (x * Config.MAP_COLUMN_HEIGHT);
        }

        public void GenerateMap(IGenerator gen)
        {
            IterateZX((x, z) =>
            {
                var bi = IndexXZ(x, z);
                var height = gen.GetHeight(x + BlockMinX, z + BlockMinZ, Config.MAP_COLUMN_HEIGHT / 2);
                for (var y = 0; y <= height; y++)
                {
                    if (y < height - 4)
                        m_blocks[bi + y] = VoxelDescriptor.Stone.Block;
                    else if (y == height)
                        m_blocks[bi + y] = VoxelDescriptor.Grass.Block;
                    else
                        m_blocks[bi + y] = VoxelDescriptor.Dirt.Block;

                }
            });

            m_mesh = new DynamicMesh(VertexPositionTextureColor.VertexDeclaration);

            var amount = Surface.Extract(m_mesh, m_blocks, Config.MAP_COLUMN_SIZE, Config.MAP_COLUMN_SIZE, Config.MAP_COLUMN_HEIGHT);
            Console.WriteLine("Extract {0}: verts - {1}", new Vector2(BlockMinX, BlockMinZ), amount);
        }

        public Voxel GetBlock(int x, int y, int z)
        {
            return m_blocks[IndexXZ(x, z) + y];
        }

        public void Render(Program program)
        {
            var p = new Vector3(BlockMinX, 0, BlockMinZ);
            var m = Matrix4.CreateTranslation(p) * Matrix4.CreateScale(1f);
            program.SetUniformMatrix4("model", m);
            m_mesh.Render(program);
        }

        protected override void DisposeManaged()
        {
            base.DisposeManaged();

            if (m_mesh)
                m_mesh.Dispose();
        }
    }

    public class MapLayer2
    {
        private Map2 m_map;
        private int m_x, m_y, m_z;
        private int m_maxHeight;
        private DynamicMesh m_mesh;

        //data is stored by Z, X, Y - (y + (x  
        private Voxel[] m_blocks;

        public MapLayer2(int x, int y, int z, Map2 map)
        {
            m_x = x;
            m_y = y;
            m_z = z;
            m_map = map;

            m_blocks = new Voxel[Config.MAP_COLUMN_SIZE_SQR];
        }

        public int MapX { get { return m_x; } }
        public int MapY { get { return m_y; } }
        public int MapZ { get { return m_z; } }
        public int BlockMinX { get { return m_x * Config.MAP_COLUMN_SIZE; } }
        public int BlockMinZ { get { return m_z * Config.MAP_COLUMN_SIZE; } }
        public int BlockMaxX { get { return (m_x + 1) * Config.MAP_COLUMN_SIZE - 1; } }
        public int BlockMaxZ { get { return (m_z + 1) * Config.MAP_COLUMN_SIZE - 1; } }

        public int IndexXZ(int x, int z)
        {
            return (z * Config.MAP_COLUMN_SIZE + x);
        }

    }

    public class VoxelDescriptor
    {
        public static readonly VoxelDescriptor Air;
        public static readonly VoxelDescriptor Dirt;
        public static readonly VoxelDescriptor Grass;
        public static readonly VoxelDescriptor Stone;

        public static readonly VoxelDescriptor[] m_blocks = new VoxelDescriptor[256];

        static VoxelDescriptor()
        {
            Air = new VoxelDescriptor(0, new Color(0, 0, 0, 0));
            Dirt = new VoxelDescriptor(1, Color.Brown);
            Grass = new VoxelDescriptor(2, Color.Green);
            Stone = new VoxelDescriptor(3, Color.Gray);

            FillEmpty();
        }

        private static void FillEmpty()
        {
            for (var i = 0; i < m_blocks.Length; i++)
                if (m_blocks[i] == null)
                    m_blocks[i] = Air;
        }

        public static VoxelDescriptor Find(byte b)
        {
            return m_blocks[b];
        }

        private byte m_id;
        private Color m_color;

        public VoxelDescriptor(byte id, Color color)
        {
            m_id = id;
            m_color = color;

            m_blocks[id] = this;
        }

        public byte ID { get { return m_id; } }
        public Color Color { get { return m_color; } }
        public Voxel Block { get { return new Voxel() { m_byte = ID }; } }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0, Size = 1)]
    public struct Voxel
    {
        public byte m_byte;

        public VoxelDescriptor Descriptor { get { return VoxelDescriptor.Find(m_byte); } }

        public bool IsEmpty { get { return m_byte == 0; } }

        //public bool IsAir { get { return m_byte == 0; } }
        //public bool IsDirt { get { return m_byte == 1; } }
        //public bool IsGrass { get { return m_byte == 2; } }
        //public bool IsStone { get { return m_byte == 3; } }

        //public static MapBlock GetType(byte b)
        //{
        //    switch (b)
        //    {
        //        case 1: return Dirt;
        //        case 2: return Grass;
        //        case 3: return Stone;
        //    }

        //    return Air;
        //}
    }
}
