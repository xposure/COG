using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COG.Framework;
using COG.Math;
using OpenTK.Graphics.OpenGL4;

namespace COG.Graphics
{
    public class StreamMesh : DisposableObject
    {
        private const int TEMP_INITIAL_SIZE = 64;

        private int m_initalSize;
        private int m_vbufferID, m_ibufferID;
        private int m_vertexPos = 0, m_indexPos = 0;
        private int m_vbufferSize = 0, m_ibufferSize = 0;
        private float[] m_vbuffer;
        private ushort[] m_ibuffer;
        private VertexDeclaration m_decl = new VertexDeclaration();

        public StreamMesh(VertexDeclaration decl)
            : this(decl, TEMP_INITIAL_SIZE)
        {
        }

        public StreamMesh(VertexDeclaration decl, int initialSize)
        {
            m_decl = decl;
            m_vbuffer = new float[m_decl.Size * initialSize];
            m_vbufferSize = m_vbuffer.Length;
            m_initalSize = initialSize;

        }

        public void Flush()
        {
            if ((m_vertexPos % m_decl.VertexSize) != 0)
                throw new Exception("End position was not a multiple of declarations vertex size.");

            if (m_vbufferID == 0 && m_vertexPos > 0)
                m_vbufferID = GL.GenBuffer();

            if (m_ibufferID == 0 && m_indexPos > 0)
                m_ibufferID = GL.GenBuffer();

            if (m_vbufferID > 0 && m_vertexPos > 0)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, m_vbufferID);
                GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(sizeof(float) * m_vertexPos), m_vbuffer, BufferUsageHint.StreamDraw);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            }

            if (m_ibufferID > 0 && m_indexPos > 0)
            {
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, m_ibufferID);
                GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(sizeof(ushort) * m_indexPos), m_ibuffer, BufferUsageHint.StreamDraw);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            }

            Render();

            m_vertexPos = 0;
            m_indexPos = 0;
        }

        public void Render()
        {
            if (m_vbufferID > 0 && m_vertexPos > 0)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, m_vbufferID);
                m_decl.Enable();
                var vertices = m_vertexPos / m_decl.Size;

                if (m_indexPos == 0)
                {
                    GL.DrawArrays(PrimitiveType.Triangles, 0, vertices);
                }
                else
                {
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, m_ibufferID);
                    GL.DrawElements(PrimitiveType.Triangles, m_indexPos, DrawElementsType.UnsignedShort, IntPtr.Zero);
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
                }
                m_decl.Disable();
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            }
        }

        private void GrowVertexBuffer()
        {
            m_vbufferSize = m_vbufferSize * 3 / 2;
            Array.Resize(ref m_vbuffer, m_vbufferSize);
            Console.WriteLine("Growing vbuffer: {0}", m_vbufferSize);

        }

        private void GrowIndexBuffer()
        {
            if (m_ibufferSize == 0)
            {
                var newSize = Utility.Max(TEMP_INITIAL_SIZE, m_initalSize);
                m_ibuffer = new ushort[newSize];
                m_ibufferSize = newSize;
            }
            else
            {
                m_ibufferSize = m_ibufferSize * 3 / 2;

                Array.Resize(ref m_ibuffer, m_ibufferSize);
                Console.WriteLine("Growing ibuffer: {0}", m_ibufferSize);
            }
        }

        public void Index(ushort i)
        {
            if (m_indexPos + 1 >= m_ibufferSize)
                GrowIndexBuffer();

            m_ibuffer[m_indexPos++] = i;
        }

        public void Triangle(ushort i0, ushort i1, ushort i2)
        {
            if (m_indexPos + 3 >= m_ibufferSize)
                GrowIndexBuffer();

            m_ibuffer[m_indexPos++] = i0;
            m_ibuffer[m_indexPos++] = i1;
            m_ibuffer[m_indexPos++] = i2;
        }

        public void Quad(ushort tl, ushort tr, ushort br, ushort bl)
        {
            if (m_indexPos + 6 >= m_ibufferSize)
                GrowIndexBuffer();

            m_ibuffer[m_indexPos++] = tl;
            m_ibuffer[m_indexPos++] = tr;
            m_ibuffer[m_indexPos++] = bl;

            m_ibuffer[m_indexPos++] = bl;
            m_ibuffer[m_indexPos++] = tr;
            m_ibuffer[m_indexPos++] = br;
        }

        public void Position(Vector2 v)
        {
            if (m_vertexPos + 2 >= m_vbufferSize)
                GrowVertexBuffer();

            m_vbuffer[m_vertexPos++] = v.X;
            m_vbuffer[m_vertexPos++] = v.Y;
        }

        public void Position(Vector3 v)
        {
            if (m_vertexPos + 3 >= m_vbufferSize)
                GrowVertexBuffer();

            m_vbuffer[m_vertexPos++] = v.X;
            m_vbuffer[m_vertexPos++] = v.Y;
            m_vbuffer[m_vertexPos++] = v.Z;
        }

        public void Position(float x, float y)
        {
            if (m_vertexPos + 2 >= m_vbufferSize)
                GrowVertexBuffer();

            m_vbuffer[m_vertexPos++] = x;
            m_vbuffer[m_vertexPos++] = y;
        }

        public void Position(float x, float y, float z)
        {
            if (m_vertexPos + 3 >= m_vbufferSize)
                GrowVertexBuffer();

            m_vbuffer[m_vertexPos++] = x;
            m_vbuffer[m_vertexPos++] = y;
            m_vbuffer[m_vertexPos++] = z;
        }

        public void TextureCoord(Vector2 v)
        {
            if (m_vertexPos + 2 >= m_vbufferSize)
                GrowVertexBuffer();

            m_vbuffer[m_vertexPos++] = v.X;
            m_vbuffer[m_vertexPos++] = v.Y;
        }

        public void TextureCoord(float u, float v)
        {
            if (m_vertexPos + 2 >= m_vbufferSize)
                GrowVertexBuffer();

            m_vbuffer[m_vertexPos++] = u;
            m_vbuffer[m_vertexPos++] = v;
        }

        public void Color(Vector3 v)
        {
            if (m_vertexPos + 3 >= m_vbufferSize)
                GrowVertexBuffer();

            m_vbuffer[m_vertexPos++] = v.X;
            m_vbuffer[m_vertexPos++] = v.Y;
            m_vbuffer[m_vertexPos++] = v.Z;
        }

        public void Color(Vector4 v)
        {
            if (m_vertexPos + 4 >= m_vbufferSize)
                GrowVertexBuffer();

            m_vbuffer[m_vertexPos++] = v.X;
            m_vbuffer[m_vertexPos++] = v.Y;
            m_vbuffer[m_vertexPos++] = v.Z;
            m_vbuffer[m_vertexPos++] = v.W;
        }

        public void Color(Color v)
        {
            if (m_vertexPos + 4 >= m_vbufferSize)
                GrowVertexBuffer();

            m_vbuffer[m_vertexPos++] = v.R;
            m_vbuffer[m_vertexPos++] = v.G;
            m_vbuffer[m_vertexPos++] = v.B;
            m_vbuffer[m_vertexPos++] = v.A;
        }

        public void Color(float r, float g, float b)
        {
            if (m_vertexPos + 3 >= m_vbufferSize)
                GrowVertexBuffer();

            m_vbuffer[m_vertexPos++] = r;
            m_vbuffer[m_vertexPos++] = g;
            m_vbuffer[m_vertexPos++] = b;
        }

        public void Color(float r, float g, float b, float a)
        {
            if (m_vertexPos + 4 >= m_vbufferSize)
                GrowVertexBuffer();

            m_vbuffer[m_vertexPos++] = r;
            m_vbuffer[m_vertexPos++] = g;
            m_vbuffer[m_vertexPos++] = b;
            m_vbuffer[m_vertexPos++] = a;
        }

        private void Destroy()
        {
            if (m_vbufferID > 0)
                GL.DeleteBuffer(m_vbufferID);

            m_vbufferID = 0;

            if (m_ibufferID > 0)
                GL.DeleteBuffer(m_ibufferID);

            m_ibufferID = 0;
        }

        protected override void DisposedUnmanaged()
        {
            base.DisposedUnmanaged();

            Destroy();
        }
    }

}
