using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COG.Framework;
using COG.Graphics;
using OpenTK;

namespace COG.Dredger.Entities
{
    public class GameEntity 
    {
        protected int m_id;
        protected IMesh m_mesh; //not owner? proably make this a mesh id?
        protected Vector3 m_position;
        protected float m_orientation;

        public virtual void Update(float dt)
        {

        }

        public virtual void Render(Program program)
        {
            if (m_mesh != null)
            {
                var matrix = Matrix4.CreateTranslation(m_position) * Matrix4.CreateRotationY(m_orientation);
                program.SetUniformMatrix4("model", matrix);
                m_mesh.Render(program);
            }
        }
    }

    public class GameEntityManager
    {

    }
}
