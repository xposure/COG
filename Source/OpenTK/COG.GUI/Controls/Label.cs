using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COG.GUI;
using OpenTK;
using OpenTK.Graphics;
using Rectangle = OpenTK.Box2;

namespace COG.GUI
{
    public class GUILabel2 : GUIElement2
    {
        private string m_text;

        public string Text { get { return m_text; } set { m_text = value; } }

        protected override void MeasureOverride(Vector2 availableSize)
        {
            m_desiredSize = m_style.font.MeasureString(m_text, availableSize);
        }

        protected override void ArrangeOverride(Rectangle area)
        {

        }

        protected override void DrawContent(RenderManager rm, Rectangle area)
        {
            rm.DrawWrappedOnWordText(0, m_style.font, new Vector2(area.X, area.Y), 1, m_text, Color4.White, 0, new Vector2(area.Width, area.Height));
        }

    }
}

public static class GUILabel2Extension
{
    public static void Label(this GUIManager2 gui, string text, params GUIOption2[] options)
    {
        DoLabel(gui, text, null, options);
    }

    public static void Label(this GUIManager2 gui, string text, GUIStyle2 style, params GUIOption2[] options)
    {
        DoLabel(gui, text, style, options);
    }

    private static GUILabel2 DoLabel(GUIManager2 gui, string text, GUIStyle2 style, GUIOption2[] options)
    {
        var label = new GUILabel2();
        label.ApplyVisual(style ?? GUIStyle2.defaultLabel, options);
        label.Text = text;
        gui.AddElement(label);

        return label;
    }
}
