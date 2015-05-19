using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COG.GUI
{
    public class GUIManager2 //: GameSystem
    {
        //private BmFont m_defaultFont;
        private GUICanvas m_canvas = new GUICanvas();
        private Stack<GUIPanel> m_panels = new Stack<GUIPanel>();

        //public BmFont DefaultFont { get { return m_defaultFont; } set { m_defaultFont = value; } }

        internal GUIPanel TopPanel
        {
            get
            {
                if (m_panels.Count == 0)
                    return m_canvas;

                return m_panels.Peek();
            }
        }

        public void BeginLayout(GUIPanel panel)
        {
            AddElement(panel);
            m_panels.Push(panel);
        }

        public void EndLayout()
        {
            m_panels.Pop();
        }

        public void AddElement(GUIElement2 element)
        {
            TopPanel.AddChild(element);
        }

        public T DoLayout<T>(GUIStyle2 style, GUIOption2[] options)
            where T : GUIPanel, new()
        {
            var panel = new T();
            panel.ApplyVisual(style, options);
            BeginLayout(panel);
            return panel;
        }

        public void Render(RenderManager renderer, params GUIOption2[] options)
        {
            Render(renderer, null, options);
        }

        public void Render(RenderManager renderer, GUIStyle2 style, params GUIOption2[] options)
        {
            if (m_panels.Count > 0)
                throw new Exception("Missing end layout");

            m_canvas.ApplyVisual(style ?? GUIStyle2.defaultLabel, options);
            m_canvas.Measure(new Vector2(screen.width, screen.height));
            m_canvas.Arrange(new Rectangle(0, 0, (int)screen.width, (int)screen.height));
            m_canvas.Render(renderer);

            m_canvas.Clear();
        }

        public override string ToString()
        {
            return m_canvas.ToString();
        }

    }
}
