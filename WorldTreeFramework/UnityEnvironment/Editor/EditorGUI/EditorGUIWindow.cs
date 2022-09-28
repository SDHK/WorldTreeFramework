
/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/28 11:13

* 描述： 编辑窗体

*/

using UnityEditor;

namespace WorldTree
{

    /// <summary>
    /// 编辑器窗体组件
    /// </summary>
    public class EditorGUIWindow : Entity
    {
        public MonoEditorGUIWindow window;
    }
    class EditorGUIWindowAddSystem : AddSystem<EditorGUIWindow>
    {
        public override void OnAdd(EditorGUIWindow self)
        {
            //测试发现 编辑器窗口 关闭后就无法开启了，所以不用对象池
            self.window = EditorWindow.GetWindow<MonoEditorGUIWindow>(false);
            self.window.entity = self.Parent;
            self.window.Show();
        }
    }

    class EditorGUIWindowRemoveSystem : RemoveSystem<EditorGUIWindow>
    {
        public override void OnRemove(EditorGUIWindow self)
        {
            if (self.window.isShow)
            {
                self.window.Close();
            }
        }
    }
}