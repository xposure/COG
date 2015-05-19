using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COG.GUI
{
    internal sealed class LayoutCache
    {
        internal GUILayoutGroup topLevel = new GUILayoutGroup();
        internal Stack<GUILayoutGroup> layoutGroups = new Stack<GUILayoutGroup>();
        internal GUILayoutGroup windows = new GUILayoutGroup();
        internal LayoutCache()
        {
            this.layoutGroups.Push(this.topLevel);
        }
        internal LayoutCache(LayoutCache other)
        {
            this.topLevel = other.topLevel;
            this.layoutGroups = other.layoutGroups;
            this.windows = other.windows;
        }
    }
}
