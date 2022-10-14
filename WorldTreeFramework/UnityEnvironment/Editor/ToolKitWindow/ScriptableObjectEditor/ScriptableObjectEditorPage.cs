using UnityEngine;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

namespace WorldTree
{
    public enum EditorFieldType
    {
        Class,
        Dictionary,
        List,
        Table,

        Int,
        Float,
        Bool,
        String,

        Object,

        GameObject,
        ScriptableObject,

    }

    [CreateAssetMenu]//创建文件夹，分别储存文本和资源文件
    public class ScriptableObjectEditorPage : ScriptableObject
    {
        [BoxGroup("资源文件生成路径")]
        [FolderPath(RequireExistingPath = true)]
        [LabelText("文件夹"), LabelWidth(100)]
        public string CreatePath;

        [BoxGroup("资源文件生成路径")]
        [Button("生成")]
        public void Create()
        {


        }


        [TableList]
        public EditorAssetClass[] classes = new EditorAssetClass[0];
    }

    [Serializable]
    public class EditorAssetClass
    {
        [ShowInInspector, HideLabel, HorizontalGroup("类名")]
        public string ClassName;
        [ShowInInspector, HideLabel, HorizontalGroup("注释")]
        public string Comment;

        [TableList]
        [ShowInInspector, HideLabel, HorizontalGroup("字段")]
        public EditorAssetField[] fields = new EditorAssetField[0];

        [HideInInspector]
        public string ClassFilePath;
    }

    [Serializable]
    public class EditorAssetField 
    {
        [ShowInInspector, HideLabel, HorizontalGroup("类型")]
        public EditorFieldType FieldType;
        [ShowInInspector, HideLabel, HorizontalGroup("名称")]
        public string FieldName;
        [ShowInInspector, HideLabel, HorizontalGroup("注释")]
        public string Comment;

    }


    public class EditorAssetBase : ScriptableObject
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
