﻿
/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/28 14:29

* 描述： 主页面

*/
using UnityEditor;
using UnityEngine;
using WorldTree;

namespace EditorTool
{
    public class EditorHomePage : Node, ComponentOf<INode>
        , AsRule<IAwakeRule>
        , AsRule<IGuiUpdateRule>
    {
        public INode page;
    }

    class EditorHomePageAddSystem : AddRule<EditorHomePage>
    {
        protected override void OnEvent(EditorHomePage self)
        {
        }
    }
    class EditorHomePageOnGUISystem : GuiUpdateRule<EditorHomePage>
    {
        protected override void OnEvent(EditorHomePage self, float deltaTime)
        {
            GUILayout.Space(20);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(GUILayout.Width(150));

            foreach (var item in self.GetBranch<ChildBranch>())
            {
                if (GUILayout.Button(item.Type.HashCore64ToType().Name))
                {
                    self.page = item;
                }
            }

            EditorGUILayout.EndVertical();

            GUI.color = Color.black;
            GUILayout.Box(default(string), GUILayout.Width(2), GUILayout.ExpandHeight(true));
            GUI.color = Color.white;


            //=====

            EditorGUILayout.BeginVertical();

            self.page?.TrySendRule<IGUIDrawSystem>();

            EditorGUILayout.EndVertical();


            EditorGUILayout.EndHorizontal();
        }
    }

    class EditorHomePageUpdateSystem : UpdateTimeRule<EditorHomePage>
    {
        protected override void OnEvent(EditorHomePage self, float deltaTime)
        {
        }
    }
}
