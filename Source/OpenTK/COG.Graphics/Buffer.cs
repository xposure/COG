using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COG.Framework;
using COG.Logging;
using OpenTK.Graphics.OpenGL4;

namespace COG.Graphics
{
    public interface IBuffer : IDisposableObject
    {
        int ID { get; }
        //BufferTarget Target { get; }
        //BufferUsageHint Usage { get; }
        //void Bind();
        //void Unbind();
    }

    public interface IWriteableBuffer<T> : IBuffer
        where T : struct
    {
        void Write(T[] data, int length);
    }


    //public class VertexBuffer<T> : DisposableObject, IWriteableBuffer<T>
    //    where T: struct, IVertexDescriptor
    //{
    //    protected VertexDeclaration m_decl;

    //    public VertexBuffer()
    //    {
    //        T t = new T();
    //        m_decl = t.Declaration;
    //    }

    //    public void Write(T[] data, int length)
    //    {

    //    }
    //}

    public class BufferManager : DisposableObject
    {
        private static readonly Logger g_logger = Logger.GetLogger(typeof(BufferManager));

        private List<IBuffer> m_buffers = new List<IBuffer>();

        public ArrayBuffer<T> CreateArrayBuffer<T>(BufferUsageHint usage)
            where T: struct
        {
            var buffer = new ArrayBuffer<T>(this, usage);
            m_buffers.Add(buffer);
            return buffer;
        }

        internal void RemoveBuffer(IBuffer buffer)
        {
            m_buffers.Remove(buffer);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            g_logger.Info("destroying {0} buffers.", m_buffers.Count);

            for (int i = m_buffers.Count - 1; i >= 0; i--)
            {
                var buffer = m_buffers[i];
                m_buffers.RemoveAt(i);

                buffer.Dispose();
            }
        }

        //internal void EnableTempVAO()
        //{

        //}


    }



    public abstract class Buffer : DisposableObject, IBuffer
    {
        protected BufferManager m_buffers;
        protected int m_bufferID;
        protected BufferTarget m_target;
        protected BufferUsageHint m_usage;
        //public abstract BufferTarget Target { get; }
        //public abstract BufferUsageHint Usage { get; }

        internal Buffer(BufferManager buffers, BufferTarget target)
            : this(buffers, target, BufferUsageHint.StreamDraw)
        {

        }

        internal Buffer(BufferManager buffers, BufferTarget target, BufferUsageHint usage)
        {
            m_buffers = buffers;
            m_target = target;
            m_usage = usage;
        }

        public int ID { get { return m_bufferID; } }

        protected void Setup()
        {
            if (m_bufferID == 0)
                m_bufferID = GL.GenBuffer();
        }

        //public void Bind()
        //{
        //    if (m_bufferID > 0)
        //        GL.BindBuffer(Target, m_bufferID);
        //}

        //public void Unbind()
        //{
        //    if (m_bufferID > 0)
        //        GL.BindBuffer(Target, 0);
        //}

        protected void Destroy()
        {
            if (m_bufferID > 0)
                GL.DeleteBuffer(m_bufferID);

            m_bufferID = 0;
        }

        protected override void DisposedUnmanaged()
        {
            Destroy();

            m_buffers.RemoveBuffer(this);
        }

    }


    public class WriteableBuffer<T> : Buffer, IWriteableBuffer<T>
        where T : struct
    {
        protected int m_structSize;

        internal WriteableBuffer(BufferManager buffers, BufferTarget target, BufferUsageHint usage)
            : base(buffers, target, usage)
        {
            m_structSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(T));
        }

        public void Write(T[] data, int length)
        {
            Setup();

            GL.BindBuffer(m_target, m_bufferID);
            GL.BufferData(m_target, new IntPtr(m_structSize * length), data, m_usage);
            GL.BindBuffer(m_target, 0);
        }
    }

    public class ArrayBuffer<T> : WriteableBuffer<T>
        where T: struct
    {
        internal ArrayBuffer(BufferManager buffers, BufferUsageHint usage)
            : base(buffers, BufferTarget.ArrayBuffer, usage)
        {

        }
    }

    //public abstract class Buffera : DisposableObject
    //{
    //    protected int m_bufferID;
    //    protected int m_structSize;

    //    public BufferTarget Target { get; }
    //    public BufferUsageHint Usage { get; }

    //    public void Write<T>(T[] data, int length)
    //        where T : struct
    //    {
    //        if (m_bufferID == 0)
    //            m_bufferID = GL.GenBuffer();

    //        GL.BindBuffer(Target, m_bufferID);
    //        GL.BufferData(Target, new IntPtr(System.Runtime.InteropServices.Marshal.SizeOf(typeof(T)) * length), data, Usage);
    //        GL.BindBuffer(Target, 0);


    //    }


    //    public void Bind()
    //    {
    //        if (m_bufferID > 0)
    //            GL.BindBuffer(Target, m_bufferID);
    //    }

    //}
}
