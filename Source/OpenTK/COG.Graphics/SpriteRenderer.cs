//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using COG.Framework;
//using OpenTK;
//using AxisAlignedBox2 = OpenTK.Box2;
//using Rectangle = OpenTK.Box2;
//using Color = OpenTK.Graphics.Color4;
//using OpenTK.Graphics.OpenGL4;

//namespace COG.Graphics
//{
//    /// <summary>
//    /// Defines sprite visual options for mirroring.
//    /// </summary>
//    [Flags]
//    public enum SpriteEffects
//    {
//        /// <summary>
//        /// No options specified.
//        /// </summary>
//        None = 0,
//        /// <summary>
//        /// Render the sprite reversed along the X axis.
//        /// </summary>
//        FlipHorizontally = 1,
//        /// <summary>
//        /// Render the sprite reversed along the Y axis.
//        /// </summary>
//        FlipVertically = 2
//    }

//    public partial class RenderManager : DisposableObject
//    {
//        private RenderQueue[] m_renderQueues = new RenderQueue[20];
//        private Stack<int> m_renderQueueStack = new Stack<int>();
//        private Stopwatch m_renderTimer = new Stopwatch();
//        private long m_renderTimeMs = 0;
//        private long m_totalRenderTimeMs = 0;
//        private int m_currentRenderQueue = 0;
//        private Texture m_defaultTexture;

//        public long RenderTimeMs { get { return m_renderTimeMs; } }
//        public long TotalRenderTimeMs { get { return m_totalRenderTimeMs; } }
//        public int CurrentRenderQueue { get { return m_currentRenderQueue; } }

//        public RenderManager()
//        {
//            for (var i = 0; i < m_renderQueues.Length; i++)
//                m_renderQueues[i] = new RenderQueue();

//            //m_defaultTexture = new Texture2D(1, 1, Color.White);
//        }

//        public void PushRenderQueue(int queue)
//        {
//            m_renderQueueStack.Push(m_currentRenderQueue);
//            m_currentRenderQueue = queue;
//        }

//        public void PopRenderQueue()
//        {
//            m_currentRenderQueue = m_renderQueueStack.Pop();
//        }

//        //public void SetSortOrder(int queue, RenderQueueSortMode sortMode)
//        //{
//        //    m_renderQueues[queue].sortMode = sortMode;
//        //}

//        public void Draw(Texture2D material, Vector2 position, AxisAlignedBox2 sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effect, float depth)
//        {
//            m_renderQueues[m_currentRenderQueue].DrawInternal(material,
//                position,
//                scale,
//                sourceRectangle,
//                color,
//                rotation,
//                origin,
//                effect,
//                depth);
//        }

//        public void Draw(Texture2D material, Vector2 position, AxisAlignedBox2 sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effect, float depth)
//        {
//            m_renderQueues[m_currentRenderQueue].DrawInternal(material,
//                position,
//                (new Vector2(scale)),
//                sourceRectangle,
//                color,
//                rotation,
//                origin,
//                effect,
//                depth);
//        }

//        public void Draw(Texture2D material, AxisAlignedBox2 destinationRectangle, AxisAlignedBox2 sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effect, float depth)
//        {
//            m_renderQueues[m_currentRenderQueue].DrawInternal(material,
//                  (new Vector2(destinationRectangle.X0, destinationRectangle.Y0)),
//                  (new Vector2(destinationRectangle.Width, destinationRectangle.Height)),
//                  sourceRectangle,
//                  color,
//                  rotation,
//                  origin,
//                  effect,
//                  depth);
//        }

//        public void Draw(Texture2D material, Vector2 position, AxisAlignedBox2 sourceRectangle, Color color)
//        {
//            Draw(m_currentRenderQueue, material, position, sourceRectangle, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
//        }

//        public void Draw(Texture2D material, AxisAlignedBox2 destinationRectangle, AxisAlignedBox2 sourceRectangle, Color color)
//        {
//            Draw(m_currentRenderQueue, material, destinationRectangle, sourceRectangle, color, 0, Vector2.Zero, SpriteEffects.None, 0f);
//        }

//        public void Draw(Texture2D material, AxisAlignedBox2 destinationRectangle, AxisAlignedBox2 sourceRectangle, Color color, float depth)
//        {
//            Draw(m_currentRenderQueue, material, destinationRectangle, sourceRectangle, color, 0, Vector2.Zero, SpriteEffects.None, depth);
//        }

//        public void Draw(Texture2D material, Vector2 position, Color color)
//        {
//            Draw(m_currentRenderQueue, material, position, AxisAlignedBox2.Null, color);
//        }

//        public void Draw(Texture2D material, Vector2 position, float rotation, Vector2 scale, Color color)
//        {
//            Draw(m_currentRenderQueue, material, position, AxisAlignedBox2.Null, color, rotation, Vector2.One / 2f, scale, SpriteEffects.None, 0f);
//        }

//        public void Draw(Texture2D material, AxisAlignedBox2 rectangle, Color color)
//        {
//            Draw(m_currentRenderQueue, material, rectangle, AxisAlignedBox2.Null, color);
//        }

//        //public void Draw(Texture2D material, AxisAlignedBox source, AxisAlignedBox rectangle, Color color)
//        //{
//        //    Draw(currentRenderQueue, material, rectangle, source, color);
//        //}

//        public void Draw(Texture2D material, AxisAlignedBox2 rectangle, Vector2 origin, Color color)
//        {
//            Draw(m_currentRenderQueue, material, rectangle, AxisAlignedBox2.Null, color, 0, Vector2.Zero, SpriteEffects.None, 0f);
//        }

//        public void Draw(Texture2D material, AxisAlignedBox2 rectangle, Color color, float depth)
//        {
//            Draw(m_currentRenderQueue, material, rectangle, AxisAlignedBox2.Null, color, 0f, Vector2.Zero, SpriteEffects.None, depth);
//        }

//        public void Draw(Texture2D material, AxisAlignedBox2 rectangle, Color color, SpriteEffects effects, float depth)
//        {
//            Draw(m_currentRenderQueue, material, rectangle, AxisAlignedBox2.Null, color, 0f, Vector2.Zero, effects, depth);
//        }

//        public void Draw(int renderQueue, Texture2D material, Vector2 position, AxisAlignedBox2 sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effect, float depth)
//        {
//            m_renderQueues[renderQueue].DrawInternal(material,
//                position,
//                scale,
//                sourceRectangle,
//                color,
//                rotation,
//                origin,
//                effect,
//                depth);
//        }

//        public void Draw(int renderQueue, Texture2D material, Vector2 position, AxisAlignedBox2 sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effect, float depth)
//        {
//            m_renderQueues[renderQueue].DrawInternal(material,
//                position,
//                (new Vector2(scale)),
//                sourceRectangle,
//                color,
//                rotation,
//                origin,
//                effect,
//                depth);
//        }

//        public void Draw(int renderQueue, Texture2D material, AxisAlignedBox2 destinationRectangle, AxisAlignedBox2 sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effect, float depth)
//        {
//            m_renderQueues[renderQueue].DrawInternal(material,
//                  (new Vector2(destinationRectangle.X0, destinationRectangle.Y0)),
//                  (new Vector2(destinationRectangle.Width, destinationRectangle.Height)),
//                  sourceRectangle,
//                  color,
//                  rotation,
//                  origin,
//                  effect,
//                  depth);
//        }

//        public void Draw(int renderQueue, Texture2D material, Vector2 position, AxisAlignedBox2 sourceRectangle, Color color)
//        {
//            Draw(renderQueue, material, position, sourceRectangle, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
//        }

//        public void Draw(int renderQueue, Texture2D material, AxisAlignedBox2 destinationRectangle, AxisAlignedBox2 sourceRectangle, Color color)
//        {
//            Draw(renderQueue, material, destinationRectangle, sourceRectangle, color, 0, Vector2.Zero, SpriteEffects.None, 0f);
//        }

//        public void Draw(int renderQueue, Texture2D material, AxisAlignedBox2 destinationRectangle, AxisAlignedBox2 sourceRectangle, Color color, float depth)
//        {
//            Draw(renderQueue, material, destinationRectangle, sourceRectangle, color, 0, Vector2.Zero, SpriteEffects.None, depth);
//        }

//        public void Draw(int renderQueue, Texture2D material, Vector2 position, Color color)
//        {
//            Draw(renderQueue, material, position, AxisAlignedBox2.Null, color);
//        }

//        public void Draw(int renderQueue, Texture2D material, AxisAlignedBox2 rectangle, Color color)
//        {
//            Draw(renderQueue, material, rectangle, AxisAlignedBox2.Null, color);
//        }

//        public void Draw(int renderQueue, Texture2D material, AxisAlignedBox2 rectangle, Color color, float depth)
//        {
//            Draw(renderQueue, material, rectangle, AxisAlignedBox2.Null, color, 0f, Vector2.Zero, SpriteEffects.None, depth);
//        }

//        public void Draw(int renderQueue, AxisAlignedBox2 rectangle, Color color, float depth)
//        {
//            Draw(renderQueue, null, rectangle, AxisAlignedBox2.Null, color, 0f, Vector2.Zero, SpriteEffects.None, depth);
//        }

//        public void Draw(AxisAlignedBox2 rectangle, Color color)
//        {
//            Draw(m_currentRenderQueue, null, rectangle, AxisAlignedBox2.Null, color, 0f, Vector2.Zero, SpriteEffects.None, 0f);
//        }

//        public void Draw(AxisAlignedBox2 rectangle, Color color, float depth)
//        {
//            Draw(m_currentRenderQueue, null, rectangle, AxisAlignedBox2.Null, color, 0f, Vector2.Zero, SpriteEffects.None, depth);
//        }

//        public void Draw(int renderQueue, Texture2D material, AxisAlignedBox2 rectangle, Color color, SpriteEffects effects, float depth)
//        {
//            Draw(renderQueue, material, rectangle, AxisAlignedBox2.Null, color, 0f, Vector2.Zero, effects, depth);
//        }

//        //public void DrawCircle(Texture2D material, Vector2 center, float radius, int segments, Color color)
//        //{
//        //    DrawCircle(currentRenderQueue, material, center, radius, segments, color);
//        //}

//        public void DrawLine(Vector2 start, Vector2 end, Color color)
//        {
//            DrawLine(m_currentRenderQueue, null, start, end, color, 1f, 0);
//        }

//        public void DrawLine(Texture2D material, Vector2 start, Vector2 end, Color color)
//        {
//            DrawLine(m_currentRenderQueue, material, start, end, color, 1f, 0);
//        }

//        public void DrawLine(Texture2D material, Vector2 start, Vector2 end, Color color, float depth)
//        {
//            DrawLine(m_currentRenderQueue, material, start, end, color, 1f, depth);
//        }

//        public void DrawLine(Vector2 start, Vector2 end, Color color, float depth)
//        {
//            DrawLine(null, start, end, color, depth);
//        }


//        public void DrawRect(Texture2D material, AxisAlignedBox2 rect, Color color)
//        {
//            DrawRect(m_currentRenderQueue, material, rect.minVector, rect.maxVector, color, 0f);
//        }

//        public void DrawRect(Texture2D material, AxisAlignedBox2 rect, Color color, float depth)
//        {
//            DrawRect(m_currentRenderQueue, material, rect.minVector, rect.maxVector, color, depth);
//        }

//        public void DrawRect(Texture2D material, Vector2 p0, Vector2 p1, Color color)
//        {
//            DrawRect(m_currentRenderQueue, material, p0, p1, color, 0f);
//        }

//        public void DrawRect(Texture2D material, Vector2 p0, Vector2 p1, Color color, float depth)
//        {
//            DrawRect(m_currentRenderQueue, material, p0, p1, color, depth);
//        }

//        //public void DrawShape(Texture2D material, Shape shape, Color color)
//        //{
//        //    DrawShape(currentRenderQueue, material, shape, color);
//        //}

//        public void DrawCircle(int renderQueue, Texture2D material, Vector2 center, float radius, int segments, Color color)
//        {
//            var step = Utility.TwoPI / segments;

//            var lp = new Vector2(Utility.Cos(0), Utility.Sin(0)) * radius + center;
//            var p = Vector2.Zero;
//            for (var i = 1; i <= segments; i++)
//            {
//                var current = step * i;
//                p.X = Utility.Cos(current) * radius + center.X;
//                p.Y = Utility.Sin(current) * radius + center.Y;

//                DrawLine(renderQueue, material, lp, p, color);

//                lp = p;
//            }
//        }

//        public void DrawLine(int renderQueue, Texture2D material, Vector2 start, Vector2 end, Color color)
//        {
//            DrawLine(renderQueue, material, start, end, color, 1f, 0);
//        }

//        public void DrawLine(int renderQueue, Texture2D texture, Vector2 start, Vector2 end, Color color, float width, float depth, float origin = 0f)
//        {
//            var invOrigin = (1f - origin) * width;
//            origin *= width;

//            var dir = start - end;
//            var perpDir = dir.Perpendicular.ToNormalized();
//            var tl = start - (perpDir * origin);
//            var tr = end - (perpDir * origin);
//            var bl = start + (perpDir * invOrigin);
//            var br = end + (perpDir * invOrigin);

//            m_renderQueues[renderQueue].MakeQuad(texture, tl, tr, bl, br, color, depth);
//        }

//        public void DrawRect(int renderQueue, Texture2D material, AxisAlignedBox2 rect, Color color)
//        {
//            DrawRect(renderQueue, material, rect.minVector, rect.maxVector, color, 0f);
//        }

//        public void DrawRect(int renderQueue, AxisAlignedBox2 rect, Color color)
//        {
//            DrawRect(renderQueue, null, rect.minVector, rect.maxVector, color, 0f);
//        }

//        public void DrawRect(AxisAlignedBox2 rect, Color color)
//        {
//            DrawRect(m_currentRenderQueue, null, rect.minVector, rect.maxVector, color, 0f);
//        }

//        public void DrawRect(int renderQueue, Texture2D material, AxisAlignedBox2 rect, Color color, float depth)
//        {
//            DrawRect(renderQueue, material, rect.minVector, rect.maxVector, color, depth);
//        }

//        public void DrawRect(int renderQueue, Texture2D material, Vector2 p0, Vector2 p1, Color color)
//        {
//            DrawRect(renderQueue, material, p0, p1, color, 0f);
//        }

//        public void DrawRect(int renderQueue, Vector2 p0, Vector2 p1, Color color)
//        {
//            DrawRect(renderQueue, null, p0, p1, color, 0f);
//        }
//        public void DrawRect(AxisAlignedBox2 rect, Color color, float width)
//        {
//            DrawRect(m_currentRenderQueue, null, rect.minVector, rect.maxVector, color, 0f, width);
//        }

//        public void DrawRect(int renderQueue, Texture2D material, Vector2 p0, Vector2 p1, Color color, float depth, float width = 1f)
//        {
//            var points = new Vector2[4];
//            points[0] = p0.Floor();
//            points[2] = p1.Floor();
//            points[1] = new Vector2(points[2].X, points[0].Y);
//            points[3] = new Vector2(points[0].X, points[2].Y);

//            for (var i = 0; i < points.Length; i++)
//                DrawLine(renderQueue, material, points[i], points[(i + 1) % points.Length], color, width, depth);
//        }

//        public void DrawCircle(Vector2 center, float radius, int segments, Color color, float width = 1f)
//        {
//            var step = Utility.TWO_PI / segments;
//            var angle = 0f;
//            var points = new Vector2[segments];
//            var pointsInset = new Vector2[segments];
//            for (var i = 0; i < points.Length; i++)
//            {
//                var cos = Math.Cos(angle);
//                var sin = Math.Sin(angle);
//                points[i].X = cos * radius + center.X;
//                points[i].Y = sin * radius + center.Y;
//                pointsInset[i].X = cos * (radius - width) + center.X;
//                pointsInset[i].Y = sin * (radius - width) + center.Y;

//                angle += step;
//            }

//            for (var i = 0; i < segments; i++)
//            {
//                var next = (i + 1) % segments;
//                var tl = points[i];
//                var tr = points[next];
//                var bl = pointsInset[i];
//                var br = pointsInset[next];

//                m_renderQueues[m_currentRenderQueue].MakeQuad(null, tl, tr, bl, br, color, 0f);
//                //queues[renderQueue].MakeQuad(material, vtl, vtr, vbl, vbr, depth);

//                //DrawLine(currentRenderQueue, null, points[i] + center, points[(i + 1) % segments] + center, ((i % 2) == 0) ? color : Color.Red, width, 0f);
//            }
//        }

//        public void DrawQuad(int renderQueue, Texture2D texture, Vector2 tl, Vector2 tr, Vector2 bl, Vector2 br, Color color, float depth)
//        {
//            m_renderQueues[renderQueue].MakeQuad(texture, tl, tr, bl, br, color, depth);
//        }

//        //public void DrawShape(int renderQueue, Texture2D material, Shape shape, Color color)
//        //{
//        //    var points = shape.derivedVertices;
//        //    for (var i = 0; i < points.Length; i++)
//        //        DrawLine(renderQueue, material, points[i], points[(i + 1) % points.Length], color);
//        //}

//        //public void Update()
//        //{
//        //    Camera2D.updateAll();
//        //}

//        //public void drawAll()
//        //{
//        //    m_camerasToRender.Clear();
//        //    Camera2D.drawAll(this);
//        //}

//        public void Render()
//        {
//            m_renderTimeMs = 0;
//            m_renderTimer.Reset();

//            m_renderTimer.Start();
//            for (var i = 0; i < m_renderQueues.Length; i++)
//                m_renderQueues[i].Render();
//            m_renderTimer.Stop();
//            m_renderTimeMs = m_renderTimer.ElapsedMilliseconds;
//            m_totalRenderTimeMs += m_renderTimeMs;
//        }

//        //public void Flush(Camera2D camera)
//        //{
//        //    m_camerasToRender.Add(camera);
//        //    camera.draw(this);
//        //}

//        //private List<Camera2D> m_camerasToRender = new List<Camera2D>();
//        ////internal void AddCameraToQueue(Camera2D camera)
//        ////{
//        ////    m_camerasToRender.Add(camera);
//        ////}

//        public void Present()
//        {
//            //Camera2D.present(m_camerasToRender);
//            //m_camerasToRender.Clear();
//        }

//        protected override void DisposeManaged()
//        {
//            base.DisposeManaged();

//            m_defaultTexture.Dispose();
//            m_defaultTexture = null;

//            for (var i = 0; i < m_renderQueues.Length; i++)
//            {
//                m_renderQueues[i].Dispose();
//                m_renderQueues[i] = null;
//            }

//            m_renderQueues = null;
//            m_renderQueueStack.Clear();
//            m_renderQueueStack = null;
//            m_renderTimer = null;
//        }

//    }

//    public struct Quad
//    {
//        public Texture2D Texture;
//        public VertexPositionTextureColor TL, TR, BL, BR;



//        public void SetColor(Color c)
//        {
//            TL.Color = c;
//            TR.Color = c;
//            BL.Color = c;
//            BR.Color = c;
//        }

//        public void SetDepth(float d)
//        {
//            TL.Position.Z = d;
//            TR.Position.Z = d;
//            BL.Position.Z = d;
//            BR.Position.Z = d;
//        }

//        public void SetTexture(Vector2 texTL, Vector2 texBR)
//        {
//            TL.Texture = texTL;
//            TR.Texture = new Vector2(texBR.X, texTL.Y);
//            BR.Texture = texBR;
//            BL.Texture = new Vector2(texTL.X, texBR.Y);
//        }

//        public void SetPosition(Vector2 tl, Vector2 tr, Vector2 bl, Vector2 br)
//        {
//            SetPositionTL(tl);
//            SetPositionTR(tr);
//            SetPositionBR(br);
//            SetPositionBL(bl);
//        }

//        public void SetPositionTL(Vector2 p)
//        {
//            TL.Position.X = p.X;
//            TL.Position.Y = p.Y;
//        }

//        public void SetPositionTR(Vector2 p)
//        {
//            TR.Position.X = p.X;
//            TR.Position.Y = p.Y;
//        }

//        public void SetPositionBR(Vector2 p)
//        {
//            BR.Position.X = p.X;
//            BR.Position.Y = p.Y;
//        }

//        public void SetPositionBL(Vector2 p)
//        {
//            BL.Position.X = p.X;
//            BL.Position.Y = p.Y;
//        }

//    }


//    public class RenderQueue : DisposableObject
//    {
//        private const int MAX_SIZE = 2048;
//        private bool m_autoDepthBias = false;
//        private float maxDepth = 0f;
//        private float minDepth = 0f;
//        //private float depthRange = 0f;

//        private Quad[] m_quads = new Quad[MAX_SIZE];
//        private DynamicMesh m_mesh;
//        //private SpriteBatch m_batch;
//        //private GeometryBatch m_batch2;
//        private Texture m_currentTexture;
//        private int m_renderIndex = 0;

//        Vector2 _texCoordTL = new Vector2(0, 0);
//        Vector2 _texCoordBR = new Vector2(0, 0);
//        Rectangle _tempRect = new Rectangle(0, 0, 0, 0);

//        protected override void DisposeManaged()
//        {
//            base.DisposeManaged();

//            if (m_mesh)
//                m_mesh.Dispose();

//            m_mesh = null;

//            //m_batch.Dispose();
//            //m_batch2.Dispose();

//            //m_batch = null;
//            //m_batch2 = null;
//            m_quads = null;
//            m_currentTexture = null; //we don't own this
//        }


//        //public RenderQueueSortMode sortMode = RenderQueueSortMode.PreserverOrder;
//        internal RenderQueue()
//        {
//            m_mesh = new DynamicMesh(VertexPositionTextureColor.VertexDeclaration, 8192);
//            //m_batch = new SpriteBatch();
//            //m_batch2 = new GeometryBatch();
//            //batch2 = new SpriteBatch2(device);
//        }

//        private IEnumerable<Quad> activeItems
//        {
//            get
//            {
//                for (var i = 0; i < m_renderIndex; i++)
//                    yield return m_quads[i];
//            }
//        }

//        private IEnumerable<Quad> reverseActiveItems
//        {
//            get
//            {
//                for (int i = m_renderIndex - 1; i >= 0; i--)
//                    yield return m_quads[i];
//            }
//        }


//        public void Set(Texture2D texture, float x, float y, float dx, float dy, float w, float h, float sin, float cos, Color color, Vector2 texCoordTL, Vector2 texCoordBR, float Depth)
//        {
//            //Not much is left of the MonoGame spritebatch, soon this will just be a textured batch quad render without annoying rotations
//            VertexPositionTextureColor vertexTL, vertexTR, vertexBL, vertexBR;
//            // TODO, Should we be just assigning the Depth Value to Z?
//            // According to http://blogs.msdn.com/b/shawnhar/archive/2011/01/12/spritebatch-billboards-in-a-3d-world.aspx
//            // We do.
//            vertexTL.Position.X = x + dx * cos - dy * sin;
//            vertexTL.Position.Y = y + dx * sin + dy * cos;
//            vertexTL.Position.Z = Depth;
//            vertexTL.Color = color;
//            vertexTL.Texture.X = texCoordTL.X;
//            vertexTL.Texture.Y = texCoordTL.Y;

//            vertexTR.Position.X = x + (dx + w) * cos - dy * sin;
//            vertexTR.Position.Y = y + (dx + w) * sin + dy * cos;
//            vertexTR.Position.Z = Depth;
//            vertexTR.Color = color;
//            vertexTR.Texture.X = texCoordBR.X;
//            vertexTR.Texture.Y = texCoordTL.Y;

//            vertexBL.Position.X = x + dx * cos - (dy + h) * sin;
//            vertexBL.Position.Y = y + dx * sin + (dy + h) * cos;
//            vertexBL.Position.Z = Depth;
//            vertexBL.Color = color;
//            vertexBL.Texture.X = texCoordTL.X;
//            vertexBL.Texture.Y = texCoordBR.Y;

//            vertexBR.Position.X = x + (dx + w) * cos - (dy + h) * sin;
//            vertexBR.Position.Y = y + (dx + w) * sin + (dy + h) * cos;
//            vertexBR.Position.Z = Depth;
//            vertexBR.Color = color;
//            vertexBR.Texture.X = texCoordBR.X;
//            vertexBR.Texture.Y = texCoordBR.Y;

//            MakeQuad(texture, vertexTL, vertexTR, vertexBL, vertexBR, Depth);
//        }

//        public void MakeQuad(Texture2D texture, Vector2 tl, Vector2 tr, Vector2 bl, Vector2 br, Color color, float depth)
//        {
//            var c32 = (Color32)color;
//            var vtl = new VertexPositionTextureColor()
//            {
//                Color = c32,
//                Position = new Vector3(tl, depth),
//                Texture = new Vector2(0, 0)
//            };

//            var vtr = new VertexPositionTextureColor()
//            {
//                Color = c32,
//                Position = new Vector3(tr, depth),
//                Texture = new Vector2(0, 1)
//            };

//            var vbl = new VertexPositionTextureColor()
//            {
//                Color = c32,
//                Position = new Vector3(bl, depth),
//                Texture = new Vector2(1, 0)
//            };

//            var vbr = new VertexPositionTextureColor()
//            {
//                Color = c32,
//                Position = new Vector3(br, depth),
//                Texture = new Vector2(1, 1)
//            };

//            MakeQuad(texture, vtl, vtr, vbl, vbr, depth);

//        }

//        public void MakeQuad(Texture2D texture, VertexPositionTextureColor tl, VertexPositionTextureColor tr, VertexPositionTextureColor bl, VertexPositionTextureColor br, float depth)
//        {
//            var quad = m_quads[m_renderIndex];
//            quad.TL = tl;
//            quad.TR = tr;
//            quad.BR = br;
//            quad.BL = bl;
//            quad.SetDepth(depth);
//            quad.Texture = texture ?? Root.instance.assets.getTexture("engine:color.ffffffff");
//            //switch (sortMode)
//            //{
//            //    case RenderQueueSortMode.Texture2D:
//            //        quad.key = texture.TextureID;
//            //        break;
//            //    case RenderQueueSortMode.ReverseOrder:
//            //    case RenderQueueSortMode.PreserverOrder:
//            //    default:
//            //        quad.key = renderableIndex;
//            //        break;
//            //}

//            m_quads[m_renderIndex++] = quad;

//            //if (m_quads.Length == m_renderIndex)
//            //{
//            //    //Array.Resize(ref renderableItems, renderableItems.Length * 3 / 2);
//            //    Array.Resize(ref m_quads, m_quads.Length * 3 / 2);
//            //    //var tempItems = new DrawItem[(renderableItems.Length * 3) / 2];
//            //    //Array.Copy(renderableItems, tempItems, renderableItems.Length);
//            //    //renderableItems = tempItems;
//            //}



//        }

//        public void MakeQuad(
//         Texture2D material,
//         Vector4 destinationRectangle,
//         Rectangle? sourceRectangle,
//         Color color,
//         float rotation,
//         Vector2 origin,
//         SpriteEffects effect,
//         float depth
//         )
//        {
//            if (sourceRectangle.HasValue)
//            {
//                _tempRect = sourceRectangle.Value;
//            }
//            else
//            {
//                _tempRect.X = 0;
//                _tempRect.Y = 0;
//                _tempRect.Width = material.Width;
//                _tempRect.Height = material.Height;
//            }

//            _texCoordTL.X = _tempRect.X / (float)material.Width;
//            _texCoordTL.Y = _tempRect.Y / (float)material.Height;
//            _texCoordBR.X = (_tempRect.X + _tempRect.Width) / (float)material.Width;
//            _texCoordBR.Y = (_tempRect.Y + _tempRect.Height) / (float)material.Height;

//            if ((effect & SpriteEffects.FlipVertically) != 0)
//            {
//                var temp = _texCoordBR.Y;
//                _texCoordBR.Y = _texCoordTL.Y;
//                _texCoordTL.Y = temp;
//            }
//            if ((effect & SpriteEffects.FlipHorizontally) != 0)
//            {
//                var temp = _texCoordBR.X;
//                _texCoordBR.X = _texCoordTL.X;
//                _texCoordTL.X = temp;
//            }

//            Set(material, destinationRectangle.X,
//                    destinationRectangle.Y,
//                    -origin.X,
//                    -origin.Y,
//                    destinationRectangle.Z,
//                    destinationRectangle.W,
//                    (float)Math.Sin(rotation),
//                    (float)Math.Cos(rotation),
//                    color,
//                    _texCoordTL,
//                    _texCoordBR, depth);

//        }

//        internal void DrawInternal(Texture2D material,
//                Vector2 position,
//                Vector2 scale,
//                AxisAlignedBox2 sourceRectangle,
//                Color color,
//                float rotation,
//                Vector2 origin,
//                SpriteEffects effect,
//                float depth)
//        {

//            minDepth = Math.Min(depth, minDepth);
//            maxDepth = Math.Max(depth, maxDepth);

//            material = material ?? Root.instance.assets.getTexture("engine:color.ffffffff");

//            Rectangle srcRectangle2;
//            if (sourceRectangle.IsNull)
//                srcRectangle2 = new Rectangle(0, 0, material.Width, material.Height);
//            else
//                srcRectangle2 = sourceRectangle.ToRect();

//            var origin2 = (new Vector2(srcRectangle2.Width, srcRectangle2.Height) * origin);// +new Vector2(srcRectangle.Value.X, srcRectangle.Value.Y);

//            var p = position;
//            var s = scale;

//            origin2 = origin * s;
//            //p.X = Math.Round(item.position.X);
//            //p.Y = Math.Round(item.position.Y);
//            //s.X = Math.Round(item.scale.X);
//            //s.Y = Math.Round(item.scale.Y);

//            MakeQuad(material, new Vector4(p.X, p.Y, s.X, s.Y), srcRectangle2,
//                color, rotation, origin2, effect, depth);

//            //var item = renderableItems[renderableIndex];
//            ////var item = new DrawItem();
//            //item.material = material ?? Root.instance.assets.getTexture("engine:color.ffffffff");
//            ////item.texture = texture ?? Root.instance.resources.basewhite;
//            //item.position = position;
//            //item.scale = scale;
//            //item.sourceRectangle = sourceRectangle;
//            //item.color = color;
//            //item.rotation = rotation;
//            //item.origin = origin;
//            //item.effect = effect;
//            //item.depth = depth;



//            //                item.applyScissor = Root.instance.graphics.scissorEnabled;
//            //                item.scissorRect = Root.instance.graphics.scissorRect;



//            // renderableItems[renderableIndex++] = item;


//        }

//        internal void Render()
//        {

//            //currentMatrix = m;
//            //depthRange = maxDepth - minDepth;
//            renderItem(0, m_renderIndex);

//            //var items = new DrawItem[renderableIndex];
//            //for (var i = 0; i < renderableIndex; i++)
//            //    items[i] = renderableItems[i];

//            //switch (sortMode)
//            //{
//            //    //case RenderQueueSortMode.Texture2D:
//            //    //    Utility.RadixSort(m_quads, renderableIndex);
//            //    //    renderItem(0, renderableIndex);
//            //    //    //renderItem(activeItems);
//            //    //    //foreach (var group in activeItems.byMaterial())
//            //    //    //    renderItem(group.byTexture());
//            //    //    break;
//            //    case RenderQueueSortMode.PreserverOrder:
//            //        renderItem(0, renderableIndex);
//            //        break;
//            //    //case RenderQueueSortMode.ReverseOrder:
//            //    //    renderItem(reverseActiveItems);
//            //    //    break;
//            //    //default:
//            //    //    throw new Exception("not implmented");
//            //    //case RenderQueueSortMode.Depth:
//            //    //    foreach (var groupz in activeItems.GroupBy(x => x.depth).OrderBy(x => x.Key))
//            //    //        foreach (var group in groupz.GroupBy(x => x.texture))
//            //    //            renderItem(group);
//            //    //    break;
//            //    //case RenderQueueSortMode.PreserverOrder:
//            //    //    var item = new DrawItem[1];
//            //    //    for (var i = 0; i < renderableIndex; i++)
//            //    //    {
//            //    //        item[0] = items[i];
//            //    //        renderItem(item[0].material, item, depthRange, m);
//            //    //    }
//            //    //    break;

//            //    //case RenderQueueSortMode.Y:

//            //    //    foreach (var groupy in items.GroupBy(x => x.position.Y).OrderBy(x => x.Key))
//            //    //        foreach (var group in groupy.GroupBy(x => x.material))
//            //    //            renderItem(group.Key, group.OrderBy(x => x.texture.id), depthRange, m);
//            //    //    break;

//            //    //case RenderQueueSortMode.YThenDepth:
//            //    //    foreach (var groupy in items.GroupBy(x => x.position.Y).OrderBy(x => x.Key))
//            //    //        foreach (var groupz in groupy.GroupBy(x => x.depth).OrderBy(x => x.Key))
//            //    //            foreach (var group in groupz.GroupBy(x => x.material))
//            //    //                renderItem(group.Key, group.OrderBy(x => x.texture.id), depthRange, m);
//            //    //    break;

//            //    //case RenderQueueSortMode.Depth:
//            //    //    foreach (var groupz in items.GroupBy(x => x.depth).OrderBy(x => x.Key))
//            //    //        foreach (var group in groupz.GroupBy(x => x.material))
//            //    //            renderItem(group.Key, group.OrderBy(x => x.texture.id), depthRange, m);
//            //    //    break;

//            //    //case RenderQueueSortMode.DepthThenY:
//            //    //    foreach (var groupz in items.GroupBy(x => x.depth).OrderBy(x => x.Key))
//            //    //        foreach (var groupy in groupz.GroupBy(x => x.position.Y).OrderBy(x => x.Key))
//            //    //            foreach (var group in groupy.GroupBy(x => x.material))
//            //    //                renderItem(group.Key, group.OrderBy(x => x.texture.id), depthRange, m);
//            //    //    break;
//            //}



//            m_renderIndex = 0;
//            minDepth = 0f;
//            maxDepth = 0f;


//            //crap.Clear();
//        }

//        private ushort m_currentIndex = 0;
//        private void renderItem(int start, int length)
//        {
//            for (var i = start; i < start + length; i++)
//            {
//                var quad = m_quads[i];
//                {
//                    updateBatchState(quad);

//                    m_mesh.Position(quad.TL.Position);
//                    m_mesh.TextureCoord(quad.TL.Texture);
//                    m_mesh.Color(quad.TL.Color);

//                    m_mesh.Position(quad.TR.Position);
//                    m_mesh.TextureCoord(quad.TR.Texture);
//                    m_mesh.Color(quad.TR.Color);

//                    m_mesh.Position(quad.BR.Position);
//                    m_mesh.TextureCoord(quad.BR.Texture);
//                    m_mesh.Color(quad.BR.Color);

//                    m_mesh.Position(quad.BL.Position);
//                    m_mesh.TextureCoord(quad.BL.Texture);
//                    m_mesh.Color(quad.BL.Color);

//                    var tl = m_currentIndex++;
//                    var tr = m_currentIndex++;
//                    var br = m_currentIndex++;
//                    var bl = m_currentIndex++;

//                    m_mesh.Quad(tl, tr, br, bl);
//                }
//            }

//            endBatchState();
//        }

//        //private void renderItem(IEnumerable<Quad> items)
//        //{
//        //    foreach (var quad in items)
//        //    {
//        //        updateBatchState(quad);
//        //        //batch.AddQuad(quad.TL, quad.TR, quad.BL, quad.BR);
//        //        //batch.DrawQuad(quad.Texture, quad, 0f);
//        //        batch2.AddQuad(quad.TL, quad.TR, quad.BL, quad.BR);
//        //        spritesSubmittedThisBatch++;
//        //    }

//        //    endBatchState();
//        //}

//        private void endBatchState()
//        {
//            if (m_currentTexture != null)
//            {
//                m_mesh.End(BufferUsageHint.StreamDraw);
//                //m_batch.End();
//                //m_batch2.End();
//                //Console.WriteLine("..end state");
//                m_currentTexture = null;
//            }
//        }

//        private void setBatchState(Quad item)
//        {
//            endBatchState();

//            m_currentTexture = item.Texture;

//            //Console.WriteLine("..begin state");
//            m_mesh.Begin();
//           // m_batch.Begin();
//           // m_batch2.Begin();
//           // m_batch2.SetTexture(item.Texture);
//        }

//        private void updateBatchState(Quad item)
//        {
//            if (m_currentTexture == null)
//                setBatchState(item);
//            else if (m_currentTexture.TextureID != item.Texture.TextureID)
//                setBatchState(item);
//            //else if (item.applyScissor != applyingScissor)
//            //    setBatchState(item);
//            //else if (item.applyScissor && applyingScissor && item.scissorRect != batch.GraphicsDevice.ScissorRectangle)
//            //    setBatchState(item);
//            else if ((m_renderIndex % MAX_SIZE) == 0)
//                setBatchState(item);

//        }
//    }

//}




