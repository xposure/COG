using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using COG.Math;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace COG.Graphics
{
    //for now i don't want the vertex logic to know about shaders
    //so i have decided to hard code their attrib locations
    public enum VertexElementSemantic : int
    {
        Position = 0,
        Texture = 1,
        Color = 2
    };

    public class VertexElement
    {
        #region Protected
        protected short m_source;
        protected int m_offset;
        protected int m_typeCount;
        protected VertexAttribPointerType m_type;
        protected VertexElementSemantic m_semantic;
        #endregion

        #region Public
        public VertexElement(int typeCount, VertexAttribPointerType type, VertexElementSemantic semantic)
            : this((short)semantic, typeCount, type, semantic)
        {

        }

        public VertexElement(short source, int typeCount, VertexAttribPointerType type, VertexElementSemantic semantic)
        {
            m_source = source;
            m_typeCount = typeCount;
            m_type = type;
            m_semantic = semantic;
        }

        public VertexElement(short source, int offset, int typeCount, VertexAttribPointerType type, VertexElementSemantic semantic)
        {
            m_source = source;
            m_offset = offset;
            m_typeCount = typeCount;
            m_type = type;
            m_semantic = semantic;
        }

        public VertexAttribPointerType Type { get { return m_type; } }
        public int TypeCount { get { return m_typeCount; } }

        public VertexElementSemantic Semantic { get { return m_semantic; } }

        public int Offset { get { return m_offset; } }

        public int Size { get { return GetTypeSize(m_typeCount, m_type); } }

        public short Source { get { return m_source; } }

        public static int GetTypeSize(int typeCount, VertexAttribPointerType type)
        {
            switch (type)
            {
                case VertexAttribPointerType.UnsignedByte:
                case VertexAttribPointerType.Byte: return typeCount * sizeof(byte);
                case VertexAttribPointerType.UnsignedInt:
                case VertexAttribPointerType.Int: return typeCount * sizeof(int);
                case VertexAttribPointerType.UnsignedShort:
                case VertexAttribPointerType.Short: return typeCount * sizeof(short);
                case VertexAttribPointerType.Float: return typeCount * sizeof(float);
                case VertexAttribPointerType.Double: return typeCount * sizeof(double);
            }

            throw new NotSupportedException();
        }

        //public static VertexElementType MultiplyTypeCount(VertexElementType type, int count)
        //{
        //    switch (type)
        //    {
        //        case VertexElementType.Float1:
        //            switch (count)
        //            {
        //                case 1:
        //                    return VertexElementType.Float1;
        //                case 2:
        //                    return VertexElementType.Float2;
        //                case 3:
        //                    return VertexElementType.Float3;
        //                case 4:
        //                    return VertexElementType.Float4;
        //            }
        //            break;

        //        case VertexElementType.Short1:
        //            switch (count)
        //            {
        //                case 1:
        //                    return VertexElementType.Short1;
        //                case 2:
        //                    return VertexElementType.Short2;
        //                case 3:
        //                    return VertexElementType.Short3;
        //                case 4:
        //                    return VertexElementType.Short4;
        //            }
        //            break;
        //    }

        //    throw new NotSupportedException("Cannot multiply base vertex element type: " + type.ToString());
        //}
        #endregion
    }

    public class VertexDeclaration : IEnumerable<VertexElement>
    {
        protected int m_stride;
        protected List<VertexElement> m_elements;

        public VertexDeclaration()
        {
            m_elements = new List<VertexElement>(4);
        }

        public VertexDeclaration(VertexElement[] elements)
        {
            m_elements = new List<VertexElement>(elements.Length);
            var offset = 0;
            for (int i = 0; i < elements.Length; i++)
            {
                m_elements.Add(new VertexElement(elements[i].Source, offset, elements[i].TypeCount, elements[i].Type, elements[i].Semantic));
                offset += elements[i].Size;
            }
            m_stride = offset;
        }

        public VertexElement AddElement(int typeCount, VertexAttribPointerType type, VertexElementSemantic semantic)
        {
            var offset = 0;
            if (m_elements.Count > 0)
                offset = m_elements[m_elements.Count - 1].Offset + m_elements[m_elements.Count - 1].Size;

            var element = new VertexElement((short)semantic, offset, typeCount, type, semantic);
            m_elements.Add(element);
            m_stride += element.Size;
            return element;

        }

        public VertexElement AddElement(int offset, int typeCount, VertexAttribPointerType type, VertexElementSemantic semantic)
        {
            var element = new VertexElement((short)semantic, offset, typeCount, type, semantic);
            m_elements.Add(element);
            return element;

        }

        public VertexElement FindElementBySemantic(VertexElementSemantic semantic)
        {
            int size = m_elements.Count;
            for (var i = 0; i < size; ++i)
            {
                if (m_elements[i].Semantic == semantic)
                    return m_elements[i];
            }

            return null;
        }

        public void Enable()
        {
            int size = m_elements.Count;
            for (var i = 0; i < size; ++i)
            {
                var el = m_elements[i];
                GL.EnableVertexAttribArray(el.Source);
                GL.VertexAttribPointer(el.Source, el.TypeCount, el.Type, false, m_stride, el.Offset);
            }
        }

        public void Disable()
        {
            int size = m_elements.Count;
            for (var i = 0; i < size; ++i)
                GL.DisableVertexAttribArray(m_elements[i].Source);
        }

        public VertexElement this[int index]
        {
            get
            {
                return m_elements[index];
            }
        }

        public int VertexSize
        {
            get
            {
                var size = 0;
                foreach (var element in m_elements)
                    size += element.Size;
                return size;
            }
        }

        public int Size
        {
            get
            {
                return m_elements.Count;
            }
        }

        public IEnumerator<VertexElement> GetEnumerator()
        {
            return m_elements.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return m_elements.GetEnumerator();
        }
    }


    //public interface IVertex
    //{
    //    VertexDeclaration Declaration { get; }
    //    float this[int index] { get; }
    //    int Length { get; }
    //}

    //[StructLayout(LayoutKind.Sequential, Pack = 1)]
    //public struct VertexPositionColor : IVertex
    //{
    //    public static readonly VertexDeclaration VertexDeclaration;

    //    static VertexPositionColor()
    //    {
    //        VertexElement[] elements = new VertexElement[] {
    //            new VertexElement(3, VertexAttribPointerType.Float, VertexElementSemantic.Position), 
    //            new VertexElement(4, VertexAttribPointerType.Float, VertexElementSemantic.Color) };
    //        VertexDeclaration declaration = new VertexDeclaration(elements);
    //        VertexDeclaration = declaration;
    //    }

    //    public Vector3 Position;
    //    public Color4 Color;

    //    public VertexPositionColor(Vector3 position, Color4 color)
    //    {
    //        Position = position;
    //        Color = color;
    //    }

    //    public VertexDeclaration Declaration { get { return VertexDeclaration; } }
    //    public int Length { get { return 5; } }
    //    public float this[int index]
    //    {
    //        switch()
    //    }

    //}

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPositionTextureColor
    {
        public static readonly VertexDeclaration VertexDeclaration;

        static VertexPositionTextureColor()
        {
            VertexElement[] elements = new VertexElement[] {
                new VertexElement(3, VertexAttribPointerType.Float, VertexElementSemantic.Position), 
                new VertexElement(2, VertexAttribPointerType.Float, VertexElementSemantic.Texture),
                new VertexElement(4, VertexAttribPointerType.Float, VertexElementSemantic.Color)
            };
            VertexDeclaration declaration = new VertexDeclaration(elements);
            VertexDeclaration = declaration;
        }

        public Vector3 Position;
        public Color Color;
        public Vector2 Texture;

        public VertexPositionTextureColor(Vector3 position, Color color)
        {
            Position = position;
            Texture = Vector2.Zero;
            Color = color;
        }
    }

}
