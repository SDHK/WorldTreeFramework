
/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/28 9:54

* 描述： 继承Mono的编辑器窗体
* 
* 可塞入实体获得生命周期进行绘制
* 

*/

using UnityEditor;
using WorldTree;

namespace EditorTool
{
    /// <summary>
    /// 继承Mono的编辑器窗体
    /// </summary>
    public class MonoEditorGUIWindow : EditorWindow
    {
        public INode entity;

        public bool isShow = false;


        private void OnEnable()
        {
            if (!isShow)
            {
                isShow = true;
            }
            entity?.TrySendRule<IEditorWindowEnableSystem>();
        }

        private void OnFocus()
        {
            entity?.TrySendRule<IEditorWindowFocusSystem>();
        }

        private void OnInspectorUpdate()
        {
            entity?.TrySendRule<IEditorWindowInspectorUpdateSystem>();
        }

        public void OnProjectChange()
        {
            entity?.TrySendRule<IEditorWindowProjectChangeSystem>();
        }

        private void OnSelectionChange()
        {
            entity?.TrySendRule<IEditorWindowSelectionChangeSystem>();
        }

        public void OnHierarchyChange()
        {
            entity?.TrySendRule<IEditorWindowHierarchyChangeSystem>();
        }
        private void OnGUI()
        {
            entity?.TrySendRule<IGUIDrawSystem>();
        }

        private void OnLostFocus()
        {
            entity?.TrySendRule<IEditorWindowLostFocusSystem>();
        }
        private void OnDisable()
        {
            entity?.TrySendRule<IEditorWindowDisableSystem>();
        }
        private void OnDestroy()
        {
            entity?.TrySendRule<IEditorWindowDestroySystem>();
            if (isShow)
            {
                isShow = false;
                entity?.Dispose();
            }
        }
    }
}

