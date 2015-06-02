using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using COG.Logging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace COG.Graphics
{
    //for now i don't want the vertex logic to know about shaders
    //so i have decided to hard code their attrib locations
    public enum VertexElementSemantic : int
    {
        Position,
        TexCoord0,
        TexCoord1,
        Color
    };

    public enum VertexUniformType
    {
        Unknown,
        Bool,
        Float,
        Double,
        Byte,
        Int,
        UnsignedInt
    }

    public enum VertexUniformSemantic
    {
        Unknown,
        Vector,
        Vector2,
        Vector3,
        Vector4,
        Matrix4,
        Texture1D,
        Texture2D,
        Texture3D
    }

    public class VertexUniform
    {
        private static readonly Logger g_logger = Logger.GetLogger(typeof(VertexUniform));

        private string m_name;
        private ushort m_location;
        private int m_typeCount;
        private VertexUniformType m_type;
        private VertexUniformSemantic m_semantic;

        public VertexUniform(string name, ushort location, int typeCount, VertexUniformSemantic semantic, VertexUniformType type)
        {
            m_name = name;
            m_location = location;
            m_typeCount = typeCount;
            m_semantic = semantic;
            m_type = type;
        }

        public string Name { get { return m_name; } }
        public VertexUniformType Type { get { return m_type; } }
        public VertexUniformSemantic Semantic { get { return m_semantic; } }
        public int TypeCount { get { return m_typeCount; } }
        public int Location { get { return m_location; } }

        public static VertexUniform CreateFromProgram(string name, ushort location, ActiveUniformType uniform)
        {
            var typeCount = GetTypeCount(uniform);
            var semantic = GetSemantic(uniform);
            var type = GetType(uniform);

            if(typeCount == -1 || semantic == VertexUniformSemantic.Unknown || type == VertexUniformType.Unknown)            
            {
                g_logger.Error("Unsupported uniform {0} with type {1}.", name, uniform);
                return null;
            }

            return new VertexUniform(name, location, typeCount, semantic, type);      
        }

        public static VertexUniformSemantic GetSemantic(ActiveUniformType type)
        {
            switch (type)
            {
                case ActiveUniformType.Bool:
                case ActiveUniformType.Double:
                case ActiveUniformType.Float:
                case ActiveUniformType.Int:
                case ActiveUniformType.UnsignedInt:
                    return VertexUniformSemantic.Vector;
                case ActiveUniformType.BoolVec2:
                case ActiveUniformType.DoubleVec2:
                case ActiveUniformType.FloatVec2:
                case ActiveUniformType.IntVec2:
                case ActiveUniformType.UnsignedIntVec2:
                    return VertexUniformSemantic.Vector2;
                case ActiveUniformType.BoolVec3:
                case ActiveUniformType.DoubleVec3:
                case ActiveUniformType.FloatVec3:
                case ActiveUniformType.IntVec3:
                case ActiveUniformType.UnsignedIntVec3:
                    return VertexUniformSemantic.Vector3;
                case ActiveUniformType.BoolVec4:
                case ActiveUniformType.DoubleVec4:
                case ActiveUniformType.FloatVec4:
                case ActiveUniformType.IntVec4:
                case ActiveUniformType.UnsignedIntVec4:
                    return VertexUniformSemantic.Vector4;
                case ActiveUniformType.FloatMat4:
                    return VertexUniformSemantic.Matrix4;
                case ActiveUniformType.Sampler1D:
                    return VertexUniformSemantic.Texture1D;
                case ActiveUniformType.Sampler2D:
                    return VertexUniformSemantic.Texture2D;
                case ActiveUniformType.Sampler3D:
                    return VertexUniformSemantic.Texture3D;

            }

            return VertexUniformSemantic.Unknown;
        }

        public static VertexUniformType GetType(ActiveUniformType type)
        {
            switch (type)
            {
                case ActiveUniformType.Bool:
                case ActiveUniformType.BoolVec2:
                case ActiveUniformType.BoolVec3:
                case ActiveUniformType.BoolVec4:
                    return VertexUniformType.Bool;
                case ActiveUniformType.Double:
                case ActiveUniformType.DoubleVec2:
                case ActiveUniformType.DoubleVec3:
                case ActiveUniformType.DoubleVec4:
                    return VertexUniformType.Double;
                case ActiveUniformType.Float:
                case ActiveUniformType.FloatMat2:
                case ActiveUniformType.FloatMat2x3:
                case ActiveUniformType.FloatMat2x4:
                case ActiveUniformType.FloatMat3:
                case ActiveUniformType.FloatMat3x2:
                case ActiveUniformType.FloatMat3x4:
                case ActiveUniformType.FloatMat4:
                case ActiveUniformType.FloatMat4x2:
                case ActiveUniformType.FloatMat4x3:
                case ActiveUniformType.FloatVec2:
                case ActiveUniformType.FloatVec3:
                case ActiveUniformType.FloatVec4:
                    return VertexUniformType.Float;
                case ActiveUniformType.Sampler1D:
                case ActiveUniformType.Sampler2D:
                case ActiveUniformType.Sampler3D:
                case ActiveUniformType.Int:
                case ActiveUniformType.IntVec2:
                case ActiveUniformType.IntVec3:
                case ActiveUniformType.IntVec4:
                    return VertexUniformType.Int;
                case ActiveUniformType.UnsignedInt:
                case ActiveUniformType.UnsignedIntVec2:
                case ActiveUniformType.UnsignedIntVec3:
                case ActiveUniformType.UnsignedIntVec4:
                    return VertexUniformType.UnsignedInt;

            }

            return VertexUniformType.Unknown;
        }

        public static int GetTypeCount(ActiveUniformType type)
        {
            switch (type)
            {
                case ActiveUniformType.Sampler1D:
                case ActiveUniformType.Sampler2D:
                case ActiveUniformType.Sampler3D:
                case ActiveUniformType.Bool:
                case ActiveUniformType.Double:
                case ActiveUniformType.Float:
                case ActiveUniformType.Int:
                case ActiveUniformType.UnsignedInt:
                    return 1;
                case ActiveUniformType.BoolVec2:
                case ActiveUniformType.DoubleVec2:
                case ActiveUniformType.FloatVec2:
                case ActiveUniformType.IntVec2:
                case ActiveUniformType.UnsignedIntVec2:
                    return 2;
                case ActiveUniformType.BoolVec3:
                case ActiveUniformType.DoubleVec3:
                case ActiveUniformType.FloatVec3:
                case ActiveUniformType.IntVec3:
                case ActiveUniformType.UnsignedIntVec3:
                    return 3;
                case ActiveUniformType.BoolVec4:
                case ActiveUniformType.DoubleVec4:
                case ActiveUniformType.FloatVec4:
                case ActiveUniformType.IntVec4:
                case ActiveUniformType.UnsignedIntVec4:
                case ActiveUniformType.FloatMat2:
                    return 4;
                case ActiveUniformType.FloatMat3x2:
                case ActiveUniformType.FloatMat2x3:
                    return 6;
                case ActiveUniformType.FloatMat2x4:
                case ActiveUniformType.FloatMat4x2:
                    return 8;
                case ActiveUniformType.FloatMat3:
                    return 9;
                case ActiveUniformType.FloatMat3x4:
                case ActiveUniformType.FloatMat4x3:
                    return 12;
                case ActiveUniformType.FloatMat4:
                    return 16;
            }

            return -1;
        }

       

        //private bool InternalSet(int p0, int p1, int p2, int p3, int size)
        //{
        //    if (m_isMatrix)
        //    {
        //        g_logger.error("Uniform is a matrix not a vec1");
        //        return false;
        //    }

        //    if (m_typeCount != size)
        //    {
        //        g_logger.error("Uniform expects a vec{0} and got vec{1}.", m_typeCount, size);
        //        return false;
        //    }

        //    if (m_type != VertexUniformType.Int)
        //        g_logger.warn("Uniform is of type {0} but was set with {1}.", m_type, "int");

        //    if (size == 1)
        //        GL.Uniform1(m_location, p0);
        //    else if (size == 2)
        //        GL.Uniform2(m_location, p0, p1);
        //    else if (size == 3)
        //        GL.Uniform3(m_location, p0, p1, p2);
        //    else if (size == 4)
        //        GL.Uniform4(m_location, p0, p1, p2, p3);
        //    else
        //        return false;

        //    return true;
        //}

  
    }

    public class VertexAttribute
    {
        private static readonly Logger g_logger = Logger.GetLogger(typeof(VertexAttribute));

        //protected string m_name;
        private ushort m_location;
        private int m_typeCount;
        private VertexAttribPointerType m_type;
        private VertexElementSemantic m_semantic;

        public VertexAttribute(/*string name, */ ushort location, int typeCount, VertexAttribPointerType type, VertexElementSemantic semantic)
        {
            m_location = location;
            m_typeCount = typeCount;
            m_type = type;
            m_semantic = semantic;
        }

        //public string Name { get { return m_name; } }
        public ushort Location { get { return m_location; } }
        public int TypeCount { get { return m_typeCount; } }
        public VertexAttribPointerType Type { get { return m_type; } }
        public VertexElementSemantic Semantic { get { return m_semantic; } }

        public override string ToString()
        {
            return string.Format("VertexAttribute: {{ Location: {0}, TypeCount: {1}, Type: {2}, Semantic: {3} }}", m_location, m_typeCount, m_type, m_semantic);
        }

        public static VertexAttribute CreateFromProgram(string name, ushort location, ActiveAttribType attrib)
        {
            int typeCount;
            VertexAttribPointerType type;
            if (ConvertAttribTypeToVertexPointer(attrib, out typeCount, out type))
            {
                try
                {
                    var semantic = (VertexElementSemantic)Enum.Parse(typeof(VertexElementSemantic), name, true);
                    return new VertexAttribute(location, typeCount, type, semantic);
                }
                catch
                {
                    g_logger.Error("Semantic '{0}' is not valid.", name);
                    //throw error
                }

            }
            return null;
        }

        public static bool ConvertAttribTypeToVertexPointer(ActiveAttribType attrib, out int typeCount, out VertexAttribPointerType type)
        {
            switch (attrib)
            {
                case ActiveAttribType.Double:
                    type = VertexAttribPointerType.Double;
                    typeCount = 1;
                    return true;
                case ActiveAttribType.DoubleVec2:
                    type = VertexAttribPointerType.Double;
                    typeCount = 2;
                    return true;
                case ActiveAttribType.DoubleVec3:
                    type = VertexAttribPointerType.Double;
                    typeCount = 3;
                    return true;
                case ActiveAttribType.DoubleVec4:
                    type = VertexAttribPointerType.Double;
                    typeCount = 4;
                    return true;
                case ActiveAttribType.Float:
                    type = VertexAttribPointerType.Float;
                    typeCount = 1;
                    return true;
                case ActiveAttribType.FloatVec2:
                    type = VertexAttribPointerType.Float;
                    typeCount = 2;
                    return true;
                case ActiveAttribType.FloatVec3:
                    type = VertexAttribPointerType.Float;
                    typeCount = 3;
                    return true;
                case ActiveAttribType.FloatVec4:
                    type = VertexAttribPointerType.Float;
                    typeCount = 4;
                    return true;
                case ActiveAttribType.Int:
                    type = VertexAttribPointerType.Int;
                    typeCount = 1;
                    return true;
                case ActiveAttribType.IntVec2:
                    type = VertexAttribPointerType.Int;
                    typeCount = 2;
                    return true;
                case ActiveAttribType.IntVec3:
                    type = VertexAttribPointerType.Int;
                    typeCount = 3;
                    return true;
                case ActiveAttribType.IntVec4:
                    type = VertexAttribPointerType.Int;
                    typeCount = 4;
                    return true;
                case ActiveAttribType.UnsignedInt:
                    type = VertexAttribPointerType.UnsignedInt;
                    typeCount = 1;
                    return true;
                case ActiveAttribType.UnsignedIntVec2:
                    type = VertexAttribPointerType.UnsignedInt;
                    typeCount = 2;
                    return true;
                case ActiveAttribType.UnsignedIntVec3:
                    type = VertexAttribPointerType.UnsignedInt;
                    typeCount = 3;
                    return true;
                case ActiveAttribType.UnsignedIntVec4:
                    type = VertexAttribPointerType.UnsignedInt;
                    typeCount = 4;
                    return true;
            }

            typeCount = -1;
            type = VertexAttribPointerType.Byte;
            return false;
        }
    }

    public class VertexElement
    {
        #region Protected
        //protected short m_source;
        protected int m_offset;
        protected int m_typeCount;
        protected VertexAttribPointerType m_type;
        protected VertexElementSemantic m_semantic;
        #endregion

        #region Public
        //public VertexElement(int typeCount, VertexAttribPointerType type, VertexElementSemantic semantic)
        //    : this((short)semantic, typeCount, type, semantic)
        //{

        //}

        public VertexElement(/*short source,*/ int typeCount, VertexAttribPointerType type, VertexElementSemantic semantic)
        {
            //m_source = source;
            m_typeCount = typeCount;
            m_type = type;
            m_semantic = semantic;
        }

        public VertexElement(/*short source,*/ int offset, int typeCount, VertexAttribPointerType type, VertexElementSemantic semantic)
        {
            //m_source = source;
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

        //public short Source { get { return m_source; } }

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
        private static readonly Logger g_logger = Logger.GetLogger(typeof(VertexDeclaration));

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
                m_elements.Add(new VertexElement(/*elements[i].Source,*/ offset, elements[i].TypeCount, elements[i].Type, elements[i].Semantic));
                offset += elements[i].Size;
            }
            m_stride = offset;
        }

        public VertexElement AddElement(int typeCount, VertexAttribPointerType type, VertexElementSemantic semantic)
        {
            var offset = 0;
            if (m_elements.Count > 0)
                offset = m_elements[m_elements.Count - 1].Offset + m_elements[m_elements.Count - 1].Size;

            var element = new VertexElement(/*(short)semantic,*/ offset, typeCount, type, semantic);
            m_elements.Add(element);
            m_stride += element.Size;
            return element;

        }

        public VertexElement AddElement(int offset, int typeCount, VertexAttribPointerType type, VertexElementSemantic semantic)
        {
            var element = new VertexElement(/*(short)semantic,*/ offset, typeCount, type, semantic);
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

        public void Enable(Program program)
        {
            int size = m_elements.Count;
            for (var i = 0; i < size; ++i)
            {
                var el = m_elements[i];
                var attr = program.FindAttributeBySemantic(el.Semantic);
                if (attr == null)
                {
                    g_logger.Warn("{0} does not have semantic {1}.", program.Uri, el.Semantic);
                    continue;
                }

                GL.EnableVertexAttribArray(attr.Location); ;
                GL.VertexAttribPointer(attr.Location, el.TypeCount, el.Type, false, m_stride, el.Offset);
            }
        }

        public void Disable(Program program)
        {
            int size = m_elements.Count;
            for (var i = 0; i < size; ++i)
            {
                var el = m_elements[i];
                var attr = program.FindAttributeBySemantic(el.Semantic);
                if (attr == null)
                    continue;

                GL.DisableVertexAttribArray(attr.Location);
            }
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

    public interface IVertexDescriptor
    {
        VertexDeclaration Declaration { get; }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPositionColor
    {
        public static readonly VertexDeclaration VertexDeclaration;

        static VertexPositionColor()
        {
            VertexElement[] elements = new VertexElement[] {
                new VertexElement(3, VertexAttribPointerType.Float, VertexElementSemantic.Position), 
                new VertexElement(4, VertexAttribPointerType.Float, VertexElementSemantic.Color)
            };
            VertexDeclaration declaration = new VertexDeclaration(elements);
            VertexDeclaration = declaration;
        }

        public Vector3 Position;
        public Color Color;

        public VertexPositionColor(Vector3 position, Color color)
        {
            Position = position;
            Color = color;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPositionTextureColor : IVertexDescriptor
    {
        public static readonly VertexDeclaration VertexDeclaration;

        static VertexPositionTextureColor()
        {
            VertexElement[] elements = new VertexElement[] {
                new VertexElement(3, VertexAttribPointerType.Float, VertexElementSemantic.Position), 
                new VertexElement(2, VertexAttribPointerType.Float, VertexElementSemantic.TexCoord0),
                new VertexElement(4, VertexAttribPointerType.Float, VertexElementSemantic.Color)
            };
            VertexDeclaration declaration = new VertexDeclaration(elements);
            VertexDeclaration = declaration;
        }

        public Vector3 Position;
        public Vector2 Texture;
        public Color Color;

        public VertexPositionTextureColor(Vector3 position, Color color)
        {
            Position = position;
            Texture = Vector2.Zero;
            Color = color;
        }

        public VertexDeclaration Declaration { get { return VertexDeclaration; } }
    }

}
