﻿
/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/28 14:29

* 描述： 主页面

*/
using System;
using UnityEditor;
using UnityEngine;
using WorldTree;

namespace EditorTool
{
    public class EditorHomePage : Node, ComponentOf<INode>
        , AsRule<Awake>
        , AsRule<GuiUpdate>
        , AsChildBranch
    {
        public INode page;
    }


    class EditorHomePageAddSystem : AddRule<EditorHomePage>
    {
        protected override void Execute(EditorHomePage self)
        {
        }
    }
    class EditorHomePageOnGUISystem : GuiUpdateRule<EditorHomePage>
    {
        protected override void Execute(EditorHomePage self)
        {
            GUILayout.Space(20);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(GUILayout.Width(150));
			
			foreach (var item in self.ChildBranch())
            {
                if (GUILayout.Button(self.CodeToType(item.Type).Name))
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

            //self.page?.TrySendRule<IGUIDrawSystem>();

            EditorGUILayout.EndVertical();


            EditorGUILayout.EndHorizontal();
        }
    }

    class EditorHomePageUpdateSystem : UpdateTimeRule<EditorHomePage>
    {
        protected override void Execute(EditorHomePage self, TimeSpan deltaTime)
        {
        }
    }
}
