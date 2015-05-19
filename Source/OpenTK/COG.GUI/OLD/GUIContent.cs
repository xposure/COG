using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COG.Graphics;

namespace COG.GUI
{
    public struct GUIContent
    {
        public Texture2D icon;
        public string text;
        public string toolip;

        public GUIContent(string text)
        {
            this.text = text;
            this.icon = null;
            this.toolip = string.Empty;
        }

        public GUIContent(string text, Texture2D icon)
        {
            this.text = text;
            this.icon = icon;
            this.toolip = string.Empty;
        }

        public GUIContent(Texture2D icon)
        {
            this.text = string.Empty;
            this.icon = icon;
            this.toolip = string.Empty;
        }

        public GUIContent(string text, string tooltip)
        {
            this.text = text;
            this.icon = null;
            this.toolip = tooltip;
        }

        public GUIContent(Texture2D icon, string tooltip)
        {
            this.text = string.Empty;
            this.icon = icon;
            this.toolip = tooltip;
        }

        public GUIContent(string text, Texture2D icon, string tooltip)
        {
            this.text = text;
            this.icon = icon;
            this.toolip = tooltip;
        }

        public static GUIContent none { get { return new GUIContent(); } }
    }

}
