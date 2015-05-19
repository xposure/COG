using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COG.GUI
{
    public enum VerticalAlignment
    {
        Top, Middle, Bottom, Stretch
    }

    public enum HorizontalAlignment
    {
        Left, Middle, Right, Stretch
    }

    public enum GUIOptionType
    {
        Width,
        Height,
        OffsetX,
        OffsetY,
        VerticalAlign,
        HorizontalAlign
    }

    public enum StackOrientation { Vertical, Horizontal }

}
