using System;
using System.Collections.Generic;
using System.Text;

namespace COG.Assets
{
    public class NullSource : AbstractSource
    {
        public NullSource(string id)
            : base(id)
        {

        }

        protected override void load()
        {
            //addEntry
        }
    }
}
