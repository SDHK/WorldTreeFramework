
/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/28 10:25

* 描述： 编辑器窗体生命周期系统

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldTree
{
    public interface IEditorWindowEnableSystem : ISendSystem { }

    /// <summary>
    /// 窗体启用系统
    /// </summary>
    public abstract class EditorWindowEnableSystem<T> : SystemBase<T, IEditorWindowEnableSystem>, IEditorWindowEnableSystem
       where T : Entity
    {
        public void Invoke(Entity self) => OnEnable(self as T);
        public abstract void OnEnable(T self);
    }

    public interface IEditorWindowFocusSystem : ISendSystem { }
    /// <summary>
    /// 窗体获得焦点时调用
    /// </summary>
    public abstract class EditorWindowFocusSystem<T> : SystemBase<T, IEditorWindowFocusSystem>, IEditorWindowFocusSystem
       where T : Entity
    {
        public void Invoke(Entity self) => OnFocus(self as T);
        public abstract void OnFocus(T self);
    }
    public interface IEditorWindowInspectorUpdateSystem : ISendSystem { }
    /// <summary>
    /// 窗体Update
    /// </summary>
    public abstract class EditorWindowInspectorUpdateSystem<T> : SystemBase<T, IEditorWindowInspectorUpdateSystem>, IEditorWindowInspectorUpdateSystem
       where T : Entity
    {
        public void Invoke(Entity self) => OnInspectorUpdate(self as T);
        public abstract void OnInspectorUpdate(T self);
    }

    public interface IEditorWindowProjectChangeSystem : ISendSystem { }
    /// <summary>
    /// 窗体在项目发生更改时调用
    /// </summary>
    public abstract class EditorWindowProjectChangeSystem<T> : SystemBase<T, IEditorWindowProjectChangeSystem>, IEditorWindowProjectChangeSystem
       where T : Entity
    {
        public void Invoke(Entity self) => OnProjectChange(self as T);
        public abstract void OnProjectChange(T self);
    }
    public interface IEditorWindowSelectionChangeSystem : ISendSystem { }
    /// <summary>
    /// 窗体在选择发生更改时调用
    /// </summary>
    public abstract class EditorWindowSelectionChangeSystem<T> : SystemBase<T, IEditorWindowSelectionChangeSystem>, IEditorWindowSelectionChangeSystem
       where T : Entity
    {
        public void Invoke(Entity self) => OnSelectionChange(self as T);
        public abstract void OnSelectionChange(T self);
    }

    public interface IEditorWindowHierarchyChangeSystem : ISendSystem { }
    /// <summary>
    /// 窗体在场景结构发生层次更改时调用
    /// </summary>
    public abstract class EditorWindowHierarchyChangeSystem<T> : SystemBase<T, IEditorWindowHierarchyChangeSystem>, IEditorWindowHierarchyChangeSystem
       where T : Entity
    {
        public void Invoke(Entity self) => OnHierarchyChange(self as T);
        public abstract void OnHierarchyChange(T self);
    }
    public interface IEditorWindowLostFocusSystem : ISendSystem { }
    /// <summary>
    /// 窗体在丢失焦点时调用
    /// </summary>
    public abstract class EditorWindowLostFocusSystem<T> : SystemBase<T, IEditorWindowLostFocusSystem>, IEditorWindowLostFocusSystem
       where T : Entity
    {
        public void Invoke(Entity self) => OnLostFocus(self as T);
        public abstract void OnLostFocus(T self);
    }
    public interface IEditorWindowDisableSystem : ISendSystem { }

    /// <summary>
    /// 窗体禁用系统
    /// </summary>
    public abstract class EditorWindowDisableSystem<T> : SystemBase<T, IEditorWindowDisableSystem>, IEditorWindowDisableSystem
       where T : Entity
    {
        public void Invoke(Entity self) => OnDisable(self as T);
        public abstract void OnDisable(T self);
    }
    public interface IEditorWindowDestroySystem : ISendSystem { }
    /// <summary>
    /// 窗体在关闭时调用
    /// </summary>
    public abstract class EditorWindowDestroySystem<T> : SystemBase<T, IEditorWindowDestroySystem>, IEditorWindowDestroySystem
       where T : Entity
    {
        public void Invoke(Entity self) => OnDestroy(self as T);
        public abstract void OnDestroy(T self);
    }
}

