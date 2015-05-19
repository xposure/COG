using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COG.GUI
{
    public struct GUIOption2
    {
        private GUIOptionType m_type;
        private object m_value;

        public GUIOptionType Type { get { return m_type; } set { m_type = value; } }
        public object Value { get { return m_value; } set { m_value = value; } }

        public GUIOption2(GUIOptionType type, object value)
        {
            m_type = type;
            m_value = value;
        }

        public void ApplyToElement(Visual element)
        {
            switch (m_type)
            {
                case GUIOptionType.Height: element.Height = Convert.ToSingle(m_value); break;
                case GUIOptionType.Width: element.Width = Convert.ToSingle(m_value); break;
                case GUIOptionType.OffsetX: element.OffsetX = Convert.ToSingle(m_value); break;
                case GUIOptionType.OffsetY: element.OffsetY = Convert.ToSingle(m_value); break;
            }
        }

        public static VerticalAlignment GetVerticalAlign(GUIOption2[] options, GUIStyle2 style)
        {
            return GetOptionByType(options, GUIOptionType.VerticalAlign, style.VerticalAlign);
        }

        public static HorizontalAlignment GetHorizontalAlign(GUIOption2[] options, GUIStyle2 style)
        {
            return GetOptionByType(options, GUIOptionType.HorizontalAlign, style.HorizontalAlign);
        }

        private static T GetOptionByType<T>(GUIOption2[] options, GUIOptionType type, T defaultValue)
        {
            if (options != null && options.Length == 1)
            {
                foreach (var opt in options)
                {
                    if (opt.m_type == type)
                        return (T)opt.m_value;
                }
            }

            return defaultValue;
        }
    }

}
