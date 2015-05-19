using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace COG.GUI
{
    internal class GUILayoutGroup : GUILayoutEntry
    {
        public List<GUILayoutEntry> entries = new List<GUILayoutEntry>();
        public bool isVertical = true;
        public bool resetCoords;
        public float spacing;
        public bool sameSize = true;
        public bool isWindow;
        public int windowID = -1;
        private int cursor;
        protected int stretchableCountX = 100;
        protected int stretchableCountY = 100;
        protected bool userSpecifiedWidth;
        protected bool userSpecifiedHeight;
        protected float childMinWidth = 100f;
        protected float childMaxWidth = 100f;
        protected float childMinHeight = 100f;
        protected float childMaxHeight = 100f;
        private RectOffset m_Margin = new RectOffset();
        public override RectOffset margin
        {
            get
            {
                return this.m_Margin;
            }
        }
        public GUILayoutGroup()
            : base(0f, 0f, 0f, 0f, GUIStyle.none)
        {
        }
        public GUILayoutGroup(GUIStyle _style, GUILayoutOption[] options)
            : base(0f, 0f, 0f, 0f, _style)
        {
            if (options != null)
            {
                this.ApplyOptions(options);
            }
            this.m_Margin.left = _style.margin.left;
            this.m_Margin.right = _style.margin.right;
            this.m_Margin.top = _style.margin.top;
            this.m_Margin.bottom = _style.margin.bottom;
        }
        public override void ApplyOptions(GUILayoutOption[] options)
        {
            if (options == null)
            {
                return;
            }
            base.ApplyOptions(options);
            for (int i = 0; i < options.Length; i++)
            {
                GUILayoutOption gUILayoutOption = options[i];
                switch (gUILayoutOption.type)
                {
                    case GUILayoutOption.Type.fixedWidth:
                    case GUILayoutOption.Type.minWidth:
                    case GUILayoutOption.Type.maxWidth:
                        this.userSpecifiedHeight = true;
                        break;
                    case GUILayoutOption.Type.fixedHeight:
                    case GUILayoutOption.Type.minHeight:
                    case GUILayoutOption.Type.maxHeight:
                        this.userSpecifiedWidth = true;
                        break;
                    case GUILayoutOption.Type.spacing:
                        this.spacing = (float)((int)gUILayoutOption.value);
                        break;
                }
            }
        }
        protected override void ApplyStyleSettings(GUIStyle style)
        {
            base.ApplyStyleSettings(style);
            RectOffset margin = style.margin;
            this.m_Margin.left = margin.left;
            this.m_Margin.right = margin.right;
            this.m_Margin.top = margin.top;
            this.m_Margin.bottom = margin.bottom;
        }
        public void ResetCursor()
        {
            this.cursor = 0;
        }
        public Box2 PeekNext()
        {
            if (this.cursor < this.entries.Count)
            {
                GUILayoutEntry gUILayoutEntry = this.entries[this.cursor];
                return gUILayoutEntry.rect;
            }
            throw new ArgumentOutOfRangeException("cursor");
            //throw new ArgumentException(string.Concat(new object[]
            //{
            //    "Getting control ",
            //    this.cursor,
            //    "'s position in a group with only ",
            //    this.entries.Count,
            //    " controls when doing ",
            //    Event.current.rawType,
            //    "\nAborting"
            //}));
        }
        public GUILayoutEntry GetNext()
        {
            if (this.cursor < this.entries.Count)
            {
                GUILayoutEntry result = (GUILayoutEntry)this.entries[this.cursor];
                this.cursor++;
                return result;
            }
            throw new ArgumentOutOfRangeException("cursor");
            //throw new ArgumentException(string.Concat(new object[]
            //{
            //    "Getting control ",
            //    this.cursor,
            //    "'s position in a group with only ",
            //    this.entries.Count,
            //    " controls when doing ",
            //    Event.current.rawType,
            //    "\nAborting"
            //}));
        }
        public Box2 GetLast()
        {
            if (this.cursor == 0)
            {
                //Debug.LogError("You cannot call GetLast immediately after beginning a group.");
                return GUILayoutEntry.kDummyRect;
            }
            if (this.cursor <= this.entries.Count)
            {
                GUILayoutEntry gUILayoutEntry = (GUILayoutEntry)this.entries[this.cursor - 1];
                return gUILayoutEntry.rect;
            }
            //Debug.LogError(string.Concat(new object[]
            //{
            //    "Getting control ",
            //    this.cursor,
            //    "'s position in a group with only ",
            //    this.entries.Count,
            //    " controls when doing ",
            //    Event.current.type
            //}));
            return GUILayoutEntry.kDummyRect;
        }
        public void Add(GUILayoutEntry e)
        {
            this.entries.Add(e);
        }
        public override void CalcWidth()
        {
            if (this.entries.Count == 0)
            {
                this.maxWidth = (this.minWidth = (float)base.style.padding.horizontal);
                return;
            }
            this.childMinWidth = 0f;
            this.childMaxWidth = 0f;
            float num = 0;
            float num2 = 0;
            this.stretchableCountX = 0;
            bool flag = true;
            if (this.isVertical)
            {
                foreach (GUILayoutEntry gUILayoutEntry in this.entries)
                {
                    gUILayoutEntry.CalcWidth();
                    RectOffset margin = gUILayoutEntry.margin;
                    if (gUILayoutEntry.style != GUIStyle.spaceStyle)
                    {
                        if (!flag)
                        {
                            num = Utility.Min(margin.left, num);
                            num2 = Utility.Min(margin.right, num2);
                        }
                        else
                        {
                            num = margin.left;
                            num2 = margin.right;
                            flag = false;
                        }
                        this.childMinWidth = Utility.Max(gUILayoutEntry.minWidth + (float)margin.horizontal, this.childMinWidth);
                        this.childMaxWidth = Utility.Max(gUILayoutEntry.maxWidth + (float)margin.horizontal, this.childMaxWidth);
                    }
                    this.stretchableCountX += gUILayoutEntry.stretchWidth;
                }
                this.childMinWidth -= (float)(num + num2);
                this.childMaxWidth -= (float)(num + num2);
            }
            else
            {
                float num3 = 0;
                foreach (GUILayoutEntry gUILayoutEntry2 in this.entries)
                {
                    gUILayoutEntry2.CalcWidth();
                    RectOffset margin2 = gUILayoutEntry2.margin;
                    if (gUILayoutEntry2.style != GUIStyle.spaceStyle)
                    {
                        float num4;
                        if (!flag)
                        {
                            num4 = ((num3 <= margin2.left) ? margin2.left : num3);
                        }
                        else
                        {
                            num4 = 0;
                            flag = false;
                        }
                        this.childMinWidth += gUILayoutEntry2.minWidth + this.spacing + (float)num4;
                        this.childMaxWidth += gUILayoutEntry2.maxWidth + this.spacing + (float)num4;
                        num3 = margin2.right;
                        this.stretchableCountX += gUILayoutEntry2.stretchWidth;
                    }
                    else
                    {
                        this.childMinWidth += gUILayoutEntry2.minWidth;
                        this.childMaxWidth += gUILayoutEntry2.maxWidth;
                        this.stretchableCountX += gUILayoutEntry2.stretchWidth;
                    }
                }
                this.childMinWidth -= this.spacing;
                this.childMaxWidth -= this.spacing;
                if (this.entries.Count != 0)
                {
                    num = ((GUILayoutEntry)this.entries[0]).margin.left;
                    num2 = num3;
                }
                else
                {
                    num2 = (num = 0);
                }
            }
            float num5;
            float num6;
            if (base.style != GUIStyle.none || this.userSpecifiedWidth)
            {
                num5 = (float)Utility.Max(base.style.padding.left, num);
                num6 = (float)Utility.Max(base.style.padding.right, num2);
            }
            else
            {
                this.m_Margin.left = num;
                this.m_Margin.right = num2;
                num6 = (num5 = 0f);
            }
            this.minWidth = Utility.Max(this.minWidth, this.childMinWidth + num5 + num6);
            if (this.maxWidth == 0f)
            {
                this.stretchWidth += this.stretchableCountX + ((!base.style.stretchWidth) ? 0 : 1);
                this.maxWidth = this.childMaxWidth + num5 + num6;
            }
            else
            {
                this.stretchWidth = 0;
            }
            this.maxWidth = Utility.Max(this.maxWidth, this.minWidth);
            if (base.style.fixedWidth != 0f)
            {
                this.maxWidth = (this.minWidth = base.style.fixedWidth);
                this.stretchWidth = 0;
            }
        }
        public override void SetHorizontal(float x, float width)
        {
            base.SetHorizontal(x, width);
            if (this.resetCoords)
            {
                x = 0f;
            }
            RectOffset padding = base.style.padding;
            if (this.isVertical)
            {
                if (base.style != GUIStyle.none)
                {
                    foreach (GUILayoutEntry gUILayoutEntry in this.entries)
                    {
                        float num = (float)Utility.Max(gUILayoutEntry.margin.left, padding.left);
                        float x2 = x + num;
                        float num2 = width - (float)Utility.Max(gUILayoutEntry.margin.right, padding.right) - num;
                        if (gUILayoutEntry.stretchWidth != 0)
                        {
                            gUILayoutEntry.SetHorizontal(x2, num2);
                        }
                        else
                        {
                            gUILayoutEntry.SetHorizontal(x2, Utility.Clamp(num2, gUILayoutEntry.maxWidth, gUILayoutEntry.minWidth));
                        }
                    }
                }
                else
                {
                    float num3 = x - (float)this.margin.left;
                    float num4 = width + (float)this.margin.horizontal;
                    foreach (GUILayoutEntry gUILayoutEntry2 in this.entries)
                    {
                        if (gUILayoutEntry2.stretchWidth != 0)
                        {
                            gUILayoutEntry2.SetHorizontal(num3 + (float)gUILayoutEntry2.margin.left, num4 - (float)gUILayoutEntry2.margin.horizontal);
                        }
                        else
                        {
                            gUILayoutEntry2.SetHorizontal(num3 + (float)gUILayoutEntry2.margin.left, Utility.Clamp(num4 - (float)gUILayoutEntry2.margin.horizontal, gUILayoutEntry2.maxWidth, gUILayoutEntry2.minWidth));
                        }
                    }
                }
            }
            else
            {
                if (base.style != GUIStyle.none)
                {
                    float num5 = (float)padding.left;
                    float num6 = (float)padding.right;
                    if (this.entries.Count != 0)
                    {
                        num5 = Utility.Max(num5, (float)((GUILayoutEntry)this.entries[0]).margin.left);
                        num6 = Utility.Max(num6, (float)((GUILayoutEntry)this.entries[this.entries.Count - 1]).margin.right);
                    }
                    x += num5;
                    width -= num6 + num5;
                }
                float num7 = width - this.spacing * (float)(this.entries.Count - 1);
                float t = 0f;
                if (this.childMinWidth != this.childMaxWidth)
                {
                    t = Utility.Clamp((num7 - this.childMinWidth) / (this.childMaxWidth - this.childMinWidth), 1f, 0f);
                }
                float num8 = 0f;
                if (num7 > this.childMaxWidth && this.stretchableCountX > 0)
                {
                    num8 = (num7 - this.childMaxWidth) / (float)this.stretchableCountX;
                }
                float num9 = 0;
                bool flag = true;
                foreach (GUILayoutEntry gUILayoutEntry3 in this.entries)
                {
                    float num10 = Utility.Lerp(gUILayoutEntry3.minWidth, gUILayoutEntry3.maxWidth, t);
                    num10 += num8 * (float)gUILayoutEntry3.stretchWidth;
                    if (gUILayoutEntry3.style != GUIStyle.spaceStyle)
                    {
                        float num11 = gUILayoutEntry3.margin.left;
                        if (flag)
                        {
                            num11 = 0;
                            flag = false;
                        }
                        float num12 = (num9 <= num11) ? num11 : num9;
                        x += (float)num12;
                        num9 = gUILayoutEntry3.margin.right;
                    }
                    gUILayoutEntry3.SetHorizontal((float)Math.Round(x), (float)Math.Round(num10));
                    x += num10 + this.spacing;
                }
            }
        }
        public override void CalcHeight()
        {
            if (this.entries.Count == 0)
            {
                this.maxHeight = (this.minHeight = (float)base.style.padding.vertical);
                return;
            }
            this.childMinHeight = (this.childMaxHeight = 0f);
            float num = 0;
            float num2 = 0;
            this.stretchableCountY = 0;
            if (this.isVertical)
            {
                float num3 = 0;
                bool flag = true;
                foreach (GUILayoutEntry gUILayoutEntry in this.entries)
                {
                    gUILayoutEntry.CalcHeight();
                    RectOffset margin = gUILayoutEntry.margin;
                    if (gUILayoutEntry.style != GUIStyle.spaceStyle)
                    {
                        float num4;
                        if (!flag)
                        {
                            num4 = Utility.Max(num3, margin.top);
                        }
                        else
                        {
                            num4 = 0;
                            flag = false;
                        }
                        this.childMinHeight += gUILayoutEntry.minHeight + this.spacing + (float)num4;
                        this.childMaxHeight += gUILayoutEntry.maxHeight + this.spacing + (float)num4;
                        num3 = margin.bottom;
                        this.stretchableCountY += gUILayoutEntry.stretchHeight;
                    }
                    else
                    {
                        this.childMinHeight += gUILayoutEntry.minHeight;
                        this.childMaxHeight += gUILayoutEntry.maxHeight;
                        this.stretchableCountY += gUILayoutEntry.stretchHeight;
                    }
                }
                this.childMinHeight -= this.spacing;
                this.childMaxHeight -= this.spacing;
                if (this.entries.Count != 0)
                {
                    num = this.entries[0].margin.top;
                    num2 = num3;
                }
                else
                {
                    num = (num2 = 0);
                }
            }
            else
            {
                bool flag2 = true;
                foreach (GUILayoutEntry gUILayoutEntry2 in this.entries)
                {
                    gUILayoutEntry2.CalcHeight();
                    RectOffset margin2 = gUILayoutEntry2.margin;
                    if (gUILayoutEntry2.style != GUIStyle.spaceStyle)
                    {
                        if (!flag2)
                        {
                            num = Utility.Min(margin2.top, num);
                            num2 = Utility.Min(margin2.bottom, num2);
                        }
                        else
                        {
                            num = margin2.top;
                            num2 = margin2.bottom;
                            flag2 = false;
                        }
                        this.childMinHeight = Utility.Max(gUILayoutEntry2.minHeight, this.childMinHeight);
                        this.childMaxHeight = Utility.Max(gUILayoutEntry2.maxHeight, this.childMaxHeight);
                    }
                    this.stretchableCountY += gUILayoutEntry2.stretchHeight;
                }
            }
            float num5;
            float num6;
            if (base.style != GUIStyle.none || this.userSpecifiedHeight)
            {
                num5 = (float)Utility.Max(base.style.padding.top, num);
                num6 = (float)Utility.Max(base.style.padding.bottom, num2);
            }
            else
            {
                this.m_Margin.top = num;
                this.m_Margin.bottom = num2;
                num6 = (num5 = 0f);
            }
            this.minHeight = Utility.Max(this.minHeight, this.childMinHeight + num5 + num6);
            if (this.maxHeight == 0f)
            {
                this.stretchHeight += this.stretchableCountY + ((!base.style.stretchHeight) ? 0 : 1);
                this.maxHeight = this.childMaxHeight + num5 + num6;
            }
            else
            {
                this.stretchHeight = 0;
            }
            this.maxHeight = Utility.Max(this.maxHeight, this.minHeight);
            if (base.style.fixedHeight != 0f)
            {
                this.maxHeight = (this.minHeight = base.style.fixedHeight);
                this.stretchHeight = 0;
            }
        }
        public override void SetVertical(float y, float height)
        {
            base.SetVertical(y, height);
            if (this.entries.Count == 0)
            {
                return;
            }
            RectOffset padding = base.style.padding;
            if (this.resetCoords)
            {
                y = 0f;
            }
            if (this.isVertical)
            {
                if (base.style != GUIStyle.none)
                {
                    float num = (float)padding.top;
                    float num2 = (float)padding.bottom;
                    if (this.entries.Count != 0)
                    {
                        num = Utility.Max(num, (float)((GUILayoutEntry)this.entries[0]).margin.top);
                        num2 = Utility.Max(num2, (float)((GUILayoutEntry)this.entries[this.entries.Count - 1]).margin.bottom);
                    }
                    y += num;
                    height -= num2 + num;
                }
                float num3 = height - this.spacing * (float)(this.entries.Count - 1);
                float t = 0f;
                if (this.childMinHeight != this.childMaxHeight)
                {
                    t = Utility.Clamp((num3 - this.childMinHeight) / (this.childMaxHeight - this.childMinHeight), 1f, 0f);
                }
                float num4 = 0f;
                if (num3 > this.childMaxHeight && this.stretchableCountY > 0)
                {
                    num4 = (num3 - this.childMaxHeight) / (float)this.stretchableCountY;
                }
                float num5 = 0;
                bool flag = true;
                foreach (GUILayoutEntry gUILayoutEntry in this.entries)
                {
                    float num6 = Utility.Lerp(gUILayoutEntry.minHeight, gUILayoutEntry.maxHeight, t);
                    num6 += num4 * (float)gUILayoutEntry.stretchHeight;
                    if (gUILayoutEntry.style != GUIStyle.spaceStyle)
                    {
                        float num7 = gUILayoutEntry.margin.top;
                        if (flag)
                        {
                            num7 = 0;
                            flag = false;
                        }
                        float num8 = (num5 <= num7) ? num7 : num5;
                        y += (float)num8;
                        num5 = gUILayoutEntry.margin.bottom;
                    }
                    gUILayoutEntry.SetVertical((float)Math.Round(y), (float)Math.Round(num6));
                    y += num6 + this.spacing;
                }
            }
            else
            {
                if (base.style != GUIStyle.none)
                {
                    foreach (GUILayoutEntry gUILayoutEntry2 in this.entries)
                    {
                        float num9 = (float)Utility.Max(gUILayoutEntry2.margin.top, padding.top);
                        float y2 = y + num9;
                        float num10 = height - (float)Utility.Max(gUILayoutEntry2.margin.bottom, padding.bottom) - num9;
                        if (gUILayoutEntry2.stretchHeight != 0)
                        {
                            gUILayoutEntry2.SetVertical(y2, num10);
                        }
                        else
                        {
                            gUILayoutEntry2.SetVertical(y2, Utility.Clamp(num10, gUILayoutEntry2.maxHeight, gUILayoutEntry2.minHeight));
                        }
                    }
                }
                else
                {
                    float num11 = y - (float)this.margin.top;
                    float num12 = height + (float)this.margin.vertical;
                    foreach (GUILayoutEntry gUILayoutEntry3 in this.entries)
                    {
                        if (gUILayoutEntry3.stretchHeight != 0)
                        {
                            gUILayoutEntry3.SetVertical(num11 + (float)gUILayoutEntry3.margin.top, num12 - (float)gUILayoutEntry3.margin.vertical);
                        }
                        else
                        {
                            gUILayoutEntry3.SetVertical(num11 + (float)gUILayoutEntry3.margin.top, Utility.Clamp(num12 - (float)gUILayoutEntry3.margin.vertical, gUILayoutEntry3.maxHeight, gUILayoutEntry3.minHeight));
                        }
                    }
                }
            }
        }
        public override string ToString()
        {
            string text = string.Empty;
            string text2 = string.Empty;
            for (int i = 0; i < GUILayoutEntry.indent; i++)
            {
                text2 += " ";
            }
            string text3 = text;
            text = string.Concat(new object[]
            {
                text3,
                base.ToString(),
                " Margins: ",
                this.childMinHeight,
                " {\n"
            });
            GUILayoutEntry.indent += 4;
            foreach (GUILayoutEntry gUILayoutEntry in this.entries)
            {
                text = text + gUILayoutEntry.ToString() + "\n";
            }
            text = text + text2 + "}";
            GUILayoutEntry.indent -= 4;
            return text;
        }
    }
}
