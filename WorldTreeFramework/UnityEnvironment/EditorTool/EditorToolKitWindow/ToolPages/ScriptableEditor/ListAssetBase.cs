
/****************************************

* 作者： 闪电黑客
* 日期： 2022/10/17 15:16

* 描述： 列表资源抽象基类

*/

using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace WorldTree
{
    /// <summary>
    /// 列表资源基类
    /// </summary>
    public abstract class ListAssetBase : ScriptableObject
    {
        public abstract void AddAsset(ScriptableObject scriptableObject);
        public abstract void RemoveAsset(ScriptableObject scriptableObject);
        public abstract void Clear();
    }


    /// <summary>
    /// 列表资源泛型基类
    /// </summary>
    [Searchable]
    public abstract class ListAssetBase<T> : ListAssetBase
        where T : ScriptableObject
    {
        /// <summary>
        /// 列表
        /// </summary>
        [TableList(ShowIndexLabels = true, ShowPaging = true, IsReadOnly = true,AlwaysExpanded =true)]
        public List<T> List =new List<T>();
        private Dictionary<string, T> dictionary;

        /// <summary>
        /// 字典
        /// </summary>
        public Dictionary<string, T> Dictionary
        {
            get
            {
                if (dictionary == null)
                {
                    dictionary = new Dictionary<string, T>();
                    foreach (var item in List)
                    {
                        dictionary.TryAdd(item.name, item);
                    }
                }
                return dictionary;
            }
        }

        public override void AddAsset(ScriptableObject scriptableObject)
        {
            if (!List.Contains((T)scriptableObject))
            {
                List.Add((T)scriptableObject);
            }
        }

        public override void RemoveAsset(ScriptableObject scriptableObject)
        {
            if (List.Contains((T)scriptableObject))
            {
                List.Remove((T)scriptableObject);
            }
        }
        public override void Clear()
        {
            List.Clear();
        }
    }
}
