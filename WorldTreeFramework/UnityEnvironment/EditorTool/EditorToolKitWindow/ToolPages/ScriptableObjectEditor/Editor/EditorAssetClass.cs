
/****************************************

* 作者： 闪电黑客
* 日期： 2022/10/17 10:27

* 描述： 资源的类型编辑

*/

using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using WorldTree;

namespace EditorTool
{

    /// <summary>
    /// 编辑资源类
    /// </summary>
    [Serializable]
    public partial class EditorAssetClass
    {
        [HideInInspector]
        public bool IsShow = false;
        [HideInInspector]
        public ScriptableObjectEditorTool scriptableObjectEditor;

        [HorizontalGroup("A")]
        [HorizontalGroup("A/A")]
        [LabelText("类名"), LabelWidth(30), GUIColor(0, 1, 0)]
        [ReadOnly]
        public string ClassName;

        [HorizontalGroup("A/A")]
        [LabelText("注释"), LabelWidth(30)]
        public string Comment;

        [EnableIf("@this.IsCreateClass()")]
        [HorizontalGroup("A/A", width: 100)]
        [Button("生成脚本")]
        public void CreateFile()
        {
            CreateClassFile();
            CreateDeleteListClassFile();
        }


        [HorizontalGroup("A/B", width: 50)]
        [LabelText("列表资源"), LabelWidth(60)]
        public bool IsList = false;

        [HorizontalGroup("A/B")]
        [LabelText("脚本"), LabelWidth(30)]
        [ReadOnly]
        public UnityEngine.Object monoScript;


        [ShowIf("IsList")]
        [HorizontalGroup("A/B")]
        [LabelText("列表脚本"), LabelWidth(60)]
        [ReadOnly]
        public UnityEngine.Object monoListScript;

        [GUIColor(0, 1, 0)]
        [HideIf("IsShow")]
        [HorizontalGroup("A/B", width: 100)]
        [Button("展开")]
        public void FoldShow()
        {
            IsShow = true;
        }
        [GUIColor(1, 0, 0)]
        [ShowIf("IsShow")]
        [HorizontalGroup("A/B", width: 100)]
        [Button("收起")]
        public void FoldHide()
        {
            IsShow = false;
        }

        [PropertySpace(5)]
        [LabelText("字段")]
        [ShowIf("IsShow")]
        [TableList(ShowIndexLabels = true)]
        public List<EditorAssetField> fields = new List<EditorAssetField>();

        [InfoBox("命名冲突", InfoMessageType.Error, "@this.IsRepeatAssetName()")]
        [VerticalGroup("资源")]
        [LabelText("新建资源"), LabelWidth(60)]
        [ShowIf("IsShow")]
        public string AssetName;

        [VerticalGroup("资源")]
        [Button("新建", ButtonSizes.Medium)]
        [ShowIf("@this.IsCreateAssetButton()")]
        public void CreateAssetButton()
        {
            CreateDeleteListClassFile();

            if (monoScript == null)
            {
                if (IsCreateClass())
                {
                    CreateClassFile();
                }
            }
            else
            {
                if (IsCreateAsset())
                {
                    Assets.Add(CreateAsset());
                    AssetName = "";

                    if (IsCreateListAsset())
                    {
                        CreateListAsset();
                    }
                    RefreshListAsset();
                }
            }
        }

        [LabelText("资源集合")]
        [ShowIf("IsShow")]
        [InlineEditor]
        [Searchable]
        //[TableList(IsReadOnly = true, ShowIndexLabels = true)]
        [ListDrawerSettings(ShowPaging = true, HideAddButton = true, ShowIndexLabels = true, CustomRemoveElementFunction = "DeleteAssetButton", OnTitleBarGUI = "RefreshListAssetButton")]
        public List<ScriptableObject> Assets = new List<ScriptableObject>();


        public void DeleteAssetButton(ScriptableObject scriptableObject)
        {
            if (EditorUtility.DisplayDialog($"删除 {ClassName} 类的资源 {scriptableObject.name} ", $"确定要删除 {scriptableObject.name} 资源吗？", "✔", "❌"))
            {
                DeleteAsset(scriptableObject);
            }
        }



        [GUIColor(0, 1, 1)]
        [ShowIf("@IsCreateListAsset()")]
        [Button("创建列表资源")]
        public void CreateListAssetButton()
        {
            if (IsCreateListAsset())
            {
                CreateListAsset();
            }
            RefreshListAsset();
        }

        [ShowIf("@IsShow&&IsList&&monoListScript!=null&&listAsset!=null")]
        [LabelText("列表资源"), LabelWidth(100)]
        [InlineEditor]
        public ListAssetBase listAsset;


        #region 刷新
        /// <summary>
        /// 刷新列表资源的GUI按钮
        /// </summary>
        public void RefreshListAssetButton()
        {
            if (IsList)
            {
                if (SirenixEditorGUI.ToolbarButton(EditorIcons.Refresh))
                {
                    RefreshListAsset();
                }
            }
        }

        /// <summary>
        /// 刷新列表资源
        /// </summary>
        public void RefreshListAsset()
        {
            if (listAsset != null)
            {
                listAsset.Clear();
                for (int i = 0; i < Assets.Count; i++)
                {
                    listAsset.AddAsset(Assets[i]);
                }
                EditorUtility.SetDirty(listAsset);
                AssetDatabase.Refresh();
            }
        }
        #endregion


        #region 创建

        /// <summary>
        /// 根据标记判断创建还是删除列表类
        /// </summary>
        public void CreateDeleteListClassFile()
        {
            if (IsCreateListClass())
            {
                CreateListClassFile();
            }
            else if (!IsList)
            {
                DeleteListClass();
            }
        }


        /// <summary>
        /// 创建类资源
        /// </summary>
        public ScriptableObject CreateAsset()
        {
            ScriptableObject Asset = ScriptableObject.CreateInstance(ClassName);
            string Folder = scriptableObjectEditor.CreateAssetPath + $"/{ClassName}/";
            Directory.CreateDirectory(Folder);//如果文件夹不存在就创建它
            AssetDatabase.CreateAsset(Asset, Folder + $"{AssetName}.asset");
            return Asset;
        }

        /// <summary>
        /// 创建列表资源
        /// </summary>
        public ListAssetBase CreateListAsset()
        {
            listAsset = ScriptableObject.CreateInstance(monoListScript.name) as ListAssetBase;
            string Folder = scriptableObjectEditor.CreateAssetPath;
            Directory.CreateDirectory(Folder);//如果文件夹不存在就创建它
            AssetDatabase.CreateAsset(listAsset, Folder + $"/{monoListScript.name}.asset");
            return listAsset;
        }

        #endregion


        #region 删除

        /// <summary>
        /// 删除所有资源
        /// </summary>
        public void DeleteAllAsset()
        {
            while (Assets.Count != 0)
            {
                if (Assets[0] != null)
                {
                    DeleteAsset(Assets[0]);
                }
            }
        }

        /// <summary>
        /// 删除一个资源
        /// </summary>
        public void DeleteAsset(ScriptableObject scriptableObject)
        {
            Assets.Remove(scriptableObject);
            string path = AssetDatabase.GetAssetPath(scriptableObject);
            AssetDatabase.DeleteAsset(path);

            if (listAsset != null)
            {
                listAsset.RemoveAsset(scriptableObject);
                EditorUtility.SetDirty(listAsset);
            }

            if (Assets.Count == 0)
            {
                AssetDatabase.DeleteAsset(Path.GetDirectoryName(path));
            }
        }

        /// <summary>
        /// 删除类脚本
        /// </summary>
        public void DeleteClass()
        {
            if (monoScript)
            {
                DeleteAllAsset();
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(monoScript));
            }
        }
        /// <summary>
        /// 删除列表类脚本和资源
        /// </summary>
        public void DeleteListClass()
        {
            if (listAsset)
            {
                string path = AssetDatabase.GetAssetPath(listAsset);
                AssetDatabase.DeleteAsset(path);
            }
            if (monoListScript)
            {
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(monoListScript));
            }
        }

        #endregion


        #region 判断


        /// <summary>
        /// 判断字段名称为空或重复
        /// </summary>
        public bool IsRepeatFieldName()
        {
            return fields.Any((item) => string.IsNullOrEmpty(item.FieldName) || fields.Count((item1) => item1.FieldName == item.FieldName) != 1);
        }

        /// <summary>
        /// 判断是否可以新建类
        /// </summary>
        public bool IsCreateClass()
        {
            return !(string.IsNullOrEmpty(ClassName) || IsRepeatFieldName() || string.IsNullOrEmpty(scriptableObjectEditor.CreateFilePath));
        }

        /// <summary>
        /// 判断资源命名重复
        /// </summary>
        public bool IsRepeatAssetName()
        {
            return Assets.Any((item) => item.name == AssetName);
        }
        /// <summary>
        /// 判断是否可以显示新建资源的按钮
        /// </summary>
        public bool IsCreateAssetButton()
        {
            return IsShow && !(string.IsNullOrEmpty(AssetName) || string.IsNullOrEmpty(scriptableObjectEditor.CreateAssetPath) || IsRepeatAssetName());
        }

        /// <summary>
        /// 判断是否可以新建资源
        /// </summary>
        public bool IsCreateAsset()
        {
            return monoScript && !(string.IsNullOrEmpty(AssetName) || string.IsNullOrEmpty(scriptableObjectEditor.CreateAssetPath) || IsRepeatAssetName());
        }

        /// <summary>
        /// 判断是否可以新建列表类
        /// </summary>
        public bool IsCreateListClass()
        {
            return IsList && monoScript && monoListScript == null;
        }
        /// <summary>
        /// 判断是否可以新建列表资源
        /// </summary>
        public bool IsCreateListAsset()
        {
            return IsList && monoListScript && listAsset == null;
        }
        #endregion
    }
}
