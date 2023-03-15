
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
    public class EditorHomePage : Node, ComponentOfNode
    {
        public INode page;
    }

    class EditorHomePageAddSystem : AddRule<EditorHomePage>
    {
        public override void OnEvent(EditorHomePage self)
        {
        }
    }
    class EditorHomePageOnGUISystem : GuiUpdateRule<EditorHomePage>
    {
        public override void OnEvent(EditorHomePage self, float deltaTime)
        {
            GUILayout.Space(20);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(GUILayout.Width(150));

            foreach (var item in self.ChildrenDictionary())
            {
                if (GUILayout.Button(item.Value.Type.Name))
                {
                    self.page = item.Value;
                }
            }

            EditorGUILayout.EndVertical();

            GUI.color = Color.black;
            GUILayout.Box(default(string), GUILayout.Width(2), GUILayout.ExpandHeight(true));
            GUI.color = Color.white;


            //=====

            EditorGUILayout.BeginVertical();

            self.page?.SendRule<IGUIDrawSystem>();

            EditorGUILayout.EndVertical();


            EditorGUILayout.EndHorizontal();
        }
    }

    class EditorHomePageUpdateSystem : UpdateRule<EditorHomePage>
    {
        public override void OnEvent(EditorHomePage self, float deltaTime)
        {
        }
    }
}
