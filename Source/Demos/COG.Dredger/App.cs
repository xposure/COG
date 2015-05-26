using System;
using System.Diagnostics;
using COG.Dredger.States;
using COG.Dredger.World;

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


        static void Test(Map map)
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
            var map = new Map(16);

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


        [STAThread]
        static void Main(string[] args)
        {
            Time(10);

            new Logging.ConsoleLogger();
            //new ConsoleListener();
            using (var engine = new Engine())
            {
                engine.Run(new MainMenu());
            }
        }
    }
}
