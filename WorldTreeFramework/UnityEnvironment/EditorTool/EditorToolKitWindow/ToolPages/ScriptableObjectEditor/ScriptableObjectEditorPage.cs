using UnityEngine;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;

namespace WorldTree
{
   

    [CreateAssetMenu]
    public class ScriptableObjectEditorPage : ScriptableObject
    {
        [BoxGroup("路径")]
        [FolderPath(RequireExistingPath = true)]
        [LabelText("脚本文件夹"), LabelWidth(100)]
        public string CreateFilePath;
        [BoxGroup("路径")]
        [FolderPath(RequireExistingPath = true)]
        [LabelText("资源文件夹"), LabelWidth(100)]
        public string CreateAssetPath;


        [InfoBox("命名冲突", InfoMessageType.Error, "@this.IsRepeatName()")]
        [BoxGroup("编辑")]
        [LabelText("新建类名"), LabelWidth(60)]
        public string ClassName;

        [BoxGroup("编辑")]
        [Button("添加", ButtonSizes.Large)]
        public void AddClass()
        {
            if (!IsRepeatName() && ClassName != "")
            {
                classes.Add(new EditorAssetClass() { ClassName = ClassName, scriptableObjectEditor = this });
                ClassName = "";
            }
        }

        public bool IsRepeatName()
        {
            return classes.Any((item) => item.ClassName == ClassName);
        }

        [LabelText("列表")]
        [Searchable]
        [ListDrawerSettings(Expanded = true, HideAddButton = true, CustomRemoveElementFunction = "RemoveClass")]
        public List<EditorAssetClass> classes = new List<EditorAssetClass>();


        public void RemoveClass(EditorAssetClass assetClass)
        {
            assetClass.RemoveAllAsset();
            classes.Remove(assetClass);
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(assetClass.monoScript));
        }
    }

}
