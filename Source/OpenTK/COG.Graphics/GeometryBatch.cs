//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using COG.Framework;

//namespace COG.Graphics
//{
//    public class GeometryBatch : DisposableObject
//    {
//        public const int INITIAL_SIZE = 1024;
//        public const int MAX_SIZE = 65536;

//        private DynamicMesh m_mesh;

//        //private HardwareVertexBuffer m_vertexBuffer;
//        //private HardwareIndexBuffer m_indexBuffer;

//        //private ushort m_indexBufferPos;
//        private ushort m_vertexBufferPos;

//        //private VertexPositionTextureColor[] m_vertices;
//        //private ushort[] m_indices;

//        private bool m_beginCalled;
//        private Texture2D m_texture = null;

//        protected override void DisposeManaged()
//        {
//            base.DisposeManaged();

//            m_mesh.Dispose();
//            m_mesh = null;
//            //m_vertexBuffer.Dispose();
//            //m_indexBuffer.Dispose();

//            //m_vertexBuffer = null;
//            //m_indexBuffer = null;
//            //m_vertices = null;
//            //m_indices = null;

//            m_texture = null; //we don't own this
//        }

//        public GeometryBatch()
//            : this(INITIAL_SIZE)
//        {
//            m_mesh = new DynamicMesh()
//            //m_vertexBuffer = new HardwareVertexBuffer(VertexPositionTextureColor.VertexDeclaration, BufferUsage.Dynamic);
//            //m_indexBuffer = new HardwareIndexBuffer(IndexType.Size16, BufferUsage.Dynamic);

//            //m_vertexBuffer.Resize(MAX_SIZE);
//            //m_indexBuffer.Resize(MAX_SIZE);

//            //m_vertices = new VertexPositionTextureColor[MAX_SIZE];
//            //m_indices = new ushort[MAX_SIZE];
//        }

//        public GeometryBatch(int initialSize)
//        {
//            m_mesh = new DynamicMesh(null, initialSize);
//        }

//        private void Flush()
//        {

//            if (m_indexBufferPos > 0)
//            {
//                Contract.RequiresNotNull(m_texture, "m_texture");

//                m_vertexBuffer.WriteData(0, m_vertexBufferPos, m_vertices);
//                m_indexBuffer.WriteData(0, m_indexBufferPos, m_indices);

//                Root.instance.renderSystem.SetTexture2D(m_texture);
//                Root.instance.renderSystem.RenderObject(m_indexBuffer, m_vertexBuffer, m_vertexBufferPos, m_indexBufferPos / 3);

//                //Console.WriteLine("....flushing {{ indices: {0}, vertices: {1}, texture: {2}}}", m_indexBufferPos, m_vertexBufferPos, m_texture.TextureID);

//                m_indexBufferPos = 0;
//                m_vertexBufferPos = 0;
//            }
//        }


//        public void SetTexture(Texture2D texture)
//        {
//            if (!m_beginCalled)
//                throw new Exception("You must call begin first");

//            var shouldFlush = !ReferenceEquals(texture, m_texture);
//            if (shouldFlush)
//            {
//                Flush();
//                m_texture = texture;
//                //Root.instance.renderSystem.SetTexture2D(m_texture);
//            }
//        }

//        public void AddTriangle(VertexPositionTextureColor p0, VertexPositionTextureColor p1, VertexPositionTextureColor p2)
//        {
//            if (m_indexBufferPos + 3 >= MAX_SIZE)
//                Flush();

//            var startPosition = m_vertexBufferPos;
//            m_vertices[m_vertexBufferPos++] = p0;
//            m_vertices[m_vertexBufferPos++] = p1;
//            m_vertices[m_vertexBufferPos++] = p2;

//            m_indices[m_indexBufferPos++] = (ushort)(startPosition + 0);
//            m_indices[m_indexBufferPos++] = (ushort)(startPosition + 2);
//            m_indices[m_indexBufferPos++] = (ushort)(startPosition + 1);
//        }

//        public void AddQuad(Texture2D texture, VertexPositionTextureColor tl, VertexPositionTextureColor tr, VertexPositionTextureColor bl, VertexPositionTextureColor br)
//        {
//            if (!ReferenceEquals(texture, m_texture))
//            {
//                Flush();
//                m_texture = texture;
//            }

//            if (m_indexBufferPos + 6 >= MAX_SIZE)
//                Flush();

//            var startPosition = m_vertexBufferPos;
//            m_vertices[m_vertexBufferPos++] = tl;
//            m_vertices[m_vertexBufferPos++] = tr;
//            m_vertices[m_vertexBufferPos++] = bl;
//            m_vertices[m_vertexBufferPos++] = br;

//            m_indices[m_indexBufferPos++] = (ushort)(startPosition + 0);
//            m_indices[m_indexBufferPos++] = (ushort)(startPosition + 2);
//            m_indices[m_indexBufferPos++] = (ushort)(startPosition + 1);
//            m_indices[m_indexBufferPos++] = (ushort)(startPosition + 1);
//            m_indices[m_indexBufferPos++] = (ushort)(startPosition + 2);
//            m_indices[m_indexBufferPos++] = (ushort)(startPosition + 3);
//        }

//        public void Begin()
//        {
//            if (m_beginCalled)
//                throw new InvalidOperationException("Begin cannot be called again until End has been successfully called.");

//            m_beginCalled = true;
//            m_texture = null;
//            m_vertexBufferPos = 0;
//            m_indexBufferPos = 0;
//        }

//        /// <summary>
//        /// Flushes all batched text and sprites to the screen.
//        /// </summary>
//        /// <remarks>This command should be called after <see cref="Begin"/> and drawing commands.</remarks>
//        public void End()
//        {
//            m_beginCalled = false;
//            Flush();
//            m_texture = null;
//        }


//    }

//}
