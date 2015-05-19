using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COG.GUI;
using OpenTK;
using Rectangle = OpenTK.Box2;

namespace COG.GUI
{
    public class StackPanel : GUIPanel
    {
        internal class VerticalStackPanel : StackPanel { public VerticalStackPanel() : base(StackOrientation.Vertical) { } }

        internal class HorizontalStackPanel : StackPanel { public HorizontalStackPanel() : base(StackOrientation.Horizontal) { } }

        private StackOrientation m_orientation = StackOrientation.Vertical;

        protected StackPanel(StackOrientation orientation)
        {
            m_orientation = orientation;
        }

        protected override void MeasureOverride(Vector2 availableSize)
        {
            m_desiredSize = Vector2.Zero;

            if (HasChildren)
            {
                if (m_orientation == StackOrientation.Vertical)
                    availableSize.Y = float.PositiveInfinity;
                else
                    availableSize.X = float.PositiveInfinity;

                foreach (var child in m_children)
                {
                    child.Measure(availableSize);
                    if (m_orientation == StackOrientation.Vertical)
                    {
                        m_desiredSize.Y += child.DesiredSize.Y;
                        m_desiredSize.X = Math.Max(m_desiredSize.X, child.DesiredSize.X);
                    }
                    else
                    {
                        m_desiredSize.Y = Math.Max(m_desiredSize.Y, child.DesiredSize.Y);
                        m_desiredSize.X += child.DesiredSize.X;
                    }
                }
            }
        }

        protected override void ArrangeOverride(Rectangle area)
        {
            if (HasChildren)
            {
                var offset = 0;
                foreach (var child in m_children)
                {
                    var width = (int)(m_orientation == StackOrientation.Vertical ? area.Width : child.DesiredSize.X);
                    var height = (int)(m_orientation == StackOrientation.Horizontal ? area.Height : child.DesiredSize.Y);

                    var x = area.X;
                    var y = area.Y;

                    if (m_orientation == StackOrientation.Vertical)
                    {
                        y += offset;
                        offset += height;
                    }
                    else
                    {
                        x += offset;
                        offset += width;
                    }

                    child.Arrange(new Rectangle(x, y, width, height));
                }
            }
        }
    }
}

public static class StackPanelExtensions
{
    public static void BeginVertical(this GUIManager2 gui, params GUIOption2[] options)
    {
        gui.DoLayout<StackPanel.VerticalStackPanel>(GUIStyle2.defaultVerticalStack, options);
    }

    public static void BeginHorizontal(this GUIManager2 gui, params GUIOption2[] options)
    {
        gui.DoLayout<StackPanel.HorizontalStackPanel>(GUIStyle2.defaultHorizontalStack, options);
    }

}