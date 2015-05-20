using System;
using System.Collections.Generic;
using System.Text;

namespace COG.Math
{
    public enum ShapeType : byte
    {
        Polygon,
        Compounded,
    }

    public interface ICollidable
    {
        AxisAlignedBox2 bounds { get; }
        ShapeType shareType { get; }
        MinimumTranslationVector intersects(ICollidable shape);
        void update(Matrix3 matrix);
        //void render(Color color);
    }

    public struct Shape : ICollidable
    {
        private bool _axesDirty;
        private Vector2[] _vertices;
        private Vector2[] _derivedVertices;
        private Axis[] _axes;

        public Shape(Vector2[] vertices)
        {
            this._axesDirty = true;
            this._vertices = (Vector2[])vertices.Clone();
            this._derivedVertices = (Vector2[])vertices.Clone();
            this._axes = null;
        }

        public Shape(AxisAlignedBox2 box)
        {
            this._axesDirty = true;
            this._vertices = (Vector2[])box.Corners.Clone();
            this._derivedVertices = (Vector2[])box.Corners.Clone();
            this._axes = null;
        }

        public ShapeType shareType { get { return ShapeType.Polygon; } }

        public Vector2[] vertices { get { return _vertices; } }

        public Vector2[] derivedVertices { get { return _derivedVertices; } }

        public AxisAlignedBox2 bounds
        {
            get
            {
                var aabb = new AxisAlignedBox2(derivedCenter, derivedCenter);
                for (var i = 0; i < _derivedVertices.Length; i++)
                    aabb.Merge(_derivedVertices[i]);

                return aabb;
            }
        }

        public Axis[] axes
        {
            get
            {
                if (_axesDirty)
                    updateAxes();

                return _axes;
            }
        }

        public Vector2 center
        {
            get
            {
                var c = Vector2.Zero;
                for (var i = 0; i < _vertices.Length; i++)
                    c += _vertices[i];

                return c / _vertices.Length;
            }
        }

        public Vector2 derivedCenter
        {
            get
            {
                var c = Vector2.Zero;
                for (var i = 0; i < _derivedVertices.Length; i++)
                    c += _derivedVertices[i];

                return c / _derivedVertices.Length;
            }
            set
            {
                for (var i = 0; i < _derivedVertices.Length; i++)
                    _derivedVertices[i] = value + _vertices[i];
                
                _axesDirty = true;
            }
        }

        public void update(AxisAlignedBox2 box)
        {
            if (_vertices.Length != 4)
                throw new Exception("can't update with an aabb unless the shape has 4 verts");

            _axesDirty = true;
            for (var i = 0; i < _vertices.Length; i++)
            {
                _vertices[i] = box.Corners[i];
                _derivedVertices[i] = box.Corners[i];
            }
        }

        public void update(Matrix3 matrix)
        {
            for (var i = 0; i < _vertices.Length; i++)
                _derivedVertices[i] = Vector2.Transform(_vertices[i], matrix);
            

            _axesDirty = true;
        }

        private void updateAxes()
        {
            if (_axesDirty)
            {
                if (_axes == null)
                    _axes = new Axis[_derivedVertices.Length];

                // loop over the vertices
                for (int i = 0; i < _derivedVertices.Length; i++)
                {
                    var axis = new Axis();

                    // get the current vertex
                    var p1 = _derivedVertices[i];
                    // get the next vertex
                    var p2 = _derivedVertices[(i + 1) % _derivedVertices.Length];
                    // subtract the two to get the edge vector
                    axis.edge = p1 - p2;
                    // get either perpendicular vector
                    axis.unit = axis.edge.Perpendicular;
                    // the perp method is just (x, y) => (-y, x) or (y, -x)
                    axis.normal = axis.unit.ToNormalized();
                    _axes[i] = axis;
                }

                _axesDirty = false;
            }
        }

        public Projection project(Axis axis)
        {
            var min = axis.normal.Dot(_derivedVertices[0]);
            var max = min;
            for (int i = 1; i < _derivedVertices.Length; i++)
            {
                // NOTE: the axis must be normalized to get accurate projections
                var p = axis.normal.Dot(_derivedVertices[i]);
                if (p < min)
                    min = p;
                else if (p > max)
                    max = p;
            }
            return new Projection(min, max);
        }

        public MinimumTranslationVector intersects(ICollidable other)
        {
            if (other.shareType == ShapeType.Polygon)
                return intersects(this, (Shape)other);
            else if (other.shareType == ShapeType.Compounded)
                return intersects(this, (CompoundShape)other);
            return MinimumTranslationVector.Zero;
        }

        //public void render(Color color)
        //{
        //    for (var i = 0; i < _derivedVertices.Length; i++)
        //        Root.instance.graphics.DrawLine(15, null, _derivedVertices[i], _derivedVertices[(i + 1) % _derivedVertices.Length], color);
        //}

        public static MinimumTranslationVector intersects(Shape shape1, Shape shape2)
        {
            var overlap = double.MaxValue;
            var smallest = Axis.Zero;
            var axes1 = shape1.axes;
            var axes2 = shape2.axes;
            // loop over the axes1
            for (int i = 0; i < axes1.Length; i++)
            {
                var axis = axes1[i];
                // project both shapes onto the axis
                var p1 = shape1.project(axis);
                var p2 = shape2.project(axis);
                // do the projections overlap?
                if (!p1.overlap(p2))
                {
                    // then we can guarantee that the shapes do not overlap
                    return MinimumTranslationVector.Zero;
                }
                else
                {
                    // get the overlap
                    var o = p1.getOverlap(p2);
                    // check for minimum
                    if (o < overlap)
                    {
                        // then set this one as the smallest
                        overlap = o;
                        smallest = axis;
                        if (p1.center < p2.center)
                            smallest.normal = -smallest.normal;
                    }
                }
            }
            // loop over the axes2
            for (int i = 0; i < axes2.Length; i++)
            {
                var axis = axes2[i];
                // project both shapes onto the axis
                var p1 = shape1.project(axis);
                var p2 = shape2.project(axis);
                // do the projections overlap?
                if (!p1.overlap(p2))
                {
                    // then we can guarantee that the shapes do not overlap
                    return MinimumTranslationVector.Zero;
                }
                else
                {
                    // get the overlap
                    var o = p1.getOverlap(p2);
                    // check for minimum
                    if (o < overlap)
                    {
                        // then set this one as the smallest
                        overlap = o;
                        smallest = axis;
                        if (p1.center < p2.center)
                            smallest.normal = -smallest.normal;
                    }
                }
            }
            // if we get here then we know that every axis had overlap on it
            // so we can guarantee an intersection
            return new MinimumTranslationVector(smallest, overlap);

        }

        public static MinimumTranslationVector intersects(Shape shape1, CompoundShape shape2)
        {
            if (shape2.shapes == null)
                return MinimumTranslationVector.Zero;

            var mtv = MinimumTranslationVector.Zero;
            var overlapped = false;
            foreach (var shape in shape2.shapes)
            {
                var nextmtv = shape1.intersects(shape);
                if (nextmtv.intersects && (nextmtv.overlap < mtv.overlap || !overlapped))
                {
                    overlapped = true;
                    mtv = nextmtv;
                }
            }

            return mtv;
        }
    }

    public struct CompoundShape : ICollidable
    {
        private List<ICollidable> _shapes;
        private Vector2 center;

        public ShapeType shareType { get { return ShapeType.Compounded; } }

        public IEnumerable<ICollidable> shapes { get { return _shapes; } }

        public void addShape(ICollidable shape)
        {
            if (_shapes == null)
                _shapes = new List<ICollidable>();
            _shapes.Add(shape);
        }

        public void update(Matrix3 matrix)
        {
            center = Vector2.Transform(Vector2.Zero, matrix);
            if (_shapes != null)
                foreach (var shape in _shapes)
                    shape.update(matrix);
        }

        public AxisAlignedBox2 bounds
        {
            get
            {
                if (_shapes != null && _shapes.Count > 0)
                {
                    var aabb = _shapes[0].bounds;
                    for (var i = 1; i < _shapes.Count; i++)
                        aabb.Merge(_shapes[i].bounds);

                    return aabb;
                }

                return AxisAlignedBox2.FromDimensions(center, Vector2.Zero);
            }
        }

        public MinimumTranslationVector intersects(ICollidable other)
        {
            if (other.shareType == ShapeType.Polygon)
                return intersects(this, (Shape)other);
            else if (other.shareType == ShapeType.Compounded)
                return intersects(this, (CompoundShape)other);

            return MinimumTranslationVector.Zero;
        }

        //public void render(Color color)
        //{
        //    if (_shapes != null)
        //        foreach (var shape in _shapes)
        //            shape.render(color);
        //}

        public static MinimumTranslationVector intersects(CompoundShape shape1, Shape shape2)
        {
            if (shape1._shapes == null)
                return MinimumTranslationVector.Zero;

            var mtv = MinimumTranslationVector.Zero;
            var overlapped = false;
            foreach (var shape in shape1._shapes)
            {
                var nextmtv = shape.intersects(shape2);
                if (nextmtv.intersects && (nextmtv.overlap < mtv.overlap || !overlapped))
                {
                    overlapped = true;
                    mtv = nextmtv;
                }
            }

            return mtv;
        }

        public static MinimumTranslationVector intersects(CompoundShape cs1, CompoundShape cs2)
        {
            if (cs1._shapes == null || cs2._shapes != null)
                return MinimumTranslationVector.Zero;

            var mtv = MinimumTranslationVector.Zero;
            var overlapped = false;
            foreach (var shape1 in cs1._shapes)
            {
                foreach (var shape2 in cs2._shapes)
                {
                    var nextmtv = shape1.intersects(shape2);
                    if (nextmtv.intersects && (nextmtv.overlap < mtv.overlap || !overlapped))
                    {
                        overlapped = true;
                        mtv = nextmtv;
                    }
                }
            }

            return mtv;
        }
    }

}

