using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COG.Framework;

namespace COG.Dredger
{
    public class Config : Registry
    {
        public const int MAP_COLUMN_SIZE = 16;
        public const int MAP_COLUMN_SIZE_SQR = MAP_COLUMN_SIZE * MAP_COLUMN_SIZE;
        public const int MAP_COLUMN_HEIGHT = 16;
        //public const int MAP_COLUMN_HEIGHT = 128;

        private Module m_module;
        private int m_windowWidth = 1024, m_windowHeight = 768;

        public Config()
        {
            m_module = new Module("dredger");
        }

        public void Load()
        {
            //read from file?
        }

        public Module Module { get { return m_module; } }
        public int WindowWidth { get { return m_windowWidth; } }
        public int WindowHeight { get { return m_windowHeight; } }
    }
}
