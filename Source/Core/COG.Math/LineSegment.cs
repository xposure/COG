using System;
using System.Collections.Generic;
using System.Text;

namespace OpenTK
{
    public struct LineSegment
    {
        public Vector2 p0;
        public Vector2 p1;

        public LineSegment(Vector2 p0, Vector2 p1)
        {
            this.p0 = p0;
            this.p1 = p1;
        }

        public Vector2 closest(Vector2 p)
        {
            var l2 = (p0 - p1).LengthSquared;
            if (l2 == 0)
                return p0;

            var t = Vector2.Dot(p - p0, p1 - p0) / l2;
            if (t < 0)
                return p0;
            else if (t > 1)
                return p1;

            return p0 + t * (p1 - p0);
        }


        public float distance(Vector2 p)
        {
            var l2 = (p0 - p1).LengthSquared;
            if (l2 == 0)
                return (p0 - p).Length;

            var t = Vector2.Dot(p - p0, p1 - p0) / l2;
            if (t < 0)
                return (p0 - p).Length;
            else if (t > 1)
                return (p1 - p).Length;

            var projection = p0 + t * (p1 - p0);
            return (p - projection).Length;
        }

        public float distanceSquared(Vector2 p)
        {
            var l2 = (p0 - p1).LengthSquared;
            if (l2 == 0)
                return (p0 - p).LengthSquared;

            var t = Vector2.Dot(p - p0, p1 - p0) / l2;
            if (t < 0)
                return (p0 - p).LengthSquared;
            else if (t > 1)
                return (p1 - p).LengthSquared;

            var projection = p0 + t * (p1 - p0);
            return (p - projection).LengthSquared;
        }
    }

}