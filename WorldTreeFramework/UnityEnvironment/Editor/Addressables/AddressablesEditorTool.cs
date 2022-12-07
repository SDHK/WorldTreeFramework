using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace EditorTool
{
    public static class AddressablesEditorTool
    {
        public static AddressableAssetSettings Settings
        {
            get { return AddressableAssetSettingsDefaultObject.Settings; }
        }


        /// <summary>
        /// 给某分组添加资源
        /// </summary>
        /// <param name="group"></param>
        /// <param name="assetPath"></param>
        public static AddressableAssetEntry CreateOrMoveEntry(this AddressableAssetGroup group, string assetPath, bool readOnly = false, bool postEvent = false)
        {
            return Settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(assetPath), group, readOnly, postEvent);
        }

        /// <summary>
        /// 获取 或 创建默认设置的分组
        /// </summary>
        /// <param name="groupName">组名</param>
        public static AddressableAssetGroup GetGroup(string groupName)
        {
            return Settings.FindGroup(groupName) ?? Settings.CreateGroup(groupName, false, false, false, Settings.DefaultGroup.Schemas);
        }

    }
}
