
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
            self.window = EditorWindow.GetWindow<MonoEditorGUIWindow>(false);//窗口是无法回收使用的对象
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