using System;
using System.Collections.Generic;
using System.Text;

namespace OpenTK
{
    public struct Axis
    {
        public Vector2 normal;
        public Vector2 unit;
        public Vector2 edge;

        public readonly static Axis Zero = new Axis(Vector2.Zero, Vector2.Zero, Vector2.Zero);

        public Axis(Vector2 n, Vector2 u, Vector2 e)
        {
            this.normal = n;
            this.unit = u;
            this.edge = e;
        }

        public override string ToString()
        {
            return string.Format("N:{0}, U:{1}, E:{2}", normal, unit, edge);
        }
    }
}
