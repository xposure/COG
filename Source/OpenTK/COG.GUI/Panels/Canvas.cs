using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using Rectangle = OpenTK.Box2;

namespace COG.GUI
{
    public class GUICanvas : GUIPanel
    {
        protected override void MeasureOverride(Vector2 availableSize)
        {
            m_desiredSize = availableSize;
            if (HasChildren)
            {
                availableSize.X = float.PositiveInfinity;
                availableSize.Y = float.PositiveInfinity;

                foreach (var child in m_children)
                    child.Measure(availableSize);
            }
        }

        protected override void ArrangeOverride(Rectangle area)
        {
            if (HasChildren)
            {
                for (var i = 0; i < m_children.Count; i++)
                {
                    var child = m_children[i];
                    child.Arrange(area);
                }
            }
        }
    }
}
