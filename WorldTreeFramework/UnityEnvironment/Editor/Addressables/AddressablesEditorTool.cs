
/****************************************

* 作者： 闪电黑客
* 日期： 2022/12/6 16:51

* 描述： 用于Addressables 编辑器工具

*/

using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

namespace EditorTool
{
    /// <summary>
    /// Addressables 编辑器工具
    /// </summary>
    public static class AddressablesEditorTool
    {
        /// <summary>
        /// 默认主设置
        /// </summary>
        public static AddressableAssetSettings Settings => AddressableAssetSettingsDefaultObject.Settings;

        /// <summary>
        /// 给某分组添加资源
        /// </summary>
        public static AddressableAssetEntry CreateOrMoveEntry(this AddressableAssetGroup group, UnityEngine.Object asset, bool readOnly = false, bool postEvent = true)
        {
            return Settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(asset)), group, readOnly, postEvent);
        }

        /// <summary>
        /// 给某分组添加资源
        /// </summary>
        public static AddressableAssetEntry CreateOrMoveEntry(this AddressableAssetGroup group, string assetPath, bool readOnly = false, bool postEvent = true)
        {
            return Settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(assetPath), group, readOnly, postEvent);
        }

        /// <summary>
        /// 移除资源
        /// </summary>
        public static bool RemoveAssetEntry(string assetPath, bool postEvent = true)
        {
            return Settings.RemoveAssetEntry(AssetDatabase.AssetPathToGUID(assetPath).ToString(), postEvent);
        }

        /// <summary>
        /// 移除资源
        /// </summary>
        public static bool RemoveAssetEntry(UnityEngine.Object asset, bool postEvent = true)
        {
            return Settings.RemoveAssetEntry(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(asset)).ToString(), postEvent);
        }
        /// <summary>
        /// 获取 或 创建一个默认设置的分组
        /// </summary>
        public static AddressableAssetGroup GetGroup(string groupName)
        {
            return Settings.FindGroup(groupName) ?? Settings.CreateGroup(groupName, false, false, false, Settings.DefaultGroup.Schemas);
        }

        /// <summary>
        /// 获取或创建分组添加资源
        /// </summary>
        public static void CreateOrMoveEntry(string groupName, string assetPath, string address)
        {
            GetGroup(groupName).CreateOrMoveEntry(assetPath).address = address;
        }

        /// <summary>
        /// 获取或创建分组添加资源
        /// </summary>
        public static void CreateOrMoveEntry(string groupName, UnityEngine.Object asset, string address)
        {
            GetGroup(groupName).CreateOrMoveEntry(asset).address = address;
        }

    }
}
