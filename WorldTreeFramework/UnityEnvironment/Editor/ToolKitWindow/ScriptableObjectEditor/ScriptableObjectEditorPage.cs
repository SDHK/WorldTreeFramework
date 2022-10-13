using UnityEngine;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

namespace WorldTree
{
    public enum EditorAssetType
    {
        Node,

        Int,
        Float,
        String,

        Object,

        GameObject,

    }
    public enum EditorAssetNodeType
    {
        Node,
        List,
        Dictionary
    }


    [CreateAssetMenu]
    public class ScriptableObjectEditorPage : ScriptableObject
    {
        [BoxGroup("资源生成路径")]
        [FolderPath(RequireExistingPath = true)]
        [LabelText("文件夹"), LabelWidth(100)]
        public string CreatePath;

        public EditorAssetNode editorData;

    }

    [Serializable]
    public class EditorAssetNode: EditorAssetBase
    {
        /// <summary>
        /// 节点转换类型
        /// </summary>
        public EditorAssetNodeType nodeType;
        
        /// <summary>
        /// 是否折叠
        /// </summary>
        public bool isFold = false;

        /// <summary>
        /// 水平排列
        /// </summary>
        public bool isHorizontal = false;

        /// <summary>
        /// 父节点
        /// </summary>
        public EditorAssetBase parentNode;

        /// <summary>
        /// 子节点
        /// </summary>
        public List<KeyValue<string, EditorAssetBase>> Node = new List<KeyValue<string, EditorAssetBase>>();

    }


    public class EditorAssetBase
    {


    }
    [Serializable]
    public struct KeyValue<K, V>
    {
        [ShowInInspector]
        public K key;
        [ShowInInspector]
        public V value;
    }



}
