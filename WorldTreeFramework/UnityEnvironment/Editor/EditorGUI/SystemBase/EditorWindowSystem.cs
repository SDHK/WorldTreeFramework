
/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/28 10:25

* 描述： 编辑器窗体生命周期系统

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldTree;

namespace EditorTool
{
    public interface IEditorWindowEnableSystem : ISendSystem { }

    /// <summary>
    /// 窗体启用系统
    /// </summary>
    public abstract class EditorWindowEnableSystem<T> : RuleBase<T, IEditorWindowEnableSystem>, IEditorWindowEnableSystem
       where T : Node
    {
        public void Invoke(Node self) => OnEnable(self as T);
        public abstract void OnEnable(T self);
    }

    public interface IEditorWindowFocusSystem : ISendSystem { }
    /// <summary>
    /// 窗体获得焦点时调用
    /// </summary>
    public abstract class EditorWindowFocusSystem<T> : RuleBase<T, IEditorWindowFocusSystem>, IEditorWindowFocusSystem
       where T : Node
    {
        public void Invoke(Node self) => OnFocus(self as T);
        public abstract void OnFocus(T self);
    }
    public interface IEditorWindowInspectorUpdateSystem : ISendSystem { }
    /// <summary>
    /// 窗体Update
    /// </summary>
    public abstract class EditorWindowInspectorUpdateSystem<T> : RuleBase<T, IEditorWindowInspectorUpdateSystem>, IEditorWindowInspectorUpdateSystem
       where T : Node
    {
        public void Invoke(Node self) => OnInspectorUpdate(self as T);
        public abstract void OnInspectorUpdate(T self);
    }

    public interface IEditorWindowProjectChangeSystem : ISendSystem { }
    /// <summary>
    /// 窗体在项目发生更改时调用
    /// </summary>
    public abstract class EditorWindowProjectChangeSystem<T> : RuleBase<T, IEditorWindowProjectChangeSystem>, IEditorWindowProjectChangeSystem
       where T : Node
    {
        public void Invoke(Node self) => OnProjectChange(self as T);
        public abstract void OnProjectChange(T self);
    }
    public interface IEditorWindowSelectionChangeSystem : ISendSystem { }
    /// <summary>
    /// 窗体在选择发生更改时调用
    /// </summary>
    public abstract class EditorWindowSelectionChangeSystem<T> : RuleBase<T, IEditorWindowSelectionChangeSystem>, IEditorWindowSelectionChangeSystem
       where T : Node
    {
        public void Invoke(Node self) => OnSelectionChange(self as T);
        public abstract void OnSelectionChange(T self);
    }

    public interface IEditorWindowHierarchyChangeSystem : ISendSystem { }
    /// <summary>
    /// 窗体在场景结构发生层次更改时调用
    /// </summary>
    public abstract class EditorWindowHierarchyChangeSystem<T> : RuleBase<T, IEditorWindowHierarchyChangeSystem>, IEditorWindowHierarchyChangeSystem
       where T : Node
    {
        public void Invoke(Node self) => OnHierarchyChange(self as T);
        public abstract void OnHierarchyChange(T self);
    }
    public interface IEditorWindowLostFocusSystem : ISendSystem { }
    /// <summary>
    /// 窗体在丢失焦点时调用
    /// </summary>
    public abstract class EditorWindowLostFocusSystem<T> : RuleBase<T, IEditorWindowLostFocusSystem>, IEditorWindowLostFocusSystem
       where T : Node
    {
        public void Invoke(Node self) => OnLostFocus(self as T);
        public abstract void OnLostFocus(T self);
    }
    public interface IEditorWindowDisableSystem : ISendSystem { }

    /// <summary>
    /// 窗体禁用系统
    /// </summary>
    public abstract class EditorWindowDisableSystem<T> : RuleBase<T, IEditorWindowDisableSystem>, IEditorWindowDisableSystem
       where T : Node
    {
        public void Invoke(Node self) => OnDisable(self as T);
        public abstract void OnDisable(T self);
    }
    public interface IEditorWindowDestroySystem : ISendSystem { }
    /// <summary>
    /// 窗体在关闭时调用
    /// </summary>
    public abstract class EditorWindowDestroySystem<T> : RuleBase<T, IEditorWindowDestroySystem>, IEditorWindowDestroySystem
       where T : Node
    {
        public void Invoke(Node self) => OnDestroy(self as T);
        public abstract void OnDestroy(T self);
    }
}

