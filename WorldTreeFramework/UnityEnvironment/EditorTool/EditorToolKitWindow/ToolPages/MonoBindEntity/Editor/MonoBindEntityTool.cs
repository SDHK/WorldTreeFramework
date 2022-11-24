
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/24 10:52

* 描述： mono组件绑定实体，脚本生成工具




* 思考：entity 绑定 Mono 事件，由mono反向 调用 实体实现的扩展方法
* 
* 通过扩展方法直接绑定组件回调
* 
* UI接口事件? 碰撞事件?
* 
* 编辑器工具
* 脚本内容全一键自动生成
* 
* 假如只有根节点则不需要Mono
* 
*/

using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace WorldTree
{
    /// <summary>
    /// mono组件绑定实体工具
    /// </summary>
    [CreateAssetMenu]
    public class MonoBindEntityTool : ScriptableObject
    {
        [BoxGroup("路径")]
        [FolderPath(RequireExistingPath = true)]
        [LabelText("脚本文件夹"), LabelWidth(100)]
        public string CreateFilePath;

        [LabelText("分组")]
        [Searchable]
        public List<ObjectBindGroup> groups = new List<ObjectBindGroup>();

    }


    [Serializable]
    public class ObjectBindGroup
    {

        MonoBindEntityTool monoBindEntityTool;

        [LabelText("游戏物体")]
        public GameObject gameObject;

        [LabelText("组件列表")]
        public List<Component> components = new List<Component>();


    }
}