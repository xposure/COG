using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COG.Framework;

namespace COG.Dredger
{
    public class Config : Registry
    {
        private Module m_module;

        public Config()
        {
            m_module = new Module("dredger");
        }


        public Module Module { get { return m_module; } }
    }
}
