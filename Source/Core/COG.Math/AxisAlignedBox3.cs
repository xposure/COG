#region LGPL License
/*
Axiom Graphics Engine Library
Copyright © 2003-2011 Axiom Project Team

The overall design, and a majority of the core engine and rendering code 
contained within this library is a derivative of the open source Object Oriented 
Graphics Engine OGRE, which can be found at http://ogre.sourceforge.net.  
Many thanks to the OGRE team for maintaining such a high quality project.

The math library included in this project, in addition to being a derivative of
the works of Ogre, also include derivative work of the free portion of the 
Wild Magic mathematics source code that is distributed with the excellent
book Game Engine Design.
http://www.wild-magic.com/

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
*/
#endregion

#region SVN Version Information
// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: AxisAlignedBox.cs 2940 2012-01-05 12:25:58Z borrillis $"/>
// </file>
#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Diagnostics;

#endregion Namespace Declarations

namespace OpenTK
{
    /// <summary>
    ///		A 3D box aligned with the x/y/z axes.
    /// </summary>
    /// <remarks>
    ///		This class represents a simple box which is aligned with the
    ///	    axes. It stores 2 points as the extremeties of
    ///	    the box, one which is the minima of all 3 axes, and the other
    ///	    which is the maxima of all 3 axes. This class is typically used
    ///	    for an axis-aligned bounding box (AABB) for collision and
    ///	    visibility determination.
    /// </remarks>
    public sealed class AxisAlignedBox3 : ICloneable
    {
        #region Fields

        internal Vector3 minVector;
        internal Vector3 maxVector;
        private Vector3[] corners = new Vector3[8];
        private bool isNull;
        private bool isInfinite;

        #endregion

        #region Constructors

        public AxisAlignedBox3()
            : this(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.5f, 0.5f, 0.5f))
        {
            isNull = true;
            isInfinite = false;
        }

        public AxisAlignedBox3(Vector3 min, Vector3 max)
        {
            SetExtents(min, max);
        }

        public AxisAlignedBox3(AxisAlignedBox3 box)
        {
            SetExtents(box.Minimum, box.Maximum);
            isNull = box.IsNull;
            isInfinite = box.IsInfinite;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        public void Transform(Matrix4 matrix)
        {
            // do nothing for a null box
            if (isNull || isInfinite)
                return;

            Vector3 min;
            Vector3 max;
            Vector3 temp;

            temp = Vector3.Transform(corners[0], matrix);
            min = max = temp;

            for (int i = 1; i < corners.Length; i++)
            {
                // Transform and check extents
                temp = Vector3.Transform(corners[i], matrix);

                if (temp.X > max.X)
                    max.X = temp.X;
                else if (temp.X < min.X)
                    min.X = temp.X;

                if (temp.Y > max.Y)
                    max.Y = temp.Y;
                else if (temp.Y < min.Y)
                    min.Y = temp.Y;

                if (temp.Z > max.Z)
                    max.Z = temp.Z;
                else if (temp.Z < min.Z)
                    min.Z = temp.Z;
            }

            SetExtents(min, max);
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateCorners()
        {
            // The order of these items is, using right-handed co-ordinates:
            // Minimum Z face, starting with Min(all), then anticlockwise
            //   around face (looking onto the face)
            // Maximum Z face, starting with Max(all), then anticlockwise
            //   around face (looking onto the face)				
            corners[0] = minVector;
            corners[1].X = minVector.X;
            corners[1].Y = maxVector.Y;
            corners[1].Z = minVector.Z;
            corners[2].X = maxVector.X;
            corners[2].Y = maxVector.Y;
            corners[2].Z = minVector.Z;
            corners[3].X = maxVector.X;
            corners[3].Y = minVector.Y;
            corners[3].Z = minVector.Z;

            corners[4] = maxVector;
            corners[5].X = minVector.X;
            corners[5].Y = maxVector.Y;
            corners[5].Z = maxVector.Z;
            corners[6].X = minVector.X;
            corners[6].Y = minVector.Y;
            corners[6].Z = maxVector.Z;
            corners[7].X = maxVector.X;
            corners[7].Y = minVector.Y;
            corners[7].Z = maxVector.Z;
        }

        /// <summary>
        ///		Sets both Minimum and Maximum at once, so that UpdateCorners only
        ///		needs to be called once as well.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public void SetExtents(Vector3 min, Vector3 max)
        {
            isNull = false;
            isInfinite = false;

            minVector = min;
            maxVector = max;

            UpdateCorners();
        }

        /// <summary>
        ///    Scales the size of the box by the supplied factor.
        /// </summary>
        /// <param name="factor">Factor of scaling to apply to the box.</param>
        public void Scale(Vector3 factor)
        {
            SetExtents(minVector * factor, maxVector * factor);
        }

        /// <summary>
        ///     Return new bounding box from the supplied dimensions.
        /// </summary>
        /// <param name="center">Center of the new box</param>
        /// <param name="size">Entire size of the new box</param>
        /// <returns>New bounding box</returns>
        public static AxisAlignedBox3 FromDimensions(Vector3 center, Vector3 size)
        {
            Vector3 halfSize = .5f * size;

            return new AxisAlignedBox3(center - halfSize, center + halfSize);
        }


        /// <summary>
        ///		Allows for merging two boxes together (combining).
        /// </summary>
        /// <param name="box">Source box.</param>
        public void Merge(AxisAlignedBox3 box)
        {
            if (box.IsNull)
            {
                // nothing to merge with in this case, just return
                return;
            }
            else if (box.IsInfinite)
            {
                this.IsInfinite = true;
            }
            else if (this.IsNull)
            {
                SetExtents(box.Minimum, box.Maximum);
            }
            else if (!this.IsInfinite)
            {
                minVector = Vector3.Min(minVector, box.minVector);
                maxVector = Vector3.Max(maxVector, box.maxVector);

                UpdateCorners();
            }
        }

        /// <summary>
        ///		Extends the box to encompass the specified point (if needed).
        /// </summary>
        /// <param name="point"></param>
        public void Merge(Vector3 point)
        {
            if (isNull || isInfinite)
            {
                // if null, use this point
                SetExtents(point, point);
            }
            else
            {
                if (point.X > maxVector.X)
                    maxVector.X = point.X;
                else if (point.X < minVector.X)
                    minVector.X = point.X;

                if (point.Y > maxVector.Y)
                    maxVector.Y = point.Y;
                else if (point.Y < minVector.Y)
                    minVector.Y = point.Y;

                if (point.Z > maxVector.Z)
                    maxVector.Z = point.Z;
                else if (point.Z < minVector.Z)
                    minVector.Z = point.Z;

                UpdateCorners();
            }
        }

        #endregion

        #region Contain methods

        /// <summary>
        /// Tests whether the given point contained by this box.
        /// </summary>
        /// <param name="v"></param>
        /// <returns>True if the vector is contained inside the box.</returns>
        public bool Contains(Vector3 v)
        {
            if (IsNull)
                return false;
            if (IsInfinite)
                return true;

            return Minimum.X <= v.X && v.X <= Maximum.X &&
                   Minimum.Y <= v.Y && v.Y <= Maximum.Y &&
                   Minimum.Z <= v.Z && v.Z <= Maximum.Z;
        }


        #endregion Contain methods

        #region Intersection Methods

        /// <summary>
        ///		Returns whether or not this box intersects another.
        /// </summary>
        /// <param name="box2"></param>
        /// <returns>True if the 2 boxes intersect, false otherwise.</returns>
        public bool Intersects(AxisAlignedBox3 box2)
        {
            // Early-fail for nulls
            if (this.IsNull || box2.IsNull)
                return false;

            if (this.IsInfinite || box2.IsInfinite)
                return true;

            // Use up to 6 separating planes
            if (this.maxVector.X <= box2.minVector.X)
                return false;
            if (this.maxVector.Y <= box2.minVector.Y)
                return false;
            if (this.maxVector.Z <= box2.minVector.Z)
                return false;

            if (this.minVector.X >= box2.maxVector.X)
                return false;
            if (this.minVector.Y >= box2.maxVector.Y)
                return false;
            if (this.minVector.Z >= box2.maxVector.Z)
                return false;

            // otherwise, must be intersecting
            return true;
        }

        /// <summary>
        ///		Tests whether this box intersects a sphere.
        /// </summary>
        /// <param name="sphere"></param>
        /// <returns>True if the sphere intersects, false otherwise.</returns>
        public bool Intersects(Sphere sphere)
        {
            return Utility.Intersects(sphere, this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plane"></param>
        /// <returns>True if the plane intersects, false otherwise.</returns>
        public bool Intersects(Plane plane)
        {
            return Utility.Intersects(plane, this);
        }

        /// <summary>
        ///		Tests whether the vector point is within this box.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns>True if the vector is within this box, false otherwise.</returns>
        public bool Intersects(Vector3 vector)
        {
            return (vector.X >= minVector.X && vector.X <= maxVector.X &&
                vector.Y >= minVector.Y && vector.Y <= maxVector.Y &&
                vector.Z >= minVector.Z && vector.Z <= maxVector.Z);
        }

        /// <summary>
        ///		Calculate the area of intersection of this box and another
        /// </summary>
        public AxisAlignedBox3 Intersection(AxisAlignedBox3 b2)
        {
            if (!Intersects(b2))
                return new AxisAlignedBox3();

            Vector3 intMin = Vector3.Zero;
            Vector3 intMax = Vector3.Zero;

            Vector3 b2max = b2.maxVector;
            Vector3 b2min = b2.minVector;

            if (b2max.X > maxVector.X && maxVector.X > b2min.X)
                intMax.X = maxVector.X;
            else
                intMax.X = b2max.X;
            if (b2max.Y > maxVector.Y && maxVector.Y > b2min.Y)
                intMax.Y = maxVector.Y;
            else
                intMax.Y = b2max.Y;
            if (b2max.Z > maxVector.Z && maxVector.Z > b2min.Z)
                intMax.Z = maxVector.Z;
            else
                intMax.Z = b2max.Z;

            if (b2min.X < minVector.X && minVector.X < b2max.X)
                intMin.X = minVector.X;
            else
                intMin.X = b2min.X;
            if (b2min.Y < minVector.Y && minVector.Y < b2max.Y)
                intMin.Y = minVector.Y;
            else
                intMin.Y = b2min.Y;
            if (b2min.Z < minVector.Z && minVector.Z < b2max.Z)
                intMin.Z = minVector.Z;
            else
                intMin.Z = b2min.Z;

            return new AxisAlignedBox3(intMin, intMax);
        }

        #endregion Intersection Methods

        #region Properties

        public Vector3 HalfSize
        {
            get
            {
                if (isNull)
                    return Vector3.Zero;

                if (isInfinite)
                    return new Vector3(float.PositiveInfinity);

                return (Maximum - Minimum) * 0.5f;
            }
        }

        /// <summary>
        ///     Get/set the size of this bounding box.
        /// </summary>
        public Vector3 Size
        {
            get
            {
                return maxVector - minVector;
            }
            set
            {
                Vector3 center = Center;
                Vector3 halfSize = .5f * value;
                minVector = center - halfSize;
                maxVector = center + halfSize;
                UpdateCorners();
            }
        }

        /// <summary>
        ///    Get/set the center point of this bounding box.
        /// </summary>
        public Vector3 Center
        {
            get
            {
                return (minVector + maxVector) * 0.5f;
            }
            set
            {
                Vector3 halfSize = .5f * Size;
                minVector = value - halfSize;
                maxVector = value + halfSize;
                UpdateCorners();
            }

        }

        /// <summary>
        ///		Get/set the maximum corner of the box.
        /// </summary>
        public Vector3 Maximum
        {
            get
            {
                return maxVector;
            }
            set
            {
                isNull = false;
                maxVector = value;
                UpdateCorners();
            }
        }

        /// <summary>
        ///		Get/set the minimum corner of the box.
        /// </summary>
        public Vector3 Minimum
        {
            get
            {
                return minVector;
            }
            set
            {
                isNull = false;
                minVector = value;
                UpdateCorners();
            }
        }

        /// <summary>
        ///		Returns an array of 8 corner points, useful for
        ///		collision vs. non-aligned objects.
        /// </summary>
        /// <remarks>
        ///		If the order of these corners is important, they are as
        ///		follows: The 4 points of the minimum Z face (note that
        ///		because we use right-handed coordinates, the minimum Z is
        ///		at the 'back' of the box) starting with the minimum point of
        ///		all, then anticlockwise around this face (if you are looking
        ///		onto the face from outside the box). Then the 4 points of the
        ///		maximum Z face, starting with maximum point of all, then
        ///		anticlockwise around this face (looking onto the face from
        ///		outside the box). Like this:
        ///		<pre>
        ///			 1-----2
        ///		    /|     /|
        ///		  /  |   /  |
        ///		5-----4   |
        ///		|   0-|--3
        ///		|  /   |  /
        ///		|/     |/
        ///		6-----7
        ///		</pre>
        /// </remarks>
        public Vector3[] Corners
        {
            get
            {
                Debug.Assert(!isNull && !isInfinite, "Cannot get the corners of a null or infinite box.");

                return corners;
            }
        }

        /// <summary>
        ///		Get/set the value of whether this box is null (i.e. not dimensions, etc).
        /// </summary>
        public bool IsNull
        {
            get
            {
                return isNull;
            }
            set
            {
                isNull = value;
                if (value)
                    isInfinite = false;
            }
        }

        /// <summary>
        /// Returns true if the box is infinite.
        /// </summary>
        public bool IsInfinite
        {
            get
            {
                return isInfinite;
            }
            set
            {
                isInfinite = value;
                if (value)
                    isNull = false;
            }
        }


        /// <summary>
        ///		Returns a null box
        /// </summary>
        public static AxisAlignedBox3 Null
        {
            get
            {
                AxisAlignedBox3 nullBox = new AxisAlignedBox3();
                // make sure it is set to null
                nullBox.IsNull = true;
                nullBox.isInfinite = false;
                return nullBox;
            }
        }

        /// <summary>
        ///     Calculate the volume of this box
        /// </summary>
        public Real Volume
        {
            get
            {
                if (isNull)
                    return 0.0f;

                if (isInfinite)
                    return Real.PositiveInfinity;

                Vector3 diff = Maximum - Minimum;
                return diff.X * diff.Y * diff.Z;
            }
        }


        #endregion

        #region Operator Overloads

        public static bool operator ==(AxisAlignedBox3 left, AxisAlignedBox3 right)
        {
            if ((object.ReferenceEquals(left, null) || left.isNull) &&
                (object.ReferenceEquals(right, null) || right.isNull))
                return true;

            else if ((object.ReferenceEquals(left, null) || left.isNull) ||
                     (object.ReferenceEquals(right, null) || right.isNull))
                return false;

            return
                (left.corners[0] == right.corners[0] && left.corners[1] == right.corners[1] && left.corners[2] == right.corners[2] &&
                left.corners[3] == right.corners[3] && left.corners[4] == right.corners[4] && left.corners[5] == right.corners[5] &&
                left.corners[6] == right.corners[6] && left.corners[7] == right.corners[7]);
        }

        public static bool operator !=(AxisAlignedBox3 left, AxisAlignedBox3 right)
        {
            if ((object.ReferenceEquals(left, null) || left.isNull) &&
                (object.ReferenceEquals(right, null) || right.isNull))
                return false;

            else if ((object.ReferenceEquals(left, null) || left.isNull) ||
                     (object.ReferenceEquals(right, null) || right.isNull))
                return true;

            return
                (left.corners[0] != right.corners[0] || left.corners[1] != right.corners[1] || left.corners[2] != right.corners[2] ||
                left.corners[3] != right.corners[3] || left.corners[4] != right.corners[4] || left.corners[5] != right.corners[5] ||
                left.corners[6] != right.corners[6] || left.corners[7] != right.corners[7]);
        }

        public override bool Equals(object obj)
        {
            return obj is AxisAlignedBox3 && this == (AxisAlignedBox3)obj;
        }

        public override int GetHashCode()
        {
            if (isNull)
                return 0;

            return corners[0].GetHashCode() ^ corners[1].GetHashCode() ^ corners[2].GetHashCode() ^ corners[3].GetHashCode() ^ corners[4].GetHashCode() ^
                corners[5].GetHashCode() ^ corners[6].GetHashCode() ^ corners[7].GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("{0}:{1}", this.minVector, this.maxVector);
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            return new AxisAlignedBox3(this);
        }

        #endregion
    }
}
