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

        //public static void CreateOrMoveEntry(UnityEngine.Object obj)
        //{
        //    Settings.CreateOrMoveEntry( )
        //}

        // 给某分组添加资源
        static AddressableAssetEntry AddAssetEntry(AddressableAssetGroup group, string assetPath)
        {
            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            AddressableAssetEntry entry = group.entries.FirstOrDefault(e => e.guid == guid);
            if (entry == null)
            {
                entry = Settings.CreateOrMoveEntry(guid, group, false, false);
            }
            // entry.address = address;//设置资源地址(完整路径)
            //entry.address = Path.GetFileNameWithoutExtension(assetPath);//设置资源地址(自身名字);
            entry.SetLabel(group.Name, true, false, false);//设置资源标签（组名）
            return entry;
        }

        /// <summary>
        /// 创建分组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public static AddressableAssetGroup CreateGroup<T>(string groupName)
        {
            AddressableAssetGroup group = Settings.FindGroup(groupName);
            if (group == null)
                group = Settings.CreateGroup(groupName, false, false, false, Settings.DefaultGroup.Schemas, typeof(T));//没有组就新建组，设置默认组的Schemas
            Settings.AddLabel(groupName, false);//添加组标签

            return group;
        }
    }
}
