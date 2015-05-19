using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rectangle = OpenTK.Box2;

namespace COG.GUI
{
    public abstract class GUIPanel : GUIElement2
    {
        protected List<GUIElement2> m_children;

        public bool HasChildren { get { return m_children != null; } }

        public void Clear()
        {
            if (m_children != null)
                m_children.Clear();
        }

        public virtual void AddChild(GUIElement2 element)
        {
            if (m_children == null)
                m_children = new List<GUIElement2>();

            m_children.Add(element);
        }

        protected override void DrawContent(RenderManager rm, Rectangle area)
        {
            if (HasChildren)
            {
                foreach (var child in m_children)
                    child.Render(rm);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendFormat("{{ desiredSize: {0}", m_desiredSize);
            if (m_children != null)
            {
                sb.Append(", children: {");
                for (var i = 0; i < m_children.Count; i++)
                    sb.AppendFormat("[{0}]: {1}{2}", i, m_children[i].ToString(), (i < m_children.Count - 1) ? "," : "");
                sb.Append(" } }");
            }
            else
            {
                sb.Append(" }");
            }
            
            return sb.ToString();
        }
    }

}
