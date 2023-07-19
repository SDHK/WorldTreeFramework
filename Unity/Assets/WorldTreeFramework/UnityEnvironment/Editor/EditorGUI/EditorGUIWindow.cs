
/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/28 11:13

* 描述： 编辑窗体

*/

using UnityEditor;
using WorldTree;

namespace EditorTool
{

    /// <summary>
    /// 编辑器窗体组件
    /// </summary>
    public class EditorGUIWindow : Node, ComponentOf<INode>
        
       
      
    {
        public MonoEditorGUIWindow window;
    }
    class EditorGUIWindowAddSystem : AddRule<EditorGUIWindow>
    {
        public override void OnEvent(EditorGUIWindow self)
        {
            //测试发现 编辑器窗口 关闭后就无法开启了，所以不用对象池
            self.window = EditorWindow.GetWindow<MonoEditorGUIWindow>(false);
            self.window.entity = self.Parent;
            self.window.Show();
        }
    }

    class EditorGUIWindowRemoveSystem : RemoveRule<EditorGUIWindow>
    {
        public override void OnEvent(EditorGUIWindow self)
        {
            if (self.window.isShow)
            {
                self.window.Close();
            }
        }
    }
}