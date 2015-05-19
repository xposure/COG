using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COG.GUI
{
    public sealed class GUILayoutOption
    {
        public enum Type
        {
            fixedWidth,
            fixedHeight,
            minWidth,
            maxWidth,
            minHeight,
            maxHeight,
            stretchWidth,
            stretchHeight,
            alignStart,
            alignMiddle,
            alignEnd,
            alignJustify,
            equalSize,
            spacing
        }

        internal GUILayoutOption.Type type;
        internal object value;
        public GUILayoutOption(GUILayoutOption.Type type, object value)
        {
            this.type = type;
            this.value = value;
        }
    }
}
