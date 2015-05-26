using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using OpenTK;

namespace COG.Dredger.World
{
    public class Map
    {
        private int m_columnsXZ;
        private int m_columnsXZSqr;
        private MapColumn[] m_columns;


        public Map(int columnsXZ)
        {
            m_columnsXZ = columnsXZ;
            m_columnsXZSqr = m_columnsXZ * m_columnsXZ;

            m_columns = new MapColumn[m_columnsXZSqr];
            m_columns.InitArray(i => { return new MapColumn(i % m_columnsXZ, i / m_columnsXZ, this); });
        }

        public int BlocksX { get { return m_columnsXZ * Config.MAP_COLUMN_SIZE; } }
        public int BlocksZ { get { return m_columnsXZ * Config.MAP_COLUMN_SIZE; } }

        public MapColumn GetMapColumn(int columnX, int columnZ)
        {
            if (columnX < 0 || columnZ < 0 || columnX >= m_columnsXZ || columnZ >= m_columnsXZ)
                return null;

            return m_columns[columnX + (columnZ * m_columnsXZ)];
        }

        public IEnumerable<MapColumn> Columns
        {
            get
            {
                for (var i = 0; i < m_columns.Length; i++)
                    yield return m_columns[i];
            }
        }

    }

    public class MapColumn
    {
        private Map m_map;
        private int m_x, m_z;
        private int m_maxHeight;

        //data is stored by Z, X, Y - (y + (x  
        private MapBlock[] m_blocks;
        private MapBlock[,,] m_blocks2;

        public MapColumn(int x, int z, Map map)
        {
            m_x = x;
            m_z = z;
            m_map = map;

            m_blocks = new MapBlock[Config.MAP_COLUMN_SIZE_SQR * Config.MAP_COLUMN_HEIGHT];
            m_blocks2 = new MapBlock[Config.MAP_COLUMN_SIZE, Config.MAP_COLUMN_SIZE, Config.MAP_COLUMN_HEIGHT];
        }

        public int MapX { get { return m_x; } }
        public int MapY { get { return m_z; } }
        public int BlockMinX { get { return m_x * Config.MAP_COLUMN_SIZE; } }
        public int BlockMinZ { get { return m_z * Config.MAP_COLUMN_SIZE; } }
        public int BlockMaxX { get { return (m_x + 1) * Config.MAP_COLUMN_SIZE - 1; } }
        public int BlockMaxZ { get { return (m_z + 1) * Config.MAP_COLUMN_SIZE - 1; } }

        public void ComputeMaxHeight()
        {
            m_maxHeight = 0;
            //var it = 0;

            IterateZX((x, z) =>
            {
                for (var y = Config.MAP_COLUMN_HEIGHT - 1; y >= m_maxHeight; --y)
                {
                    //it++;
                    var index = (x * Config.MAP_COLUMN_SIZE) + (z * Config.MAP_COLUMN_SIZE * Config.MAP_COLUMN_SIZE) + y;
                    //var index = x + (z * Config.MAP_COLUMN_SIZE) + (y * Config.MAP_COLUMN_SIZE * Config.MAP_COLUMN_SIZE);
                    var block = m_blocks[index];
                    if (!block.IsAir)
                    {
                        Utility.Max(m_maxHeight, y);
                    }
                }
            });
            //Console.WriteLine("it: {0}", it);
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
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MapBlock
    {
        private long m_byte;

        public bool IsAir { get { return m_byte == 0; } }
    }
}
