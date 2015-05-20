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
//     <id value="$Id: Vector2.cs 2940 2012-01-05 12:25:58Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Globalization;
using System.Runtime.InteropServices;

using T = COG.Math.Real;

#endregion Namespace Declarations


namespace COG.Math
{
    /// <summary>
    ///     2 dimensional vector.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector2
    {
        #region Fields

        public T X, Y;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets length of this vector
        /// </summary>
        public T Length { get { return Utility.Sqrt(X * X + Y * Y); } }

        /// <summary>
        /// Gets the squared length of this vector
        /// </summary>
        public T LengthSquared { get { return X * X + Y * Y; } }

        /// <summary>
        /// Gets a vector perpendicular to this, which has the same magnitude.
        /// </summary>
        public Vector2 Perpendicular { get { return new Vector2(this.Y, -this.X); } }


        #endregion

        #region Swizzle

        /// <summary>
        /// Gets or sets an OpenTK.Vector2h with the Y and X components of this instance.
        /// </summary>
        public Vector2 Yx { get { return new Vector2(Y, X); } set { Y = value.X; X = value.Y; } }

        #endregion

        #region Static

        private static readonly Vector2 zeroVector = new Vector2(0.0f, 0.0f);

        /// <summary>
        ///		Gets a Vector2 with all components set to 0.
        /// </summary>
        public static Vector2 Zero { get { return zeroVector; } }

        public static Vector2 One { get { return new Vector2(1, 1); } }

        public static Vector2 forward { get { return new Vector2(1, 0); } }

        public static Vector2 UnitX { get { return new Vector2(1, 0); } }

        public static Vector2 UnitY { get { return new Vector2(0, 1); } }

        #endregion

        #region Constructors

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="x">X position.</param>
        /// <param name="y">Y position</param>
        public Vector2(T x, T y)
        {
            this.X = x;
            this.Y = y;
        }

        public Vector2(T v)
        {
            this.X = v;
            this.Y = v;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        ///		Normalizes the vector.
        /// </summary>
        /// <remarks>
        ///		This method normalises the vector such that it's
        ///		length / magnitude is 1. The result is called a unit vector.
        ///		<p/>
        ///		This function will not crash for zero-sized vectors, but there
        ///		will be no changes made to their components.
        ///	</remarks>
        ///	<returns>The previous length of the vector.</returns>
        public T Normalize()
        {
            T length = Utility.Sqrt(this.X * this.X + this.Y * this.Y);

            // Will also work for zero-sized vectors, but will change nothing
            if (length > T.Epsilon)
            {
                T inverseLength = 1.0f / length;

                this.X *= inverseLength;
                this.Y *= inverseLength;
            }

            return length;
        }

        /// <summary>
        /// Gets a normalized (unit length) vector of this vector
        /// </summary>
        /// <returns></returns>
        public Vector2 ToNormalized()
        {
            Vector2 vec = this;
            vec.Normalize();

            return vec;
        }

        /// <summary>
        /// Calculates the 2 dimensional cross-product of 2 vectors, which results
        /// in a T value which is 2 times the area of the triangle
        /// defined by the two vectors. It also is the magnitude of the 3D vector that is perpendicular
        /// to the 2D vectors if the 2D vectors are projected to 3D space.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public T Cross(Vector2 vector)
        {
            return this.X * vector.Y - this.Y * vector.X;
        }

        /// <summary>
        /// Calculates the 2 dimensional dot-product of 2 vectors, 
        /// which is equal to the cosine of the angle between the vectors, times the lengths of each of the vectors.
        /// A.Dot(B) == |A| * |B| * cos(fi)
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public T Dot(Vector2 vector)
        {
            return this.X * vector.X + this.Y * vector.Y;
        }

        public Vector2 Rotate(Vector2 pivot, T amount)
        {
            var rot = pivot.GetRotation(this);
            rot += amount;

            var dir = new Vector2(Utility.Cos(rot), Utility.Sin(rot));
            return dir * (this - pivot).Length + pivot;
        }

        public Real GetRotation(Vector2 dst)
        {
            if (this == dst)
                return Utility.ATan2(0, 0);

            var diff = (dst - this).ToNormalized();
            return Utility.ATan2(diff.Y, diff.X);
        }

        public Real GetRotation()
        {
            var diff = this.ToNormalized();
            return Utility.ATan2(diff.Y, diff.X);
        }

        public static T Dot(Vector2 p0, Vector2 p1)
        {
            return p0.Dot(p1);
        }

        public static Vector2 Transform(Vector2 position, Matrix4 matrix)
        {
            Transform(ref position, ref matrix, out position);
            return position;
        }

        public static void Transform(ref Vector2 position, ref Matrix4 matrix, out Vector2 result)
        {
            result = new Vector2((position.X * matrix.M00) + (position.Y * matrix.M10) + matrix.M30,
                                 (position.X * matrix.M01) + (position.Y * matrix.M11) + matrix.M31);
        }
        #endregion

        #region Overloaded operators + CLS compliant method equivalents

        /// <summary>
        ///		User to compare two Vector2i instances for equality.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>true or false</returns>
        public static bool operator ==(Vector2 left, Vector2 right)
        {
            return (left.X == right.X && left.Y == right.Y);
        }

        /// <summary>
        ///		User to compare two Vector2i instances for inequality.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>true or false</returns>
        public static bool operator !=(Vector2 left, Vector2 right)
        {
            return (left.X != right.X || left.Y != right.Y);
        }

        /// <summary>
        ///		Used when a Vector2i is multiplied by another vector.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector2 Multiply(Vector2 left, Vector2 right)
        {
            return left * right;
        }

        /// <summary>
        ///		Used when a Vector2i is multiplied by another vector.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector2 operator *(Vector2 left, Vector2 right)
        {
            //return new Vector2i( left.x * right.x, left.y * right.y, left.z * right.z );
            Vector2 retVal;
            retVal.X = left.X * right.X;
            retVal.Y = left.Y * right.Y;
            return retVal;
        }

        /// <summary>
        /// Used to divide a vector by a scalar value.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Vector2 Divide(Vector2 left, int scalar)
        {
            return left / scalar;
        }

        /// <summary>
        ///		Used when a Vector2i is divided by another vector.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector2 operator /(Vector2 left, Vector2 right)
        {
            Vector2 vector;

            vector.X = left.X / right.X;
            vector.Y = left.Y / right.Y;

            return vector;
        }

        /// <summary>
        ///		Used when a Vector2i is divided by another vector.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector2 Divide(Vector2 left, Vector2 right)
        {
            return left / right;
        }

        /// <summary>
        /// Used to divide a vector by a scalar value.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Vector2 operator /(Vector2 left, T scalar)
        {
            //Debug.Assert(scalar != 0, "Cannot divide a Vector2i by zero.");

            Vector2 vector;

            // get the inverse of the scalar up front to avoid doing multiple divides later
            var inverse = 1f / scalar;

            vector.X = (left.X * inverse);
            vector.Y = (left.Y * inverse);

            return vector;
        }

        /// <summary>
        ///		Used when a Vector2i is added to another Vector2i.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector2 Add(Vector2 left, Vector2 right)
        {
            return left + right;
        }

        /// <summary>
        ///		Used when a Vector2i is added to another Vector2i.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector2 operator +(Vector2 left, Vector2 right)
        {
            return new Vector2(left.X + right.X, left.Y + right.Y);
        }

        /// <summary>
        ///		Used when a Vector2i is multiplied by a scalar value.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Vector2 Multiply(Vector2 left, T scalar)
        {
            return left * scalar;
        }

        /// <summary>
        ///		Used when a Vector2i is multiplied by a scalar value.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Vector2 operator *(Vector2 left, T scalar)
        {
            //return new Vector2i( left.x * scalar, left.y * scalar, left.z * scalar );
            Vector2 retVal;
            retVal.X = left.X * scalar;
            retVal.Y = left.Y * scalar;
            return retVal;
        }

        /// <summary>
        ///		Used when a scalar value is multiplied by a Vector2i.
        /// </summary>
        /// <param name="scalar"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector2 Multiply(T scalar, Vector2 right)
        {
            return scalar * right;
        }

        /// <summary>
        ///		Used when a scalar value is multiplied by a Vector2i.
        /// </summary>
        /// <param name="scalar"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector2 operator *(T scalar, Vector2 right)
        {
            //return new Vector2i( right.x * scalar, right.y * scalar, right.z * scalar );
            Vector2 retVal;
            retVal.X = right.X * scalar;
            retVal.Y = right.Y * scalar;
            return retVal;
        }

        /// <summary>
        ///		Used to subtract a Vector2i from another Vector2i.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector2 Subtract(Vector2 left, Vector2 right)
        {
            return left - right;
        }

        /// <summary>
        ///		Used to subtract a Vector2i from another Vector2i.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector2 operator -(Vector2 left, Vector2 right)
        {
            return new Vector2(left.X - right.X, left.Y - right.Y);
        }

        /// <summary>
        ///		Used to negate the elements of a vector.
        /// </summary>
        /// <param name="left"></param>
        /// <returns></returns>
        public static Vector2 Negate(Vector2 left)
        {
            return -left;
        }

        /// <summary>
        ///		Used to negate the elements of a vector.
        /// </summary>
        /// <param name="left"></param>
        /// <returns></returns>
        public static Vector2 operator -(Vector2 left)
        {
            return new Vector2(-left.X, -left.Y);
        }

        /// <summary>
        ///    Returns true if the vector's scalar components are all smaller
        ///    that the ones of the vector it is compared against.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator >(Vector2 left, Vector2 right)
        {
            if (left.X > right.X && left.Y > right.Y)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///    Returns true if the vector's scalar components are all greater
        ///    that the ones of the vector it is compared against.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator <(Vector2 left, Vector2 right)
        {
            if (left.X < right.X && left.Y < right.Y)
            {
                return true;
            }

            return false;
        }

        public static Vector2 operator %(Vector2 left, T right)
        {
            return new Vector2(left.X % right, left.Y % right);
        }

        ///// <summary>
        /////		Used to access a Vector by index 0 = x, 1 = y, 2 = z.  
        ///// </summary>
        ///// <remarks>
        /////		Uses unsafe pointer arithmetic to reduce the code required.
        /////	</remarks>
        //public int this[int index]
        //{
        //    get
        //    {
        //        Debug.Assert(index >= 0 && index < 3, "Indexer boundaries overrun in Vector2i.");

        //        // using pointer arithmetic here for less code.  Otherwise, we'd have a big switch statement.
        //        unsafe
        //        {
        //            fixed (int* pX = &x)
        //                return *(pX + index);
        //        }
        //    }
        //    set
        //    {
        //        Debug.Assert(index >= 0 && index < 3, "Indexer boundaries overrun in Vector2i.");

        //        // using pointer arithmetic here for less code.  Otherwise, we'd have a big switch statement.
        //        unsafe
        //        {
        //            fixed (int* pX = &x)
        //                *(pX + index) = value;
        //        }
        //    }
        //}

        #endregion

        #region Object overrides

        public override bool Equals(object obj)
        {
            if (!(obj is Vector2))
                return false;

            var other = (Vector2)obj;
            return this == other;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "Vector2({0}, {1})", this.X, this.Y);
        }

        #endregion

        #region Parse from string

        public Vector2 Parse(string s)
        {
            // the format is "Vector2(x, y)"
            if (!s.StartsWith("Vector2("))
            {
                throw new FormatException();
            }

            string[] values = s.Substring(8).TrimEnd('}').Split(',');

            return new Vector2(T.Parse(values[0], CultureInfo.InvariantCulture),
                                T.Parse(values[1], CultureInfo.InvariantCulture));
        }

        #endregion
    }
}