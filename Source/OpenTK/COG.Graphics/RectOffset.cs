//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace COG.Graphics
//{
//    public struct RectOffset
//    {
//        //public Vector2 offset;
//        public Vector2 min;
//        public Vector2 max;

//        public RectOffset(Vector2 min, Vector2 max)
//        {
//            this.min = min;
//            this.max = max;
//        }

//        public RectOffset(float left, float right, float top, float bottom)
//        {
//            this.min = new Vector2(left, top);
//            this.max = new Vector2(right, bottom);
//        }

//        public float left { get { return min.X; } set { min.X = value; } }
//        public float right { get { return max.X; } set { max.X = value; } }
//        public float top { get { return min.Y; } set { min.Y = value; } }
//        public float bottom { get { return max.Y; } set { max.Y = value; } }

//        public float horizontal { get { return left + right; } }
//        public float vertical { get { return top + bottom; } }

//        public Vector2 size { get { return new Vector2(horizontal, vertical); } }
//        //public Vector2 right { get { return new Vector2(max.X, 0); } }

//        //public Vector2 top { get { return new Vector2(0, min.Y); } }
//        //public Vector2 bottom { get { return new Vector2(0, max.Y); } }

//        //public Vector2 sizeX { get { return new Vector2(min.X + max.X, 0); } }
//        //public Vector2 sizeY { get { return new Vector2(0, min.Y + max.Y); } }
//        //public float xWidth { get { return left.X + right.X; } }
//        //public float yHeight { get { return top.Y + bottom.Y; } }

//        //public Vector2 size { get { return new Vector2(xWidth, yHeight); } }

//        //public AxisAlignedBox topLeft { get { return AxisAlignedBox.FromRect(Vector2.Zero + offset, min); } }
//        //public AxisAlignedBox left { get { return AxisAlignedBox.FromRect(new Vector2(0, min.Y) Vector2.Zero + offset, min); } }
//        //public AxisAlignedBox add(AxisAlignedBox box)
//        //{
//        //    return new AxisAlignedBox(box.minVector , box.maxVector + min + max);
//        //}
//    }
//}

