using System;
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





        [STAThread]
        static void Main(string[] args)
        {
            new Logging.ConsoleLogger();
            //new ConsoleListener();
            using (var engine = new Engine())
            {
                engine.Run(new MainMenu());
            }
        }
    }
}
