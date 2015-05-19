using System;
using System.Collections.Generic;
using COG.Graphics;
using OpenTK;
using OpenTK.Graphics;
using Rectangle = OpenTK.Box2;

namespace COG.GUI
{

    internal class GUIGroupLayout2
    {
        //public 
        public void doLabel(GUIContent content, GUIStyle style, GUILayoutOption[] options)
        {

            //gui.label2(GetRect(content, style, options), content, style);
        }
    }

    public struct GUIRenderable
    {
        public GUIContent content;
        public GUIDrawArguments arguments;
    }

    public class GUIGroup
    {
        public List<GUIRenderable> items = new List<GUIRenderable>();

        private Vector2 position = Vector2.Zero;

        internal void reset()
        {
            items.Clear();
        }
    }

    public sealed partial class GUIManager
    {
        public GUISkin skin = new GUISkin();

        public readonly GUILayout layout;

        public Color4 buttonBackground = Color4.DarkBlue;
        public Color4 buttonBackgroundHover = Color4.Blue;
        public Color4 buttonBorder = Color4.Wheat;
        public Color4 buttonBorderHover = Color4.White;
        public Color4 buttonTextColor = Color4.Wheat;
        public Color4 buttonTextColorHover = Color4.White;
        public Color4 labelTextColor = Color4.White;
        public Box2 lastHitRect = Box2.Null;

        private Vector2 _normalizedViewSize = Vector2.One;
        private Vector2 buttonPadding = new Vector2(5, 5);
        private Stack<Rectangle> clips = new Stack<Rectangle>();
        private Vector2 groupOffset = Vector2.Zero;
        private Stack<Vector2> groups = new Stack<Vector2>();
        private RenderManager m_renderer;
        
        public BmFont defaultFont;


        private Camera2D m_camera;

        //private List<

        //private 

        public bool mouseUsed = false;


        public GUIManager()
        {
            layout = new GUILayout(this);
        }

        //public Vector2 normalizedViewSize
        //{
        //    get { return _normalizedViewSize; }
        //    set
        //    {
        //        _normalizedViewSize = value;
        //        viewMatrixDirty = true;
        //    }
        //}

        //public Matrix4 ViewMatrix
        //{
        //    get
        //    {
        //        if (viewMatrixDirty)
        //        {
        //            ReCreateViewMatrix();
        //        }
        //        return viewMatrix;
        //    }
        //}

        //public Viewport viewport
        //{
        //    get
        //    {
        //        if (viewMatrixDirty)
        //            ReCreateViewMatrix();
        //        return _viewport;
        //    }
        //}

        //public void beginClip(Vector2 p, Vector2 s)
        //{
        //    var r = new Rectangle((int)(p.X + groupOffset.X), (int)(p.Y + groupOffset.Y), (int)s.X, (int)s.Y);
        //    if (m_renderer.scissorEnabled)
        //        clips.Push(m_renderer.scissorRect);

        //    m_renderer.beginScissor(r);
        //}

        public void beginGroup(Vector2 p)
        {
            groups.Push(p);
            groupOffset += p;
        }

        public bool buttonold(Vector2 p, string text)
        {
            var rect = defaultFont.MeasureString(text);
            return buttonold(0, 1f, p.ToAABB(rect), defaultFont, 0f, text);
        }

        public bool buttonold(Box2 rect, string text)
        {
            return buttonold(0, 1f, rect, defaultFont, 0f, text);
        }

        public bool buttonold(int renderQueue, float scale, Box2 rect, BmFont font, float depth, string text)
        {
            rect.SetExtents(rect.minVector + groupOffset, rect.maxVector + groupOffset + buttonPadding + buttonPadding);
            var mp = Root.instance.input.MousePosition;
            var gp = screenToGUI(mp);

            var isOver = rect.Contains(gp) && Root.instance.input.HasFocus;

            var bg = isOver ? skin.button.hover.texture : skin.button.normal.texture;
            var border = isOver ? buttonBorderHover : buttonBorder;
            var color = isOver ? buttonTextColorHover : buttonTextColor;

            //m_renderer.beginScissor(rect);
            m_renderer.Draw(renderQueue, rect, buttonBackground, depth);
            m_renderer.DrawRect(renderQueue, rect, buttonBorder);
            m_renderer.DrawText(renderQueue, font, scale, rect.minVector + buttonPadding, text, color, depth);
            //m_renderer.endScissor();

            return Root.instance.rawinput.IsLeftMouseDown && isOver && Root.instance.input.HasFocus;
            return false;
        }

        //public void endClip()
        //{
        //    if (!m_renderer.scissorEnabled)
        //        throw new Exception("called end clip without the scissorrect enabled");

        //    m_renderer.endScissor();
        //    if (clips.Count > 0)
        //    {
        //        var r = clips.Pop();
        //        m_renderer.beginScissor(r);
        //    }
        //}

        public void endGroup()
        {
            var p = groups.Pop();
            groupOffset -= p;
        }

        #region label

        public void label2(Box2 p, object arg)
        {
            if (arg == null)
                throw new ArgumentNullException("arg");

            doLabel(p, new GUIContent(arg.ToString()), skin.label);
        }

        public void label2(Box2 p, object arg, GUIStyle style)
        {
            if (arg == null)
                throw new ArgumentNullException("arg");

            doLabel(p, new GUIContent(arg.ToString()), style);
        }

        public void label2(Box2 p, string text)
        {
            doLabel(p, new GUIContent(text), skin.label);
        }

        public void label2(Box2 p, string text, params object[] args)
        {
            doLabel(p, new GUIContent(string.Format(text, args)), skin.label);
        }

        public void label2(Box2 p, string text, GUIStyle style)
        {
            doLabel(p, new GUIContent(text), style);
        }

        public void label2(Box2 p, Texture2D image)
        {
            doLabel(p, new GUIContent(image), skin.label);
        }

        public void label2(Box2 p, Texture2D image, GUIStyle style)
        {
            doLabel(p, new GUIContent(image), style);
        }

        public void label2(Box2 p, GUIContent content)
        {
            doLabel(p, content, skin.label);
        }

        public void label2(Box2 p, GUIContent content, GUIStyle style)
        {
            doLabel(p, content, style);
        }

        private void doLabel(Box2 p, GUIContent content, GUIStyle style)
        {
            style.Draw(m_renderer, p, content, false, false, false, false);
        }

        #endregion

        #region box
        public void box(Box2 p, object arg)
        {
            if (arg == null)
                throw new ArgumentNullException("arg");

            doBox(p, new GUIContent(arg.ToString()), skin.box);
        }

        public void box(Box2 p)
        {
            doBox(p, GUIContent.none, skin.box);
        }

        public void box(Box2 p, GUIStyle style)
        {
            doBox(p, GUIContent.none, style);
        }

        public void box(Box2 p, object arg, GUIStyle style)
        {
            if (arg == null)
                throw new ArgumentNullException("arg");

            doBox(p, new GUIContent(arg.ToString()), style);
        }

        public void box(Box2 p, string text)
        {
            doBox(p, new GUIContent(text), skin.box);
        }

        public void box(Box2 p, string text, params object[] args)
        {
            doBox(p, new GUIContent(string.Format(text, args)), skin.box);
        }

        public void box(Box2 p, string text, GUIStyle style)
        {
            doBox(p, new GUIContent(text), style);
        }

        public void box(Box2 p, Texture2D image)
        {
            doBox(p, new GUIContent(image), skin.box);
        }

        public void box(Box2 p, Texture2D image, GUIStyle style)
        {
            doBox(p, new GUIContent(image), style);
        }

        public void box(Box2 p, GUIContent content)
        {
            doBox(p, content, skin.box);
        }

        public void box(Box2 p, GUIContent content, GUIStyle style)
        {
            doBox(p, content, style);
        }

        private void doBox(Box2 p, GUIContent content, GUIStyle style)
        {
            style.Draw(m_renderer, p, content, false, false, false, false);
        }
        #endregion

        #region button
        public bool button(Box2 p, object arg)
        {
            if (arg == null)
                throw new ArgumentNullException("arg");

            return doButton(p, new GUIContent(arg.ToString()), skin.button);
        }

        public bool button(Box2 p, object arg, GUIStyle style)
        {
            if (arg == null)
                throw new ArgumentNullException("arg");

            return doButton(p, new GUIContent(arg.ToString()), style);
        }

        public bool button(Box2 p, string text)
        {
            return doButton(p, new GUIContent(text), skin.button);
        }

        public bool button(Box2 p, string text, params object[] args)
        {
            return doButton(p, new GUIContent(string.Format(text, args)), skin.button);
        }

        public bool button(Box2 p, string text, GUIStyle style)
        {
            return doButton(p, new GUIContent(text), style);
        }

        public bool button(Box2 p, Texture2D image)
        {
            return doButton(p, new GUIContent(image), skin.button);
        }

        public bool button(Box2 p, Texture2D image, GUIStyle style)
        {
            return doButton(p, new GUIContent(image), style);
        }

        public bool button(Box2 p, GUIContent content)
        {
            return doButton(p, content, skin.button);
        }

        public bool button(Box2 p, GUIContent content, GUIStyle style)
        {
            return doButton(p, content, style);
        }

        private bool doButton(Box2 p, GUIContent content, GUIStyle style)
        {
            var mp = screenToGUI(Root.instance.input.MousePosition);
            var isHover = p.Contains(mp) && Root.instance.input.HasFocus;
            var isActive = !mouseUsed && isHover && Root.instance.rawinput.IsLeftMouseDown && Root.instance.input.HasFocus;
            var wasActive = !mouseUsed && isHover && Root.instance.rawinput.WasLeftMousePressed && Root.instance.input.HasFocus;

            if (isActive || wasActive)
                mouseUsed = true;

            style.Draw(m_renderer, p, content, isHover, isActive, false, false);

            return wasActive;
        }
        #endregion button

        public void label(Vector2 p, string text)
        {
            label(0, 1f, p, defaultFont, 0f, skin.label.normal.textColor, text);
        }

        public void label(Vector2 p, float scale, string text)
        {
            label(0, scale, p, defaultFont, 0f, skin.label.normal.textColor, text);
        }

        public void label(int renderQueue, float scale, Vector2 p, BmFont font, float depth, Color4 color, string text, params object[] args)
        {
            label(renderQueue, scale, p, font ?? defaultFont, depth, color, string.Format(text, args));
        }

        public void label(int renderQueue, float scale, Vector2 p, BmFont font, float depth, Color4 color, string text)
        {
            m_renderer.DrawText(renderQueue, font ?? defaultFont, scale, p + groupOffset, text, color, depth);
        }

        public void label(int renderQueue, float scale, Box2 rect, BmFont font, float depth, Color4 color, string text)
        {
            //m_renderer.beginScissor(rect);
            m_renderer.DrawText(renderQueue, font ?? defaultFont, scale, rect.minVector + groupOffset, text, color, depth);
            //m_renderer.endScissor();
        }

        //public GUILayout layout(Vector2 position)
        //{
        //    return layout(position, GUIAnchor.UpperLeft);
        //}

        //public GUILayout layout(Vector2 position, GUIAnchor anchor)
        //{
        //    var layout = new GUILayout();
        //    layout.position = groupOffset + position;
        //    layout.gui = this;
        //    layout.anchor = anchor;

        //    return layout;

        //}

        public Vector2 screenToGUI(Vector2 p)
        {
            return Vector2.Transform(p, Matrix4.Invert(m_camera.ViewMatrix4));
        }

        public void Init()
        {
            //defaultFont = Root.instance.resources.findFont("content/fonts/arial.fnt");
            m_camera = new Camera2D(Vector2.Zero);
            m_renderer = new RenderManager();
            //updateViewport();
            //ReCreateViewMatrix();
        }

        //internal void render()
        //{

        //    m_renderer.
        //    m_renderer.Render();

        //    Event.Invoke("ongui");

        //    m_renderer.graphicsDevice.SetRenderTarget(null);
        //    m_renderer.graphicsDevice.Viewport = viewport;
        //    m_renderer.render(ViewMatrix);

        //}

        public void Present()
        {
            m_renderer.Present();
        }

        public void Flush()
        {
            if (groups.Count != 0)
                throw new Exception("missing endGroup call");

            if (clips.Count != 0)
                throw new Exception("missing endClip call");

            m_renderer.Flush(m_camera);
            mouseUsed = false;
        }

        //private void ReCreateViewMatrix()
        //{
        //    updateViewport();
        //    viewMatrix = Matrix.CreateScale(new Vector3(/*new Vector2(viewport.Width, viewport.Height) / */normalizedViewSize, 1f));
        //    //* Matrix.CreateTranslation(new Vector3(_viewport.Width, _viewport.Height, 0) / 2f);
        //    viewMatrixDirty = false;
        //}

        //private void updateViewport()
        //{
        //    _viewport.Width = (int)(m_renderer.graphicsDevice.PresentationParameters.Bounds.Width * _normalizedViewSize.X);
        //    _viewport.Height = (int)(m_renderer.graphicsDevice.PresentationParameters.Bounds.Height * _normalizedViewSize.Y);

        //    if (target == null || target.Width < _viewport.Width || target.Height < _viewport.Height)
        //    {
        //        if (target != null)
        //            target.Dispose();

        //        var w = Helpers.NextPow(_viewport.Width);
        //        var h = Helpers.NextPow(_viewport.Height);

        //        target = new RenderTarget2D(m_renderer.graphicsDevice, w, h, false, SurfaceFormat.Color4, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
        //    }
        //}
    }
}