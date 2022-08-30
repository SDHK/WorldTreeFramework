using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WorldTree
{

    public static class GUIDefault
    {
        private static string textFoldoutOn = "▼";
        private static string textFoldoutOff = "▶";
        private static string textClose = "X";
        private static string textLeft = "<";
        private static string textRight = ">";

        public static int size = 1;

        private static GUIStyle styleBlack;
        private static GUIStyle styleBlack1;
        private static GUIStyle styleBlack2;
        private static GUIStyle styleBlack3;
        private static GUIStyle styleBlack4;
        private static GUIStyle styleRed;
        private static GUIStyle styleBlue;
        private static GUIStyle styleTransparent;
        private static GUIStyle styleLine;

        public static GUILayoutOption OptionWidth2 => GUILayout.Width(2 * size);
        public static GUILayoutOption OptionWidth25 => GUILayout.Width(25 * size);
        public static GUILayoutOption OptionWidth40 => GUILayout.Width(40 * size);
        public static GUILayoutOption OptionWidth60 => GUILayout.Width(60 * size);
        public static GUILayoutOption OptionWidth80 => GUILayout.Width(120 * size);

        public static GUILayoutOption OptionHeight2 => GUILayout.Height(2 * size);
        public static GUILayoutOption OptionHeight25 => GUILayout.Height(25 * size);



        public static GUIStyle StyleBlack => styleBlack?.SetGUIStyle() ?? (styleBlack = ColorGUIStyle(Color.black));
        public static GUIStyle StyleBlack1 => styleBlack1?.SetGUIStyle() ?? (styleBlack1 = ColorGUIStyle(new Color(0.1f, 0.1f, 0.1f)));
        public static GUIStyle StyleBlack2 => styleBlack2?.SetGUIStyle() ?? (styleBlack2 = ColorGUIStyle(new Color(0.2f, 0.2f, 0.2f)));
        public static GUIStyle StyleBlack3 => styleBlack3?.SetGUIStyle() ?? (styleBlack3 = ColorGUIStyle(new Color(0.3f, 0.3f, 0.3f)));
        public static GUIStyle StyleBlack4 => styleBlack4?.SetGUIStyle() ?? (styleBlack4 = ColorGUIStyle(new Color(0.4f, 0.4f, 0.4f)));
        public static GUIStyle StyleRed => styleRed?.SetGUIStyle() ?? (styleRed = ColorGUIStyle(new Color(0.5f, 0, 0), TextAnchor.MiddleCenter));
        public static GUIStyle StyleBlue => styleBlue?.SetGUIStyle() ?? (styleBlue = ColorGUIStyle(new Color(0.2f, 0.3f, 0.5f)));
        public static GUIStyle StyleTransparent => styleTransparent?.SetGUIStyle() ?? (styleTransparent = ColorGUIStyle(new Color(0, 0, 0, 0)));
        public static GUIStyle StyleLine => styleLine ?? (styleLine = ColorGUIStyle(new Color(0.1f, 0.1f, 0.1f), name: "Label"));

        //new Color(0, 1, 1)

        private static GUIStyle SetGUIStyle(this GUIStyle style)
        {
            style.fontSize = 16 * size;
            return style;
        }
        private static GUIStyle ColorGUIStyle(Color color, TextAnchor textAnchor = TextAnchor.MiddleLeft, string name = "Box")
        {
            GUIStyle style = new GUIStyle(name);
            style.normal.background = new Texture2D(1, 1, Texture2D.grayTexture.format, true);
            style.normal.background.SetPixel(0, 0, color);
            style.alignment = textAnchor;
            style.fontSize = 16;
            style.normal.textColor = Color.white;
            style.fontStyle = FontStyle.Bold;
            style.border = new RectOffset(1, 1, 1, 1);
            //style.padding = new RectOffset(1, 1, 1, 1);

            style.normal.background.Apply();
            return style;
        }


        public static void Button(string text, Action action, params GUILayoutOption[] options)
        {
            if (GUILayout.Button(text, StyleBlack3, options))
            {
                action();
            }
        }


        public static void CloseButton(Action action)
        {
          
            if (GUILayout.Button(textClose, StyleRed, OptionWidth25, OptionHeight25))
            {
                action();
            }
        }

        public static void FoldoutButton(bool foldout, Action<bool> action)
        {
            if (GUILayout.Button(foldout ? textFoldoutOn : textFoldoutOff, StyleTransparent, OptionWidth25, OptionHeight25))
            {
                action(foldout);
            }
        }


        public static void LeftButton(Action action)
        {
            if (GUILayout.Button(textLeft, StyleTransparent, OptionWidth25, OptionHeight25))
            {
                action();
            }
        }

        public static void RightButton(Action action)
        {
            if (GUILayout.Button(textRight, StyleTransparent, OptionWidth25, OptionHeight25))
            {
                action();
            }
        }


        public static void LineHorizontal()
        {
            GUILayout.Box(default(string), StyleLine, OptionHeight2, GUILayout.ExpandWidth(true));
        }

        public static void LineVertical()
        {
            GUILayout.Box(default(string), StyleLine, OptionWidth2, GUILayout.ExpandHeight(true));
        }


    }
}
