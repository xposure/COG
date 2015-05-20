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
//     <id value="$Id: Matrix4.cs 2940 2012-01-05 12:25:58Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

//

#endregion Namespace Declarations

namespace COG.Math
{
    /// <summary>
    ///		Class encapsulating a standard 4x4 homogenous matrix.
    /// </summary>
    /// <remarks>
    ///		The engine uses column vectors when applying matrix multiplications,
    ///		This means a vector is represented as a single column, 4-row
    ///		matrix. This has the effect that the tranformations implemented
    ///		by the matrices happens right-to-left e.g. if vector V is to be
    ///		transformed by M1 then M2 then M3, the calculation would be
    ///		M3 * M2 * M1 * V. The order that matrices are concatenated is
    ///		vital since matrix multiplication is not cummatative, i.e. you
    ///		can get a different result if you concatenate in the wrong order.
    /// 		<p/>
    ///		The use of column vectors and right-to-left ordering is the
    ///		standard in most mathematical texts, and is the same as used in
    ///		OpenGL. It is, however, the opposite of Direct3D, which has
    ///		inexplicably chosen to differ from the accepted standard and uses
    ///		row vectors and left-to-right matrix multiplication.
    ///		<p/>
    ///		The engine deals with the differences between D3D and OpenGL etc.
    ///		internally when operating through different render systems. The engine
    ///		users only need to conform to standard maths conventions, i.e.
    ///		right-to-left matrix multiplication, (The engine transposes matrices it
    ///		passes to D3D to compensate).
    ///		<p/>
    ///		The generic form M * V which shows the layout of the matrix 
    ///		entries is shown below:
    ///		<p/>
    ///		| m[0][0]  m[0][1]  m[0][2]  m[0][3] |   {x}
    ///		| m[1][0]  m[1][1]  m[1][2]  m[1][3] |   {y}
    ///		| m[2][0]  m[2][1]  m[2][2]  m[2][3] |   {z}
    ///		| m[3][0]  m[3][1]  m[3][2]  m[3][3] |   {1}
    ///	</remarks>
    ///	<ogre headerVersion="1.18" sourceVersion="1.8" />
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix4
    {
        #region Member variables

        public Real M00, M01, M02, M03;
        public Real M10, M11, M12, M13;
        public Real M20, M21, M22, M23;
        public Real M30, M31, M32, M33;

        private static readonly Matrix4 zeroMatrix = new Matrix4(
            0, 0, 0, 0,
            0, 0, 0, 0,
            0, 0, 0, 0,
            0, 0, 0, 0);

        private static readonly Matrix4 identityMatrix = new Matrix4(
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1);

        // NOTE: This is different from what is in OGRE. Not sure why this is the case ATM, however, do not change it.
        private static readonly Matrix4 clipSpace2dToImageSpace = new Matrix4(
            //0.5f,  0.0f, 0.0f, -0.5f,
            //0.0f, -0.5f, 0.0f, -0.5f,
            //0.0f,  0.0f, 0.0f,  1.0f,
            //0.0f,  0.0f, 0.0f,  1.0f );
            0.5f, 0.0f, 0.0f, 0.5f,
            0.0f, -0.5f, 0.0f, 0.5f,
            0.0f, 0.0f, 1.0f, 0.0f,
            0.0f, 0.0f, 0.0f, 1.0f);

        #endregion

        #region Constructors

        public Matrix4(Vector4 row0, Vector4 row1, Vector4 row2, Vector4 row3)
            : this(
                row0.X, row0.Y, row0.Z, row0.W,
                row1.X, row1.Y, row1.Z, row1.W,
                row2.X, row2.Y, row2.Z, row2.W,
                row3.X, row3.Y, row3.Z, row3.W
            )
        {

        }
        /// <summary>
        ///		Creates a new Matrix4 with all the specified parameters.
        /// </summary>
        public Matrix4(Real m00, Real m01, Real m02, Real m03,
                        Real m10, Real m11, Real m12, Real m13,
                        Real m20, Real m21, Real m22, Real m23,
                        Real m30, Real m31, Real m32, Real m33)
        {
            this.M00 = m00;
            this.M01 = m01;
            this.M02 = m02;
            this.M03 = m03;
            this.M10 = m10;
            this.M11 = m11;
            this.M12 = m12;
            this.M13 = m13;
            this.M20 = m20;
            this.M21 = m21;
            this.M22 = m22;
            this.M23 = m23;
            this.M30 = m30;
            this.M31 = m31;
            this.M32 = m32;
            this.M33 = m33;
        }

        #endregion

        #region Static properties

        /// <summary>
        ///    Returns a matrix with the following form:
        ///    | 1,0,0,0 |
        ///    | 0,1,0,0 |
        ///    | 0,0,1,0 |
        ///    | 0,0,0,1 |
        /// </summary>
        public static Matrix4 Identity { get { return identityMatrix; } }

        /// <summary>
        ///    Returns a matrix with all elements set to 0.
        /// </summary>
        public static Matrix4 Zero { get { return zeroMatrix; } }

        public static Matrix4 ClipSpace2DToImageSpace { get { return clipSpace2dToImageSpace; } }

        public static Matrix4 CreateLookAt(Vector3 cameraPosition, Vector3 cameraTarget, Vector3 cameraUpVector)
        {
            Matrix4 matrix;
            CreateLookAt(ref cameraPosition, ref cameraTarget, ref cameraUpVector, out matrix);
            return matrix;
        }

        public static void CreateLookAt(ref Vector3 cameraPosition, ref Vector3 cameraTarget, ref Vector3 cameraUpVector, out Matrix4 result)
        {
            var vector = Vector3.Normalize(cameraPosition - cameraTarget);
            var vector2 = Vector3.Normalize(Vector3.Cross(cameraUpVector, vector));
            var vector3 = Vector3.Cross(vector, vector2);
            result.M00 = vector2.X;
            result.M01 = vector3.X;
            result.M02 = vector.X;
            result.M03 = 0f;
            result.M10 = vector2.Y;
            result.M11 = vector3.Y;
            result.M12 = vector.Y;
            result.M13 = 0f;
            result.M20 = vector2.Z;
            result.M21 = vector3.Z;
            result.M22 = vector.Z;
            result.M23 = 0f;
            result.M30 = -Vector3.Dot(vector2, cameraPosition);
            result.M31 = -Vector3.Dot(vector3, cameraPosition);
            result.M32 = -Vector3.Dot(vector, cameraPosition);
            result.M33 = 1f;
        }

        public static Matrix4 CreateOrthographic(float width, float height, float zNearPlane, float zFarPlane)
        {
            Matrix4 matrix;
            CreateOrthographic(width, height, zNearPlane, zFarPlane, out matrix);
            return matrix;
        }


        public static void CreateOrthographic(float width, float height, float zNearPlane, float zFarPlane, out Matrix4 result)
        {
            result.M00 = 2f / width;
            result.M01 = result.M02 = result.M03 = 0f;
            result.M11 = 2f / height;
            result.M10 = result.M12 = result.M13 = 0f;
            result.M22 = 1f / (zNearPlane - zFarPlane);
            result.M20 = result.M21 = result.M23 = 0f;
            result.M30 = result.M31 = 0f;
            result.M32 = zNearPlane / (zNearPlane - zFarPlane);
            result.M33 = 1f;
        }


        public static Matrix4 CreateOrthographicOffCenter(float left, float right, float bottom, float top, float zNearPlane, float zFarPlane)
        {
            Matrix4 matrix;
            CreateOrthographicOffCenter(left, right, bottom, top, zNearPlane, zFarPlane, out matrix);
            return matrix;
        }


        public static void CreateOrthographicOffCenter(float left, float right, float bottom, float top, float zNearPlane, float zFarPlane, out Matrix4 result)
        {
            result.M00 = (float)(2.0 / ((double)right - (double)left));
            result.M01 = 0.0f;
            result.M02 = 0.0f;
            result.M03 = 0.0f;
            result.M10 = 0.0f;
            result.M11 = (float)(2.0 / ((double)top - (double)bottom));
            result.M12 = 0.0f;
            result.M13 = 0.0f;
            result.M20 = 0.0f;
            result.M21 = 0.0f;
            result.M22 = (float)(1.0 / ((double)zNearPlane - (double)zFarPlane));
            result.M23 = 0.0f;
            result.M30 = (float)(((double)left + (double)right) / ((double)left - (double)right));
            result.M31 = (float)(((double)top + (double)bottom) / ((double)bottom - (double)top));
            result.M32 = (float)((double)zNearPlane / ((double)zNearPlane - (double)zFarPlane));
            result.M33 = 1.0f;
        }


        public static Matrix4 CreateTranslation(float xPosition, float yPosition, float zPosition)
        {
            Matrix4 result;
            CreateTranslation(xPosition, yPosition, zPosition, out result);
            return result;
        }

        public static void CreateTranslation(ref Vector3 position, out Matrix4 result)
        {
            result.M00 = 1;
            result.M01 = 0;
            result.M02 = 0;
            result.M03 = 0;
            result.M10 = 0;
            result.M11 = 1;
            result.M12 = 0;
            result.M13 = 0;
            result.M20 = 0;
            result.M21 = 0;
            result.M22 = 1;
            result.M23 = 0;
            result.M30 = position.X;
            result.M31 = position.Y;
            result.M32 = position.Z;
            result.M33 = 1;
        }

        public static Matrix4 CreatePerspective(float width, float height, float nearPlaneDistance, float farPlaneDistance)
        {
            Matrix4 matrix;
            CreatePerspective(width, height, nearPlaneDistance, farPlaneDistance, out matrix);
		    return matrix;
        }


        public static void CreatePerspective(float width, float height, float nearPlaneDistance, float farPlaneDistance, out Matrix4 result)
        {
            if (nearPlaneDistance <= 0f)
		    {
		        throw new ArgumentException("nearPlaneDistance <= 0");
		    }
		    if (farPlaneDistance <= 0f)
		    {
		        throw new ArgumentException("farPlaneDistance <= 0");
		    }
		    if (nearPlaneDistance >= farPlaneDistance)
		    {
		        throw new ArgumentException("nearPlaneDistance >= farPlaneDistance");
		    }
		    result.M00 = (2f * nearPlaneDistance) / width;
		    result.M01 = result.M02 = result.M03 = 0f;
		    result.M11 = (2f * nearPlaneDistance) / height;
		    result.M10 = result.M12 = result.M13 = 0f;
		    result.M22 = farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
		    result.M20 = result.M21 = 0f;
		    result.M23 = -1f;
		    result.M30 = result.M31 = result.M33 = 0f;
		    result.M32 = (nearPlaneDistance * farPlaneDistance) / (nearPlaneDistance - farPlaneDistance);
        }


        public static Matrix4 CreatePerspectiveFieldOfView(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
        {
            Matrix4 result;
            CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlaneDistance, farPlaneDistance, out result);
            return result;
        }


        public static void CreatePerspectiveFieldOfView(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance, out Matrix4 result)
        {
            if ((fieldOfView <= 0f) || (fieldOfView >= 3.141593f))
		    {
		        throw new ArgumentException("fieldOfView <= 0 or >= PI");
		    }
		    if (nearPlaneDistance <= 0f)
		    {
		        throw new ArgumentException("nearPlaneDistance <= 0");
		    }
		    if (farPlaneDistance <= 0f)
		    {
		        throw new ArgumentException("farPlaneDistance <= 0");
		    }
		    if (nearPlaneDistance >= farPlaneDistance)
		    {
		        throw new ArgumentException("nearPlaneDistance >= farPlaneDistance");
		    }
            float yMax = nearPlaneDistance * (float)System.Math.Tan(0.5f * fieldOfView);
            float yMin = -yMax;
            float xMin = yMin * aspectRatio;
            float xMax = yMax * aspectRatio;

            CreatePerspectiveOffCenter(xMin, xMax, yMin, yMax, nearPlaneDistance, farPlaneDistance, out result);
        }


        public static Matrix4 CreatePerspectiveOffCenter(float left, float right, float bottom, float top, float nearPlaneDistance, float farPlaneDistance)
        {
            Matrix4 result;
            CreatePerspectiveOffCenter(left, right, bottom, top, nearPlaneDistance, farPlaneDistance, out result);
            return result;
        }


        public static void CreatePerspectiveOffCenter(float left, float right, float bottom, float top, float zNear, float zFar, out Matrix4 result)
        {
            if (zNear <= 0f)
            {
                throw new ArgumentException("nearPlaneDistance <= 0");
            }
            if (zFar <= 0f)
            {
                throw new ArgumentException("farPlaneDistance <= 0");
            }
            if (zNear >= zFar)
            {
                throw new ArgumentException("nearPlaneDistance >= farPlaneDistance");
            }
            float x = (2.0f * zNear) / (right - left);
            float y = (2.0f * zNear) / (top - bottom);
            float a = (right + left) / (right - left);
            float b = (top + bottom) / (top - bottom);
            float c = -(zFar + zNear) / (zFar - zNear);
            float d = -(2.0f * zFar * zNear) / (zFar - zNear);

            result = new Matrix4(x, 0, 0, 0,
                                        0, y, 0, 0,
                                        a, b, c, -1,
                                        0, 0, d, 0);
        }

        public static Matrix4 CreateTranslation(Vector3 position)
        {
            Matrix4 result;
            CreateTranslation(ref position, out result);
            return result;
        }


        public static void CreateTranslation(float xPosition, float yPosition, float zPosition, out Matrix4 result)
        {
            result.M00 = 1;
            result.M01 = 0;
            result.M02 = 0;
            result.M03 = 0;
            result.M10 = 0;
            result.M11 = 1;
            result.M12 = 0;
            result.M13 = 0;
            result.M20 = 0;
            result.M21 = 0;
            result.M22 = 1;
            result.M23 = 0;
            result.M30 = xPosition;
            result.M31 = yPosition;
            result.M32 = zPosition;
            result.M33 = 1;
        }

        public static Matrix4 CreateRotationX(float radians)
        {
            Matrix4 result;
            CreateRotationX(radians, out result);
            return result;
        }


        public static void CreateRotationX(float radians, out Matrix4 result)
        {
            result = Matrix4.Identity;

            var val1 = Utility.Cos(radians);
            var val2 = Utility.Sin(radians);

            result.M11 = val1;
            result.M12 = val2;
            result.M21 = -val2;
            result.M22 = val1;
        }

        public static Matrix4 CreateRotationY(float radians)
        {
            Matrix4 result;
            CreateRotationY(radians, out result);
            return result;
        }


        public static void CreateRotationY(float radians, out Matrix4 result)
        {
            result = Matrix4.Identity;

            var val1 = Utility.Cos(radians);
            var val2 = Utility.Sin(radians);

            result.M00 = val1;
            result.M02 = -val2;
            result.M20 = val2;
            result.M22 = val1;
        }


        public static Matrix4 CreateRotationZ(float radians)
        {
            Matrix4 result;
            CreateRotationZ(radians, out result);
            return result;
        }


        public static void CreateRotationZ(float radians, out Matrix4 result)
        {
            result = Matrix4.Identity;

            var val1 = Utility.Cos(radians);
            var val2 = Utility.Sin(radians);

            result.M00 = val1;
            result.M01 = val2;
            result.M10 = -val2;
            result.M11 = val1;
        }

        public static Matrix4 CreateScale(float scale)
        {
            Matrix4 result;
            CreateScale(scale, scale, scale, out result);
            return result;
        }


        public static void CreateScale(float scale, out Matrix4 result)
        {
            CreateScale(scale, scale, scale, out result);
        }


        public static Matrix4 CreateScale(float xScale, float yScale, float zScale)
        {
            Matrix4 result;
            CreateScale(xScale, yScale, zScale, out result);
            return result;
        }


        public static void CreateScale(float xScale, float yScale, float zScale, out Matrix4 result)
        {
            result = new Matrix4();
            result.M00 = xScale;
            result.M02 = 0;
            result.M03 = 0;
            result.M03 = 0;
            result.M10 = 0;
            result.M11 = yScale;
            result.M12 = 0;
            result.M13 = 0;
            result.M20 = 0;
            result.M21 = 0;
            result.M22 = zScale;
            result.M23 = 0;
            result.M30 = 0;
            result.M31 = 0;
            result.M32 = 0;
            result.M33 = 1;
        }


        public static Matrix4 CreateScale(Vector3 scales)
        {
            Matrix4 result;
            CreateScale(ref scales, out result);
            return result;
        }


        public static void CreateScale(ref Vector3 scales, out Matrix4 result)
        {
            result.M00 = scales.X;
            result.M01 = 0;
            result.M02 = 0;
            result.M03 = 0;
            result.M10 = 0;
            result.M11 = scales.Y;
            result.M12 = 0;
            result.M13 = 0;
            result.M20 = 0;
            result.M21 = 0;
            result.M22 = scales.Z;
            result.M23 = 0;
            result.M30 = 0;
            result.M31 = 0;
            result.M32 = 0;
            result.M33 = 1;
        }
        #endregion

        #region Public properties

        public Vector4 Row0 { get { return new Vector4(M00, M01, M02, M03); } }
        public Vector4 Row1 { get { return new Vector4(M10, M11, M12, M13); } }
        public Vector4 Row2 { get { return new Vector4(M20, M21, M22, M23); } }
        public Vector4 Row3 { get { return new Vector4(M30, M31, M32, M33); } }

        /// <summary>
        ///		Gets/Sets the Translation portion of the matrix.
        ///		| 0 0 0 Tx|
        ///		| 0 0 0 Ty|
        ///		| 0 0 0 Tz|
        ///		| 0 0 0  1 |
        /// </summary>
        public Vector3 Translation
        {
            get { return new Vector3(this.M03, this.M13, this.M23); }
            set
            {
                this.M03 = value.X;
                this.M13 = value.Y;
                this.M23 = value.Z;
            }
        }

        /// <summary>
        ///		Gets/Sets the Scale portion of the matrix.
        ///		|Sx 0  0  0 |
        ///		| 0 Sy 0  0 |
        ///		| 0  0 Sz 0 |
        ///		| 0  0  0  0 |
        /// </summary>
        /// <remarks>
        ///     Note that the property reflects the real scale only when there isn't any rotation, 
        /// i.e. the 3x3 rotation portion of the matrix was a <see cref="Matrix3.Identiy"/> before a scale was set.
        /// If you need to obtain the current scale of a rotated matrix, use the more expensive <see cref="ExtractRotation"/> method.
        /// </remarks>
        public Vector3 Scale
        {
            get { return new Vector3(this.M00, this.M11, this.M22); }
            set
            {
                this.M00 = value.X;
                this.M11 = value.Y;
                this.M22 = value.Z;
            }
        }

        /// <summary>
        /// Check whether or not the matrix is affine matrix.
        /// </summary>
        /// <remarks>
        /// An affine matrix is a 4x4 matrix with row 3 equal to (0, 0, 0, 1),
        /// e.g. no projective coefficients.
        /// </remarks>
        public bool IsAffine { get { return M30 == 0 && M31 == 0 && M32 == 0 && M33 == 1; } }

        /// <summary>
        ///    Gets the determinant of this matrix.
        /// </summary>
        public Real Determinant
        {
            get
            {
                // note: this is an expanded version of the Ogre determinant() method, to give better performance in C#. Generated using a script
                Real result = M00 * (M11 * (M22 * M33 - M32 * M23) - M12 * (M21 * M33 - M31 * M23) + M13 * (M21 * M32 - M31 * M22)) -
                              M01 * (M10 * (M22 * M33 - M32 * M23) - M12 * (M20 * M33 - M30 * M23) + M13 * (M20 * M32 - M30 * M22)) +
                              M02 * (M10 * (M21 * M33 - M31 * M23) - M11 * (M20 * M33 - M30 * M23) + M13 * (M20 * M31 - M30 * M21)) -
                              M03 * (M10 * (M21 * M32 - M31 * M22) - M11 * (M20 * M32 - M30 * M22) + M12 * (M20 * M31 - M30 * M21));

                return result;
            }
        }

        #endregion

        #region Static methods

        /// <summary>
        ///		Used to allow assignment from a Matrix3 to a Matrix4 object.
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Matrix4 FromMatrix3(Matrix3 right)
        {
            return right;
        }

        /// <summary>
        /// Creates a translation Matrix
        /// </summary>
        /// <param name="position"></param>
        /// <param name="scale"></param>
        /// <param name="orientation"></param>
        /// <returns></returns>
        public static Matrix4 Compose(Vector3 translation, Vector3 scale, Quaternion orientation)
        {
            // Ordering:
            //    1. Scale
            //    2. Rotate
            //    3. Translate

            Matrix3 rot3x3, scale3x3;
            rot3x3 = orientation.ToRotationMatrix();
            scale3x3 = Matrix3.Zero;
            scale3x3.m00 = scale.X;
            scale3x3.m11 = scale.Y;
            scale3x3.m22 = scale.Z;

            // Set up final matrix with scale, rotation and translation
            Matrix4 result = rot3x3 * scale3x3;
            result.Translation = translation;

            return result;
        }

        /// <summary>
        /// Creates an inverse translation Matrix
        /// </summary>
        /// <param name="translation"></param>
        /// <param name="scale"></param>
        /// <param name="orientation"></param>
        /// <returns></returns>
        public static Matrix4 ComposeInverse(Vector3 translation, Vector3 scale, Quaternion orientation)
        {
            // Invert the parameters
            Vector3 invTranslate = -translation;
            Vector3 invScale = new Vector3(1f / scale.X, 1f / scale.Y, 1f / scale.Z);
            Quaternion invRot = orientation.Inverse();

            // Because we're inverting, order is translation, rotation, scale
            // So make translation relative to scale & rotation
            invTranslate *= invScale; // scale
            invTranslate = invRot * invTranslate; // rotate

            // Next, make a 3x3 rotation matrix and apply inverse scale
            Matrix3 rot3x3, scale3x3;
            rot3x3 = invRot.ToRotationMatrix();
            scale3x3 = Matrix3.Zero;
            scale3x3.m00 = invScale.X;
            scale3x3.m11 = invScale.Y;
            scale3x3.m22 = invScale.Z;

            // Set up final matrix with scale, rotation and translation
            Matrix4 result = scale3x3 * rot3x3;
            result.Translation = invTranslate;

            return result;
        }

        #endregion

        #region Public methods

        /// <summary>
        ///    Returns a 3x3 portion of this 4x4 matrix.
        /// </summary>
        /// <returns></returns>
        public Matrix3 GetMatrix3()
        {
            return
                new Matrix3(
                    this.M00, this.M01, this.M02,
                    this.M10, this.M11, this.M12,
                    this.M20, this.M21, this.M22);
        }

        /// <summary>
        ///    Returns an inverted matrix.
        /// </summary>
        /// <returns></returns>
        public Matrix4 Inverse()
        {
            return Adjoint() * (1.0f / this.Determinant);
        }

        /// <summary>
        ///     Returns an inverted affine matrix.
        /// </summary>
        /// <returns></returns>
        public Matrix4 InverseAffine()
        {
            Debug.Assert(IsAffine);

            Real t00 = M22 * M11 - M21 * M12;
            Real t10 = M20 * M12 - M22 * M10;
            Real t20 = M21 * M10 - M20 * M11;

            Real invDet = 1 / (M00 * t00 + M01 * t10 + M02 * t20);

            t00 *= invDet;
            t10 *= invDet;
            t20 *= invDet;

            M00 *= invDet;
            M01 *= invDet;
            M02 *= invDet;

            Real r00 = t00;
            Real r01 = M02 * M21 - M01 * M22;
            Real r02 = M01 * M12 - M02 * M11;

            Real r10 = t10;
            Real r11 = M00 * M22 - M02 * M20;
            Real r12 = M02 * M10 - M00 * M12;

            Real r20 = t20;
            Real r21 = M01 * M20 - M00 * M21;
            Real r22 = M00 * M11 - M01 * M10;

            Real r03 = -(r00 * M03 + r01 * M13 + r02 * M23);
            Real r13 = -(r10 * M03 + r11 * M13 + r12 * M23);
            Real r23 = -(r20 * M03 + r21 * M13 + r22 * M23);

            return new Matrix4(
                r00, r01, r02, r03,
                r10, r11, r12, r13,
                r20, r21, r22, r23,
                0, 0, 0, 1);
        }

        /// <summary>
        ///    Swap the rows of the matrix with the columns.
        /// </summary>
        /// <returns>A transposed Matrix.</returns>
        public Matrix4 Transpose()
        {
            return new Matrix4(this.M00, this.M10, this.M20, this.M30,
                                this.M01, this.M11, this.M21, this.M31,
                                this.M02, this.M12, this.M22, this.M32,
                                this.M03, this.M13, this.M23, this.M33);
        }

        /// <summary>
        /// 3-D Vector transformation specially for affine matrix.
        /// </summary>
        /// <remarks>
        /// Transforms the given 3-D vector by the matrix, projecting the
        /// result back into <i>w</i> = 1.
        /// The matrix must be an affine matrix. <see cref="Matrix4.IsAffine"/>.
        /// </remarks>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector3 TransformAffine(Vector3 v)
        {
            Debug.Assert(IsAffine);

            return new Vector3(
                M00 * v.X + M01 * v.Y + M02 * v.Z + M03,
                M10 * v.X + M11 * v.Y + M12 * v.Z + M13,
                M20 * v.X + M21 * v.Y + M22 * v.Z + M23);
        }

        /// <summary>
        /// 4-D Vector transformation specially for affine matrix.
        /// </summary>
        /// <remarks>
        /// The matrix must be an affine matrix. <see cref="Matrix4.IsAffine"/>.
        /// </remarks>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector4 TransformAffine(Vector4 v)
        {
            Debug.Assert(IsAffine);

            return new Vector4(
                M00 * v.X + M01 * v.Y + M02 * v.Z + M03 * v.W,
                M10 * v.X + M11 * v.Y + M12 * v.Z + M13 * v.W,
                M20 * v.X + M21 * v.Y + M22 * v.Z + M23 * v.W,
                v.W);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void MakeRealArray(Real[] reals)
        {
            reals[0] = this.M00;
            reals[1] = this.M01;
            reals[2] = this.M02;
            reals[3] = this.M03;
            reals[4] = this.M10;
            reals[5] = this.M11;
            reals[6] = this.M12;
            reals[7] = this.M13;
            reals[8] = this.M20;
            reals[9] = this.M21;
            reals[10] = this.M22;
            reals[11] = this.M23;
            reals[12] = this.M30;
            reals[13] = this.M31;
            reals[14] = this.M32;
            reals[15] = this.M33;
        }

        public void MakeFloatArray(float[] floats)
        {
            floats[0] = this.M00;
            floats[1] = this.M01;
            floats[2] = this.M02;
            floats[3] = this.M03;
            floats[4] = this.M10;
            floats[5] = this.M11;
            floats[6] = this.M12;
            floats[7] = this.M13;
            floats[8] = this.M20;
            floats[9] = this.M21;
            floats[10] = this.M22;
            floats[11] = this.M23;
            floats[12] = this.M30;
            floats[13] = this.M31;
            floats[14] = this.M32;
            floats[15] = this.M33;
        }

        /// <summary>
        ///     Extract the 3x3 matrix representing the current rotation. 
        /// </summary>
        /// <param name="rotation"></param>
        public Matrix3 ExtractRotation()
        {
            Vector3 axis = Vector3.Zero;
            Matrix3 rotation = Matrix3.Identity;

            axis.X = this.M00;
            axis.Y = this.M10;
            axis.Z = this.M20;
            axis.Normalize();
            rotation.m00 = axis.X;
            rotation.m10 = axis.Y;
            rotation.m20 = axis.Z;

            axis.X = this.M01;
            axis.Y = this.M11;
            axis.Z = this.M21;
            axis.Normalize();
            rotation.m01 = axis.X;
            rotation.m11 = axis.Y;
            rotation.m21 = axis.Z;

            axis.X = this.M02;
            axis.Y = this.M12;
            axis.Z = this.M22;
            axis.Normalize();
            rotation.m02 = axis.X;
            rotation.m12 = axis.Y;
            rotation.m22 = axis.Z;

            return rotation;
        }

        /// <summary>
        ///     Extract scaling information.
        /// </summary>
        /// <returns></returns>
        public Vector3 ExtractScale()
        {
            Vector3 scale = Vector3.UnitScale;
            Vector3 axis = Vector3.Zero;

            axis.X = this.M00;
            axis.Y = this.M10;
            axis.Z = this.M20;
            scale.X = axis.Length;

            axis.X = this.M01;
            axis.Y = this.M11;
            axis.Z = this.M21;
            scale.Y = axis.Length;

            axis.X = this.M02;
            axis.Y = this.M12;
            axis.Z = this.M22;
            scale.Z = axis.Length;

            return scale;
        }

        /// <summary>
        ///    Decompose the matrix.
        /// </summary>
        /// <param name="translation"></param>
        /// <param name="scale"></param>
        /// <param name="orientation"></param>
        public void Decompose(out Vector3 translation, out Vector3 scale, out Quaternion orientation)
        {
            scale = Vector3.UnitScale;
            Matrix3 rotation = Matrix3.Identity;
            Vector3 axis = Vector3.Zero;

            axis.X = this.M00;
            axis.Y = this.M10;
            axis.Z = this.M20;
            scale.X = axis.Normalize(); // Normalize() returns the vector's length before it was normalized
            rotation.m00 = axis.X;
            rotation.m10 = axis.Y;
            rotation.m20 = axis.Z;

            axis.X = this.M01;
            axis.Y = this.M11;
            axis.Z = this.M21;
            scale.Y = axis.Normalize();
            rotation.m01 = axis.X;
            rotation.m11 = axis.Y;
            rotation.m21 = axis.Z;

            axis.X = this.M02;
            axis.Y = this.M12;
            axis.Z = this.M22;
            scale.Z = axis.Normalize();
            rotation.m02 = axis.X;
            rotation.m12 = axis.Y;
            rotation.m22 = axis.Z;

            orientation = Quaternion.FromRotationMatrix(rotation);
            translation = this.Translation;
        }

        public static Matrix4 Invert(Matrix4 matrix)
        {
            Invert(ref matrix, out matrix);
            return matrix;
        }


        public static void Invert(ref Matrix4 matrix, out Matrix4 result)
        {
            float num1 = matrix.M00;
            float num2 = matrix.M01;
            float num3 = matrix.M02;
            float num4 = matrix.M03;
            float num5 = matrix.M10;
            float num6 = matrix.M11;
            float num7 = matrix.M12;
            float num8 = matrix.M13;
            float num9 = matrix.M20;
            float num10 = matrix.M21;
            float num11 = matrix.M22;
            float num12 = matrix.M23;
            float num13 = matrix.M30;
            float num14 = matrix.M31;
            float num15 = matrix.M32;
            float num16 = matrix.M33;
            float num17 = (float)((double)num11 * (double)num16 - (double)num12 * (double)num15);
            float num18 = (float)((double)num10 * (double)num16 - (double)num12 * (double)num14);
            float num19 = (float)((double)num10 * (double)num15 - (double)num11 * (double)num14);
            float num20 = (float)((double)num9 * (double)num16 - (double)num12 * (double)num13);
            float num21 = (float)((double)num9 * (double)num15 - (double)num11 * (double)num13);
            float num22 = (float)((double)num9 * (double)num14 - (double)num10 * (double)num13);
            float num23 = (float)((double)num6 * (double)num17 - (double)num7 * (double)num18 + (double)num8 * (double)num19);
            float num24 = (float)-((double)num5 * (double)num17 - (double)num7 * (double)num20 + (double)num8 * (double)num21);
            float num25 = (float)((double)num5 * (double)num18 - (double)num6 * (double)num20 + (double)num8 * (double)num22);
            float num26 = (float)-((double)num5 * (double)num19 - (double)num6 * (double)num21 + (double)num7 * (double)num22);
            float num27 = (float)(1.0 / ((double)num1 * (double)num23 + (double)num2 * (double)num24 + (double)num3 * (double)num25 + (double)num4 * (double)num26));

            result.M00 = num23 * num27;
            result.M10 = num24 * num27;
            result.M20 = num25 * num27;
            result.M30 = num26 * num27;
            result.M01 = (float)-((double)num2 * (double)num17 - (double)num3 * (double)num18 + (double)num4 * (double)num19) * num27;
            result.M11 = (float)((double)num1 * (double)num17 - (double)num3 * (double)num20 + (double)num4 * (double)num21) * num27;
            result.M21 = (float)-((double)num1 * (double)num18 - (double)num2 * (double)num20 + (double)num4 * (double)num22) * num27;
            result.M31 = (float)((double)num1 * (double)num19 - (double)num2 * (double)num21 + (double)num3 * (double)num22) * num27;
            float num28 = (float)((double)num7 * (double)num16 - (double)num8 * (double)num15);
            float num29 = (float)((double)num6 * (double)num16 - (double)num8 * (double)num14);
            float num30 = (float)((double)num6 * (double)num15 - (double)num7 * (double)num14);
            float num31 = (float)((double)num5 * (double)num16 - (double)num8 * (double)num13);
            float num32 = (float)((double)num5 * (double)num15 - (double)num7 * (double)num13);
            float num33 = (float)((double)num5 * (double)num14 - (double)num6 * (double)num13);
            result.M02 = (float)((double)num2 * (double)num28 - (double)num3 * (double)num29 + (double)num4 * (double)num30) * num27;
            result.M12 = (float)-((double)num1 * (double)num28 - (double)num3 * (double)num31 + (double)num4 * (double)num32) * num27;
            result.M22 = (float)((double)num1 * (double)num29 - (double)num2 * (double)num31 + (double)num4 * (double)num33) * num27;
            result.M32 = (float)-((double)num1 * (double)num30 - (double)num2 * (double)num32 + (double)num3 * (double)num33) * num27;
            float num34 = (float)((double)num7 * (double)num12 - (double)num8 * (double)num11);
            float num35 = (float)((double)num6 * (double)num12 - (double)num8 * (double)num10);
            float num36 = (float)((double)num6 * (double)num11 - (double)num7 * (double)num10);
            float num37 = (float)((double)num5 * (double)num12 - (double)num8 * (double)num9);
            float num38 = (float)((double)num5 * (double)num11 - (double)num7 * (double)num9);
            float num39 = (float)((double)num5 * (double)num10 - (double)num6 * (double)num9);
            result.M03 = (float)-((double)num2 * (double)num34 - (double)num3 * (double)num35 + (double)num4 * (double)num36) * num27;
            result.M13 = (float)((double)num1 * (double)num34 - (double)num3 * (double)num37 + (double)num4 * (double)num38) * num27;
            result.M23 = (float)-((double)num1 * (double)num35 - (double)num2 * (double)num37 + (double)num4 * (double)num39) * num27;
            result.M33 = (float)((double)num1 * (double)num36 - (double)num2 * (double)num38 + (double)num3 * (double)num39) * num27;


            /*
                        
                        
///
// Use Laplace expansion theorem to calculate the inverse of a 4x4 matrix
// 
// 1. Calculate the 2x2 determinants needed the 4x4 determinant based on the 2x2 determinants 
// 3. Create the adjugate matrix, which satisfies: A * adj(A) = det(A) * I
// 4. Divide adjugate matrix with the determinant to find the inverse
            
float det1, det2, det3, det4, det5, det6, det7, det8, det9, det10, det11, det12;
float detMatrix;
FindDeterminants(ref matrix, out detMatrix, out det1, out det2, out det3, out det4, out det5, out det6, 
                 out det7, out det8, out det9, out det10, out det11, out det12);
            
float invDetMatrix = 1f / detMatrix;
            
Matrix ret; // Allow for matrix and result to point to the same structure
            
ret.M11 = (matrix.M22*det12 - matrix.M23*det11 + matrix.M24*det10) * invDetMatrix;
ret.M12 = (-matrix.M12*det12 + matrix.M13*det11 - matrix.M14*det10) * invDetMatrix;
ret.M13 = (matrix.M42*det6 - matrix.M43*det5 + matrix.M44*det4) * invDetMatrix;
ret.M14 = (-matrix.M32*det6 + matrix.M33*det5 - matrix.M34*det4) * invDetMatrix;
ret.M21 = (-matrix.M21*det12 + matrix.M23*det9 - matrix.M24*det8) * invDetMatrix;
ret.M22 = (matrix.M11*det12 - matrix.M13*det9 + matrix.M14*det8) * invDetMatrix;
ret.M23 = (-matrix.M41*det6 + matrix.M43*det3 - matrix.M44*det2) * invDetMatrix;
ret.M24 = (matrix.M31*det6 - matrix.M33*det3 + matrix.M34*det2) * invDetMatrix;
ret.M31 = (matrix.M21*det11 - matrix.M22*det9 + matrix.M24*det7) * invDetMatrix;
ret.M32 = (-matrix.M11*det11 + matrix.M12*det9 - matrix.M14*det7) * invDetMatrix;
ret.M33 = (matrix.M41*det5 - matrix.M42*det3 + matrix.M44*det1) * invDetMatrix;
ret.M34 = (-matrix.M31*det5 + matrix.M32*det3 - matrix.M34*det1) * invDetMatrix;
ret.M41 = (-matrix.M21*det10 + matrix.M22*det8 - matrix.M23*det7) * invDetMatrix;
ret.M42 = (matrix.M11*det10 - matrix.M12*det8 + matrix.M13*det7) * invDetMatrix;
ret.M43 = (-matrix.M41*det4 + matrix.M42*det2 - matrix.M43*det1) * invDetMatrix;
ret.M44 = (matrix.M31*det4 - matrix.M32*det2 + matrix.M33*det1) * invDetMatrix;
            
result = ret;
*/
        }
        #endregion

        #region Private methods

        /// <summary>
        ///    Used to generate the adjoint of this matrix.
        /// </summary>
        /// <returns>The adjoint matrix of the current instance.</returns>
        private Matrix4 Adjoint()
        {
            // note: this is an expanded version of the Ogre adjoint() method, to give better performance in C#. Generated using a script
            Real val0 = M11 * (M22 * M33 - M32 * M23) - M12 * (M21 * M33 - M31 * M23) + M13 * (M21 * M32 - M31 * M22);
            Real val1 = -(M01 * (M22 * M33 - M32 * M23) - M02 * (M21 * M33 - M31 * M23) + M03 * (M21 * M32 - M31 * M22));
            Real val2 = M01 * (M12 * M33 - M32 * M13) - M02 * (M11 * M33 - M31 * M13) + M03 * (M11 * M32 - M31 * M12);
            Real val3 = -(M01 * (M12 * M23 - M22 * M13) - M02 * (M11 * M23 - M21 * M13) + M03 * (M11 * M22 - M21 * M12));
            Real val4 = -(M10 * (M22 * M33 - M32 * M23) - M12 * (M20 * M33 - M30 * M23) + M13 * (M20 * M32 - M30 * M22));
            Real val5 = M00 * (M22 * M33 - M32 * M23) - M02 * (M20 * M33 - M30 * M23) + M03 * (M20 * M32 - M30 * M22);
            Real val6 = -(M00 * (M12 * M33 - M32 * M13) - M02 * (M10 * M33 - M30 * M13) + M03 * (M10 * M32 - M30 * M12));
            Real val7 = M00 * (M12 * M23 - M22 * M13) - M02 * (M10 * M23 - M20 * M13) + M03 * (M10 * M22 - M20 * M12);
            Real val8 = M10 * (M21 * M33 - M31 * M23) - M11 * (M20 * M33 - M30 * M23) + M13 * (M20 * M31 - M30 * M21);
            Real val9 = -(M00 * (M21 * M33 - M31 * M23) - M01 * (M20 * M33 - M30 * M23) + M03 * (M20 * M31 - M30 * M21));
            Real val10 = M00 * (M11 * M33 - M31 * M13) - M01 * (M10 * M33 - M30 * M13) + M03 * (M10 * M31 - M30 * M11);
            Real val11 = -(M00 * (M11 * M23 - M21 * M13) - M01 * (M10 * M23 - M20 * M13) + M03 * (M10 * M21 - M20 * M11));
            Real val12 = -(M10 * (M21 * M32 - M31 * M22) - M11 * (M20 * M32 - M30 * M22) + M12 * (M20 * M31 - M30 * M21));
            Real val13 = M00 * (M21 * M32 - M31 * M22) - M01 * (M20 * M32 - M30 * M22) + M02 * (M20 * M31 - M30 * M21);
            Real val14 = -(M00 * (M11 * M32 - M31 * M12) - M01 * (M10 * M32 - M30 * M12) + M02 * (M10 * M31 - M30 * M11));
            Real val15 = M00 * (M11 * M22 - M21 * M12) - M01 * (M10 * M22 - M20 * M12) + M02 * (M10 * M21 - M20 * M11);

            return new Matrix4(val0, val1, val2, val3, val4, val5, val6, val7, val8, val9, val10, val11, val12, val13, val14, val15);
        }

        #endregion

        #region Operator overloads + CLS compliant method equivalents

        /// <summary>
        ///		Used to multiply (concatenate) two 4x4 Matrices.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Matrix4 Multiply(Matrix4 left, Matrix4 right)
        {
            return left * right;
        }

        /// <summary>
        ///		Used to multiply (concatenate) two 4x4 Matrices.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Matrix4 operator *(Matrix4 left, Matrix4 right)
        {
            Matrix4 result = new Matrix4();

            result.M00 = left.M00 * right.M00 + left.M01 * right.M10 + left.M02 * right.M20 + left.M03 * right.M30;
            result.M01 = left.M00 * right.M01 + left.M01 * right.M11 + left.M02 * right.M21 + left.M03 * right.M31;
            result.M02 = left.M00 * right.M02 + left.M01 * right.M12 + left.M02 * right.M22 + left.M03 * right.M32;
            result.M03 = left.M00 * right.M03 + left.M01 * right.M13 + left.M02 * right.M23 + left.M03 * right.M33;

            result.M10 = left.M10 * right.M00 + left.M11 * right.M10 + left.M12 * right.M20 + left.M13 * right.M30;
            result.M11 = left.M10 * right.M01 + left.M11 * right.M11 + left.M12 * right.M21 + left.M13 * right.M31;
            result.M12 = left.M10 * right.M02 + left.M11 * right.M12 + left.M12 * right.M22 + left.M13 * right.M32;
            result.M13 = left.M10 * right.M03 + left.M11 * right.M13 + left.M12 * right.M23 + left.M13 * right.M33;

            result.M20 = left.M20 * right.M00 + left.M21 * right.M10 + left.M22 * right.M20 + left.M23 * right.M30;
            result.M21 = left.M20 * right.M01 + left.M21 * right.M11 + left.M22 * right.M21 + left.M23 * right.M31;
            result.M22 = left.M20 * right.M02 + left.M21 * right.M12 + left.M22 * right.M22 + left.M23 * right.M32;
            result.M23 = left.M20 * right.M03 + left.M21 * right.M13 + left.M22 * right.M23 + left.M23 * right.M33;

            result.M30 = left.M30 * right.M00 + left.M31 * right.M10 + left.M32 * right.M20 + left.M33 * right.M30;
            result.M31 = left.M30 * right.M01 + left.M31 * right.M11 + left.M32 * right.M21 + left.M33 * right.M31;
            result.M32 = left.M30 * right.M02 + left.M31 * right.M12 + left.M32 * right.M22 + left.M33 * right.M32;
            result.M33 = left.M30 * right.M03 + left.M31 * right.M13 + left.M32 * right.M23 + left.M33 * right.M33;

            return result;
        }

        /// <summary>
        ///		Transforms the given 3-D vector by the matrix, projecting the 
        ///		result back into <i>w</i> = 1.
        ///		<p/>
        ///		This means that the initial <i>w</i> is considered to be 1.0,
        ///		and then all the tree elements of the resulting 3-D vector are
        ///		divided by the resulting <i>w</i>.
        /// </summary>
        /// <param name="matrix">A Matrix4.</param>
        /// <param name="vector">A Vector3.</param>
        /// <returns>A new vector.</returns>
        public static Vector3 Multiply(Matrix4 matrix, Vector3 vector)
        {
            return matrix * vector;
        }

        /// <summary>
        ///		Transforms a plane using the specified transform.
        /// </summary>
        /// <param name="matrix">Transformation matrix.</param>
        /// <param name="plane">Plane to transform.</param>
        /// <returns>A transformed plane.</returns>
        public static Plane Multiply(Matrix4 matrix, Plane plane)
        {
            return matrix * plane;
        }

        /// <summary>
        ///		Transforms the given 3-D vector by the matrix, projecting the 
        ///		result back into <i>w</i> = 1.
        ///		<p/>
        ///		This means that the initial <i>w</i> is considered to be 1.0,
        ///		and then all the tree elements of the resulting 3-D vector are
        ///		divided by the resulting <i>w</i>.
        /// </summary>
        /// <param name="matrix">A Matrix4.</param>
        /// <param name="vector">A Vector3.</param>
        /// <returns>A new vector.</returns>
        public static Vector3 operator *(Matrix4 matrix, Vector3 vector)
        {
            Vector3 result = new Vector3();

            Real inverseW = 1.0f / (matrix.M30 * vector.X + matrix.M31 * vector.Y + matrix.M32 * vector.Z + matrix.M33);

            result.X = ((matrix.M00 * vector.X) + (matrix.M01 * vector.Y) + (matrix.M02 * vector.Z) + matrix.M03) * inverseW;
            result.Y = ((matrix.M10 * vector.X) + (matrix.M11 * vector.Y) + (matrix.M12 * vector.Z) + matrix.M13) * inverseW;
            result.Z = ((matrix.M20 * vector.X) + (matrix.M21 * vector.Y) + (matrix.M22 * vector.Z) + matrix.M23) * inverseW;

            return result;
        }

        /// <summary>
        ///		Used to multiply a Matrix4 object by a scalar value..
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Matrix4 operator *(Matrix4 left, Real scalar)
        {
            Matrix4 result = new Matrix4();

            result.M00 = left.M00 * scalar;
            result.M01 = left.M01 * scalar;
            result.M02 = left.M02 * scalar;
            result.M03 = left.M03 * scalar;

            result.M10 = left.M10 * scalar;
            result.M11 = left.M11 * scalar;
            result.M12 = left.M12 * scalar;
            result.M13 = left.M13 * scalar;

            result.M20 = left.M20 * scalar;
            result.M21 = left.M21 * scalar;
            result.M22 = left.M22 * scalar;
            result.M23 = left.M23 * scalar;

            result.M30 = left.M30 * scalar;
            result.M31 = left.M31 * scalar;
            result.M32 = left.M32 * scalar;
            result.M33 = left.M33 * scalar;

            return result;
        }

        /// <summary>
        ///		Used to multiply a transformation to a Plane.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="plane"></param>
        /// <returns></returns>
        public static Plane operator *(Matrix4 left, Plane plane)
        {
            Plane result = new Plane();

            Vector3 planeNormal = plane.Normal;

            result.Normal = new Vector3(
                left.M00 * planeNormal.X + left.M01 * planeNormal.Y + left.M02 * planeNormal.Z,
                left.M10 * planeNormal.X + left.M11 * planeNormal.Y + left.M12 * planeNormal.Z,
                left.M20 * planeNormal.X + left.M21 * planeNormal.Y + left.M22 * planeNormal.Z);

            Vector3 pt = planeNormal * -plane.D;
            pt = left * pt;

            result.D = -pt.Dot(result.Normal);

            return result;
        }

        /// <summary>
        ///		Used to add two matrices together.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Matrix4 Add(Matrix4 left, Matrix4 right)
        {
            return left + right;
        }

        /// <summary>
        ///		Used to add two matrices together.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Matrix4 operator +(Matrix4 left, Matrix4 right)
        {
            Matrix4 result = new Matrix4();

            result.M00 = left.M00 + right.M00;
            result.M01 = left.M01 + right.M01;
            result.M02 = left.M02 + right.M02;
            result.M03 = left.M03 + right.M03;

            result.M10 = left.M10 + right.M10;
            result.M11 = left.M11 + right.M11;
            result.M12 = left.M12 + right.M12;
            result.M13 = left.M13 + right.M13;

            result.M20 = left.M20 + right.M20;
            result.M21 = left.M21 + right.M21;
            result.M22 = left.M22 + right.M22;
            result.M23 = left.M23 + right.M23;

            result.M30 = left.M30 + right.M30;
            result.M31 = left.M31 + right.M31;
            result.M32 = left.M32 + right.M32;
            result.M33 = left.M33 + right.M33;

            return result;
        }

        /// <summary>
        ///		Used to subtract two matrices.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Matrix4 Subtract(Matrix4 left, Matrix4 right)
        {
            return left - right;
        }

        /// <summary>
        ///		Used to subtract two matrices.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Matrix4 operator -(Matrix4 left, Matrix4 right)
        {
            Matrix4 result = new Matrix4();

            result.M00 = left.M00 - right.M00;
            result.M01 = left.M01 - right.M01;
            result.M02 = left.M02 - right.M02;
            result.M03 = left.M03 - right.M03;

            result.M10 = left.M10 - right.M10;
            result.M11 = left.M11 - right.M11;
            result.M12 = left.M12 - right.M12;
            result.M13 = left.M13 - right.M13;

            result.M20 = left.M20 - right.M20;
            result.M21 = left.M21 - right.M21;
            result.M22 = left.M22 - right.M22;
            result.M23 = left.M23 - right.M23;

            result.M30 = left.M30 - right.M30;
            result.M31 = left.M31 - right.M31;
            result.M32 = left.M32 - right.M32;
            result.M33 = left.M33 - right.M33;

            return result;
        }

        /// <summary>
        /// Compares two Matrix4 instances for equality.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>true if the Matrix 4 instances are equal, false otherwise.</returns>
        public static bool operator ==(Matrix4 left, Matrix4 right)
        {
            if (
                left.M00 == right.M00 && left.M01 == right.M01 && left.M02 == right.M02 && left.M03 == right.M03 &&
                left.M10 == right.M10 && left.M11 == right.M11 && left.M12 == right.M12 && left.M13 == right.M13 &&
                left.M20 == right.M20 && left.M21 == right.M21 && left.M22 == right.M22 && left.M23 == right.M23 &&
                left.M30 == right.M30 && left.M31 == right.M31 && left.M32 == right.M32 && left.M33 == right.M33)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Compares two Matrix4 instances for inequality.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>true if the Matrix 4 instances are not equal, false otherwise.</returns>
        public static bool operator !=(Matrix4 left, Matrix4 right)
        {
            return !(left == right);
        }

        /// <summary>
        ///		Used to allow assignment from a Matrix3 to a Matrix4 object.
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public static implicit operator Matrix4(Matrix3 right)
        {
            Matrix4 result = Matrix4.Identity;

            result.M00 = right.m00;
            result.M01 = right.m01;
            result.M02 = right.m02;
            result.M10 = right.m10;
            result.M11 = right.m11;
            result.M12 = right.m12;
            result.M20 = right.m20;
            result.M21 = right.m21;
            result.M22 = right.m22;

            return result;
        }

        /// <summary>
        ///    Allows the Matrix to be accessed like a 2d array (i.e. matrix[2,3])
        /// </summary>
        /// <remarks>
        ///    This indexer is only provided as a convenience, and is <b>not</b> recommended for use in
        ///    intensive applications.  
        /// </remarks>
        public Real this[int row, int col]
        {
            get
            {
                //Debug.Assert((row >= 0 && row < 4) && (col >= 0 && col < 4), "Attempt to access Matrix4 indexer out of bounds.");

                unsafe
                {
                    fixed (Real* pM = &M00)
                    {
                        return *(pM + ((4 * row) + col));
                    }
                }
            }
            set
            {
                //Debug.Assert((row >= 0 && row < 4) && (col >= 0 && col < 4), "Attempt to access Matrix4 indexer out of bounds.");

                unsafe
                {
                    fixed (Real* pM = &M00)
                    {
                        *(pM + ((4 * row) + col)) = value;
                    }
                }
            }
        }

        /// <summary>
        ///		Allows the Matrix to be accessed linearly (m[0] -> m[15]).  
        /// </summary>
        /// <remarks>
        ///    This indexer is only provided as a convenience, and is <b>not</b> recommended for use in
        ///    intensive applications.  
        /// </remarks>
        public Real this[int index]
        {
            get
            {
                //Debug.Assert(index >= 0 && index < 16, "Attempt to access Matrix4 linear indexer out of bounds.");

                unsafe
                {
                    fixed (Real* pMatrix = &this.M00)
                    {
                        return *(pMatrix + index);
                    }
                }
            }
            set
            {
                //Debug.Assert(index >= 0 && index < 16, "Attempt to access Matrix4 linear indexer out of bounds.");

                unsafe
                {
                    fixed (Real* pMatrix = &this.M00)
                    {
                        *(pMatrix + index) = value;
                    }
                }
            }
        }

        #endregion

        #region Object overloads

        /// <summary>
        ///		Overrides the Object.ToString() method to provide a text representation of 
        ///		a Matrix4.
        /// </summary>
        /// <returns>A string representation of a vector3.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat(" | {0} {1} {2} {3} |\n", this.M00, this.M01, this.M02, this.M03);
            sb.AppendFormat(" | {0} {1} {2} {3} |\n", this.M10, this.M11, this.M12, this.M13);
            sb.AppendFormat(" | {0} {1} {2} {3} |\n", this.M20, this.M21, this.M22, this.M23);
            sb.AppendFormat(" | {0} {1} {2} {3} |\n", this.M30, this.M31, this.M32, this.M33);

            return sb.ToString();
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
            int hashCode = 0;

            unsafe
            {
                fixed (Real* pM = &M00)
                {
                    for (int i = 0; i < 16; i++)
                    {
                        hashCode ^= (int)(*(pM + i));
                    }
                }
            }

            return hashCode;
        }

        /// <summary>
        ///		Compares this Matrix to another object.  This should be done because the 
        ///		equality operators (==, !=) have been overriden by this class.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return obj is Matrix4 && this == (Matrix4)obj;
        }

        #endregion
    }
}
