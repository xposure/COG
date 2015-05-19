using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using Rectangle = OpenTK.Box2;

namespace COG.GUI
{
    public abstract class GUIElement2 : Visual
    {
        private struct InsetOutset
        {
            Vector2 Outset, Inset;
        }

        protected internal void Measure(Vector2 availableSize)
        {
            if (!float.IsNaN(m_width))
                availableSize.X = m_width;

            if (!float.IsNaN(m_height))
                availableSize.Y = m_height;

            if (!float.IsPositiveInfinity(availableSize.X))
                availableSize.X -= (m_style.Border.horizontal + m_style.Padding.horizontal + m_style.Margin.horizontal);

            if (!float.IsPositiveInfinity(availableSize.Y))
                availableSize.Y -= (m_style.Border.vertical + m_style.Padding.vertical + m_style.Margin.vertical);

            MeasureOverride(availableSize);

            if (!float.IsNaN(m_width) && m_width >= 0)
                m_desiredSize.X = m_width;
            else
                m_desiredSize.X += (m_style.Border.horizontal + m_style.Padding.horizontal + m_style.Margin.horizontal);

            if (!float.IsNaN(m_height) && m_height >= 0)
                m_desiredSize.Y = m_height;
            else
                m_desiredSize.Y += (m_style.Border.vertical + m_style.Padding.vertical + m_style.Margin.vertical);
        }

        protected internal void Arrange(Rectangle area)
        {
            var x = area.X;
            var y = area.Y;
            var w = (int)DesiredSize.X;
            var h = (int)DesiredSize.Y;
            var fW = area.Width - w;
            var fH = area.Height - h;

            if (fH > 0 && m_vAlign != VerticalAlignment.Top)
            {
                if (m_vAlign == VerticalAlignment.Stretch)
                {
                    if (h < area.Height)
                        h = area.Height;
                }
                else if (m_vAlign == VerticalAlignment.Bottom)
                    y += fH;
                else if (m_vAlign == VerticalAlignment.Middle)
                    y += fH / 2;
            }

            if (fW > 0 && m_hAlign != HorizontalAlignment.Left)
            {
                if (m_hAlign == HorizontalAlignment.Stretch)
                {
                    if (w < area.Width)
                        w = area.Width;
                }
                else if (m_hAlign == HorizontalAlignment.Right)
                    x += fW;
                else if (m_hAlign == HorizontalAlignment.Middle)
                    x += fW / 2;
            }

            if (!float.IsNaN(m_offsetX))
                x += (int)m_offsetX;

            if (!float.IsNaN(m_offsetY))
                y += (int)m_offsetY;

            m_childArea = new Rectangle(x, y, w, h);
            m_borderArea = Rectangle.Deflate(m_childArea, m_style.Margin);
            m_paddingArea = Rectangle.Deflate(m_borderArea, m_style.Border);
            m_contentArea = Rectangle.Deflate(m_paddingArea, m_style.Padding);

            ArrangeOverride(m_contentArea);
        }

        protected internal virtual void Render(RenderManager rm)
        {
            DrawBackground(rm, m_borderArea);
            DrawBorders(rm, m_borderArea, m_style.Border);

            rm.DrawRect(Box2.FromRect(m_contentArea), Color4.Red);
            DrawContent(rm, m_contentArea);

            rm.DrawRect(Box2.FromRect(m_paddingArea), Color4.Green);
            rm.DrawRect(Box2.FromRect(m_borderArea), Color4.Blue);
            rm.DrawRect(Box2.FromRect(m_childArea), Color4.Gray);
        }

        //research rounded corners
        protected virtual void DrawBorders(RenderManager rm, Rectangle area, RectOffset bRect)
        {
            var inset = new Vector2(1, 1);
            var points = new Vector2[4] { 
                    new Vector2(area.X, area.Y),
                    new Vector2(area.X + area.Width, area.Y),
                    new Vector2(area.X + area.Width, area.Y + area.Height),
                    new Vector2(area.X, area.Y + area.Height)
                };


            var pointsInset = new Vector2[4];
            pointsInset[0] = (inset * new Vector2(bRect.left, bRect.top)); inset = -inset.Perpendicular;
            pointsInset[1] = (inset * new Vector2(bRect.right, bRect.top)); inset = -inset.Perpendicular;
            pointsInset[2] = (inset * new Vector2(bRect.right, bRect.bottom)); inset = -inset.Perpendicular;
            pointsInset[3] = (inset * new Vector2(bRect.left, bRect.bottom)); inset = -inset.Perpendicular;

            for (var i = 0; i < 4; i++)
            {
                var next = (i + 1) % 4;
                var tl = points[i];
                var tr = points[next];
                var bl = points[i] + pointsInset[i];
                var br = points[next] + pointsInset[next];

                rm.DrawQuad(0, null, tl, tr, bl, br, Color4.White, 0f);
            }
        }

        protected virtual void DrawBackground(RenderManager rm, Rectangle area)
        {
            rm.Draw(Box2.FromRect(area), Color4.Black);
        }

        protected abstract void DrawContent(RenderManager rm, Rectangle area);

        protected abstract void MeasureOverride(Vector2 availableSize);
        protected virtual void ArrangeOverride(Rectangle area) { }


    }
}
