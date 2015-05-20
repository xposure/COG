using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using COG.Framework;
using COG.Math;
using OpenTK.Graphics.OpenGL4;
//using OpenTK;
//using Rectangle = OpenTK.Box2;
//using Color = OpenTK.Graphics.Color4;
//using OpenTK.Graphics.OpenGL4;

namespace COG.Graphics
{
    /// <summary>
    /// Defines sprite visual options for mirroring.
    /// </summary>
    [Flags]
    public enum SpriteEffects
    {
        /// <summary>
        /// No options specified.
        /// </summary>
        None = 0,
        /// <summary>
        /// Render the sprite reversed along the X axis.
        /// </summary>
        FlipHorizontally = 1,
        /// <summary>
        /// Render the sprite reversed along the Y axis.
        /// </summary>
        FlipVertically = 2
    }

    //public partial class RenderManager : DisposableObject
    //{
    //    private RenderQueue[] m_renderQueues = new RenderQueue[20];
    //    private Stack<int> m_renderQueueStack = new Stack<int>();
    //    private Stopwatch m_renderTimer = new Stopwatch();
    //    private long m_renderTimeMs = 0;
    //    private long m_totalRenderTimeMs = 0;
    //    private int m_currentRenderQueue = 0;
    //    private Texture m_defaultTexture;

    //    public long RenderTimeMs { get { return m_renderTimeMs; } }
    //    public long TotalRenderTimeMs { get { return m_totalRenderTimeMs; } }
    //    public int CurrentRenderQueue { get { return m_currentRenderQueue; } }

    //    public RenderManager()
    //    {
    //        for (var i = 0; i < m_renderQueues.Length; i++)
    //            m_renderQueues[i] = new RenderQueue();

    //        //m_defaultTexture = new Texture2D(1, 1, Color.White);
    //    }

    //    public void PushRenderQueue(int queue)
    //    {
    //        m_renderQueueStack.Push(m_currentRenderQueue);
    //        m_currentRenderQueue = queue;
    //    }

    //    public void PopRenderQueue()
    //    {
    //        m_currentRenderQueue = m_renderQueueStack.Pop();
    //    }

    //    //public void SetSortOrder(int queue, RenderQueueSortMode sortMode)
    //    //{
    //    //    m_renderQueues[queue].sortMode = sortMode;
    //    //}

    //    public void Draw(Texture2D material, Vector2 position, AxisAlignedBox2 sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effect, float depth)
    //    {
    //        m_renderQueues[m_currentRenderQueue].DrawInternal(material,
    //            position,
    //            scale,
    //            sourceRectangle,
    //            color,
    //            rotation,
    //            origin,
    //            effect,
    //            depth);
    //    }

    //    public void Draw(Texture2D material, Vector2 position, AxisAlignedBox2 sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effect, float depth)
    //    {
    //        m_renderQueues[m_currentRenderQueue].DrawInternal(material,
    //            position,
    //            (new Vector2(scale)),
    //            sourceRectangle,
    //            color,
    //            rotation,
    //            origin,
    //            effect,
    //            depth);
    //    }

    //    public void Draw(Texture2D material, AxisAlignedBox2 destinationRectangle, AxisAlignedBox2 sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effect, float depth)
    //    {
    //        m_renderQueues[m_currentRenderQueue].DrawInternal(material,
    //              (new Vector2(destinationRectangle.X0, destinationRectangle.Y0)),
    //              (new Vector2(destinationRectangle.Width, destinationRectangle.Height)),
    //              sourceRectangle,
    //              color,
    //              rotation,
    //              origin,
    //              effect,
    //              depth);
    //    }

    //    public void Draw(Texture2D material, Vector2 position, AxisAlignedBox2 sourceRectangle, Color color)
    //    {
    //        Draw(m_currentRenderQueue, material, position, sourceRectangle, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
    //    }

    //    public void Draw(Texture2D material, AxisAlignedBox2 destinationRectangle, AxisAlignedBox2 sourceRectangle, Color color)
    //    {
    //        Draw(m_currentRenderQueue, material, destinationRectangle, sourceRectangle, color, 0, Vector2.Zero, SpriteEffects.None, 0f);
    //    }

    //    public void Draw(Texture2D material, AxisAlignedBox2 destinationRectangle, AxisAlignedBox2 sourceRectangle, Color color, float depth)
    //    {
    //        Draw(m_currentRenderQueue, material, destinationRectangle, sourceRectangle, color, 0, Vector2.Zero, SpriteEffects.None, depth);
    //    }

    //    public void Draw(Texture2D material, Vector2 position, Color color)
    //    {
    //        Draw(m_currentRenderQueue, material, position, AxisAlignedBox2.Null, color);
    //    }

    //    public void Draw(Texture2D material, Vector2 position, float rotation, Vector2 scale, Color color)
    //    {
    //        Draw(m_currentRenderQueue, material, position, AxisAlignedBox2.Null, color, rotation, Vector2.One / 2f, scale, SpriteEffects.None, 0f);
    //    }

    //    public void Draw(Texture2D material, AxisAlignedBox2 rectangle, Color color)
    //    {
    //        Draw(m_currentRenderQueue, material, rectangle, AxisAlignedBox2.Null, color);
    //    }

    //    //public void Draw(Texture2D material, AxisAlignedBox source, AxisAlignedBox rectangle, Color color)
    //    //{
    //    //    Draw(currentRenderQueue, material, rectangle, source, color);
    //    //}

    //    public void Draw(Texture2D material, AxisAlignedBox2 rectangle, Vector2 origin, Color color)
    //    {
    //        Draw(m_currentRenderQueue, material, rectangle, AxisAlignedBox2.Null, color, 0, Vector2.Zero, SpriteEffects.None, 0f);
    //    }

    //    public void Draw(Texture2D material, AxisAlignedBox2 rectangle, Color color, float depth)
    //    {
    //        Draw(m_currentRenderQueue, material, rectangle, AxisAlignedBox2.Null, color, 0f, Vector2.Zero, SpriteEffects.None, depth);
    //    }

    //    public void Draw(Texture2D material, AxisAlignedBox2 rectangle, Color color, SpriteEffects effects, float depth)
    //    {
    //        Draw(m_currentRenderQueue, material, rectangle, AxisAlignedBox2.Null, color, 0f, Vector2.Zero, effects, depth);
    //    }

    //    public void Draw(int renderQueue, Texture2D material, Vector2 position, AxisAlignedBox2 sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effect, float depth)
    //    {
    //        m_renderQueues[renderQueue].DrawInternal(material,
    //            position,
    //            scale,
    //            sourceRectangle,
    //            color,
    //            rotation,
    //            origin,
    //            effect,
    //            depth);
    //    }

    //    public void Draw(int renderQueue, Texture2D material, Vector2 position, AxisAlignedBox2 sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effect, float depth)
    //    {
    //        m_renderQueues[renderQueue].DrawInternal(material,
    //            position,
    //            (new Vector2(scale)),
    //            sourceRectangle,
    //            color,
    //            rotation,
    //            origin,
    //            effect,
    //            depth);
    //    }

    //    public void Draw(int renderQueue, Texture2D material, AxisAlignedBox2 destinationRectangle, AxisAlignedBox2 sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effect, float depth)
    //    {
    //        m_renderQueues[renderQueue].DrawInternal(material,
    //              (new Vector2(destinationRectangle.X0, destinationRectangle.Y0)),
    //              (new Vector2(destinationRectangle.Width, destinationRectangle.Height)),
    //              sourceRectangle,
    //              color,
    //              rotation,
    //              origin,
    //              effect,
    //              depth);
    //    }

    //    public void Draw(int renderQueue, Texture2D material, Vector2 position, AxisAlignedBox2 sourceRectangle, Color color)
    //    {
    //        Draw(renderQueue, material, position, sourceRectangle, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
    //    }

    //    public void Draw(int renderQueue, Texture2D material, AxisAlignedBox2 destinationRectangle, AxisAlignedBox2 sourceRectangle, Color color)
    //    {
    //        Draw(renderQueue, material, destinationRectangle, sourceRectangle, color, 0, Vector2.Zero, SpriteEffects.None, 0f);
    //    }

    //    public void Draw(int renderQueue, Texture2D material, AxisAlignedBox2 destinationRectangle, AxisAlignedBox2 sourceRectangle, Color color, float depth)
    //    {
    //        Draw(renderQueue, material, destinationRectangle, sourceRectangle, color, 0, Vector2.Zero, SpriteEffects.None, depth);
    //    }

    //    public void Draw(int renderQueue, Texture2D material, Vector2 position, Color color)
    //    {
    //        Draw(renderQueue, material, position, AxisAlignedBox2.Null, color);
    //    }

    //    public void Draw(int renderQueue, Texture2D material, AxisAlignedBox2 rectangle, Color color)
    //    {
    //        Draw(renderQueue, material, rectangle, AxisAlignedBox2.Null, color);
    //    }

    //    public void Draw(int renderQueue, Texture2D material, AxisAlignedBox2 rectangle, Color color, float depth)
    //    {
    //        Draw(renderQueue, material, rectangle, AxisAlignedBox2.Null, color, 0f, Vector2.Zero, SpriteEffects.None, depth);
    //    }

    //    public void Draw(int renderQueue, AxisAlignedBox2 rectangle, Color color, float depth)
    //    {
    //        Draw(renderQueue, null, rectangle, AxisAlignedBox2.Null, color, 0f, Vector2.Zero, SpriteEffects.None, depth);
    //    }

    //    public void Draw(AxisAlignedBox2 rectangle, Color color)
    //    {
    //        Draw(m_currentRenderQueue, null, rectangle, AxisAlignedBox2.Null, color, 0f, Vector2.Zero, SpriteEffects.None, 0f);
    //    }

    //    public void Draw(AxisAlignedBox2 rectangle, Color color, float depth)
    //    {
    //        Draw(m_currentRenderQueue, null, rectangle, AxisAlignedBox2.Null, color, 0f, Vector2.Zero, SpriteEffects.None, depth);
    //    }

    //    public void Draw(int renderQueue, Texture2D material, AxisAlignedBox2 rectangle, Color color, SpriteEffects effects, float depth)
    //    {
    //        Draw(renderQueue, material, rectangle, AxisAlignedBox2.Null, color, 0f, Vector2.Zero, effects, depth);
    //    }

    //    //public void DrawCircle(Texture2D material, Vector2 center, float radius, int segments, Color color)
    //    //{
    //    //    DrawCircle(currentRenderQueue, material, center, radius, segments, color);
    //    //}

    //    public void DrawLine(Vector2 start, Vector2 end, Color color)
    //    {
    //        DrawLine(m_currentRenderQueue, null, start, end, color, 1f, 0);
    //    }

    //    public void DrawLine(Texture2D material, Vector2 start, Vector2 end, Color color)
    //    {
    //        DrawLine(m_currentRenderQueue, material, start, end, color, 1f, 0);
    //    }

    //    public void DrawLine(Texture2D material, Vector2 start, Vector2 end, Color color, float depth)
    //    {
    //        DrawLine(m_currentRenderQueue, material, start, end, color, 1f, depth);
    //    }

    //    public void DrawLine(Vector2 start, Vector2 end, Color color, float depth)
    //    {
    //        DrawLine(null, start, end, color, depth);
    //    }


    //    public void DrawRect(Texture2D material, AxisAlignedBox2 rect, Color color)
    //    {
    //        DrawRect(m_currentRenderQueue, material, rect.minVector, rect.maxVector, color, 0f);
    //    }

    //    public void DrawRect(Texture2D material, AxisAlignedBox2 rect, Color color, float depth)
    //    {
    //        DrawRect(m_currentRenderQueue, material, rect.minVector, rect.maxVector, color, depth);
    //    }

    //    public void DrawRect(Texture2D material, Vector2 p0, Vector2 p1, Color color)
    //    {
    //        DrawRect(m_currentRenderQueue, material, p0, p1, color, 0f);
    //    }

    //    public void DrawRect(Texture2D material, Vector2 p0, Vector2 p1, Color color, float depth)
    //    {
    //        DrawRect(m_currentRenderQueue, material, p0, p1, color, depth);
    //    }

    //    //public void DrawShape(Texture2D material, Shape shape, Color color)
    //    //{
    //    //    DrawShape(currentRenderQueue, material, shape, color);
    //    //}

    //    public void DrawCircle(int renderQueue, Texture2D material, Vector2 center, float radius, int segments, Color color)
    //    {
    //        var step = Utility.TwoPI / segments;

    //        var lp = new Vector2(Utility.Cos(0), Utility.Sin(0)) * radius + center;
    //        var p = Vector2.Zero;
    //        for (var i = 1; i <= segments; i++)
    //        {
    //            var current = step * i;
    //            p.X = Utility.Cos(current) * radius + center.X;
    //            p.Y = Utility.Sin(current) * radius + center.Y;

    //            DrawLine(renderQueue, material, lp, p, color);

    //            lp = p;
    //        }
    //    }

    //    public void DrawLine(int renderQueue, Texture2D material, Vector2 start, Vector2 end, Color color)
    //    {
    //        DrawLine(renderQueue, material, start, end, color, 1f, 0);
    //    }

    //    public void DrawLine(int renderQueue, Texture2D texture, Vector2 start, Vector2 end, Color color, float width, float depth, float origin = 0f)
    //    {
    //        var invOrigin = (1f - origin) * width;
    //        origin *= width;

    //        var dir = start - end;
    //        var perpDir = new Vector2(dir.Y, -dir.X);
    //        perpDir.Normalize();

    //        var tl = start - (perpDir * origin);
    //        var tr = end - (perpDir * origin);
    //        var bl = start + (perpDir * invOrigin);
    //        var br = end + (perpDir * invOrigin);

    //        m_renderQueues[renderQueue].MakeQuad(texture, tl, tr, bl, br, color, depth);
    //    }

    //    public void DrawRect(int renderQueue, Texture2D material, AxisAlignedBox2 rect, Color color)
    //    {
    //        DrawRect(renderQueue, material, rect.minVector, rect.maxVector, color, 0f);
    //    }

    //    public void DrawRect(int renderQueue, AxisAlignedBox2 rect, Color color)
    //    {
    //        DrawRect(renderQueue, null, rect.minVector, rect.maxVector, color, 0f);
    //    }

    //    public void DrawRect(AxisAlignedBox2 rect, Color color)
    //    {
    //        DrawRect(m_currentRenderQueue, null, rect.minVector, rect.maxVector, color, 0f);
    //    }

    //    public void DrawRect(int renderQueue, Texture2D material, AxisAlignedBox2 rect, Color color, float depth)
    //    {
    //        DrawRect(renderQueue, material, rect.minVector, rect.maxVector, color, depth);
    //    }

    //    public void DrawRect(int renderQueue, Texture2D material, Vector2 p0, Vector2 p1, Color color)
    //    {
    //        DrawRect(renderQueue, material, p0, p1, color, 0f);
    //    }

    //    public void DrawRect(int renderQueue, Vector2 p0, Vector2 p1, Color color)
    //    {
    //        DrawRect(renderQueue, null, p0, p1, color, 0f);
    //    }
    //    public void DrawRect(AxisAlignedBox2 rect, Color color, float width)
    //    {
    //        DrawRect(m_currentRenderQueue, null, rect.minVector, rect.maxVector, color, 0f, width);
    //    }

    //    public void DrawRect(int renderQueue, Texture2D material, Vector2 p0, Vector2 p1, Color color, float depth, float width = 1f)
    //    {
    //        var points = new Vector2[4];
    //        points[0] = p0.Floor();
    //        points[2] = p1.Floor();
    //        points[1] = new Vector2(points[2].X, points[0].Y);
    //        points[3] = new Vector2(points[0].X, points[2].Y);

    //        for (var i = 0; i < points.Length; i++)
    //            DrawLine(renderQueue, material, points[i], points[(i + 1) % points.Length], color, width, depth);
    //    }

    //    public void DrawCircle(Vector2 center, float radius, int segments, Color color, float width = 1f)
    //    {
    //        var step = Utility.TWO_PI / segments;
    //        var angle = 0f;
    //        var points = new Vector2[segments];
    //        var pointsInset = new Vector2[segments];
    //        for (var i = 0; i < points.Length; i++)
    //        {
    //            var cos = Math.Cos(angle);
    //            var sin = Math.Sin(angle);
    //            points[i].X = cos * radius + center.X;
    //            points[i].Y = sin * radius + center.Y;
    //            pointsInset[i].X = cos * (radius - width) + center.X;
    //            pointsInset[i].Y = sin * (radius - width) + center.Y;

    //            angle += step;
    //        }

    //        for (var i = 0; i < segments; i++)
    //        {
    //            var next = (i + 1) % segments;
    //            var tl = points[i];
    //            var tr = points[next];
    //            var bl = pointsInset[i];
    //            var br = pointsInset[next];

    //            m_renderQueues[m_currentRenderQueue].MakeQuad(null, tl, tr, bl, br, color, 0f);
    //            //queues[renderQueue].MakeQuad(material, vtl, vtr, vbl, vbr, depth);

    //            //DrawLine(currentRenderQueue, null, points[i] + center, points[(i + 1) % segments] + center, ((i % 2) == 0) ? color : Color.Red, width, 0f);
    //        }
    //    }

    //    public void DrawQuad(int renderQueue, Texture2D texture, Vector2 tl, Vector2 tr, Vector2 bl, Vector2 br, Color color, float depth)
    //    {
    //        m_renderQueues[renderQueue].MakeQuad(texture, tl, tr, bl, br, color, depth);
    //    }

    //    //public void DrawShape(int renderQueue, Texture2D material, Shape shape, Color color)
    //    //{
    //    //    var points = shape.derivedVertices;
    //    //    for (var i = 0; i < points.Length; i++)
    //    //        DrawLine(renderQueue, material, points[i], points[(i + 1) % points.Length], color);
    //    //}

    //    //public void Update()
    //    //{
    //    //    Camera2D.updateAll();
    //    //}

    //    //public void drawAll()
    //    //{
    //    //    m_camerasToRender.Clear();
    //    //    Camera2D.drawAll(this);
    //    //}

    //    public void Render()
    //    {
    //        m_renderTimeMs = 0;
    //        m_renderTimer.Reset();

    //        m_renderTimer.Start();
    //        for (var i = 0; i < m_renderQueues.Length; i++)
    //            m_renderQueues[i].Render();
    //        m_renderTimer.Stop();
    //        m_renderTimeMs = m_renderTimer.ElapsedMilliseconds;
    //        m_totalRenderTimeMs += m_renderTimeMs;
    //    }

    //    //public void Flush(Camera2D camera)
    //    //{
    //    //    m_camerasToRender.Add(camera);
    //    //    camera.draw(this);
    //    //}

    //    //private List<Camera2D> m_camerasToRender = new List<Camera2D>();
    //    ////internal void AddCameraToQueue(Camera2D camera)
    //    ////{
    //    ////    m_camerasToRender.Add(camera);
    //    ////}

    //    public void Present()
    //    {
    //        //Camera2D.present(m_camerasToRender);
    //        //m_camerasToRender.Clear();
    //    }

    //    protected override void DisposeManaged()
    //    {
    //        base.DisposeManaged();

    //        m_defaultTexture.Dispose();
    //        m_defaultTexture = null;

    //        for (var i = 0; i < m_renderQueues.Length; i++)
    //        {
    //            m_renderQueues[i].Dispose();
    //            m_renderQueues[i] = null;
    //        }

    //        m_renderQueues = null;
    //        m_renderQueueStack.Clear();
    //        m_renderQueueStack = null;
    //        m_renderTimer = null;
    //    }

    //}

    public struct Quad
    {
        public Texture2D Texture;
        public VertexPositionTextureColor TL, TR, BL, BR;

        public void SetColor(Color c)
        {
            TL.Color = c;
            TR.Color = c;
            BL.Color = c;
            BR.Color = c;
        }

        public void SetDepth(float d)
        {
            TL.Position.Z = d;
            TR.Position.Z = d;
            BL.Position.Z = d;
            BR.Position.Z = d;
        }

        public void SetTexture(Vector2 texTL, Vector2 texBR)
        {
            TL.Texture = texTL;
            TR.Texture = new Vector2(texBR.X, texTL.Y);
            BR.Texture = texBR;
            BL.Texture = new Vector2(texTL.X, texBR.Y);
        }

        //public void SetPosition(Vector2 tl, Vector2 tr, Vector2 bl, Vector2 br)
        //{
        //    SetPositionTL(tl);
        //    SetPositionTR(tr);
        //    SetPositionBR(br);
        //    SetPositionBL(bl);
        //}

        //public void SetPositionTL(Vector2 p)
        //{
        //    TL.Position.X = p.X;
        //    TL.Position.Y = p.Y;
        //}

        //public void SetPositionTR(Vector2 p)
        //{
        //    TR.Position.X = p.X;
        //    TR.Position.Y = p.Y;
        //}

        //public void SetPositionBR(Vector2 p)
        //{
        //    BR.Position.X = p.X;
        //    BR.Position.Y = p.Y;
        //}

        //public void SetPositionBL(Vector2 p)
        //{
        //    BL.Position.X = p.X;
        //    BL.Position.Y = p.Y;
        //}
        public void RotateFast(float rotation, Vector2 origin)
        {
            var x = TL.Position.X;
            var y = TL.Position.Y;
            var h = TL.Position.Y - BL.Position.Y;
            var w = TR.Position.X - TL.Position.X;

            var dx = -origin.X * w;
            var dy = -origin.Y * h;
            var cos = Utility.Cos(rotation);
            var sin = Utility.Sin(rotation);

            TL.Position.X = x + dx * cos - dy * sin;
            TL.Position.Y = y + dx * sin + dy * cos;

            TR.Position.X = x + (dx + w) * cos - dy * sin;
            TR.Position.Y = y + (dx + w) * sin + dy * cos;

            BL.Position.X = x + dx * cos - (dy + h) * sin;
            BL.Position.Y = y + dx * sin + (dy + h) * cos;

            BR.Position.X = x + (dx + w) * cos - (dy + h) * sin;
            BR.Position.Y = y + (dx + w) * sin + (dy + h) * cos;
        }

        public void FLipVertically()
        {
            var temp = TL.Texture.Y;
            TL.Texture.Y = BL.Texture.Y;
            BL.Texture.Y = temp;

            temp = TR.Texture.Y;
            TR.Texture.Y = BR.Texture.Y;
            BR.Texture.Y = temp;
        }

        public void FlipHorizontally()
        {
            var temp = TL.Texture.X;
            TL.Texture.X = BL.Texture.X;
            BL.Texture.X = temp;

            temp = TR.Texture.X;
            TR.Texture.X = BR.Texture.X;
            BR.Texture.X = temp;
        }

        public static Quad Create(Texture2D texture, float x0, float y0, float x1, float y1)
        {
            var quad = new Quad();

            quad.TL.Position.X = x0;
            quad.TL.Position.Y = y0;

            quad.TR.Position.X = x1;
            quad.TR.Position.Y = y0;
            quad.TR.Texture.X = 1;

            quad.BR.Position.X = x1;
            quad.BR.Position.Y = y1;
            quad.BR.Texture = new Vector2(1, 1);

            quad.BL.Position.X = x0;
            quad.BL.Position.Y = y1;
            quad.BL.Texture.Y = 1;

            quad.Texture = texture;

            return quad;
        }


    }


    public class SpriteRenderer : DisposableObject
    {
        private ushort m_currentIndex = 0;
        private Quad[] m_quads = new Quad[2048];
        private StreamMesh m_mesh;
        private Texture m_currentTexture;
        private int m_renderIndex = 0;

        //Vector2 _texCoordTL = new Vector2(0, 0);
        //Vector2 _texCoordBR = new Vector2(0, 0);
        //Rectangle _tempRect = new Rectangle(0, 0, 0, 0);

        protected override void DisposeManaged()
        {
            base.DisposeManaged();

            if (m_mesh)
                m_mesh.Dispose();

            m_mesh = null;

            m_quads = null;
            m_currentTexture = null; //we don't own this
        }


        public SpriteRenderer()
        {
            m_mesh = new StreamMesh(VertexPositionTextureColor.VertexDeclaration, 8192);
        }

        public void AddQuad(Quad quad)
        {
            if (m_renderIndex + 1 >= m_quads.Length)
                Array.Resize(ref m_quads, m_quads.Length * 3 / 2);

            m_quads[m_renderIndex++] = quad;
        }

        public void AddQuads(Quad[] quads)
        {
            while (m_renderIndex + quads.Length >= m_quads.Length)
                Array.Resize(ref m_quads, m_quads.Length * 3 / 2);

            for (var i = 0; i < quads.Length; ++i)
                m_quads[m_renderIndex++] = quads[i];
        }
        //private void AddRotatedQuad(Texture2D texture, float x, float y, float dx, float dy, float w, float h, float sin, float cos, Color color, Vector2 texCoordTL, Vector2 texCoordBR, float depth)
        //{
        //    VertexPositionTextureColor vertexTL, vertexTR, vertexBL, vertexBR;
        //    vertexTL.Position.X = x + dx * cos - dy * sin;
        //    vertexTL.Position.Y = y + dx * sin + dy * cos;
        //    vertexTL.Position.Z = depth;
        //    vertexTL.Color = color;
        //    vertexTL.Texture.X = texCoordTL.X;
        //    vertexTL.Texture.Y = texCoordTL.Y;

        //    vertexTR.Position.X = x + (dx + w) * cos - dy * sin;
        //    vertexTR.Position.Y = y + (dx + w) * sin + dy * cos;
        //    vertexTR.Position.Z = depth;
        //    vertexTR.Color = color;
        //    vertexTR.Texture.X = texCoordBR.X;
        //    vertexTR.Texture.Y = texCoordTL.Y;

        //    vertexBL.Position.X = x + dx * cos - (dy + h) * sin;
        //    vertexBL.Position.Y = y + dx * sin + (dy + h) * cos;
        //    vertexBL.Position.Z = depth;
        //    vertexBL.Color = color;
        //    vertexBL.Texture.X = texCoordTL.X;
        //    vertexBL.Texture.Y = texCoordBR.Y;

        //    vertexBR.Position.X = x + (dx + w) * cos - (dy + h) * sin;
        //    vertexBR.Position.Y = y + (dx + w) * sin + (dy + h) * cos;
        //    vertexBR.Position.Z = depth;
        //    vertexBR.Color = color;
        //    vertexBR.Texture.X = texCoordBR.X;
        //    vertexBR.Texture.Y = texCoordBR.Y;

        //    AddQuad(texture, vertexTL, vertexTR, vertexBL, vertexBR, depth);
        //}

        //public void MakeQuad(Texture2D texture, Vector2 tl, Vector2 tr, Vector2 bl, Vector2 br, Color color, float depth)
        //{
        //    var vtl = new VertexPositionTextureColor()
        //    {
        //        Color = color,
        //        Position = new Vector3(tl.X, tl.Y, depth),
        //        Texture = new Vector2(0, 0)
        //    };

        //    var vtr = new VertexPositionTextureColor()
        //    {
        //        Color = color,
        //        Position = new Vector3(tr.X, tr.Y, depth),
        //        Texture = new Vector2(0, 1)
        //    };

        //    var vbl = new VertexPositionTextureColor()
        //    {
        //        Color = color,
        //        Position = new Vector3(bl.X, bl.Y, depth),
        //        Texture = new Vector2(1, 0)
        //    };

        //    var vbr = new VertexPositionTextureColor()
        //    {
        //        Color = color,
        //        Position = new Vector3(br.X, br.Y, depth),
        //        Texture = new Vector2(1, 1)
        //    };

        //    AddQuad(texture, vtl, vtr, vbl, vbr, depth);

        //}

        //public void AddQuad(Texture2D texture, VertexPositionTextureColor tl, VertexPositionTextureColor tr, VertexPositionTextureColor bl, VertexPositionTextureColor br, float depth)
        //{
        //    var quad = m_quads[m_renderIndex];
        //    quad.TL = tl;
        //    quad.TR = tr;
        //    quad.BR = br;
        //    quad.BL = bl;
        //    quad.SetDepth(depth);
        //    quad.Texture = texture;// ?? Root.instance.assets.getTexture("engine:color.ffffffff");

        //    m_quads[m_renderIndex++] = quad;
        //}

        //public void AddQuad(Texture2D texture, Vector4 destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effect, float depth)
        //{
        //    if (sourceRectangle.HasValue)
        //    {
        //        _tempRect = sourceRectangle.Value;
        //    }
        //    else
        //    {
        //        _tempRect.X = 0;
        //        _tempRect.Y = 0;
        //        _tempRect.Width = texture.Width;
        //        _tempRect.Height = texture.Height;
        //    }

        //    _texCoordTL.X = _tempRect.X / (float)texture.Width;
        //    _texCoordTL.Y = _tempRect.Y / (float)texture.Height;
        //    _texCoordBR.X = (_tempRect.X + _tempRect.Width) / (float)texture.Width;
        //    _texCoordBR.Y = (_tempRect.Y + _tempRect.Height) / (float)texture.Height;

        //    if ((effect & SpriteEffects.FlipVertically) != 0)
        //    {
        //        var temp = _texCoordBR.Y;
        //        _texCoordBR.Y = _texCoordTL.Y;
        //        _texCoordTL.Y = temp;
        //    }
        //    if ((effect & SpriteEffects.FlipHorizontally) != 0)
        //    {
        //        var temp = _texCoordBR.X;
        //        _texCoordBR.X = _texCoordTL.X;
        //        _texCoordTL.X = temp;
        //    }

        //    AddRotatedQuad(texture, destinationRectangle.X,
        //            destinationRectangle.Y,
        //            -origin.X,
        //            -origin.Y,
        //            destinationRectangle.Z,
        //            destinationRectangle.W,
        //            (float)Utility.Sin(rotation),
        //            (float)Utility.Cos(rotation),
        //            color,
        //            _texCoordTL,
        //            _texCoordBR, depth);

        //}

        //internal void DrawInternal(Texture2D material,
        //        Vector2 position,
        //        Vector2 scale,
        //        AxisAlignedBox2 sourceRectangle,
        //        Color color,
        //        float rotation,
        //        Vector2 origin,
        //        SpriteEffects effect,
        //        float depth)
        //{

        //    //material = material;//?? Root.instance.assets.getTexture("engine:color.ffffffff");

        //    Rectangle srcRectangle2;
        //    if (sourceRectangle.IsNull)
        //        srcRectangle2 = new Rectangle(0, 0, material.Width, material.Height);
        //    else
        //        srcRectangle2 = sourceRectangle.ToRect();

        //    var origin2 = (new Vector2(srcRectangle2.Width, srcRectangle2.Height) * origin);// +new Vector2(srcRectangle.Value.X, srcRectangle.Value.Y);

        //    var p = position;
        //    var s = scale;

        //    origin2 = origin * s;


        //    AddQuad(material, new Vector4(p.X, p.Y, s.X, s.Y), srcRectangle2,
        //        color, rotation, origin2, effect, depth);

        //}

        public void Render()
        {
            renderItem(0, m_renderIndex);
            m_renderIndex = 0;
        }

        private void ChangeTexture(Texture2D texture)
        {
            m_mesh.Flush();
            m_currentTexture = texture;

            if (m_currentTexture)
                m_currentTexture.Bind();


        }

        private void renderItem(int start, int length)
        {
            ChangeTexture(m_quads[start].Texture);

            for (var i = start; i < start + length; i++)
            {
                var quad = m_quads[i];
                {
                    if (m_currentTexture.TextureID != quad.Texture.TextureID)
                        ChangeTexture(quad.Texture);

                    m_mesh.Position(quad.TL.Position);
                    m_mesh.TextureCoord(quad.TL.Texture);
                    m_mesh.Color(quad.TL.Color);

                    m_mesh.Position(quad.TR.Position);
                    m_mesh.TextureCoord(quad.TR.Texture);
                    m_mesh.Color(quad.TR.Color);

                    m_mesh.Position(quad.BR.Position);
                    m_mesh.TextureCoord(quad.BR.Texture);
                    m_mesh.Color(quad.BR.Color);

                    m_mesh.Position(quad.BL.Position);
                    m_mesh.TextureCoord(quad.BL.Texture);
                    m_mesh.Color(quad.BL.Color);

                    var tl = m_currentIndex++;
                    var tr = m_currentIndex++;
                    var br = m_currentIndex++;
                    var bl = m_currentIndex++;

                    m_mesh.Quad(tl, tr, br, bl);
                }
            }

            ChangeTexture(null);
        }

        //private void renderItem(IEnumerable<Quad> items)
        //{
        //    foreach (var quad in items)
        //    {
        //        updateBatchState(quad);
        //        //batch.AddQuad(quad.TL, quad.TR, quad.BL, quad.BR);
        //        //batch.DrawQuad(quad.Texture, quad, 0f);
        //        batch2.AddQuad(quad.TL, quad.TR, quad.BL, quad.BR);
        //        spritesSubmittedThisBatch++;
        //    }

        //    endBatchState();
        //}

        //private void endBatchState()
        //{
        //    if (m_currentTexture != null)
        //    {
        //        m_mesh.End(BufferUsageHint.StreamDraw);
        //        //m_batch.End();
        //        //m_batch2.End();
        //        //Console.WriteLine("..end state");
        //        m_currentTexture = null;
        //    }
        //}

        //private void setBatchState(Quad item)
        //{
        //    endBatchState();

        //    m_currentTexture = item.Texture;

        //    //Console.WriteLine("..begin state");
        //    m_mesh.Begin();
        //    // m_batch.Begin();
        //    // m_batch2.Begin();
        //    // m_batch2.SetTexture(item.Texture);
        //}

        //private void updateBatchState(Quad item)
        //{
        //    if (m_currentTexture == null)
        //        setBatchState(item);
        //    else if (m_currentTexture.TextureID != item.Texture.TextureID)
        //        setBatchState(item);
        //    //else if (item.applyScissor != applyingScissor)
        //    //    setBatchState(item);
        //    //else if (item.applyScissor && applyingScissor && item.scissorRect != batch.GraphicsDevice.ScissorRectangle)
        //    //    setBatchState(item);
        //    else if ((m_renderIndex % MAX_SIZE) == 0)
        //        setBatchState(item);

        //}
    }

}




