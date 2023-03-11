
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
            entity?.SendRule<IEditorWindowEnableSystem>();
        }

        private void OnFocus()
        {
            entity?.SendRule<IEditorWindowFocusSystem>();
        }

        private void OnInspectorUpdate()
        {
            entity?.SendRule<IEditorWindowInspectorUpdateSystem>();
        }

        public void OnProjectChange()
        {
            entity?.SendRule<IEditorWindowProjectChangeSystem>();
        }

        private void OnSelectionChange()
        {
            entity?.SendRule<IEditorWindowSelectionChangeSystem>();
        }

        public void OnHierarchyChange()
        {
            entity?.SendRule<IEditorWindowHierarchyChangeSystem>();
        }
        private void OnGUI()
        {
            entity?.SendRule<IGUIDrawSystem>();
        }

        private void OnLostFocus()
        {
            entity?.SendRule<IEditorWindowLostFocusSystem>();
        }
        private void OnDisable()
        {
            entity?.SendRule<IEditorWindowDisableSystem>();
        }
        private void OnDestroy()
        {
            entity?.SendRule<IEditorWindowDestroySystem>();
            if (isShow)
            {
                isShow = false;
                entity?.Dispose();
            }
        }
    }
}

