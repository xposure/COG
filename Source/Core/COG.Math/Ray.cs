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
//     <id value="$Id: Ray.cs 2940 2012-01-05 12:25:58Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;

//using System.Collections;

#endregion Namespace Declarations

namespace OpenTK
{
    /// <summary>
    /// 	Representation of a ray in space, ie a line with an origin and direction.
    /// </summary>
    public class Ray2
    {
        #region Fields

        internal Vector2 origin;
        internal Vector2 direction;

        #endregion

        #region Constructors

        /// <summary>
        ///    Default constructor.
        /// </summary>
        public Ray2()
        {
            origin = Vector2.Zero;
            direction = Vector2.UnitX;
        }

        /// <summary>
        ///    Constructor.
        /// </summary>
        /// <param name="origin">Starting point of the ray.</param>
        /// <param name="direction">Direction the ray is pointing.</param>
        public Ray2(Vector2 origin, Vector2 direction)
        {
            this.origin = origin;
            this.direction = direction;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the position of a point t units along the ray.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Vector2 GetPoint(float t)
        {
            return origin + (direction * t);
        }

        /// <summary>
        /// Gets the position of a point t units along the ray.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Vector2 this[float t] { get { return origin + (direction * t); } }

        #endregion Methods

        #region Intersection Methods

        /// <summary>
        ///    Tests whether this ray intersects the given box.
        /// </summary>
        /// <param name="box"></param>
        /// <returns>
        ///		Struct containing info on whether there was a hit, and the distance from the 
        ///		origin of this ray where the intersect happened.
        ///	</returns>
        public IntersectResult Intersects(AxisAlignedBox2 box)
        {
            return Utility.Intersects(this, box);
        }

        /// <summary>
        ///		Tests whether this ray intersects the given plane. 
        /// </summary>
        /// <param name="plane"></param>
        /// <returns>
        ///		Struct containing info on whether there was a hit, and the distance from the 
        ///		origin of this ray where the intersect happened.
        ///	</returns>
        //public IntersectResult Intersects(Plane plane)
        //{
        //    return Utility.Intersects(this, plane);
        //}

        /// <summary>
        ///		Tests whether this ray intersects the given sphere. 
        /// </summary>
        /// <param name="circle"></param>
        /// <returns>
        ///		Struct containing info on whether there was a hit, and the distance from the 
        ///		origin of this ray where the intersect happened.
        ///	</returns>
        public IntersectResult Intersects(Circle circle)
        {
            return Utility.Intersects(this, circle);
        }

        /// <summary>
        ///		Tests whether this ray intersects the given PlaneBoundedVolume. 
        /// </summary>
        /// <param name="volume"></param>
        /// <returns>
        ///		Struct containing info on whether there was a hit, and the distance from the 
        ///		origin of this ray where the intersect happened.
        ///	</returns>
        //public IntersectResult Intersects(PlaneBoundedVolume volume)
        //{
        //    return Utility.Intersects(this, volume);
        //}

        #endregion Intersection Methods

        #region Operator Overloads

        /// <summary>
        /// Gets the position of a point t units along the ray.
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vector2 operator *(Ray2 ray, float t)
        {
            return ray.origin + (ray.direction * t);
        }

        public static bool operator ==(Ray2 left, Ray2 right)
        {
            return left.direction == right.direction && left.origin == right.origin;
        }

        public static bool operator !=(Ray2 left, Ray2 right)
        {
            return left.direction != right.direction || left.origin != right.origin;
        }

        public override bool Equals(object obj)
        {
            return obj is Ray2 && this == (Ray2)obj;
        }

        public override int GetHashCode()
        {
            return direction.GetHashCode() ^ origin.GetHashCode();
        }

        #endregion Operator Overloads

        #region Properties

        /// <summary>
        ///    Gets/Sets the origin of the ray.
        /// </summary>
        public Vector2 Origin { get { return origin; } set { origin = value; } }

        /// <summary>
        ///    Gets/Sets the direction this ray is pointing.
        /// </summary>
        /// <remarks>
        ///    A ray has no length, so the direction goes to infinity.
        /// </remarks>
        public Vector2 Direction { get { return direction; } set { direction = value; } }

        #endregion
    }
}