﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COG.Graphics
{
    /// <summary>
    /// Stores the width and height of a rectangle.
    /// </summary>
    public struct Size : IEquatable<Size>
    {
        #region Fields

        int width, height;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a new Size instance.
        /// </summary>
        /// <param name="width">The width of this instance.</param>
        /// <param name="height">The height of this instance.</param>
        public Size(int width, int height)
            : this()
        {
            Width = width;
            Height = height;
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Gets or sets the width of this instance.
        /// </summary>
        public int Width
        {
            get { return width; }
            set
            {
                if (width < 0)
                    throw new ArgumentOutOfRangeException();
                width = value;
            }
        }

        /// <summary>
        /// Gets or sets the height of this instance.
        /// </summary>
        public int Height
        {
            get { return height; }
            set
            {
                if (height < 0)
                    throw new ArgumentOutOfRangeException();
                height = value;
            }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> that indicates whether this instance is empty or zero.
        /// </summary>
        public bool IsEmpty
        {
            get { return Width == 0 && Height == 0; }
        }

        /// <summary>
        /// Returns a Size instance equal to (0, 0).
        /// </summary>
        public static readonly Size Empty = new Size();

        /// <summary>
        /// Returns a Size instance equal to (0, 0).
        /// </summary>
        public static readonly Size Zero = new Size();

        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left is equal to right; false otherwise.</returns>
        public static bool operator ==(Size left, Size right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two instances for inequality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left is not equal to right; false otherwise.</returns>
        public static bool operator !=(Size left, Size right)
        {
            return !left.Equals(right);
        }

        ///// <summary>
        ///// Converts an OpenTK.Size instance to a System.Drawing.Size.
        ///// </summary>
        ///// <param name="size">
        ///// The <see cref="Size"/> instance to convert.
        ///// </param>
        ///// <returns>
        ///// A <see cref="System.Drawing.Size"/> instance equivalent to size.
        ///// </returns>
        //public static implicit operator System.Drawing.Size(Size size)
        //{
        //    return new System.Drawing.Size(size.Width, size.Height);
        //}

        ///// <summary>
        ///// Converts a System.Drawing.Size instance to an OpenTK.Size.
        ///// </summary>
        ///// <param name="size">
        ///// The <see cref="System.Drawing.Size"/> instance to convert.
        ///// </param>
        ///// <returns>
        ///// A <see cref="Size"/> instance equivalent to size.
        ///// </returns>
        //public static implicit operator Size(System.Drawing.Size size)
        //{
        //    return new Size(size.Width, size.Height);
        //}

        ///// <summary>
        ///// Converts an OpenTK.Point instance to a System.Drawing.SizeF.
        ///// </summary>
        ///// <param name="size">
        ///// The <see cref="Size"/> instance to convert.
        ///// </param>
        ///// <returns>
        ///// A <see cref="System.Drawing.SizeF"/> instance equivalent to size.
        ///// </returns>
        //public static implicit operator System.Drawing.SizeF(Size size)
        //{
        //    return new System.Drawing.SizeF(size.Width, size.Height);
        //}

        /// <summary>
        /// Indicates whether this instance is equal to the specified object.
        /// </summary>
        /// <param name="obj">The object instance to compare to.</param>
        /// <returns>True, if both instances are equal; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Size)
                return Equals((Size)obj);

            return false;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A <see cref="System.Int32"/> that represents the hash code for this instance./></returns>
        public override int GetHashCode()
        {
            return Width.GetHashCode() ^ Height.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that describes this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that describes this instance.</returns>
        public override string ToString()
        {
            return String.Format("{{{0}, {1}}}", Width, Height);
        }

        #endregion

        #region IEquatable<Size> Members

        /// <summary>
        /// Indicates whether this instance is equal to the specified Size.
        /// </summary>
        /// <param name="other">The instance to compare to.</param>
        /// <returns>True, if both instances are equal; false otherwise.</returns>
        public bool Equals(Size other)
        {
            return Width == other.Width && Height == other.Height;
        }

        #endregion
    }
}
