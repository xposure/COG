using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace COG.GUI
{
    public struct GUIDrawArguments
    {
        public Box2 position;
        public Box2 content;
        public bool isHover;
        public bool isActive;
        public bool on;
        public bool hasKeyboardFocus;
    }
}
