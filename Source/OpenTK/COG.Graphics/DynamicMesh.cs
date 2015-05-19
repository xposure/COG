using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COG.Framework;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace COG.Graphics
{
    /// <summary>
    /// This is designed for performance, all checks are up to the user.
    /// </summary>
    public class DynamicMesh : DisposableObject
    {
        #region Const

        private const int TEMP_INITIAL_INDEX_SIZE = sizeof(UInt16) * TEMP_INITIAL_SIZE;
        private const int TEMP_INITIAL_SIZE = 64;
        private const int TEMP_INITIAL_VERTEX_SIZE = TEMP_VERTEXSIZE_GUESS * TEMP_INITIAL_SIZE;
        private const int TEMP_VERTEXSIZE_GUESS = sizeof(float) * 12;

        #endregion Const

        private int m_bufferID;
        private int m_pos = 0;
        private int m_bufferSize = 0;
        private float[] m_buffer;
        private VertexDeclaration m_decl = new VertexDeclaration();

        public DynamicMesh(VertexDeclaration decl)
            : this(decl, TEMP_INITIAL_SIZE)
        {
        }

        public DynamicMesh(VertexDeclaration decl, int initialSize)
        {
            m_decl = decl;
            m_buffer = new float[m_decl.Size * initialSize];
            m_bufferSize = m_buffer.Length;
        }

        public void Begin()
        {
            m_pos = 0;
        }

        public void End(BufferUsageHint usage)
        {
            if ((m_pos % m_decl.VertexSize) != 0)
                throw new Exception("End position was not a multiple of declarations vertex size.");

            if (m_bufferID == 0)
                m_bufferID = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, m_bufferID);

            var vertexBufferSize = new IntPtr(sizeof(float) * m_pos);
            GL.BufferData(BufferTarget.ArrayBuffer, vertexBufferSize, m_buffer, usage);
        }

        public void Render()
        {
            if (m_bufferID > 0 && m_pos > 0)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, m_bufferID);
                m_decl.Enable();

                var vertices = m_pos / m_decl.Size;
                //var triangles = vertices;
                GL.DrawArrays(PrimitiveType.Triangles, 0, vertices);

                m_decl.Disable();
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            }
        }

        private void GrowVertexBuffer()
        {
            var newSize = m_buffer.Length * 3 / 2;
            Array.Resize(ref m_buffer, newSize);
            m_bufferSize = m_buffer.Length;
        }

        public void Position(Vector2 v)
        {
            if (m_pos + 2 >= m_bufferSize)
                GrowVertexBuffer();

            m_buffer[m_pos++] = v.X;
            m_buffer[m_pos++] = v.Y;
        }

        public void Position(Vector3 v)
        {
            if (m_pos + 3 >= m_bufferSize)
                GrowVertexBuffer();

            m_buffer[m_pos++] = v.X;
            m_buffer[m_pos++] = v.Y;
            m_buffer[m_pos++] = v.Z;
        }

        public void Position(float x, float y)
        {
            if (m_pos + 2 >= m_bufferSize)
                GrowVertexBuffer();

            m_buffer[m_pos++] = x;
            m_buffer[m_pos++] = y;
        }

        public void Position(float x, float y, float z)
        {
            if (m_pos + 3 >= m_bufferSize)
                GrowVertexBuffer();

            m_buffer[m_pos++] = x;
            m_buffer[m_pos++] = y;
            m_buffer[m_pos++] = z;
        }

        public void TextureCoord(Vector2 v)
        {
            if (m_pos + 2 >= m_bufferSize)
                GrowVertexBuffer();

            m_buffer[m_pos++] = v.X;
            m_buffer[m_pos++] = v.Y;
        }

        public void TextureCoord(float u, float v)
        {
            if (m_pos + 2 >= m_bufferSize)
                GrowVertexBuffer();

            m_buffer[m_pos++] = u;
            m_buffer[m_pos++] = v;
        }

        public void Color(Vector3 v)
        {
            if (m_pos + 3 >= m_bufferSize)
                GrowVertexBuffer();

            m_buffer[m_pos++] = v.X;
            m_buffer[m_pos++] = v.Y;
            m_buffer[m_pos++] = v.Z;
        }

        public void Color(Vector4 v)
        {
            if (m_pos + 4 >= m_bufferSize)
                GrowVertexBuffer();

            m_buffer[m_pos++] = v.X;
            m_buffer[m_pos++] = v.Y;
            m_buffer[m_pos++] = v.Z;
            m_buffer[m_pos++] = v.W;
        }

        public void Color(Color4 v)
        {
            if (m_pos + 4 >= m_bufferSize)
                GrowVertexBuffer();

            m_buffer[m_pos++] = v.R;
            m_buffer[m_pos++] = v.G;
            m_buffer[m_pos++] = v.B;
            m_buffer[m_pos++] = v.A;
        }

        public void Color(float r, float g, float b)
        {
            if (m_pos + 3 >= m_bufferSize)
                GrowVertexBuffer();

            m_buffer[m_pos++] = r;
            m_buffer[m_pos++] = g;
            m_buffer[m_pos++] = b;
        }

        public void Color(float r, float g, float b, float a)
        {
            if (m_pos + 4 >= m_bufferSize)
                GrowVertexBuffer();

            m_buffer[m_pos++] = r;
            m_buffer[m_pos++] = g;
            m_buffer[m_pos++] = b;
            m_buffer[m_pos++] = a;
        }

        private void Destroy()
        {
            if (m_bufferID > 0)
                GL.DeleteBuffer(m_bufferID);

            m_bufferID = 0;
        }

        protected override void DisposedUnmanaged()
        {
            base.DisposedUnmanaged();
            
            Destroy();
        }
    }
}
