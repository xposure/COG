using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace COG.Graphics
{
    public  class Camera : Frustum
    {
        #region Variables
        protected Vector3 m_up = Vector3.UnitY;

        protected float m_targetFov = 0.7f;

        //protected ViewFrustum
        #endregion Variables
        //unused, not sure how to do reflections yet
        //private bool m_reflected = false;

        #region Ctor
        #endregion Ctor

        #region Properties
        public Vector3 Up { get { return m_up; } }
        #endregion Properties

        //public void LookThrough()
        //{


        //    //check if reflected and do something else?

        //}
        public void LookAt(Vector3 destPoint)
        {
            LookAt(Position, destPoint);
        }

        private void LookAt(Vector3 sourcePoint, Vector3 destPoint)
        {
            Direction = Vector3.Normalize(destPoint - sourcePoint);

            //float dot = Vector3.Dot(-Vector3.UnitZ, forwardVector);

            //if (Math.Abs(dot - (-1.0f)) < 0.000001f)
            //{
            //    Orientation = new Quaternion(m_up.X, m_up.Y, m_up.Z, 3.1415926535897932f);
            //    return;
            //}
            //if (Math.Abs(dot - (1.0f)) < 0.000001f)
            //{
            //    Orientation = Quaternion.Identity;
            //    return;
            //}

            //float rotAngle = (float)Math.Acos(dot);
            //Vector3 rotAxis = Vector3.Cross(-Vector3.UnitZ, forwardVector);
            //rotAxis = Vector3.Normalize(rotAxis);
            //Orientation = Quaternion.FromAxisAngle(rotAxis, rotAngle);
        }

        public void Update(float delta)
        {
            var activeFov = FieldOfView;
            float diff = Math.Abs(activeFov - m_targetFov);
            if (diff < 1f)
            {
                FieldOfView = m_targetFov;
                return;
            }

            if (activeFov < m_targetFov)
            {
                activeFov += delta;
                if (activeFov >= m_targetFov)
                    activeFov = m_targetFov;
                FieldOfView = activeFov;
            }
            else if (activeFov > m_targetFov)
            {
                activeFov -= delta;
                if (activeFov <= m_targetFov)
                    activeFov = m_targetFov;
                FieldOfView = activeFov;
            }
        }

        public void SetTargetFieldOfView(float fov)
        {
            m_targetFov = fov;
        }

    }
}
