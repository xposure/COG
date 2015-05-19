using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace COG.GUI
{
    public class GUIStyle2
    {
        public static readonly GUIStyle2 none = new GUIStyle2();

        public string name;
        public float FixedWidth = float.NaN;
        public float FixedHeight = float.NaN;
        public VerticalAlignment VerticalAlign = VerticalAlignment.Top;
        public HorizontalAlignment HorizontalAlign = HorizontalAlignment.Left;
        public RectOffset Border, Margin, Padding;

        //        public RectOffset overflow;

        public GUIStyle2State normal = new GUIStyle2State(Color4.White);
        public GUIStyle2State hover = new GUIStyle2State(Color4.Wheat);
        public GUIStyle2State active = new GUIStyle2State(Color4.Wheat);
        public GUIStyle2State focused = new GUIStyle2State(Color4.Wheat);
        public GUIStyle2State disabled = new GUIStyle2State(Color4.Gray);
        public GUIStyle2State onNormal = new GUIStyle2State(Color4.White);
        public GUIStyle2State onHover = new GUIStyle2State(Color4.Wheat);
        public GUIStyle2State onActive = new GUIStyle2State(Color4.Wheat);
        public GUIStyle2State onFocused = new GUIStyle2State(Color4.Wheat);
        //public GUIStyle2State disabledNormal = new GUIStyleState(Color4.Gray);
        //public GUIStyle2State disabledHover = new GUIStyleState(Color4.Gray);
        //public GUIStyle2State disabledActive = new GUIStyleState(Color4.Gray);

        public Vector2 contentOffset;
        //public string fontName;// = "content/fonts/arial.fnt";
        public float fontSize = 1;

        private BmFont m_font;
        public BmFont font
        {
            get
            {

                //                return Root.instance.gui.defaultFont;
                if (m_font == null)
                    m_font = Root.instance.assets.getFont("dodger:arial"); //HACK!
                return m_font;
            }
        }

        public static GUIStyle2 defaultLabel
        {
            get
            {
                var style = new GUIStyle2();
                style.name = "label";
                style.normal = new GUIStyle2State(Color4.FromRGBA(230, 230, 230, 255));
                style.Border = new RectOffset(4, 4, 4, 4);
                style.Margin = new RectOffset(4, 4, 4, 4);
                style.Padding = new RectOffset(4, 4, 4, 4); ;
                return style;
            }
        }

        public static GUIStyle2 defaultVerticalStack
        {
            get
            {
                var style = new GUIStyle2();
                style.name = "verticalStack";
                style.HorizontalAlign = HorizontalAlignment.Stretch;
                style.normal = new GUIStyle2State(Color4.FromRGBA(230, 230, 230, 255));
                style.Border = new RectOffset(4, 4, 4, 4);
                style.Margin = new RectOffset(4, 4, 4, 4);
                style.Padding = new RectOffset(4, 4, 4, 4);
                return style;
            }
        }

        public static GUIStyle2 defaultHorizontalStack
        {
            get
            {
                var style = new GUIStyle2();
                style.name = "horizontalStack";
                style.VerticalAlign = VerticalAlignment.Stretch;
                style.normal = new GUIStyle2State(Color4.FromRGBA(230, 230, 230, 255));
                style.Border = new RectOffset(4, 4, 4, 4);
                style.Margin = new RectOffset(4, 4, 4, 4);
                style.Padding = new RectOffset(4, 4, 4, 4); ;
                return style;
            }
        }

        public static GUIStyle2 defaultBox
        {
            get
            {
                var style = new GUIStyle2();
                style.name = "button";
                style.normal = new GUIStyle2State("content/textures/gui/box.png", Color4.FromRGBA(230, 230, 230, 255), Color4.FromRGBA(64, 64, 192, 255));
                style.Border = new RectOffset(6, 6, 6, 6);
                style.Margin = new RectOffset(4, 4, 4, 4);
                style.Padding = new RectOffset(4, 4, 4, 4);
                return style;
            }
        }

        public static GUIStyle2 defaultButton
        {
            get
            {
                var style = new GUIStyle2();
                style.name = "button";
                style.normal = new GUIStyle2State("content/textures/gui/button.png", Color4.FromRGBA(230, 230, 230, 255), Color4.FromRGBA(64, 64, 192, 255));
                style.hover = new GUIStyle2State("content/textures/gui/button-hover.png", Color4.FromRGBA(255, 255, 255, 255), Color4.FromRGBA(64, 64, 224, 255));
                style.active = new GUIStyle2State("content/textures/gui/button-on-active.png", Color4.FromRGBA(230, 230, 230, 255), Color4.FromRGBA(64, 64, 255, 255));
                style.onNormal = new GUIStyle2State("content/textures/gui/button-on.png", Color4.FromRGBA(230, 230, 230, 255), Color4.FromRGBA(64, 64, 192, 255));
                style.onHover = new GUIStyle2State("content/textures/gui/button-on-hover.png", Color4.FromRGBA(230, 230, 230, 255), Color4.FromRGBA(64, 64, 224, 255));
                style.onActive = new GUIStyle2State("content/textures/gui/button-on-active.png", Color4.FromRGBA(230, 230, 230, 255), Color4.FromRGBA(64, 64, 255, 255));
                style.Border = new RectOffset(6, 6, 6, 4);
                style.Margin = new RectOffset(4, 4, 4, 4);
                style.Padding = new RectOffset(6, 6, 3, 3);
                return style;
            }
        }

    }
}
