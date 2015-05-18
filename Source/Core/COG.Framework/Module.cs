using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COG.Framework
{
    public class Module
    {
        private string m_name;

        public Module(string moduleName)
        {
            Contract.RequiresNotEmpty(moduleName, "module");
            m_name = moduleName;
        }

        public string Name { get { return m_name; } }

        public SimpleUri CreateUri(string objectName)
        {
            return new SimpleUri(m_name, objectName);
        }
    }
}
