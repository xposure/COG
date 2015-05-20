using System;
using System.Diagnostics;
using System.Globalization;

namespace OpenTK
{
    public struct Vector3i : IComparable<Vector3i>, IEquatable<Vector3i>
    {
        #region Fields

        /// <summary>X component.</summary>
        public int x;
        /// <summary>Y component.</summary>
        public int y;
        /// <summary>Z component.</summary>
        public int z;

        private static readonly Vector3i zeroVector = new Vector3i(0, 0, 0);
        private static readonly Vector3i unitX = new Vector3i(1, 0, 0);
        private static readonly Vector3i unitY = new Vector3i(0, 1, 0);
        private static readonly Vector3i unitZ = new Vector3i(0, 0, 1);
        private static readonly Vector3i negativeUnitX = new Vector3i(-1, 0, 0);
        private static readonly Vector3i negativeUnitY = new Vector3i(0, -1, 0);
        private static readonly Vector3i negativeUnitZ = new Vector3i(0, 0, -1);
        private static readonly Vector3i unitVector = new Vector3i(1, 1, 1);

        #endregion

        #region Constructors

        /// <summary>
        ///		Creates a new 3 dimensional Vector.
        /// </summary>
        public Vector3i(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;

        }

        /// <summary>
        ///		Creates a new 3 dimensional Vector.
        /// </summary>
        public Vector3i(int unitDimension)
            : this(unitDimension, unitDimension, unitDimension)
        {
        }

        /// <summary>
        ///		Creates a new 3 dimensional Vector.
        /// </summary>
        public Vector3i(int[] coordinates)
        {
            if (coordinates.Length != 3)
                throw new ArgumentException("The coordinates array must be of length 3 to specify the x, y, and z coordinates.");
            this.x = coordinates[0];
            this.y = coordinates[1];
            this.z = coordinates[2];
        }

        #endregion

        #region Overloaded operators + CLS compliant method equivalents

        /// <summary>
        ///		User to compare two Vector3i instances for equality.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>true or false</returns>
        public static bool operator ==(Vector3i left, Vector3i right)
        {
            return (left.x == right.x && left.y == right.y && left.z == right.z);
        }

        /// <summary>
        ///		User to compare two Vector3i instances for inequality.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>true or false</returns>
        public static bool operator !=(Vector3i left, Vector3i right)
        {
            return (left.x != right.x || left.y != right.y || left.z != right.z);
        }

        /// <summary>
        ///		Used when a Vector3i is multiplied by another vector.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector3i Multiply(Vector3i left, Vector3i right)
        {
            return left * right;
        }

        /// <summary>
        ///		Used when a Vector3i is multiplied by another vector.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector3i operator *(Vector3i left, Vector3i right)
        {
            //return new Vector3i( left.x * right.x, left.y * right.y, left.z * right.z );
            Vector3i retVal;
            retVal.x = left.x * right.x;
            retVal.y = left.y * right.y;
            retVal.z = left.z * right.z;
            return retVal;
        }

        /// <summary>
        /// Used to divide a vector by a scalar value.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Vector3i Divide(Vector3i left, int scalar)
        {
            return left / scalar;
        }

        /// <summary>
        ///		Used when a Vector3i is divided by another vector.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector3i operator /(Vector3i left, Vector3i right)
        {
            Vector3i vector;

            vector.x = left.x / right.x;
            vector.y = left.y / right.y;
            vector.z = left.z / right.z;

            return vector;
        }

        /// <summary>
        ///		Used when a Vector3i is divided by another vector.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector3i Divide(Vector3i left, Vector3i right)
        {
            return left / right;
        }

        /// <summary>
        /// Used to divide a vector by a scalar value.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Vector3i operator /(Vector3i left, float scalar)
        {
            Debug.Assert(scalar != 0, "Cannot divide a Vector3i by zero.");

            Vector3i vector;

            // get the inverse of the scalar up front to avoid doing multiple divides later
            float inverse = 1f / scalar;

            vector.x = (int)(left.x * inverse);
            vector.y = (int)(left.y * inverse);
            vector.z = (int)(left.z * inverse);

            return vector;
        }


        /// <summary>
        ///		Used when a Vector3i is added to another Vector3i.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector3i Add(Vector3i left, Vector3i right)
        {
            return left + right;
        }

        /// <summary>
        ///		Used when a Vector3i is added to another Vector3i.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector3i operator +(Vector3i left, Vector3i right)
        {
            return new Vector3i(left.x + right.x, left.y + right.y, left.z + right.z);
        }

        /// <summary>
        ///		Used when a Vector3i is multiplied by a scalar value.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Vector3i Multiply(Vector3i left, int scalar)
        {
            return left * scalar;
        }

        /// <summary>
        ///		Used when a Vector3i is multiplied by a scalar value.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Vector3i operator *(Vector3i left, int scalar)
        {
            //return new Vector3i( left.x * scalar, left.y * scalar, left.z * scalar );
            Vector3i retVal;
            retVal.x = left.x * scalar;
            retVal.y = left.y * scalar;
            retVal.z = left.z * scalar;
            return retVal;
        }

        /// <summary>
        ///		Used when a scalar value is multiplied by a Vector3i.
        /// </summary>
        /// <param name="scalar"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector3i Multiply(int scalar, Vector3i right)
        {
            return scalar * right;
        }

        /// <summary>
        ///		Used when a scalar value is multiplied by a Vector3i.
        /// </summary>
        /// <param name="scalar"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector3i operator *(int scalar, Vector3i right)
        {
            //return new Vector3i( right.x * scalar, right.y * scalar, right.z * scalar );
            Vector3i retVal;
            retVal.x = right.x * scalar;
            retVal.y = right.y * scalar;
            retVal.z = right.z * scalar;
            return retVal;
        }

        /// <summary>
        ///		Used to subtract a Vector3i from another Vector3i.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector3i Subtract(Vector3i left, Vector3i right)
        {
            return left - right;
        }

        /// <summary>
        ///		Used to subtract a Vector3i from another Vector3i.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector3i operator -(Vector3i left, Vector3i right)
        {
            return new Vector3i(left.x - right.x, left.y - right.y, left.z - right.z);
        }


        /// <summary>
        ///		Used to negate the elements of a vector.
        /// </summary>
        /// <param name="left"></param>
        /// <returns></returns>
        public static Vector3i Negate(Vector3i left)
        {
            return -left;
        }

        /// <summary>
        ///		Used to negate the elements of a vector.
        /// </summary>
        /// <param name="left"></param>
        /// <returns></returns>
        public static Vector3i operator -(Vector3i left)
        {
            return new Vector3i(-left.x, -left.y, -left.z);
        }

        /// <summary>
        ///    Returns true if the vector's scalar components are all smaller
        ///    that the ones of the vector it is compared against.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator >(Vector3i left, Vector3i right)
        {
            if (left.x > right.x && left.y > right.y && left.z > right.z)
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
        public static bool operator <(Vector3i left, Vector3i right)
        {
            if (left.x < right.x && left.y < right.y && left.z < right.z)
            {
                return true;
            }

            return false;
        }

        public static Vector3i operator %(Vector3i left, int right)
        {
            return new Vector3i(left.x % right, left.y % right, left.z % right);
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
        //        Debug.Assert(index >= 0 && index < 3, "Indexer boundaries overrun in Vector3i.");

        //        // using pointer arithmetic here for less code.  Otherwise, we'd have a big switch statement.
        //        unsafe
        //        {
        //            fixed (int* pX = &x)
        //                return *(pX + index);
        //        }
        //    }
        //    set
        //    {
        //        Debug.Assert(index >= 0 && index < 3, "Indexer boundaries overrun in Vector3i.");

        //        // using pointer arithmetic here for less code.  Otherwise, we'd have a big switch statement.
        //        unsafe
        //        {
        //            fixed (int* pX = &x)
        //                *(pX + index) = value;
        //        }
        //    }
        //}

        #endregion

        #region Public methods

        /// <summary>
        /// Returns the distance to another vector.
        /// </summary>
        /// <param name="second"></param>
        /// <returns></returns>
        public int Distance(Vector3i second)
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
        public int DistanceSquared(Vector3i second)
        {
            return (this - second).LengthSquared;
        }

        public object[] ToObjectArray()
        {
            return new object[] { x, y, z };
        }
        public int[] ToArray()
        {
            return new int[] { x, y, z };
        }

        public bool IsAnyComponentGreaterThan(Vector3i vector)
        {
            return (this.x > vector.x || this.y > vector.y || this.z > vector.z);
        }

        public bool IsAnyComponentGreaterThanOrEqualTo(Vector3i vector)
        {
            return (this.x >= vector.x || this.y >= vector.y || this.z >= vector.z);
        }

        public bool IsAnyComponentLessThan(Vector3i vector)
        {
            return (this.x < vector.x || this.y < vector.y || this.z < vector.z);
        }
        public bool IsAnyComponentLessThanOrEqualTo(Vector3i vector)
        {
            return (this.x <= vector.x || this.y <= vector.y || this.z <= vector.z);
        }

        public Vector3i Offset(int x, int y, int z)
        {
            return new Vector3i(this.x + x, this.y + y, this.z + z);
        }

        /// <summary>
        /// Performs a Dot Product operation on 2 vectors.
        /// </summary>
        /// <remarks>
        /// A dot product of two vectors v1 and v2 equals to |v1|*|v2|*cos(fi)
        /// where fi is the angle between the vectors and |v1| and |v2| are the vector lengths.
        /// For unit vectors (whose length is one) the dot product will obviously be just cos(fi).
        /// For example, if the unit vectors are parallel the result is cos(0) = 1,
        /// if they are perpendicular the result is cos(PI/2) = 0.
        /// The dot product may be calculated on vectors with any length however.
        /// A zero vector is treated as perpendicular to any vector (result is 0).
        /// </remarks>
        /// <param name="vector">The vector to perform the Dot Product against.</param>
        /// <returns>Products of vector lengths and cosine of the angle between them. </returns>
        public int Dot(Vector3i vector)
        {
            return x * vector.x + y * vector.y + z * vector.z;
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
        public int AbsDot(Vector3i vec)
        {
            return System.Math.Abs(x * vec.x) + System.Math.Abs(y * vec.y) + System.Math.Abs(z * vec.z);
        }

        /// <summary>
        ///		Performs a Cross Product operation on 2 vectors, which returns a vector that is perpendicular
        ///		to the intersection of the 2 vectors.  Useful for finding face normals.
        /// </summary>
        /// <param name="vector">A vector to perform the Cross Product against.</param>
        /// <returns>A new Vector3i perpedicular to the 2 original vectors.</returns>
        public Vector3i Cross(Vector3i vector)
        {
            return new Vector3i(
                (this.y * vector.z) - (this.z * vector.y),
                (this.z * vector.x) - (this.x * vector.z),
                (this.x * vector.y) - (this.y * vector.x)
                );

        }

        /// <summary>
        ///		Finds a vector perpendicular to this one.
        /// </summary>
        /// <returns></returns>
        public Vector3i Perpendicular()
        {
            Vector3i result = this.Cross(Vector3i.UnitX);

            // check length
            if (result.LengthSquared == 0)
            {
                // This vector is the Y axis multiplied by a scalar, so we have to use another axis
                result = this.Cross(Vector3i.UnitY);
            }

            return result;
        }

        ///<overloads>
        ///<summary>Returns wether this vector is within a positional tolerance of another vector</summary>
        ///<param name="right">The vector to compare with</param>
        ///</overloads>
        ///<remarks>Uses a defalut tolerance of 1E-03</remarks>
        public bool PositionEquals(Vector3i right)
        {
            return this.x == right.x && this.y == right.y && this.z == right.z;
        }

        /// <summary>
        ///		Finds the midpoint between the supplied Vector and this vector.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vector3i MidPoint(Vector3i vector)
        {
            return new Vector3i((this.x + vector.x) / 2,
                (this.y + vector.y) / 2,
                (this.z + vector.z) / 2);
        }

        /// <summary>
        ///		Compares the supplied vector and updates it's x/y/z components of they are higher in value.
        /// </summary>
        /// <param name="compare"></param>
        public void Ceil(Vector3i compare)
        {
            if (compare.x > x)
                x = compare.x;
            if (compare.y > y)
                y = compare.y;
            if (compare.z > z)
                z = compare.z;
        }

        /// <summary>
        ///		Compares the supplied vector and updates it's x/y/z components of they are lower in value.
        /// </summary>
        /// <param name="compare"></param>
        /// <returns></returns>
        public void Floor(Vector3i compare)
        {
            if (compare.x < x)
                x = compare.x;
            if (compare.y < y)
                y = compare.y;
            if (compare.z < z)
                z = compare.z;
        }

        public bool AllComponentsLessThan(int limit)
        {
            return Utility.Abs(x) < limit && Utility.Abs(y) < limit && Utility.Abs(z) < limit;
        }

        public bool DifferenceLessThan(Vector3i other, int limit)
        {
            return Utility.Abs(x - other.x) < limit && Utility.Abs(y - other.y) < limit && Utility.Abs(z - other.z) < limit;
        }

        //public Vector3 ToNormalized()
        //{
        //    float length = Utility.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z);

        //    // Will also work for zero-sized vectors, but will change nothing
        //    if (length > float.Epsilon)
        //    {
        //        float inverseLength = 1f / length;

        //        return new Vector3((int)(this.x * inverseLength),
        //            (int)(this.y * inverseLength),
        //            (int)(this.z * inverseLength));
        //    }

        //    return new Vector3(this.x, this.y, this.z);
        //}

        #endregion

        #region Public properties
        public bool IsZero
        {
            get
            {
                return this.x == 0 && this.y == 0 && this.z == 0;
            }
        }

        /// <summary>
        /// Returns true if this vector is zero length.
        /// </summary>
        public bool IsZeroLength
        {
            get
            {
                return ((x * x) + (y * y) + (z * z)) == 0;
            }
        }

        /// <summary>
        ///    Gets the length (magnitude) of this Vector3i.  The Sqrt operation is expensive, so 
        ///    only use this if you need the exact length of the Vector.  If vector lengths are only going
        ///    to be compared, use LengthSquared instead.
        /// </summary>
        public int Length
        {
            get
            {
                return (int)Utility.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z);
            }
        }

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
        public int LengthSquared
        {
            get
            {
                return (this.x * this.x + this.y * this.y + this.z * this.z);
            }
        }

        #endregion

        #region Static Constant Properties

        /// <summary>
        ///		Gets a Vector3i with all components set to 0.
        /// </summary>
        public static Vector3i Zero
        {
            get
            {
                return zeroVector;
            }
        }

        /// <summary>
        ///		Gets a Vector3i with all components set to 1.
        /// </summary>
        public static Vector3i UnitScale
        {
            get
            {
                return unitVector;
            }
        }

        /// <summary>
        ///		Gets a Vector3i with the X set to 1, and the others set to 0.
        /// </summary>
        public static Vector3i UnitX
        {
            get
            {
                return unitX;
            }
        }

        /// <summary>
        ///		Gets a Vector3i with the Y set to 1, and the others set to 0.
        /// </summary>
        public static Vector3i UnitY
        {
            get
            {
                return unitY;
            }
        }

        /// <summary>
        ///		Gets a Vector3i with the Z set to 1, and the others set to 0.
        /// </summary>
        public static Vector3i UnitZ
        {
            get
            {
                return unitZ;
            }
        }

        /// <summary>
        ///		Gets a Vector3i with the X set to -1, and the others set to 0.
        /// </summary>
        public static Vector3i NegativeUnitX
        {
            get
            {
                return negativeUnitX;
            }
        }

        /// <summary>
        ///		Gets a Vector3i with the Y set to -1, and the others set to 0.
        /// </summary>
        public static Vector3i NegativeUnitY
        {
            get
            {
                return negativeUnitY;
            }
        }

        /// <summary>
        ///		Gets a Vector3i with the Z set to -1, and the others set to 0.
        /// </summary>
        public static Vector3i NegativeUnitZ
        {
            get
            {
                return negativeUnitZ;
            }
        }

        #endregion

        #region Object overloads

        /// <summary>
        ///		Overrides the Object.ToString() method to provide a text representation of 
        ///		a Vector3i.
        /// </summary>
        /// <returns>A string representation of a Vector3i.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}", this.x, this.y, this.z);
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
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
        }

        /// <summary>
        ///		Compares this Vector to another object.  This should be done because the 
        ///		equality operators (==, !=) have been overriden by this class.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return (obj is Vector3i) && (this == (Vector3i)obj);
        }

        #endregion

        #region Parse method, implemented for factories

        /// <summary>
        ///		Parses a string and returns Vector3i.
        /// </summary>
        /// <param name="vector">
        ///     A string representation of a Vector3i as it's returned from Vector3i.ToString()
        /// </param>
        /// <returns>
        ///     A new Vector3i.
        /// </returns>
        public static Vector3i Parse(string vector)
        {
            // the format is "Vector3i(x, y, z)"
            if (!vector.StartsWith("Vector3i("))
                throw new FormatException();

            string[] vals = vector.Substring(8).TrimEnd(')').Split(',');

            return new Vector3i(int.Parse(vals[0].Trim(), CultureInfo.InvariantCulture),
                                int.Parse(vals[1].Trim(), CultureInfo.InvariantCulture),
                                int.Parse(vals[2].Trim(), CultureInfo.InvariantCulture));
        }

        #endregion

        public int CompareTo(Vector3i other)
        {
            return (int)(this.LengthSquared - other.LengthSquared);
        }

        public bool Equals(Vector3i other)
        {
            return this == other;
        }
    }
}