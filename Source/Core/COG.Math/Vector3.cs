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
//     <id value="$Id: Vector3.cs 2940 2012-01-05 12:25:58Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

using T = COG.Math.Real;

#endregion Namespace Declarations

namespace COG.Math
{
    /// <summary>
    ///    Standard 3-dimensional vector.
    /// </summary>
    /// <remarks>
    ///	    A direction in 3D space represented as distances along the 3
    ///	    orthoganal axes (x, y, z). Note that positions, directions and
    ///	    scaling factors can be represented by a vector, depending on how
    ///	    you interpret the values.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3
    {
        #region Fields

        /// <summary>X component.</summary>
        public T X;

        /// <summary>Y component.</summary>
        public T Y;

        /// <summary>Z component.</summary>
        public T Z;

        private static readonly Vector3 positiveInfinityVector = new Vector3(T.PositiveInfinity, T.PositiveInfinity, T.PositiveInfinity);
        public static Vector3 PositiveInfinity { get { return positiveInfinityVector; } }

        private static readonly Vector3 negativeInfinityVector = new Vector3(T.NegativeInfinity, T.NegativeInfinity, T.NegativeInfinity);
        public static Vector3 NegativeInfinity { get { return negativeInfinityVector; } }

        private static readonly Vector3 invalidVector = new Vector3(T.NaN, T.NaN, T.NaN);
        public static Vector3 Invalid { get { return invalidVector; } }

        private static readonly Vector3 zeroVector = new Vector3(0.0f, 0.0f, 0.0f);
        private static readonly Vector3 unitX = new Vector3(1.0f, 0.0f, 0.0f);
        private static readonly Vector3 unitY = new Vector3(0.0f, 1.0f, 0.0f);
        private static readonly Vector3 unitZ = new Vector3(0.0f, 0.0f, 1.0f);
        private static readonly Vector3 negativeUnitX = new Vector3(-1.0f, 0.0f, 0.0f);
        private static readonly Vector3 negativeUnitY = new Vector3(0.0f, -1.0f, 0.0f);
        private static readonly Vector3 negativeUnitZ = new Vector3(0.0f, 0.0f, -1.0f);
        private static readonly Vector3 unitVector = new Vector3(1.0f, 1.0f, 1.0f);

        #endregion

        #region Constructors

        /// <summary>
        ///		Creates a new 3 dimensional Vector.
        /// </summary>
        public Vector3(T x, T y, T z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Vector3(Vector2 v, T z)
        {
            this.X = v.X;
            this.Y = v.Y;
            this.Z = z;
        }

        /// <summary>
        ///		Creates a new 3 dimensional Vector.
        /// </summary>
        public Vector3(T unitDimension)
            : this(unitDimension, unitDimension, unitDimension) { }

        /// <summary>
        ///		Creates a new 3 dimensional Vector.
        /// </summary>
        public Vector3(T[] coordinates)
        {
            if (coordinates.Length != 3)
            {
                throw new ArgumentException("The coordinates array must be of length 3 to specify the x, y, and z coordinates.");
            }
            this.X = coordinates[0];
            this.Y = coordinates[1];
            this.Z = coordinates[2];
        }

        #endregion

        #region Overloaded operators + CLS compliant method equivalents

        /// <summary>
        ///		User to compare two Vector3 instances for equality.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>true or false</returns>
        public static bool operator ==(Vector3 left, Vector3 right)
        {
            if (left.IsInvalid && right.IsInvalid)
            {
                return true;
            }
            if (left.IsInvalid || right.IsInvalid)
            {
                return false;
            }
            return (left.X == right.X && left.Y == right.Y && left.Z == right.Z);
        }

        /// <summary>
        ///		User to compare two Vector3 instances for inequality.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>true or false</returns>
        public static bool operator !=(Vector3 left, Vector3 right)
        {
            if (left.IsInvalid && right.IsInvalid)
            {
                return false;
            }
            if (left.IsInvalid || right.IsInvalid)
            {
                return true;
            }
            return (left.X != right.X || left.Y != right.Y || left.Z != right.Z);
        }

        /// <summary>
        ///		Used when a Vector3 is multiplied by another vector.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector3 Multiply(Vector3 left, Vector3 right)
        {
            return left * right;
        }

        /// <summary>
        ///		Used when a Vector3 is multiplied by another vector.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector3 operator *(Vector3 left, Vector3 right)
        {
            //return new Vector3( left.x * right.x, left.y * right.y, left.z * right.z );
            Vector3 retVal;
            retVal.X = left.X * right.X;
            retVal.Y = left.Y * right.Y;
            retVal.Z = left.Z * right.Z;
            return retVal;
        }

        /// <summary>
        /// Used to divide a vector by a scalar value.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Vector3 Divide(Vector3 left, T scalar)
        {
            return left / scalar;
        }

        /// <summary>
        ///		Used when a Vector3 is divided by another vector.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector3 operator /(Vector3 left, Vector3 right)
        {
            Vector3 vector;

            vector.X = left.X / right.X;
            vector.Y = left.Y / right.Y;
            vector.Z = left.Z / right.Z;

            return vector;
        }

        /// <summary>
        ///		Used when a Vector3 is divided by another vector.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector3 Divide(Vector3 left, Vector3 right)
        {
            return left / right;
        }

        /// <summary>
        /// Used to divide a vector by a scalar value.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Vector3 operator /(Vector3 left, T scalar)
        {
            Debug.Assert(scalar != 0.0f, "Cannot divide a Vector3 by zero.");

            Vector3 vector;

            // get the inverse of the scalar up front to avoid doing multiple divides later
            T inverse = 1.0f / scalar;

            vector.X = left.X * inverse;
            vector.Y = left.Y * inverse;
            vector.Z = left.Z * inverse;

            return vector;
        }

        /// <summary>
        ///		Used when a Vector3 is added to another Vector3.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector3 Add(Vector3 left, Vector3 right)
        {
            return left + right;
        }

        /// <summary>
        ///		Used when a Vector3 is added to another Vector3.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector3 operator +(Vector3 left, Vector3 right)
        {
            return new Vector3(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        /// <summary>
        ///		Used when a Vector3 is multiplied by a scalar value.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Vector3 Multiply(Vector3 left, T scalar)
        {
            return left * scalar;
        }

        /// <summary>
        ///		Used when a Vector3 is multiplied by a scalar value.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Vector3 operator *(Vector3 left, T scalar)
        {
            //return new Vector3( left.x * scalar, left.y * scalar, left.z * scalar );
            Vector3 retVal;
            retVal.X = left.X * scalar;
            retVal.Y = left.Y * scalar;
            retVal.Z = left.Z * scalar;
            return retVal;
        }

        /// <summary>
        ///		Used when a scalar value is multiplied by a Vector3.
        /// </summary>
        /// <param name="scalar"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector3 Multiply(T scalar, Vector3 right)
        {
            return scalar * right;
        }

        /// <summary>
        ///		Used when a scalar value is multiplied by a Vector3.
        /// </summary>
        /// <param name="scalar"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector3 operator *(T scalar, Vector3 right)
        {
            //return new Vector3( right.x * scalar, right.y * scalar, right.z * scalar );
            Vector3 retVal;
            retVal.X = right.X * scalar;
            retVal.Y = right.Y * scalar;
            retVal.Z = right.Z * scalar;
            return retVal;
        }

        /// <summary>
        ///		Used to subtract a Vector3 from another Vector3.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector3 Subtract(Vector3 left, Vector3 right)
        {
            return left - right;
        }

        /// <summary>
        ///		Used to subtract a Vector3 from another Vector3.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector3 operator -(Vector3 left, Vector3 right)
        {
            return new Vector3(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }

        /// <summary>
        ///		Used to negate the elements of a vector.
        /// </summary>
        /// <param name="left"></param>
        /// <returns></returns>
        public static Vector3 Negate(Vector3 left)
        {
            return -left;
        }

        /// <summary>
        ///		Used to negate the elements of a vector.
        /// </summary>
        /// <param name="left"></param>
        /// <returns></returns>
        public static Vector3 operator -(Vector3 left)
        {
            return new Vector3(-left.X, -left.Y, -left.Z);
        }

        /// <summary>
        ///    Returns true if the vector's scalar components are all smaller
        ///    that the ones of the vector it is compared against.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator >(Vector3 left, Vector3 right)
        {
            if (left.X > right.X && left.Y > right.Y && left.Z > right.Z)
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
        public static bool operator <(Vector3 left, Vector3 right)
        {
            if (left.X < right.X && left.Y < right.Y && left.Z < right.Z)
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
        public static Vector3 operator %(Vector3 left, T right)
        {
            return new Vector3(left.X % right, left.Y % right, left.Z % right);
        }

        //public static Vector3 operator %(Vector3 left, int right)
        //{
        //    return new Vector3((int)left.x % right, (int)left.y % right, (int)left.z % right);
        //}

        /// <summary>
        ///    
        /// </summary>
        /// <param name="vec3"></param>
        /// <returns></returns>
        public static explicit operator Vector4(Vector3 vec3)
        {
            return new Vector4(vec3.X, vec3.Y, vec3.Z, 1.0f);
        }

        public static implicit operator Vector3(Color c)
        {
            return new Vector3(c.R, c.G, c.B);
        }

        public static implicit operator Color(Vector3 v)
        {
            return new Color(v.X, v.Y, v.Z, 1f);
        }

        /// <summary>
        ///		Used to access a Vector by index 0 = x, 1 = y, 2 = z.  
        /// </summary>
        /// <remarks>
        ///		Uses unsafe pointer arithmetic to reduce the code required.
        ///	</remarks>
        public T this[int index]
        {
            get
            {
                Debug.Assert(index >= 0 && index < 3, "Indexer boundaries overrun in Vector3.");

                // using pointer arithmetic here for less code.  Otherwise, we'd have a big switch statement.
                unsafe
                {
                    fixed (T* pX = &X)
                    {
                        return *(pX + index);
                    }
                }
            }
            set
            {
                Debug.Assert(index >= 0 && index < 3, "Indexer boundaries overrun in Vector3.");

                // using pointer arithmetic here for less code.  Otherwise, we'd have a big switch statement.
                unsafe
                {
                    fixed (T* pX = &X)
                    {
                        *(pX + index) = value;
                    }
                }
            }
        }

        #endregion

        #region Public methods

        #region Swizzle

        #region 2-component

        /// <summary>
        /// Gets or sets an OpenTK.Vector2 with the X and Y components of this instance.
        /// </summary>
        public Vector2 Xy { get { return new Vector2(X, Y); } set { X = value.X; Y = value.Y; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector2 with the X and Z components of this instance.
        /// </summary>
        public Vector2 Xz { get { return new Vector2(X, Z); } set { X = value.X; Z = value.Y; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector2 with the Y and X components of this instance.
        /// </summary>
        public Vector2 Yx { get { return new Vector2(Y, X); } set { Y = value.X; X = value.Y; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector2 with the Y and Z components of this instance.
        /// </summary>
        public Vector2 Yz { get { return new Vector2(Y, Z); } set { Y = value.X; Z = value.Y; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector2 with the Z and X components of this instance.
        /// </summary>
        public Vector2 Zx { get { return new Vector2(Z, X); } set { Z = value.X; X = value.Y; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector2 with the Z and Y components of this instance.
        /// </summary>
        public Vector2 Zy { get { return new Vector2(Z, Y); } set { Z = value.X; Y = value.Y; } }

        #endregion

        #region 3-component

        /// <summary>
        /// Gets or sets an OpenTK.Vector3 with the X, Z, and Y components of this instance.
        /// </summary>
        public Vector3 Xzy { get { return new Vector3(X, Z, Y); } set { X = value.X; Z = value.Y; Y = value.Z; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector3 with the Y, X, and Z components of this instance.
        /// </summary>
        public Vector3 Yxz { get { return new Vector3(Y, X, Z); } set { Y = value.X; X = value.Y; Z = value.Z; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector3 with the Y, Z, and X components of this instance.
        /// </summary>
        public Vector3 Yzx { get { return new Vector3(Y, Z, X); } set { Y = value.X; Z = value.Y; X = value.Z; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector3 with the Z, X, and Y components of this instance.
        /// </summary>
        public Vector3 Zxy { get { return new Vector3(Z, X, Y); } set { Z = value.X; X = value.Y; Y = value.Z; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector3 with the Z, Y, and X components of this instance.
        /// </summary>
        public Vector3 Zyx { get { return new Vector3(Z, Y, X); } set { Z = value.X; Y = value.Y; X = value.Z; } }

        #endregion

        #endregion

        /// <summary>
        /// Returns the distance to another vector.
        /// </summary>
        /// <param name="second"></param>
        /// <returns></returns>
        public T Distance(Vector3 second)
        {
            return (this - second).Length;
        }

        /// <summary>
        ///     Returns the square of the distance to another vector.
        /// <remarks>
        ///     This method is for efficiency - calculating the actual
        ///     distance to another vector requires a square root, which is
        ///     expensive in terms of the operations required. This method
        ///     returns the square of the distance to another vector, i.e.
        ///     the same as the distance but before the square root is taken.
        ///     Use this if you want to find the longest / shortest distance
        ///     without incurring the square root.
        /// </remarks>
        /// </summary>
        /// <param name="second"></param>
        /// <returns></returns>
        public T DistanceSquared(Vector3 second)
        {
            return (this - second).LengthSquared;
        }

        public object[] ToObjectArray()
        {
            return new object[] {
			                    	X, Y, Z
			                    };
        }

        public T[] ToArray()
        {
            return new T[] {
			                  	X, Y, Z
			                  };
        }

        public bool IsAnyComponentGreaterThan(Vector3 vector)
        {
            return (this.X > vector.X || this.Y > vector.Y || this.Z > vector.Z);
        }

        public bool IsAnyComponentGreaterThanOrEqualTo(Vector3 vector)
        {
            return (this.X >= vector.X || this.Y >= vector.Y || this.Z >= vector.Z);
        }

        public bool IsAnyComponentLessThan(Vector3 vector)
        {
            return (this.X < vector.X || this.Y < vector.Y || this.Z < vector.Z);
        }

        public bool IsAnyComponentLessThanOrEqualTo(Vector3 vector)
        {
            return (this.X <= vector.X || this.Y <= vector.Y || this.Z <= vector.Z);
        }

        public Vector3 Offset(T x, T y, T z)
        {
            return new Vector3(this.X + x, this.Y + y, this.Z + z);
        }

        /// <summary>
        /// Performs a Dot Product operation on 2 vectors.
        /// </summary>
        /// <remarks>
        /// A dot product of two vectors v1 and v2 equals to |v1|*|v2|*cos(fi)
        /// where fi is the angle between the vectors and |v1| and |v2| are the vector lengths.
        /// For unit vectors (whose length is one) the dot product will obviously be just cos(fi).
        /// For example, if the unit vectors are parallel the result is cos(0) = 1.0f,
        /// if they are perpendicular the result is cos(PI/2) = 0.0f.
        /// The dot product may be calculated on vectors with any length however.
        /// A zero vector is treated as perpendicular to any vector (result is 0.0f).
        /// </remarks>
        /// <param name="vector">The vector to perform the Dot Product against.</param>
        /// <returns>Products of vector lengths and cosine of the angle between them. </returns>
        public T Dot(Vector3 vector)
        {
            return X * vector.X + Y * vector.Y + Z * vector.Z;
        }

        /// <summary>
        ///     Calculates the absolute dot (scalar) product of this vector with another.
        /// </summary>
        /// <remarks>
        ///     This function work similar dotProduct, except it use absolute value
        ///     of each component of the vector to computing.
        /// </remarks>
        /// <param name="vec">
        ///     vec Vector with which to calculate the absolute dot product (together
        ///     with this one).
        /// </param>
        /// <returns>A float representing the absolute dot product value.</returns>
        public T AbsDot(Vector3 vec)
        {
            return System.Math.Abs(X * vec.X) + System.Math.Abs(Y * vec.Y) + System.Math.Abs(Z * vec.Z);
        }

        /// <summary>
        ///		Performs a Cross Product operation on 2 vectors, which returns a vector that is perpendicular
        ///		to the intersection of the 2 vectors.  Useful for finding face normals.
        /// </summary>
        /// <param name="vector">A vector to perform the Cross Product against.</param>
        /// <returns>A new Vector3 perpedicular to the 2 original vectors.</returns>
        public Vector3 Cross(Vector3 vector)
        {
            return new Vector3(
                (this.Y * vector.Z) - (this.Z * vector.Y),
                (this.Z * vector.X) - (this.X * vector.Z),
                (this.X * vector.Y) - (this.Y * vector.X)
                );
        }

        /// <summary>
        ///		Finds a vector perpendicular to this one.
        /// </summary>
        /// <returns></returns>
        public Vector3 Perpendicular()
        {
            Vector3 result = this.Cross(Vector3.UnitX);

            // check length
            if (result.LengthSquared < T.Epsilon)
            {
                // This vector is the Y axis multiplied by a scalar, so we have to use another axis
                result = this.Cross(Vector3.UnitY);
            }

            return result;
        }

        public static Vector3 SymmetricRandom()
        {
            return SymmetricRandom(1f, 1f, 1f);
        }

        public static Vector3 SymmetricRandom(Vector3 maxComponentMagnitude)
        {
            return SymmetricRandom(maxComponentMagnitude.X, maxComponentMagnitude.Y, maxComponentMagnitude.Z);
        }

        public static Vector3 SymmetricRandom(T maxComponentMagnitude)
        {
            return SymmetricRandom(maxComponentMagnitude, maxComponentMagnitude, maxComponentMagnitude);
        }

        public static Vector3 SymmetricRandom(T xMult, T yMult, T zMult)
        {
            return new Vector3(
                (xMult == 0) ? 0 : xMult * Utility.SymmetricRandom(),
                (yMult == 0) ? 0 : yMult * Utility.SymmetricRandom(),
                (zMult == 0) ? 0 : zMult * Utility.SymmetricRandom()
                );
        }

        public static Vector3 Transform(Vector3 position, Matrix4 matrix)
        {
            Transform(ref position, ref matrix, out position);
            return position;
        }

        public static void Transform(ref Vector3 position, ref Matrix4 matrix, out Vector3 result)
        {
            result = new Vector3((position.X * matrix.M00) + (position.Y * matrix.M10) + (position.Z * matrix.M20) + matrix.M30,
                                 (position.X * matrix.M01) + (position.Y * matrix.M11) + (position.Z * matrix.M21) + matrix.M31,
                                 (position.X * matrix.M02) + (position.Y * matrix.M12) + (position.Z * matrix.M22) + matrix.M32);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="up"></param>
        /// <returns></returns>
        public Vector3 RandomDeviant(T angle, Vector3 up)
        {
            Vector3 newUp = (up == Vector3.Zero) ? this.Perpendicular() : up;

            // rotate up vector by random amount around this
            Quaternion q = Quaternion.FromAngleAxis(Utility.UnitRandom() * Utility.TWO_PI, this);
            newUp = q * newUp;

            // finally, rotate this by given angle around randomized up vector
            q = Quaternion.FromAngleAxis(angle, newUp);

            return q * this;
        }

        ///<overloads>
        ///<summary>Returns wether this vector is within a positional tolerance of another vector</summary>
        ///<param name="right">The vector to compare with</param>
        ///</overloads>
        ///<remarks>Uses a defalut tolerance of 1E-03</remarks>
        public bool PositionEquals(Vector3 right)
        {
            return PositionEquals(right, 1e-03f);
        }

        /// <param name="tolerance">The amount that each element of the vector may vary by and still be considered equal.</param>
        public bool PositionEquals(Vector3 right, T tolerance)
        {
            return Utility.RealEqual(X, right.X, tolerance) &&
                   Utility.RealEqual(Y, right.Y, tolerance) &&
                   Utility.RealEqual(Z, right.Z, tolerance);
        }

        /// <summary>
        ///		Finds the midpoint between the supplied Vector and this vector.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vector3 MidPoint(Vector3 vector)
        {
            return new Vector3((this.X + vector.X) * 0.5f,
                                (this.Y + vector.Y) * 0.5f,
                                (this.Z + vector.Z) * 0.5f);
        }

        /// <summary>
        ///		Compares the supplied vector and updates it's x/y/z components of they are higher in value.
        /// </summary>
        /// <param name="compare"></param>
        public void Ceil(Vector3 compare)
        {
            if (compare.X > X)
            {
                X = compare.X;
            }
            if (compare.Y > Y)
            {
                Y = compare.Y;
            }
            if (compare.Z > Z)
            {
                Z = compare.Z;
            }
        }

        /// <summary>
        ///		Compares the supplied vector and updates it's x/y/z components of they are lower in value.
        /// </summary>
        /// <param name="compare"></param>
        /// <returns></returns>
        public void Floor(Vector3 compare)
        {
            if (compare.X < X)
            {
                X = compare.X;
            }
            if (compare.Y < Y)
            {
                Y = compare.Y;
            }
            if (compare.Z < Z)
            {
                Z = compare.Z;
            }
        }

        public bool AllComponentsLessThan(T limit)
        {
            return Utility.Abs(X) < limit && Utility.Abs(Y) < limit && Utility.Abs(Z) < limit;
        }

        public bool DifferenceLessThan(Vector3 other, T limit)
        {
            return Utility.Abs(X - other.X) < limit && Utility.Abs(Y - other.Y) < limit && Utility.Abs(Z - other.Z) < limit;
        }

        /// <summary>
        ///		Gets the shortest arc quaternion to rotate this vector to the destination vector. 
        /// </summary>
        /// <remarks>
        ///		Don't call this if you think the dest vector can be close to the inverse
        ///		of this vector, since then ANY axis of rotation is ok.
        ///	</remarks>
        public Quaternion GetRotationTo(Vector3 destination)
        {
            return GetRotationTo(destination, Vector3.Zero);
        }

        /// <summary>
        /// Gets the shortest arc quaternion to rotate this vector to the destination vector. 
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="fallbackAxis"></param>
        /// <returns></returns>
        /// <remarks>
        ///		Don't call this if you think the dest vector can be close to the inverse
        ///		of this vector, since then ANY axis of rotation is ok.
        ///	</remarks>
        public Quaternion GetRotationTo(Vector3 destination, Vector3 fallbackAxis)
        {
            // Based on Stan Melax's article in Game Programming Gems
            Quaternion q;

            // Copy, since cannot modify local
            Vector3 v0 = new Vector3(this.X, this.Y, this.Z);
            Vector3 v1 = destination;

            // normalize both vectors 
            v0.Normalize();
            v1.Normalize();

            // get the cross product of the vectors
            Vector3 c = v0.Cross(v1);

            T d = v0.Dot(v1);

            // If dot == 1, vectors are the same
            if (d >= 1.0f)
            {
                return Quaternion.Identity;
            }

            if (d < (1e-6f - 1.0f))
            {
                if (fallbackAxis != Vector3.Zero)
                {
                    // rotate 180 degrees about the fallback axis
                    q = Quaternion.FromAngleAxis(Utility.PI, fallbackAxis);
                }
                else
                {
                    // Generate an axis
                    Vector3 axis = Vector3.UnitX.Cross(this);
                    if (axis.IsZeroLength) // pick another if colinear
                    {
                        axis = Vector3.UnitY.Cross(this);
                    }
                    axis.Normalize();
                    q = Quaternion.FromAngleAxis(Utility.PI, axis);
                }
            }
            else
            {
                T s = Utility.Sqrt((1 + d) * 2);
                T inverse = 1 / s;

                q.x = c.X * inverse;
                q.y = c.Y * inverse;
                q.z = c.Z * inverse;
                q.w = s * 0.5f;
            }
            return q;
        }

        public Vector3 ToNormalized()
        {
            Vector3 vec = this;
            vec.Normalize();
            return vec;
        }

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
            T length = Utility.Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z);

            // Will also work for zero-sized vectors, but will change nothing
            if (length > T.Epsilon)
            {
                T inverseLength = 1.0f / length;

                this.X *= inverseLength;
                this.Y *= inverseLength;
                this.Z *= inverseLength;
            }

            return length;
        }

        /// <summary>
        ///    Calculates a reflection vector to the plane with the given normal.
        /// </summary>
        /// <remarks>
        ///    Assumes this vector is pointing AWAY from the plane, invert if not.
        /// </remarks>
        /// <param name="normal">Normal vector on which this vector will be reflected.</param>
        /// <returns></returns>
        public Vector3 Reflect(Vector3 normal)
        {
            return this - (2 * this.Dot(normal) * normal);
        }

        #endregion

        #region Public properties

        public bool IsZero { get { return this.X == 0f && this.Y == 0f && this.Z == 0f; } }

        /// <summary>
        /// Returns true if this vector is zero length.
        /// </summary>
        public bool IsZeroLength
        {
            get
            {
                T sqlen = (X * X) + (Y * Y) + (Z * Z);
                return (sqlen < (1e-06f * 1e-06f));
            }
        }

        public bool IsInvalid { get { return T.IsNaN(this.X) || T.IsNaN(this.Y) || T.IsNaN(this.Z); } }

        /// <summary>
        ///    Gets the length (magnitude) of this Vector3.  The Sqrt operation is expensive, so 
        ///    only use this if you need the exact length of the Vector.  If vector lengths are only going
        ///    to be compared, use LengthSquared instead.
        /// </summary>
        public T Length { get { return Utility.Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z); } }

        /// <summary>
        ///    Returns the length (magnitude) of the vector squared.
        /// </summary>
        /// <remarks>
        ///     This  property is for efficiency - calculating the actual
        ///     length of a vector requires a square root, which is expensive
        ///     in terms of the operations required. This method returns the
        ///     square of the length of the vector, i.e. the same as the
        ///     length but before the square root is taken. Use this if you
        ///     want to find the longest / shortest vector without incurring
        ///     the square root.
        /// </remarks>
        public T LengthSquared { get { return (this.X * this.X + this.Y * this.Y + this.Z * this.Z); } }

        #endregion

        #region Static Constant Properties

        /// <summary>
        ///		Gets a Vector3 with all components set to 0.
        /// </summary>
        public static Vector3 Zero { get { return zeroVector; } }

        /// <summary>
        ///		Gets a Vector3 with all components set to 1.
        /// </summary>
        public static Vector3 UnitScale { get { return unitVector; } }

        /// <summary>
        ///		Gets a Vector3 with the X set to 1, and the others set to 0.
        /// </summary>
        public static Vector3 UnitX { get { return unitX; } }

        /// <summary>
        ///		Gets a Vector3 with the Y set to 1, and the others set to 0.
        /// </summary>
        public static Vector3 UnitY { get { return unitY; } }

        /// <summary>
        ///		Gets a Vector3 with the Z set to 1, and the others set to 0.
        /// </summary>
        public static Vector3 UnitZ { get { return unitZ; } }

        /// <summary>
        ///		Gets a Vector3 with the X set to -1, and the others set to 0.
        /// </summary>
        public static Vector3 NegativeUnitX { get { return negativeUnitX; } }

        /// <summary>
        ///		Gets a Vector3 with the Y set to -1, and the others set to 0.
        /// </summary>
        public static Vector3 NegativeUnitY { get { return negativeUnitY; } }

        /// <summary>
        ///		Gets a Vector3 with the Z set to -1, and the others set to 0.
        /// </summary>
        public static Vector3 NegativeUnitZ { get { return negativeUnitZ; } }

        public static Vector3 Normalize(Vector3 v)
        {
            return v.ToNormalized();
        }

        public static Vector3 Cross(Vector3 a, Vector3 b)
        {
            return a.Cross(b);
        }

        public static T Dot(Vector3 a, Vector3 b)
        {
            return a.Dot(b);
        }
        #endregion

        #region Object overloads

        /// <summary>
        ///		Overrides the Object.ToString() method to provide a text representation of 
        ///		a Vector3.
        /// </summary>
        /// <returns>A string representation of a vector3.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Vector3({0}, {1}, {2})", this.X, this.Y, this.Z);
        }

        /// <summary>
        ///		Provides a unique hash code based on the member variables of this
        ///		class.  This should be done because the equality operators (==, !=)
        ///		have been overriden by this class.
        ///		<p/>
        ///		The standard implementation is a simple XOR operation between all local
        ///		member variables.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }

        /// <summary>
        ///		Compares this Vector to another object.  This should be done because the 
        ///		equality operators (==, !=) have been overriden by this class.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return (obj is Vector3) && (this == (Vector3)obj);
        }

        #endregion

        #region Parse method, implemented for factories

        /// <summary>
        ///		Parses a string and returns Vector3.
        /// </summary>
        /// <param name="vector">
        ///     A string representation of a Vector3 as it's returned from Vector3.ToString()
        /// </param>
        /// <returns>
        ///     A new Vector3.
        /// </returns>
        public static Vector3 Parse(string vector)
        {
            // the format is "Vector3(x, y, z)"
            if (!vector.StartsWith("Vector3("))
            {
                throw new FormatException();
            }

            string[] vals = vector.Substring(8).TrimEnd(')').Split(',');

            return new Vector3(T.Parse(vals[0].Trim(), CultureInfo.InvariantCulture),
                                T.Parse(vals[1].Trim(), CultureInfo.InvariantCulture),
                                T.Parse(vals[2].Trim(), CultureInfo.InvariantCulture));
        }

        #endregion
    }
}
