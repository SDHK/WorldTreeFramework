
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
        public Node entity;

        public bool isShow = false;


        private void OnEnable()
        {
            if (!isShow)
            {
                isShow = true;
            }
            entity?.SendSystem<IEditorWindowEnableSystem>();
        }

        private void OnFocus()
        {
            entity?.SendSystem<IEditorWindowFocusSystem>();
        }

        private void OnInspectorUpdate()
        {
            entity?.SendSystem<IEditorWindowInspectorUpdateSystem>();
        }

        public void OnProjectChange()
        {
            entity?.SendSystem<IEditorWindowProjectChangeSystem>();
        }

        private void OnSelectionChange()
        {
            entity?.SendSystem<IEditorWindowSelectionChangeSystem>();
        }

        public void OnHierarchyChange()
        {
            entity?.SendSystem<IEditorWindowHierarchyChangeSystem>();
        }
        private void OnGUI()
        {
            entity?.SendSystem<IGUIDrawSystem>();
        }

        private void OnLostFocus()
        {
            entity?.SendSystem<IEditorWindowLostFocusSystem>();
        }
        private void OnDisable()
        {
            entity?.SendSystem<IEditorWindowDisableSystem>();
        }
        private void OnDestroy()
        {
            entity?.SendSystem<IEditorWindowDestroySystem>();
            if (isShow)
            {
                isShow = false;
                entity?.Dispose();
            }
        }
    }
}

