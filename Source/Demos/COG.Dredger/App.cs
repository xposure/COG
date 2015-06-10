using System;
using System.Diagnostics;
using COG.Dredger.States;

namespace COG.Dredger
{
    class App
    {
        /* 
         * 6 buffers 
         *
            world
            128x128x128 = small ??
            256x256x128 = med
            512x512x128 = large
            1024x1024x128 = massive

            MapColumn
            16x16x128 	= 32768 blocks
            8x8   		= 64 MapColumn on small
            8*8*8*6*6	= 9126 vertices (8 cubes * 6 sides * 6 vertices)

            MapColumnLayer
            9126 * 16 * 16	= 2359296 (2.25mb)
            2.26mb x 8 x 8  = 144mb in vertices at 1byte for small


        */


        static void Test(Map2 map)
        {
            foreach (var column in map.Columns)
                column.ComputeMaxHeight();
        }

        //static void Test1(Map map)
        //{
        //    foreach (var column in map.Columns)
        //        column.ComputeMaxHeightFast();
        //}

        //static void Test2(Map map)
        //{
        //    foreach (var column in map.Columns)
        //        column.ComputeMaxHeightSlow();
        //}

        //static void Test3(Map map)
        //{
        //    foreach (var column in map.Columns)
        //        column.ComputeMaxHeightSuperSlow();
        //}

        static void Time(int iterations)
        {
            var map = new Map2(16);

            for (var i = 0; i < iterations; i++)
            {
                double dt0, dt1;
                //{
                //    var sw = Stopwatch.StartNew();
                //    Test2(map);
                //    sw.Stop();
                //    Console.WriteLine("Test2: {0}", sw.Elapsed);
                //    dt1 = sw.Elapsed.TotalSeconds;
                //}
                //{
                //    var sw = Stopwatch.StartNew();
                //    Test1(map);
                //    sw.Stop();
                //    Console.WriteLine("Test1: {0}", sw.Elapsed);
                //    dt0 = sw.Elapsed.TotalSeconds;
                //}
                //{
                //    var sw = Stopwatch.StartNew();
                //    Test3(map);
                //    sw.Stop();
                //    Console.WriteLine("Test3: {0}", sw.Elapsed);
                //    dt1 = sw.Elapsed.TotalSeconds;
                //}
                {
                    var sw = Stopwatch.StartNew();
                    Test(map);
                    sw.Stop();
                    Console.WriteLine("Test: {0}", sw.Elapsed.TotalSeconds / iterations);
                    dt1 = sw.Elapsed.TotalSeconds;
                }

                //var dt = dt1 - dt0;

                //if(dt < 0)
                //    Console.WriteLine("Test2 fast by: {0}", -dt);
                //else
                //    Console.WriteLine("Test1 fast by: {0}", dt);

                Console.WriteLine();

            }

        }

        public static int IndexXZ(int x, int z, int ChunksHorizontal = 2, int ChunkSize = 2)
        {
            var cx = x / ChunkSize;
            var cz = z / ChunkSize;

            var start = ((cz * ChunksHorizontal) + (cx)) * ChunkSize * ChunkSize;
            var nx = x % ChunkSize;
            var nz = z % ChunkSize;


            var index = start + (nz * ChunkSize) + nx;

            Console.WriteLine("x:{0}, z:{1}, index: {2}", x, z, index);
            return index;
        }

        public static int CellXZ(int x, int z, int ChunksHorizontal = 2, int ChunkSize = 2)
        {
            var cx = x / ChunkSize;
            var cz = z / ChunkSize;

            var start = ((cz * ChunksHorizontal) + (cx)) * ChunkSize * ChunkSize;
            var nx = x % ChunkSize;
            var nz = z % ChunkSize;


            var index = start + (nz * ChunkSize) + nx;

            Console.WriteLine("x:{0}, z:{1}, index: {2}", x, z, index);
            return index;
        }

        public static int ChunkXY(int x, int z, int ChunksHorizontal = 2, int ChunkSize = 2, int ChunkHeight = 2)
        {
            var cx = x / ChunkSize;
            var cz = z / ChunkSize;

            var start = ((cz * ChunksHorizontal) + (cx)) * ChunkSize * ChunkSize * ChunkHeight;
            var nx = x % ChunkSize;
            var nz = z % ChunkSize;


            var index = start + (nz * ChunkSize * ChunkHeight) + (nx * ChunkSize);

            Console.WriteLine("x:{0}, z:{1}, index: {2}", x, z, index);
            return index;
        }

        [STAThread]
        static void Main(string[] args)
        {
            //foreach (var p in GridRayTracer.raytrace(5, 5, 5, 0, 0, 0))
            //{
            //    Console.WriteLine(p);
            //}

            for (var y = 0; y < 2; y++)
                for (var z = 0; z < 4; z++)
                    for (var x = 0; x < 4; x++)
                        ChunkXY(x, z, 2, 2, 2);




            //for (var y = 0; y < 2; y++)
            //    for (var z = 0; z < 4; z++)
            //        for (var x = 0; x < 4; x++)
            //            IndexXYZ(x, z, 2, 2, 2);

            //    IndexXZ(0, 0);
            //IndexXZ(2, 1);
            //IndexXZ(2, 2);
            //IndexXZ(1, 3);
            //IndexXZ(3, 2);
            Console.Read();
            //for (var i = 0f; i < 2f; i += 0.05f)
            //{
            //    var truncate = Math.Truncate(i + 0.2f);
            //    var fraction = (i - truncate) / 2f;

            //    Console.WriteLine("{0}:{1}", i, fraction);
            //}
            //Time(10);

            new Logging.ConsoleLogger();
            //new ConsoleListener();
            using (var engine = new Engine())
            {
                engine.Run(new MainMenu());
            }
        }
    }
}
