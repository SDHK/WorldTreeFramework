
/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/29 16:17

* 描述： 

*/

using UnityEngine;

namespace WorldTree
{

    public abstract class GUIBase : Entity
    {
        /// <summary>
        /// 文本内容
        /// </summary>
        public string text = default(string);


        public GUIStyle style;

        public GUILayoutOption[] options;



        /// <summary>
        /// GUI样式
        /// </summary>
        public GUIStyle Style
        {
            get
            {
                if (style == null)
                {
                    style = this.Root.ObjectPoolManager.Get<GUIStyle>();
                }
                return style;
            }
            set { style = value; }
        }



        /// <summary>
        /// 绝对宽度
        /// </summary>
        public float Width
        {
            get
            {
                return Style.fixedWidth;
            }
            set { Style.fixedWidth = value; }
        }

        /// <summary>
        /// 绝对高度
        /// </summary>
        public float Height
        {
            get
            {
                return Style.fixedHeight;
            }
            set { Style.fixedHeight = value; }
        }

        /// <summary>
        /// 内边距
        /// </summary>
        public RectOffset Padding
        {
            get
            {
                return Style.padding;
            }
            set { Style.padding = value; }
        }


        /// <summary>
        /// 字体大小
        /// </summary>
        public int FontSize
        {
            get
            {
                return Style.fontSize;
            }
            set { Style.fontSize = value; }
        }


        /// <summary>
        /// 字体样式
        /// </summary>
        public FontStyle FontStyle
        {
            get
            {
                return Style.fontStyle;
            }
            set { Style.fontStyle = value; }
        }

        /// <summary>
        /// 字体颜色
        /// </summary>
        public Color FontColor
        {
            get
            {
                return Style.normal.textColor;
            }
            set { Style.normal.textColor = value; }
        }

        /// <summary>
        /// 字体锚点
        /// </summary>
        public TextAnchor FontAnchor
        {
            get
            {
                return Style.alignment;
            }
            set { Style.alignment = value; }
        }

        /// <summary>
        /// 图片
        /// </summary>
        public Texture2D Texture
        {
            get
            {
                return Style.normal.background;
            }
            set { Style.normal.background = value; }
        }



        /// <summary>
        /// 自动拉伸宽度
        /// </summary>
        public bool StretchWidth
        {
            get
            {
                return Style.stretchWidth;
            }
            set { Style.stretchWidth = value; }
        }


        /// <summary>
        /// 自动拉伸高度
        /// </summary>
        public bool StretchHeight
        {
            get
            {
                return Style.stretchHeight;
            }
            set { Style.stretchHeight = value; }
        }

        /// <summary>
        /// 设置操作
        /// </summary>
        public void SetOption(params GUILayoutOption[] options)
        {
            this.options = options;
        }

    }



}
