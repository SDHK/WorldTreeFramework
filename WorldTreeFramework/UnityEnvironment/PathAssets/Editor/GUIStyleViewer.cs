using UnityEditor;
using UnityEngine;
using System;



[Serializable]
public class GUIStyleViewer : EditorWindow
{

    private Vector2 scrollPosition = new Vector2(0, 0);
    private string search = "";
    private GUIStyle textStyle;

    private static GUIStyleViewer window;

    [MenuItem("SDHK_ToolKit/GUIStyleViewer")]
    private static void OpenStyleViewer()
    {
        window = GetWindow<GUIStyleViewer>(false, "查看内置GUIStyle");
    }

    void OnGUI()
    {
        if (textStyle == null)
        {
            textStyle = new GUIStyle("HeaderLabel");
            textStyle.fontSize = 20;
        }

        GUILayout.BeginHorizontal("HelpBox");
        GUILayout.Label("点击示例，可以将其名字复制下来", textStyle);
        GUILayout.FlexibleSpace();
        GUILayout.Label("Search:");
        search = EditorGUILayout.TextField(search);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("PopupCurveSwatchBackground");
        GUILayout.Label("示例", textStyle, GUILayout.Width(400));
        GUILayout.Label("名字", textStyle, GUILayout.Width(400));
        GUILayout.EndHorizontal();

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        foreach (var style in GUI.skin.customStyles)
        {
            if (style.name.ToLower().Contains(search.ToLower()))
            {
                GUILayout.Space(10);
                GUILayout.BeginHorizontal("PopupCurveSwatchBackground");
                if (GUILayout.Button(style.name, style, GUILayout.Width(400)))
                {
                    EditorGUIUtility.systemCopyBuffer = style.name;
                    Debug.LogError(style.name);
                }
                EditorGUILayout.SelectableLabel(style.name, GUILayout.Width(400));
                GUILayout.EndHorizontal();
            }
        }

        GUILayout.EndScrollView();
    }
}