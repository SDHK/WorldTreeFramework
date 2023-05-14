
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
    public interface IEditorWindowEnableSystem : ISendRule { }

    /// <summary>
    /// 窗体启用系统
    /// </summary>
    public abstract class EditorWindowEnableSystem<T> : SendRuleBase<T, IEditorWindowEnableSystem>, IEditorWindowEnableSystem
       where T : class, INode, AsRule<IEditorWindowEnableSystem>
    { }

    public interface IEditorWindowFocusSystem : ISendRule { }
    /// <summary>
    /// 窗体获得焦点时调用
    /// </summary>
    public abstract class EditorWindowFocusSystem<T> : SendRuleBase<T, IEditorWindowFocusSystem>, IEditorWindowFocusSystem
       where T : class, INode, AsRule<IEditorWindowFocusSystem>
    { }
    public interface IEditorWindowInspectorUpdateSystem : ISendRule { }
    /// <summary>
    /// 窗体Update
    /// </summary>
    public abstract class EditorWindowInspectorUpdateSystem<T> : SendRuleBase<T, IEditorWindowInspectorUpdateSystem>, IEditorWindowInspectorUpdateSystem
       where T : class, INode, AsRule<IEditorWindowInspectorUpdateSystem>
    { }

    public interface IEditorWindowProjectChangeSystem : ISendRule { }
    /// <summary>
    /// 窗体在项目发生更改时调用
    /// </summary>
    public abstract class EditorWindowProjectChangeSystem<T> : SendRuleBase<T, IEditorWindowProjectChangeSystem>, IEditorWindowProjectChangeSystem
       where T : class, INode, AsRule<IEditorWindowProjectChangeSystem>
    { }
    public interface IEditorWindowSelectionChangeSystem : ISendRule { }
    /// <summary>
    /// 窗体在选择发生更改时调用
    /// </summary>
    public abstract class EditorWindowSelectionChangeSystem<T> : SendRuleBase<T, IEditorWindowSelectionChangeSystem>, IEditorWindowSelectionChangeSystem
       where T : class, INode, AsRule<IEditorWindowSelectionChangeSystem>
    { }

    public interface IEditorWindowHierarchyChangeSystem : ISendRule { }
    /// <summary>
    /// 窗体在场景结构发生层次更改时调用
    /// </summary>
    public abstract class EditorWindowHierarchyChangeSystem<T> : SendRuleBase<T, IEditorWindowHierarchyChangeSystem>, IEditorWindowHierarchyChangeSystem
       where T : class, INode, AsRule<IEditorWindowHierarchyChangeSystem>
    { }
    public interface IEditorWindowLostFocusSystem : ISendRule { }
    /// <summary>
    /// 窗体在丢失焦点时调用
    /// </summary>
    public abstract class EditorWindowLostFocusSystem<T> : SendRuleBase<T, IEditorWindowLostFocusSystem>, IEditorWindowLostFocusSystem
       where T : class, INode, AsRule<IEditorWindowLostFocusSystem>
    { }
    public interface IEditorWindowDisableSystem : ISendRule { }

    /// <summary>
    /// 窗体禁用系统
    /// </summary>
    public abstract class EditorWindowDisableSystem<T> : SendRuleBase<T, IEditorWindowDisableSystem>, IEditorWindowDisableSystem
       where T : class, INode, AsRule<IEditorWindowDisableSystem>
    { }
    public interface IEditorWindowDestroySystem : ISendRule { }
    /// <summary>
    /// 窗体在关闭时调用
    /// </summary>
    public abstract class EditorWindowDestroySystem<T> : SendRuleBase<T, IEditorWindowDestroySystem>, IEditorWindowDestroySystem
       where T : class, INode, AsRule<IEditorWindowDestroySystem>
    { }
}

