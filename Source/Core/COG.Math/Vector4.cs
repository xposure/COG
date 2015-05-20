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
//     <id value="$Id: Vector4.cs 2940 2012-01-05 12:25:58Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

#endregion Namespace Declarations

namespace COG.Math
{
    /// <summary>
    /// 4D homogeneous vector.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector4
    {
        #region Member variables

        public Real X, Y, Z, W;

        private static readonly Vector4 zeroVector = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);

        #endregion

        #region Constructors

        /// <summary>
        ///		Creates a new 4 dimensional Vector.
        /// </summary>
        public Vector4(Real x, Real y, Real z, Real w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        #endregion

        #region Properties

        /// <summary>
        ///		Gets a Vector4 with all components set to 0.
        /// </summary>
        public static Vector4 Zero { get { return zeroVector; } }

        #endregion Properties

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
        /// Gets or sets an OpenTK.Vector2 with the X and W components of this instance.
        /// </summary>
        public Vector2 Xw { get { return new Vector2(X, W); } set { X = value.X; W = value.Y; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector2 with the Y and X components of this instance.
        /// </summary>
        public Vector2 Yx { get { return new Vector2(Y, X); } set { Y = value.X; X = value.Y; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector2 with the Y and Z components of this instance.
        /// </summary>
        public Vector2 Yz { get { return new Vector2(Y, Z); } set { Y = value.X; Z = value.Y; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector2 with the Y and W components of this instance.
        /// </summary>
        public Vector2 Yw { get { return new Vector2(Y, W); } set { Y = value.X; W = value.Y; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector2 with the Z and X components of this instance.
        /// </summary>
        public Vector2 Zx { get { return new Vector2(Z, X); } set { Z = value.X; X = value.Y; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector2 with the Z and Y components of this instance.
        /// </summary>
        public Vector2 Zy { get { return new Vector2(Z, Y); } set { Z = value.X; Y = value.Y; } }

        /// <summary>
        /// Gets an OpenTK.Vector2 with the Z and W components of this instance.
        /// </summary>
        public Vector2 Zw { get { return new Vector2(Z, W); } set { Z = value.X; W = value.Y; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector2 with the W and X components of this instance.
        /// </summary>
        public Vector2 Wx { get { return new Vector2(W, X); } set { W = value.X; X = value.Y; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector2 with the W and Y components of this instance.
        /// </summary>
        public Vector2 Wy { get { return new Vector2(W, Y); } set { W = value.X; Y = value.Y; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector2 with the W and Z components of this instance.
        /// </summary>
        public Vector2 Wz { get { return new Vector2(W, Z); } set { W = value.X; Z = value.Y; } }

        #endregion

        #region 3-component

        /// <summary>
        /// Gets or sets an OpenTK.Vector3 with the X, Y, and Z components of this instance.
        /// </summary>
        public Vector3 Xyz { get { return new Vector3(X, Y, Z); } set { X = value.X; Y = value.Y; Z = value.Z; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector3 with the X, Y, and Z components of this instance.
        /// </summary>
        public Vector3 Xyw { get { return new Vector3(X, Y, W); } set { X = value.X; Y = value.Y; W = value.Z; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector3 with the X, Z, and Y components of this instance.
        /// </summary>
        public Vector3 Xzy { get { return new Vector3(X, Z, Y); } set { X = value.X; Z = value.Y; Y = value.Z; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector3 with the X, Z, and W components of this instance.
        /// </summary>
        public Vector3 Xzw { get { return new Vector3(X, Z, W); } set { X = value.X; Z = value.Y; W = value.Z; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector3 with the X, W, and Y components of this instance.
        /// </summary>
        public Vector3 Xwy { get { return new Vector3(X, W, Y); } set { X = value.X; W = value.Y; Y = value.Z; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector3 with the X, W, and Z components of this instance.
        /// </summary>
        public Vector3 Xwz { get { return new Vector3(X, W, Z); } set { X = value.X; W = value.Y; Z = value.Z; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector3 with the Y, X, and Z components of this instance.
        /// </summary>
        public Vector3 Yxz { get { return new Vector3(Y, X, Z); } set { Y = value.X; X = value.Y; Z = value.Z; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector3 with the Y, X, and W components of this instance.
        /// </summary>
        public Vector3 Yxw { get { return new Vector3(Y, X, W); } set { Y = value.X; X = value.Y; W = value.Z; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector3 with the Y, Z, and X components of this instance.
        /// </summary>
        public Vector3 Yzx { get { return new Vector3(Y, Z, X); } set { Y = value.X; Z = value.Y; X = value.Z; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector3 with the Y, Z, and W components of this instance.
        /// </summary>
        public Vector3 Yzw { get { return new Vector3(Y, Z, W); } set { Y = value.X; Z = value.Y; W = value.Z; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector3 with the Y, W, and X components of this instance.
        /// </summary>
        public Vector3 Ywx { get { return new Vector3(Y, W, X); } set { Y = value.X; W = value.Y; X = value.Z; } }

        /// <summary>
        /// Gets an OpenTK.Vector3 with the Y, W, and Z components of this instance.
        /// </summary>
        public Vector3 Ywz { get { return new Vector3(Y, W, Z); } set { Y = value.X; W = value.Y; Z = value.Z; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector3 with the Z, X, and Y components of this instance.
        /// </summary>
        public Vector3 Zxy { get { return new Vector3(Z, X, Y); } set { Z = value.X; X = value.Y; Y = value.Z; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector3 with the Z, X, and W components of this instance.
        /// </summary>
        public Vector3 Zxw { get { return new Vector3(Z, X, W); } set { Z = value.X; X = value.Y; W = value.Z; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector3 with the Z, Y, and X components of this instance.
        /// </summary>
        public Vector3 Zyx { get { return new Vector3(Z, Y, X); } set { Z = value.X; Y = value.Y; X = value.Z; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector3 with the Z, Y, and W components of this instance.
        /// </summary>
        public Vector3 Zyw { get { return new Vector3(Z, Y, W); } set { Z = value.X; Y = value.Y; W = value.Z; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector3 with the Z, W, and X components of this instance.
        /// </summary>
        public Vector3 Zwx { get { return new Vector3(Z, W, X); } set { Z = value.X; W = value.Y; X = value.Z; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector3 with the Z, W, and Y components of this instance.
        /// </summary>
        public Vector3 Zwy { get { return new Vector3(Z, W, Y); } set { Z = value.X; W = value.Y; Y = value.Z; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector3 with the W, X, and Y components of this instance.
        /// </summary>
        public Vector3 Wxy { get { return new Vector3(W, X, Y); } set { W = value.X; X = value.Y; Y = value.Z; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector3 with the W, X, and Z components of this instance.
        /// </summary>
        public Vector3 Wxz { get { return new Vector3(W, X, Z); } set { W = value.X; X = value.Y; Z = value.Z; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector3 with the W, Y, and X components of this instance.
        /// </summary>
        public Vector3 Wyx { get { return new Vector3(W, Y, X); } set { W = value.X; Y = value.Y; X = value.Z; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector3 with the W, Y, and Z components of this instance.
        /// </summary>
        public Vector3 Wyz { get { return new Vector3(W, Y, Z); } set { W = value.X; Y = value.Y; Z = value.Z; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector3 with the W, Z, and X components of this instance.
        /// </summary>
        public Vector3 Wzx { get { return new Vector3(W, Z, X); } set { W = value.X; Z = value.Y; X = value.Z; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector3 with the W, Z, and Y components of this instance.
        /// </summary>
        public Vector3 Wzy { get { return new Vector3(W, Z, Y); } set { W = value.X; Z = value.Y; Y = value.Z; } }

        #endregion

        #region 4-component

        /// <summary>
        /// Gets or sets an OpenTK.Vector4 with the X, Y, W, and Z components of this instance.
        /// </summary>
        public Vector4 Xywz { get { return new Vector4(X, Y, W, Z); } set { X = value.X; Y = value.Y; W = value.Z; Z = value.W; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector4 with the X, Z, Y, and W components of this instance.
        /// </summary>
        public Vector4 Xzyw { get { return new Vector4(X, Z, Y, W); } set { X = value.X; Z = value.Y; Y = value.Z; W = value.W; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector4 with the X, Z, W, and Y components of this instance.
        /// </summary>
        public Vector4 Xzwy { get { return new Vector4(X, Z, W, Y); } set { X = value.X; Z = value.Y; W = value.Z; Y = value.W; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector4 with the X, W, Y, and Z components of this instance.
        /// </summary>
        public Vector4 Xwyz { get { return new Vector4(X, W, Y, Z); } set { X = value.X; W = value.Y; Y = value.Z; Z = value.W; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector4 with the X, W, Z, and Y components of this instance.
        /// </summary>
        public Vector4 Xwzy { get { return new Vector4(X, W, Z, Y); } set { X = value.X; W = value.Y; Z = value.Z; Y = value.W; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector4 with the Y, X, Z, and W components of this instance.
        /// </summary>
        public Vector4 Yxzw { get { return new Vector4(Y, X, Z, W); } set { Y = value.X; X = value.Y; Z = value.Z; W = value.W; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector4 with the Y, X, W, and Z components of this instance.
        /// </summary>
        public Vector4 Yxwz { get { return new Vector4(Y, X, W, Z); } set { Y = value.X; X = value.Y; W = value.Z; Z = value.W; } }

        /// <summary>
        /// Gets an OpenTK.Vector4 with the Y, Y, Z, and W components of this instance.
        /// </summary>
        public Vector4 Yyzw { get { return new Vector4(Y, Y, Z, W); } set { X = value.X; Y = value.Y; Z = value.Z; W = value.W; } }

        /// <summary>
        /// Gets an OpenTK.Vector4 with the Y, Y, W, and Z components of this instance.
        /// </summary>
        public Vector4 Yywz { get { return new Vector4(Y, Y, W, Z); } set { X = value.X; Y = value.Y; W = value.Z; Z = value.W; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector4 with the Y, Z, X, and W components of this instance.
        /// </summary>
        public Vector4 Yzxw { get { return new Vector4(Y, Z, X, W); } set { Y = value.X; Z = value.Y; X = value.Z; W = value.W; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector4 with the Y, Z, W, and X components of this instance.
        /// </summary>
        public Vector4 Yzwx { get { return new Vector4(Y, Z, W, X); } set { Y = value.X; Z = value.Y; W = value.Z; X = value.W; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector4 with the Y, W, X, and Z components of this instance.
        /// </summary>
        public Vector4 Ywxz { get { return new Vector4(Y, W, X, Z); } set { Y = value.X; W = value.Y; X = value.Z; Z = value.W; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector4 with the Y, W, Z, and X components of this instance.
        /// </summary>
        public Vector4 Ywzx { get { return new Vector4(Y, W, Z, X); } set { Y = value.X; W = value.Y; Z = value.Z; X = value.W; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector4 with the Z, X, Y, and Z components of this instance.
        /// </summary>
        public Vector4 Zxyw { get { return new Vector4(Z, X, Y, W); } set { Z = value.X; X = value.Y; Y = value.Z; W = value.W; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector4 with the Z, X, W, and Y components of this instance.
        /// </summary>
        public Vector4 Zxwy { get { return new Vector4(Z, X, W, Y); } set { Z = value.X; X = value.Y; W = value.Z; Y = value.W; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector4 with the Z, Y, X, and W components of this instance.
        /// </summary>
        public Vector4 Zyxw { get { return new Vector4(Z, Y, X, W); } set { Z = value.X; Y = value.Y; X = value.Z; W = value.W; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector4 with the Z, Y, W, and X components of this instance.
        /// </summary>
        public Vector4 Zywx { get { return new Vector4(Z, Y, W, X); } set { Z = value.X; Y = value.Y; W = value.Z; X = value.W; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector4 with the Z, W, X, and Y components of this instance.
        /// </summary>
        public Vector4 Zwxy { get { return new Vector4(Z, W, X, Y); } set { Z = value.X; W = value.Y; X = value.Z; Y = value.W; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector4 with the Z, W, Y, and X components of this instance.
        /// </summary>
        public Vector4 Zwyx { get { return new Vector4(Z, W, Y, X); } set { Z = value.X; W = value.Y; Y = value.Z; X = value.W; } }

        /// <summary>
        /// Gets an OpenTK.Vector4 with the Z, W, Z, and Y components of this instance.
        /// </summary>
        public Vector4 Zwzy { get { return new Vector4(Z, W, Z, Y); } set { X = value.X; W = value.Y; Z = value.Z; Y = value.W; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector4 with the W, X, Y, and Z components of this instance.
        /// </summary>
        public Vector4 Wxyz { get { return new Vector4(W, X, Y, Z); } set { W = value.X; X = value.Y; Y = value.Z; Z = value.W; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector4 with the W, X, Z, and Y components of this instance.
        /// </summary>
        public Vector4 Wxzy { get { return new Vector4(W, X, Z, Y); } set { W = value.X; X = value.Y; Z = value.Z; Y = value.W; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector4 with the W, Y, X, and Z components of this instance.
        /// </summary>
        public Vector4 Wyxz { get { return new Vector4(W, Y, X, Z); } set { W = value.X; Y = value.Y; X = value.Z; Z = value.W; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector4 with the W, Y, Z, and X components of this instance.
        /// </summary>
        public Vector4 Wyzx { get { return new Vector4(W, Y, Z, X); } set { W = value.X; Y = value.Y; Z = value.Z; X = value.W; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector4 with the W, Z, X, and Y components of this instance.
        /// </summary>
        public Vector4 Wzxy { get { return new Vector4(W, Z, X, Y); } set { W = value.X; Z = value.Y; X = value.Z; Y = value.W; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector4 with the W, Z, Y, and X components of this instance.
        /// </summary>
        public Vector4 Wzyx { get { return new Vector4(W, Z, Y, X); } set { W = value.X; Z = value.Y; Y = value.Z; X = value.W; } }

        /// <summary>
        /// Gets an OpenTK.Vector4 with the W, Z, Y, and W components of this instance.
        /// </summary>
        public Vector4 Wzyw { get { return new Vector4(W, Z, Y, W); } set { X = value.X; Z = value.Y; Y = value.Z; W = value.W; } }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        ///     Calculates the dot (scalar) product of this vector with another.
        /// </summary>
        /// <param name="vec">
        ///     Vector with which to calculate the dot product (together with this one).
        /// </param>
        /// <returns>A Real representing the dot product value.</returns>
        public Real Dot(Vector4 vec)
        {
            return X * vec.X + Y * vec.Y + Z * vec.Z + W * vec.W;
        }

        public Real Normalize()
        {
            Real length = Utility.Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z + this.W * this.W);

            // Will also work for zero-sized vectors, but will change nothing
            if (length > Real.Epsilon)
            {
                Real inverseLength = 1.0f / length;

                this.X *= inverseLength;
                this.Y *= inverseLength;
                this.Z *= inverseLength;
                this.W *= inverseLength;
            }
            
            return length;
        }
        #endregion Methods

        #region Operator overloads + CLS compliant method equivalents

        /// <summary>
        ///		
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Vector4 Multiply(Vector4 vector, Matrix4 matrix)
        {
            return vector * matrix;
        }

        /// <summary>
        ///		
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Vector4 operator *(Matrix4 matrix, Vector4 vector)
        {
            Vector4 result = new Vector4();

            result.X = vector.X * matrix.M00 + vector.Y * matrix.M01 + vector.Z * matrix.M02 + vector.W * matrix.M03;
            result.Y = vector.X * matrix.M10 + vector.Y * matrix.M11 + vector.Z * matrix.M12 + vector.W * matrix.M13;
            result.Z = vector.X * matrix.M20 + vector.Y * matrix.M21 + vector.Z * matrix.M22 + vector.W * matrix.M23;
            result.W = vector.X * matrix.M30 + vector.Y * matrix.M31 + vector.Z * matrix.M32 + vector.W * matrix.M33;

            return result;
        }

        // TODO: Find the signifance of having 2 overloads with opposite param lists that do transposed operations
        public static Vector4 operator *(Vector4 vector, Matrix4 matrix)
        {
            Vector4 result = new Vector4();

            result.X = vector.X * matrix.M00 + vector.Y * matrix.M10 + vector.Z * matrix.M20 + vector.W * matrix.M30;
            result.Y = vector.X * matrix.M01 + vector.Y * matrix.M11 + vector.Z * matrix.M21 + vector.W * matrix.M31;
            result.Z = vector.X * matrix.M02 + vector.Y * matrix.M12 + vector.Z * matrix.M22 + vector.W * matrix.M32;
            result.W = vector.X * matrix.M03 + vector.Y * matrix.M13 + vector.Z * matrix.M23 + vector.W * matrix.M33;

            return result;
        }

        /// <summary>
        ///		Multiplies a Vector4 by a scalar value.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Vector4 operator *(Vector4 vector, Real scalar)
        {
            Vector4 result = new Vector4();

            result.X = vector.X * scalar;
            result.Y = vector.Y * scalar;
            result.Z = vector.Z * scalar;
            result.W = vector.W * scalar;

            return result;
        }

        /// <summary>
        ///		User to compare two Vector4 instances for equality.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>true or false</returns>
        public static bool operator ==(Vector4 left, Vector4 right)
        {
            return (left.X == right.X &&
                     left.Y == right.Y &&
                     left.Z == right.Z &&
                     left.W == right.W);
        }

        /// <summary>
        ///		Used to add a Vector4 to another Vector4.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector4 operator +(Vector4 left, Vector4 right)
        {
            return new Vector4(left.X + right.X, left.Y + right.Y, left.Z + right.Z, left.W + right.W);
        }

        /// <summary>
        ///		Used to subtract a Vector4 from another Vector4.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector4 operator -(Vector4 left, Vector4 right)
        {
            return new Vector4(left.X - right.X, left.Y - right.Y, left.Z - right.Z, left.W - right.W);
        }

        /// <summary>
        ///		Used to negate the elements of a vector.
        /// </summary>
        /// <param name="left"></param>
        /// <returns></returns>
        public static Vector4 operator -(Vector4 left)
        {
            return new Vector4(-left.X, -left.Y, -left.Z, -left.W);
        }

        /// <summary>
        ///		User to compare two Vector4 instances for inequality.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>true or false</returns>
        public static bool operator !=(Vector4 left, Vector4 right)
        {
            return (left.X != right.X ||
                     left.Y != right.Y ||
                     left.Z != right.Z ||
                     left.W != right.W);
        }

        //public static implicit operator Vector4(Color c)
        //{
        //    return new Vector4(c.R, c.G, c.B, c.A);
        //}

        //public static implicit operator Color(Vector4 v)
        //{
        //    return new Color(v.X, v.Y, v.Z, v.W);
        //}

        /// <summary>
        ///		Used to access a Vector by index 0 = this.x, 1 = this.y, 2 = this.z, 3 = this.w.  
        /// </summary>
        /// <remarks>
        ///		Uses unsafe pointer arithmetic to reduce the code required.
        ///	</remarks>
        public Real this[int index]
        {
            get
            {
                Debug.Assert(index >= 0 && index < 4, "Indexer boundaries overrun in Vector4.");

                // using pointer arithmetic here for less code.  Otherwise, we'd have a big switch statement.
                unsafe
                {
                    fixed (Real* pX = &X)
                    {
                        return *(pX + index);
                    }
                }
            }
            set
            {
                Debug.Assert(index >= 0 && index < 4, "Indexer boundaries overrun in Vector4.");

                // using pointer arithmetic here for less code.  Otherwise, we'd have a big switch statement.
                unsafe
                {
                    fixed (Real* pX = &X)
                    {
                        *(pX + index) = value;
                    }
                }
            }
        }

        #endregion

        #region Object overloads

        /// <summary>
        ///		Overrides the Object.ToString() method to provide a text representation of 
        ///		a Vector4.
        /// </summary>
        /// <returns>A string representation of a Vector4.</returns>
        public override string ToString()
        {
            return string.Format("<{0},{1},{2},{3}>", this.X, this.Y, this.Z, this.W);
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
            return this.X.GetHashCode() ^ this.Y.GetHashCode() ^ this.Z.GetHashCode() ^ this.W.GetHashCode();
        }

        /// <summary>
        ///		Compares this Vector to another object.  This should be done because the 
        ///		equality operators (==, !=) have been overriden by this class.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return obj is Vector4 && this == (Vector4)obj;
        }

        #endregion

        #region Parse method, implemented for factories

        /// <summary>
        ///		Parses a string and returns Vector4.
        /// </summary>
        /// <param name="vector">
        ///     A string representation of a Vector4. ( as its returned from Vector4.ToString() )
        /// </param>
        /// <returns>
        ///     A new Vector4.
        /// </returns>
        public static Vector4 Parse(string vector)
        {
            string[] vals = vector.TrimStart('<').TrimEnd('>').Split(',');

            return new Vector4(Real.Parse(vals[0].Trim()), Real.Parse(vals[1].Trim()), Real.Parse(vals[2].Trim()), Real.Parse(vals[3].Trim()));
        }

        #endregion
    }
}
