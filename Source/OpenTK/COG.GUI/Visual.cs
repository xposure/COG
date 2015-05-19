using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using Rectangle = OpenTK.Box2;

namespace COG.GUI
{
    public class Visual
    {
        protected GUIStyle2 m_style;
        public GUIStyle2 Style { get { return m_style; } }

        protected VerticalAlignment m_vAlign;
        protected HorizontalAlignment m_hAlign;

        protected float m_width = float.NaN;
        protected float m_height = float.NaN;
        protected float m_offsetX = float.NaN;
        protected float m_offsetY = float.NaN;
        protected Vector2 m_desiredSize;
        protected Rectangle m_childArea, m_borderArea, m_paddingArea, m_contentArea;

        public float Width { get { return m_width; } set { m_width = value; } }
        public float Height { get { return m_height; } set { m_height = value; } }
        public float OffsetX { get { return m_offsetX; } set { m_offsetX = value; } }
        public float OffsetY { get { return m_offsetY; } set { m_offsetY = value; } }
        public Vector2 DesiredSize { get { return m_desiredSize; } set { m_desiredSize = value; } }

        public bool HasVisibleArea
        {
            get { return m_contentArea.Area > 0; }
        }

        public virtual void ApplyVisual(GUIStyle2 style, GUIOption2[] options)
        {
            m_style = style;
            m_width = float.NaN;
            m_height = float.NaN;
            m_offsetX = float.NaN;
            m_offsetY = float.NaN;

            m_vAlign = GUIOption2.GetVerticalAlign(options, style);
            m_hAlign = GUIOption2.GetHorizontalAlign(options, style);

            if (options != null)
                foreach (var option in options)
                    option.ApplyToElement(this);
        }

        protected Visual()
        {

        }

        public override string ToString()
        {
            return string.Format("{{ desiredSize: {0}}}", m_desiredSize.ToString());
        }
    }
}
