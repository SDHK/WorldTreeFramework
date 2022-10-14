using UnityEngine;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

namespace WorldTree
{

    [CreateAssetMenu(fileName = "EditorAssetNode", menuName = "ScriptableObjects/EditorAssetNode")]
    public class EditorAssetNode : EditorAssetBase
    {

        /// <summary>
        /// 是否折叠
        /// </summary>
        public bool isFold;

        /// <summary>
        /// 水平排列
        /// </summary>
        public bool isHorizontal;

        /// <summary>
        /// 父节点
        /// </summary>
        public EditorAssetBase parentNode;

        /// <summary>
        /// 子节点
        /// </summary>
        public List<KeyValue<string, EditorAssetBase>> Node = new List<KeyValue<string, EditorAssetBase>>();


    }
}
