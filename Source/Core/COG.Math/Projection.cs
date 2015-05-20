using System;
using System.Collections.Generic;
using System.Text;

namespace COG.Math
{
    public struct Projection
    {
        public double min;
        public double max;
        public double center;

        public Projection(double min, double max)
        {
            this.min = min;
            this.max = max;
            this.center = (min + max) / 2;
        }

        public bool overlap(Projection other)
        {
            return (other.min >= min && other.min <= max) || (other.max >= min && other.max <= max) ||
                (min >= other.min && min <= other.max) || (max >= other.min && max <= other.max);
        }

        public double getOverlap(Projection other)
        {
            if (min == other.min && max == other.max)
                return max - min;
            else if (min < other.min && max > other.max) //contains other
                return other.max - other.min;
            else if (other.min < min && other.max > max)
                return max - min;
            else if (min < other.min)
                return max - other.min;

            return other.max - min;
        }

        public override string ToString()
        {
            return string.Format("min: {0}, max: {1}", min, max);
        }
    }
}
