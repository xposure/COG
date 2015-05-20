#region LGPL License

/*
Axiom Graphics Engine Library
Copyright © 2003-2011 Axiom Project Team

The overall design, and a majority of the core engine and rendering code
contained within this library is a derivative of the open source Object Oriented
Graphics Engine OGRE, which can be found at http://ogre.sourceforge.net.
Many thanks to the OGRE team for maintaining such a high quality project.

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

#endregion LGPL License

#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: ColorEx.cs 2940 2012-01-05 12:25:58Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using COG.Framework;

#endregion Namespace Declarations

namespace COG.Math
{
    /// <summary>
    ///		This class is necessary so we can store the color components as floating
    ///		point values in the range [0,1].  It serves as an intermediary to System.Drawing.Color, which
    ///		stores them as byte values.  This doesn't allow for slow color component
    ///		interpolation, because with the values always being cast back to a byte would lose
    ///		any small interpolated values (i.e. 223 - .25 as a byte is 223).
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Color : IComparable
    {
        #region Member variables

        /// <summary>
        ///		Alpha value [0,1].
        /// </summary>
        public float A;

        /// <summary>
        ///		Red color component [0,1].
        /// </summary>
        public float R;

        /// <summary>
        ///		Green color component [0,1].
        /// </summary>
        public float G;

        /// <summary>
        ///		Blue color component [0,1].
        /// </summary>
        public float B;

        #endregion Member variables

        #region Constructors
        //public Color()
        //{
        //    A = 1;
        //    R = 1;
        //    B = 1;
        //    G = 1;
        //}
        /// <summary>
        ///	Constructor taking RGB values
        /// </summary>
        /// <param name="r">Red color component.</param>
        /// <param name="g">Green color component.</param>
        /// <param name="b">Blue color component.</param>
        public Color(float r, float g, float b)
            : this(1.0f, r, g, b) { }

        /// <summary>
        ///		Constructor taking all component values.
        /// </summary>
        /// <param name="a">Alpha value.</param>
        /// <param name="r">Red color component.</param>
        /// <param name="g">Green color component.</param>
        /// <param name="b">Blue color component.</param>
        public Color(float a, float r, float g, float b)
        {
            Contract.Requires(a >= 0.0f && a <= 1.0f);
            Contract.Requires(r >= 0.0f && r <= 1.0f);
            Contract.Requires(g >= 0.0f && g <= 1.0f);
            Contract.Requires(b >= 0.0f && b <= 1.0f);

            this.A = a;
            this.R = r;
            this.G = g;
            this.B = b;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">The ColorEx instance to copy</param>
        public Color(Color other)
            : this()
        {
            this.A = other.A;
            this.R = other.R;
            this.G = other.G;
            this.B = other.B;
        }

        #endregion Constructors

        #region Methods

        public int ToRGBA()
        {
            int result = 0;

            result += ((int)(R * 255.0f)) << 24;
            result += ((int)(G * 255.0f)) << 16;
            result += ((int)(B * 255.0f)) << 8;
            result += ((int)(A * 255.0f));

            return result;
        }

        public static Color FromRGBA(int r, int g, int b, int a)
        {
            return new Color(a / 255f, r / 255f, g / 255f, b / 255f);
        }

        /// <summary>
        ///		Converts this color value to packed ABGR format.
        /// </summary>
        /// <returns></returns>
        public int ToABGR()
        {
            int result = 0;

            result += ((int)(A * 255.0f)) << 24;
            result += ((int)(B * 255.0f)) << 16;
            result += ((int)(G * 255.0f)) << 8;
            result += ((int)(R * 255.0f));

            return result;
        }

        /// <summary>
        ///		Converts this color value to packed ARBG format.
        /// </summary>
        /// <returns></returns>
        public int ToARGB()
        {
            int result = 0;

            result += ((int)(A * 255.0f)) << 24;
            result += ((int)(R * 255.0f)) << 16;
            result += ((int)(G * 255.0f)) << 8;
            result += ((int)(B * 255.0f));

            return result;
        }

        /// <summary>
        ///		Populates the color components in a 4 elements array in RGBA order.
        /// </summary>
        /// <remarks>
        ///		Primarily used to help in OpenGL.
        /// </remarks>
        /// <returns></returns>
        public void ToArrayRGBA(float[] vals)
        {
            vals[0] = R;
            vals[1] = G;
            vals[2] = B;
            vals[3] = A;
        }

        /// <summary>
        /// Clamps color value to the range [0, 1]
        /// </summary>
        public void Saturate()
        {
            R = Utility.Clamp(R, 1.0f, 0.0f);
            G = Utility.Clamp(G, 1.0f, 0.0f);
            B = Utility.Clamp(B, 1.0f, 0.0f);
            A = Utility.Clamp(A, 1.0f, 0.0f);
        }

        /// <summary>
        /// Clamps color value to the range [0, 1] in a copy
        /// </summary>
        public Color SaturateCopy()
        {
            Color saturated;
            saturated.R = Utility.Clamp(R, 1.0f, 0.0f);
            saturated.G = Utility.Clamp(G, 1.0f, 0.0f);
            saturated.B = Utility.Clamp(B, 1.0f, 0.0f);
            saturated.A = Utility.Clamp(A, 1.0f, 0.0f);

            return saturated;
        }

        #endregion Methods

        #region Operators

        public static bool operator ==(Color left, Color right)
        {
            return left.A == right.A &&
                   left.B == right.B &&
                   left.G == right.G &&
                   left.R == right.R;
        }

        public static bool operator !=(Color left, Color right)
        {
            return !(left == right);
        }

        public static Color operator *(Color left, Color right)
        {
            Color retVal = left;
            retVal.A *= right.A;
            retVal.R *= right.R;
            retVal.G *= right.G;
            retVal.B *= right.B;
            return retVal;
        }

        public static Color operator *(Color left, float scalar)
        {
            Color retVal = left;
            retVal.A *= scalar;
            retVal.R *= scalar;
            retVal.G *= scalar;
            retVal.B *= scalar;
            return retVal;
        }

        public static Color operator /(Color left, Color right)
        {
            Color retVal = left;
            retVal.A /= right.A;
            retVal.R /= right.R;
            retVal.G /= right.G;
            retVal.B /= right.B;
            return retVal;
        }

        public static Color operator /(Color left, float scalar)
        {
            Color retVal = left;
            retVal.A /= scalar;
            retVal.R /= scalar;
            retVal.G /= scalar;
            retVal.B /= scalar;
            return retVal;
        }

        public static Color operator -(Color left, Color right)
        {
            Color retVal = left;
            retVal.A -= right.A;
            retVal.R -= right.R;
            retVal.G -= right.G;
            retVal.B -= right.B;
            return retVal;
        }

        public static Color operator +(Color left, Color right)
        {
            Color retVal = left;
            retVal.A += right.A;
            retVal.R += right.R;
            retVal.G += right.G;
            retVal.B += right.B;
            return retVal;
        }

        public static implicit operator Vector3(Color c)
        {
            return new Vector3(c.R, c.G, c.B);
        }

        public static implicit operator Color(Vector3 v)
        {
            return new Color(v.X, v.Y, v.Z, 1f);
        }

        public static implicit operator Vector4(Color c)
        {
            return new Vector4(c.R, c.G, c.B, c.A);
        }

        public static implicit operator Color(Vector4 v)
        {
            return new Color(v.X, v.Y, v.Z, v.W);
        }

        public static explicit operator Color32(Color c)
        {
            return new Color32(c.A, c.R, c.G, c.B);
        }
        #endregion Operators

        #region Static color properties

        /// <summary>
        ///		The color Transparent.
        /// </summary>
        public static Color Transparent
        {
            get
            {
                Color retVal;
                retVal.A = 0f;
                retVal.R = 1f;
                retVal.G = 1f;
                retVal.B = 1f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color AliceBlue.
        /// </summary>
        public static Color AliceBlue
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.9411765f;
                retVal.G = 0.972549f;
                retVal.B = 1.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color AntiqueWhite.
        /// </summary>
        public static Color AntiqueWhite
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.9803922f;
                retVal.G = 0.9215686f;
                retVal.B = 0.8431373f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Aqua.
        /// </summary>
        public static Color Aqua
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.0f;
                retVal.G = 1.0f;
                retVal.B = 1.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Aquamarine.
        /// </summary>
        public static Color Aquamarine
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.4980392f;
                retVal.G = 1.0f;
                retVal.B = 0.8313726f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Azure.
        /// </summary>
        public static Color Azure
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.9411765f;
                retVal.G = 1.0f;
                retVal.B = 1.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Beige.
        /// </summary>
        public static Color Beige
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.9607843f;
                retVal.G = 0.9607843f;
                retVal.B = 0.8627451f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Bisque.
        /// </summary>
        public static Color Bisque
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 1.0f;
                retVal.G = 0.8941177f;
                retVal.B = 0.7686275f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Black.
        /// </summary>
        public static Color Black
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.0f;
                retVal.G = 0.0f;
                retVal.B = 0.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color BlanchedAlmond.
        /// </summary>
        public static Color BlanchedAlmond
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 1.0f;
                retVal.G = 0.9215686f;
                retVal.B = 0.8039216f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Blue.
        /// </summary>
        public static Color Blue
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.0f;
                retVal.G = 0.0f;
                retVal.B = 1.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color BlueViolet.
        /// </summary>
        public static Color BlueViolet
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.5411765f;
                retVal.G = 0.1686275f;
                retVal.B = 0.8862745f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Brown.
        /// </summary>
        public static Color Brown
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.6470588f;
                retVal.G = 0.1647059f;
                retVal.B = 0.1647059f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color BurlyWood.
        /// </summary>
        public static Color BurlyWood
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.8705882f;
                retVal.G = 0.7215686f;
                retVal.B = 0.5294118f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color CadetBlue.
        /// </summary>
        public static Color CadetBlue
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.372549f;
                retVal.G = 0.6196079f;
                retVal.B = 0.627451f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Chartreuse.
        /// </summary>
        public static Color Chartreuse
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.4980392f;
                retVal.G = 1.0f;
                retVal.B = 0.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Chocolate.
        /// </summary>
        public static Color Chocolate
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.8235294f;
                retVal.G = 0.4117647f;
                retVal.B = 0.1176471f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Coral.
        /// </summary>
        public static Color Coral
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 1.0f;
                retVal.G = 0.4980392f;
                retVal.B = 0.3137255f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color CornflowerBlue.
        /// </summary>
        public static Color CornflowerBlue
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.3921569f;
                retVal.G = 0.5843138f;
                retVal.B = 0.9294118f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Cornsilk.
        /// </summary>
        public static Color Cornsilk
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 1.0f;
                retVal.G = 0.972549f;
                retVal.B = 0.8627451f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Crimson.
        /// </summary>
        public static Color Crimson
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.8627451f;
                retVal.G = 0.07843138f;
                retVal.B = 0.2352941f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Cyan.
        /// </summary>
        public static Color Cyan
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.0f;
                retVal.G = 1.0f;
                retVal.B = 1.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color DarkBlue.
        /// </summary>
        public static Color DarkBlue
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.0f;
                retVal.G = 0.0f;
                retVal.B = 0.5450981f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color DarkCyan.
        /// </summary>
        public static Color DarkCyan
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.0f;
                retVal.G = 0.5450981f;
                retVal.B = 0.5450981f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color DarkGoldenrod.
        /// </summary>
        public static Color DarkGoldenrod
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.7215686f;
                retVal.G = 0.5254902f;
                retVal.B = 0.04313726f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color DarkGray.
        /// </summary>
        public static Color DarkGray
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.6627451f;
                retVal.G = 0.6627451f;
                retVal.B = 0.6627451f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color DarkGreen.
        /// </summary>
        public static Color DarkGreen
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.0f;
                retVal.G = 0.3921569f;
                retVal.B = 0.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color DarkKhaki.
        /// </summary>
        public static Color DarkKhaki
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.7411765f;
                retVal.G = 0.7176471f;
                retVal.B = 0.4196078f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color DarkMagenta.
        /// </summary>
        public static Color DarkMagenta
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.5450981f;
                retVal.G = 0.0f;
                retVal.B = 0.5450981f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color DarkOliveGreen.
        /// </summary>
        public static Color DarkOliveGreen
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.3333333f;
                retVal.G = 0.4196078f;
                retVal.B = 0.1843137f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color DarkOrange.
        /// </summary>
        public static Color DarkOrange
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 1.0f;
                retVal.G = 0.5490196f;
                retVal.B = 0.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color DarkOrchid.
        /// </summary>
        public static Color DarkOrchid
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.6f;
                retVal.G = 0.1960784f;
                retVal.B = 0.8f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color DarkRed.
        /// </summary>
        public static Color DarkRed
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.5450981f;
                retVal.G = 0.0f;
                retVal.B = 0.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color DarkSalmon.
        /// </summary>
        public static Color DarkSalmon
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.9137255f;
                retVal.G = 0.5882353f;
                retVal.B = 0.4784314f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color DarkSeaGreen.
        /// </summary>
        public static Color DarkSeaGreen
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.5607843f;
                retVal.G = 0.7372549f;
                retVal.B = 0.5450981f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color DarkSlateBlue.
        /// </summary>
        public static Color DarkSlateBlue
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.282353f;
                retVal.G = 0.2392157f;
                retVal.B = 0.5450981f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color DarkSlateGray.
        /// </summary>
        public static Color DarkSlateGray
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.1843137f;
                retVal.G = 0.3098039f;
                retVal.B = 0.3098039f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color DarkTurquoise.
        /// </summary>
        public static Color DarkTurquoise
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.0f;
                retVal.G = 0.8078431f;
                retVal.B = 0.8196079f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color DarkViolet.
        /// </summary>
        public static Color DarkViolet
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.5803922f;
                retVal.G = 0.0f;
                retVal.B = 0.827451f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color DeepPink.
        /// </summary>
        public static Color DeepPink
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 1.0f;
                retVal.G = 0.07843138f;
                retVal.B = 0.5764706f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color DeepSkyBlue.
        /// </summary>
        public static Color DeepSkyBlue
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.0f;
                retVal.G = 0.7490196f;
                retVal.B = 1.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color DimGray.
        /// </summary>
        public static Color DimGray
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.4117647f;
                retVal.G = 0.4117647f;
                retVal.B = 0.4117647f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color DodgerBlue.
        /// </summary>
        public static Color DodgerBlue
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.1176471f;
                retVal.G = 0.5647059f;
                retVal.B = 1.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Firebrick.
        /// </summary>
        public static Color Firebrick
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.6980392f;
                retVal.G = 0.1333333f;
                retVal.B = 0.1333333f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color FloralWhite.
        /// </summary>
        public static Color FloralWhite
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 1.0f;
                retVal.G = 0.9803922f;
                retVal.B = 0.9411765f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color ForestGreen.
        /// </summary>
        public static Color ForestGreen
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.1333333f;
                retVal.G = 0.5450981f;
                retVal.B = 0.1333333f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Fuchsia.
        /// </summary>
        public static Color Fuchsia
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 1.0f;
                retVal.G = 0.0f;
                retVal.B = 1.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Gainsboro.
        /// </summary>
        public static Color Gainsboro
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.8627451f;
                retVal.G = 0.8627451f;
                retVal.B = 0.8627451f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color GhostWhite.
        /// </summary>
        public static Color GhostWhite
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.972549f;
                retVal.G = 0.972549f;
                retVal.B = 1.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Gold.
        /// </summary>
        public static Color Gold
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 1.0f;
                retVal.G = 0.8431373f;
                retVal.B = 0.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Goldenrod.
        /// </summary>
        public static Color Goldenrod
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.854902f;
                retVal.G = 0.6470588f;
                retVal.B = 0.1254902f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Gray.
        /// </summary>
        public static Color Gray
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.5019608f;
                retVal.G = 0.5019608f;
                retVal.B = 0.5019608f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Green.
        /// </summary>
        public static Color Green
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.0f;
                retVal.G = 0.5019608f;
                retVal.B = 0.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color GreenYellow.
        /// </summary>
        public static Color GreenYellow
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.6784314f;
                retVal.G = 1.0f;
                retVal.B = 0.1843137f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Honeydew.
        /// </summary>
        public static Color Honeydew
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.9411765f;
                retVal.G = 1.0f;
                retVal.B = 0.9411765f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color HotPink.
        /// </summary>
        public static Color HotPink
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 1.0f;
                retVal.G = 0.4117647f;
                retVal.B = 0.7058824f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color IndianRed.
        /// </summary>
        public static Color IndianRed
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.8039216f;
                retVal.G = 0.3607843f;
                retVal.B = 0.3607843f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Indigo.
        /// </summary>
        public static Color Indigo
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.2941177f;
                retVal.G = 0.0f;
                retVal.B = 0.509804f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Ivory.
        /// </summary>
        public static Color Ivory
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 1.0f;
                retVal.G = 1.0f;
                retVal.B = 0.9411765f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Khaki.
        /// </summary>
        public static Color Khaki
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.9411765f;
                retVal.G = 0.9019608f;
                retVal.B = 0.5490196f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Lavender.
        /// </summary>
        public static Color Lavender
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.9019608f;
                retVal.G = 0.9019608f;
                retVal.B = 0.9803922f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color LavenderBlush.
        /// </summary>
        public static Color LavenderBlush
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 1.0f;
                retVal.G = 0.9411765f;
                retVal.B = 0.9607843f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color LawnGreen.
        /// </summary>
        public static Color LawnGreen
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.4862745f;
                retVal.G = 0.9882353f;
                retVal.B = 0.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color LemonChiffon.
        /// </summary>
        public static Color LemonChiffon
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 1.0f;
                retVal.G = 0.9803922f;
                retVal.B = 0.8039216f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color LightBlue.
        /// </summary>
        public static Color LightBlue
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.6784314f;
                retVal.G = 0.8470588f;
                retVal.B = 0.9019608f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color LightCoral.
        /// </summary>
        public static Color LightCoral
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.9411765f;
                retVal.G = 0.5019608f;
                retVal.B = 0.5019608f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color LightCyan.
        /// </summary>
        public static Color LightCyan
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.8784314f;
                retVal.G = 1.0f;
                retVal.B = 1.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color LightGoldenrodYellow.
        /// </summary>
        public static Color LightGoldenrodYellow
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.9803922f;
                retVal.G = 0.9803922f;
                retVal.B = 0.8235294f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color LightGreen.
        /// </summary>
        public static Color LightGreen
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.5647059f;
                retVal.G = 0.9333333f;
                retVal.B = 0.5647059f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color LightGray.
        /// </summary>
        public static Color LightGray
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.827451f;
                retVal.G = 0.827451f;
                retVal.B = 0.827451f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color LightPink.
        /// </summary>
        public static Color LightPink
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 1.0f;
                retVal.G = 0.7137255f;
                retVal.B = 0.7568628f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color LightSalmon.
        /// </summary>
        public static Color LightSalmon
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 1.0f;
                retVal.G = 0.627451f;
                retVal.B = 0.4784314f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color LightSeaGreen.
        /// </summary>
        public static Color LightSeaGreen
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.1254902f;
                retVal.G = 0.6980392f;
                retVal.B = 0.6666667f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color LightSkyBlue.
        /// </summary>
        public static Color LightSkyBlue
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.5294118f;
                retVal.G = 0.8078431f;
                retVal.B = 0.9803922f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color LightSlateGray.
        /// </summary>
        public static Color LightSlateGray
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.4666667f;
                retVal.G = 0.5333334f;
                retVal.B = 0.6f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color LightSteelBlue.
        /// </summary>
        public static Color LightSteelBlue
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.6901961f;
                retVal.G = 0.7686275f;
                retVal.B = 0.8705882f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color LightYellow.
        /// </summary>
        public static Color LightYellow
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 1.0f;
                retVal.G = 1.0f;
                retVal.B = 0.8784314f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Lime.
        /// </summary>
        public static Color Lime
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.0f;
                retVal.G = 1.0f;
                retVal.B = 0.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color LimeGreen.
        /// </summary>
        public static Color LimeGreen
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.1960784f;
                retVal.G = 0.8039216f;
                retVal.B = 0.1960784f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Linen.
        /// </summary>
        public static Color Linen
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.9803922f;
                retVal.G = 0.9411765f;
                retVal.B = 0.9019608f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Magenta.
        /// </summary>
        public static Color Magenta
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 1.0f;
                retVal.G = 0.0f;
                retVal.B = 1.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Maroon.
        /// </summary>
        public static Color Maroon
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.5019608f;
                retVal.G = 0.0f;
                retVal.B = 0.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color MediumAquamarine.
        /// </summary>
        public static Color MediumAquamarine
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.4f;
                retVal.G = 0.8039216f;
                retVal.B = 0.6666667f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color MediumBlue.
        /// </summary>
        public static Color MediumBlue
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.0f;
                retVal.G = 0.0f;
                retVal.B = 0.8039216f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color MediumOrchid.
        /// </summary>
        public static Color MediumOrchid
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.7294118f;
                retVal.G = 0.3333333f;
                retVal.B = 0.827451f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color MediumPurple.
        /// </summary>
        public static Color MediumPurple
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.5764706f;
                retVal.G = 0.4392157f;
                retVal.B = 0.8588235f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color MediumSeaGreen.
        /// </summary>
        public static Color MediumSeaGreen
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.2352941f;
                retVal.G = 0.7019608f;
                retVal.B = 0.4431373f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color MediumSlateBlue.
        /// </summary>
        public static Color MediumSlateBlue
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.4823529f;
                retVal.G = 0.4078431f;
                retVal.B = 0.9333333f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color MediumSpringGreen.
        /// </summary>
        public static Color MediumSpringGreen
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.0f;
                retVal.G = 0.9803922f;
                retVal.B = 0.6039216f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color MediumTurquoise.
        /// </summary>
        public static Color MediumTurquoise
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.282353f;
                retVal.G = 0.8196079f;
                retVal.B = 0.8f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color MediumVioletRed.
        /// </summary>
        public static Color MediumVioletRed
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.7803922f;
                retVal.G = 0.08235294f;
                retVal.B = 0.5215687f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color MidnightBlue.
        /// </summary>
        public static Color MidnightBlue
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.09803922f;
                retVal.G = 0.09803922f;
                retVal.B = 0.4392157f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color MintCream.
        /// </summary>
        public static Color MintCream
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.9607843f;
                retVal.G = 1.0f;
                retVal.B = 0.9803922f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color MistyRose.
        /// </summary>
        public static Color MistyRose
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 1.0f;
                retVal.G = 0.8941177f;
                retVal.B = 0.8823529f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Moccasin.
        /// </summary>
        public static Color Moccasin
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 1.0f;
                retVal.G = 0.8941177f;
                retVal.B = 0.7098039f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color NavajoWhite.
        /// </summary>
        public static Color NavajoWhite
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 1.0f;
                retVal.G = 0.8705882f;
                retVal.B = 0.6784314f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Navy.
        /// </summary>
        public static Color Navy
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.0f;
                retVal.G = 0.0f;
                retVal.B = 0.5019608f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color OldLace.
        /// </summary>
        public static Color OldLace
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.9921569f;
                retVal.G = 0.9607843f;
                retVal.B = 0.9019608f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Olive.
        /// </summary>
        public static Color Olive
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.5019608f;
                retVal.G = 0.5019608f;
                retVal.B = 0.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color OliveDrab.
        /// </summary>
        public static Color OliveDrab
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.4196078f;
                retVal.G = 0.5568628f;
                retVal.B = 0.1372549f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Orange.
        /// </summary>
        public static Color Orange
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 1.0f;
                retVal.G = 0.6470588f;
                retVal.B = 0.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color OrangeRed.
        /// </summary>
        public static Color OrangeRed
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 1.0f;
                retVal.G = 0.2705882f;
                retVal.B = 0.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Orchid.
        /// </summary>
        public static Color Orchid
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.854902f;
                retVal.G = 0.4392157f;
                retVal.B = 0.8392157f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color PaleGoldenrod.
        /// </summary>
        public static Color PaleGoldenrod
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.9333333f;
                retVal.G = 0.9098039f;
                retVal.B = 0.6666667f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color PaleGreen.
        /// </summary>
        public static Color PaleGreen
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.5960785f;
                retVal.G = 0.9843137f;
                retVal.B = 0.5960785f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color PaleTurquoise.
        /// </summary>
        public static Color PaleTurquoise
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.6862745f;
                retVal.G = 0.9333333f;
                retVal.B = 0.9333333f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color PaleVioletRed.
        /// </summary>
        public static Color PaleVioletRed
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.8588235f;
                retVal.G = 0.4392157f;
                retVal.B = 0.5764706f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color PapayaWhip.
        /// </summary>
        public static Color PapayaWhip
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 1.0f;
                retVal.G = 0.9372549f;
                retVal.B = 0.8352941f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color PeachPuff.
        /// </summary>
        public static Color PeachPuff
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 1.0f;
                retVal.G = 0.854902f;
                retVal.B = 0.7254902f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Peru.
        /// </summary>
        public static Color Peru
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.8039216f;
                retVal.G = 0.5215687f;
                retVal.B = 0.2470588f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Pink.
        /// </summary>
        public static Color Pink
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 1.0f;
                retVal.G = 0.7529412f;
                retVal.B = 0.7960784f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Plum.
        /// </summary>
        public static Color Plum
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.8666667f;
                retVal.G = 0.627451f;
                retVal.B = 0.8666667f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color PowderBlue.
        /// </summary>
        public static Color PowderBlue
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.6901961f;
                retVal.G = 0.8784314f;
                retVal.B = 0.9019608f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Purple.
        /// </summary>
        public static Color Purple
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.5019608f;
                retVal.G = 0.0f;
                retVal.B = 0.5019608f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Red.
        /// </summary>
        public static Color Red
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 1.0f;
                retVal.G = 0.0f;
                retVal.B = 0.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color RosyBrown.
        /// </summary>
        public static Color RosyBrown
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.7372549f;
                retVal.G = 0.5607843f;
                retVal.B = 0.5607843f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color RoyalBlue.
        /// </summary>
        public static Color RoyalBlue
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.254902f;
                retVal.G = 0.4117647f;
                retVal.B = 0.8823529f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color SaddleBrown.
        /// </summary>
        public static Color SaddleBrown
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.5450981f;
                retVal.G = 0.2705882f;
                retVal.B = 0.07450981f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Salmon.
        /// </summary>
        public static Color Salmon
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.9803922f;
                retVal.G = 0.5019608f;
                retVal.B = 0.4470588f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color SandyBrown.
        /// </summary>
        public static Color SandyBrown
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.9568627f;
                retVal.G = 0.6431373f;
                retVal.B = 0.3764706f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color SeaGreen.
        /// </summary>
        public static Color SeaGreen
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.1803922f;
                retVal.G = 0.5450981f;
                retVal.B = 0.3411765f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color SeaShell.
        /// </summary>
        public static Color SeaShell
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 1.0f;
                retVal.G = 0.9607843f;
                retVal.B = 0.9333333f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Sienna.
        /// </summary>
        public static Color Sienna
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.627451f;
                retVal.G = 0.3215686f;
                retVal.B = 0.1764706f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Silver.
        /// </summary>
        public static Color Silver
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.7529412f;
                retVal.G = 0.7529412f;
                retVal.B = 0.7529412f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color SkyBlue.
        /// </summary>
        public static Color SkyBlue
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.5294118f;
                retVal.G = 0.8078431f;
                retVal.B = 0.9215686f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color SlateBlue.
        /// </summary>
        public static Color SlateBlue
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.4156863f;
                retVal.G = 0.3529412f;
                retVal.B = 0.8039216f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color SlateGray.
        /// </summary>
        public static Color SlateGray
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.4392157f;
                retVal.G = 0.5019608f;
                retVal.B = 0.5647059f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Snow.
        /// </summary>
        public static Color Snow
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 1.0f;
                retVal.G = 0.9803922f;
                retVal.B = 0.9803922f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color SpringGreen.
        /// </summary>
        public static Color SpringGreen
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.0f;
                retVal.G = 1.0f;
                retVal.B = 0.4980392f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color SteelBlue.
        /// </summary>
        public static Color SteelBlue
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.2745098f;
                retVal.G = 0.509804f;
                retVal.B = 0.7058824f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Tan.
        /// </summary>
        public static Color Tan
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.8235294f;
                retVal.G = 0.7058824f;
                retVal.B = 0.5490196f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Teal.
        /// </summary>
        public static Color Teal
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.0f;
                retVal.G = 0.5019608f;
                retVal.B = 0.5019608f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Thistle.
        /// </summary>
        public static Color Thistle
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.8470588f;
                retVal.G = 0.7490196f;
                retVal.B = 0.8470588f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Tomato.
        /// </summary>
        public static Color Tomato
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 1.0f;
                retVal.G = 0.3882353f;
                retVal.B = 0.2784314f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Turquoise.
        /// </summary>
        public static Color Turquoise
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.2509804f;
                retVal.G = 0.8784314f;
                retVal.B = 0.8156863f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Violet.
        /// </summary>
        public static Color Violet
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.9333333f;
                retVal.G = 0.509804f;
                retVal.B = 0.9333333f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Wheat.
        /// </summary>
        public static Color Wheat
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.9607843f;
                retVal.G = 0.8705882f;
                retVal.B = 0.7019608f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color White.
        /// </summary>
        public static Color White
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 1.0f;
                retVal.G = 1.0f;
                retVal.B = 1.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color WhiteSmoke.
        /// </summary>
        public static Color WhiteSmoke
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.9607843f;
                retVal.G = 0.9607843f;
                retVal.B = 0.9607843f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Yellow.
        /// </summary>
        public static Color Yellow
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 1.0f;
                retVal.G = 1.0f;
                retVal.B = 0.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color YellowGreen.
        /// </summary>
        public static Color YellowGreen
        {
            get
            {
                Color retVal;
                retVal.A = 1.0f;
                retVal.R = 0.6039216f;
                retVal.G = 0.8039216f;
                retVal.B = 0.1960784f;
                return retVal;
            }
        }

        //TODO : Move this to StringConverter
        public static Color Parse_0_255_String(string parsableText)
        {
            Color retVal;
            if (parsableText == null)
            {
                throw new ArgumentException("The parsableText parameter cannot be null.");
            }
            string[] vals = parsableText.TrimStart('(', '[', '<').TrimEnd(')', ']', '>').Split(',');
            if (vals.Length < 3)
            {
                throw new FormatException(string.Format("Cannot parse the text '{0}' because it must of the form (r,g,b) or (r,g,b,a)",
                                                          parsableText));
            }
            //float r, g, b, a;
            try
            {
                retVal.R = int.Parse(vals[0].Trim()) / 255f;
                retVal.G = int.Parse(vals[1].Trim()) / 255f;
                retVal.B = int.Parse(vals[2].Trim()) / 255f;
                if (vals.Length == 4)
                {
                    retVal.A = int.Parse(vals[3].Trim()) / 255f;
                }
                else
                {
                    retVal.A = 1.0f;
                }
            }
            catch //(Exception e)
            {
                throw new FormatException("The parts of the ColorEx in Parse_0_255 must be integers");
            }
            return retVal;
        }

        //TODO : Move this to StringConverter
        public string To_0_255_String()
        {
            return string.Format("({0},{1},{2},{3})",
                                  (int)(R * 255f),
                                  (int)(G * 255f),
                                  (int)(B * 255f),
                                  (int)(A * 255f));
        }

        #endregion Static color properties

        #region Object overloads

        /// <summary>
        ///    Override GetHashCode.
        /// </summary>
        /// <remarks>
        ///    Done mainly to quash warnings, no real need for it.
        /// </remarks>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.ToARGB();
        }

        public override bool Equals(object obj)
        {
            if (obj is Color)
            {
                return this == (Color)obj;
            }
            else
            {
                return false;
            }
        }

        public override string ToString()
        {
            return this.To_0_255_String();
        }

        #endregion Object overloads

        #region IComparable Members

        /// <summary>
        ///    Used to compare 2 ColorEx objects for equality.
        /// </summary>
        /// <param name="obj">An instance of a ColorEx object to compare to this instance.</param>
        /// <returns>0 if they are equal, 1 if they are not.</returns>
        public int CompareTo(object obj)
        {
            Color other = (Color)obj;

            if (this.A == other.A &&
                this.R == other.R &&
                this.G == other.G &&
                this.B == other.B)
            {
                return 0;
            }

            return 1;
        }

        #endregion IComparable Members

        #region ICloneable Implementation

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public Color Clone()
        {
            Color clone;
            clone.A = this.A;
            clone.R = this.R;
            clone.G = this.G;
            clone.B = this.B;
            return clone;
        }

        #endregion ICloneable Implementation


        public static Color FromNonPremultiplied(float a, float r, float g, float b)
        {
            return new Color(a, r * a, g * a, b * a);
        }
    }
}
