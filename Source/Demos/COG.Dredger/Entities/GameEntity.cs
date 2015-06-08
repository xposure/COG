using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COG.Dredger.Rendering;
using COG.Framework;
using COG.Graphics;
using OpenTK;

namespace COG.Dredger.Entities
{
    public class GameEntity 
    {
        protected int m_id;
        protected IMesh m_mesh; //not owner? proably make this a mesh id?
        protected Vector3 m_position = Vector3.Zero;
        protected Vector3 m_scale = Vector3.One;
        protected float m_orientation = 0f;
        protected Matrix4 m_modelMatrix;
        protected bool m_matrixDirty = true;

        public Matrix4 ModelMatrix
        {
            get
            {
                if (m_matrixDirty)
                    UpdateModelMatrix();

                return m_modelMatrix;
            }
        }

        public void UpdateModelMatrix()
        {
            m_modelMatrix = Matrix4.CreateTranslation(m_position) * Matrix4.CreateRotationY(m_orientation) * Matrix4.CreateScale(m_scale);
            m_matrixDirty = false;
        }

        public virtual void Update(float dt)
        {

        }

        public virtual void Render(MeshRenderer renderer)
        {
            if (m_mesh != null)
            {
                renderer.Render(m_mesh, ModelMatrix);
                //var matrix = Matrix4.CreateTranslation(m_position) * Matrix4.CreateRotationY(m_orientation);
                //program.SetUniformMatrix4("model", matrix);
                //m_mesh.Render(program);
            }
        }
    }

    public class GameEntityManager
    {

        


    }
}
