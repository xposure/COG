using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COG.Graphics;
using OpenTK.Graphics;

namespace COG.GUI
{
    public class GUIStyle2State
    {
        public string texture;

        private Texture2D _material;

        //kdpublic TextureRef texture;
        public Color4 textColor = Color4.White;
        public Color4 backgroundColor = Color4.White;

        public Texture2D material
        {
            get
            {
                if (_material == null && !string.IsNullOrEmpty(texture))
                {
                    _material = Root.instance.assets.getTexture(texture);
                    //_material.SetSamplerState(SamplerState.LinearClamp);
                }

                return _material;
            }
        }

        public GUIStyle2State()
        {

        }

        public GUIStyle2State(Color4 textColor)
        {
            this.textColor = textColor;
        }
        public GUIStyle2State(string texture, Color4 textColor, Color4 backgroundColor)
        {
            this.texture = texture;
            this.textColor = textColor;
            this.backgroundColor = backgroundColor;
        }
    }
}
