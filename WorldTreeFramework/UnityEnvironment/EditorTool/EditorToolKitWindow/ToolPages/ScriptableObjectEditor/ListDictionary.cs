
/****************************************

* 作者： 闪电黑客
* 日期： 2022/10/17 10:29

* 描述： 列表字典
* 
* 字典的序列化解决办法，
* 因为字典无法序列化，
* 所以先用List存储序列化。
* 读取时通过List转为Dictionary

*/

using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WorldTree
{
    /// <summary>
    /// 键值结构体
    /// </summary>
    [Serializable]
    public struct KeyValue<K, V>
    {
        [ShowInInspector]
        public K key;
        [ShowInInspector]
        public V value;
    }
    /// <summary>
    /// 列表字典
    /// </summary>
    /// <typeparam name="Key">键</typeparam>
    /// <typeparam name="Value">值</typeparam>
    [Serializable]
    public class ListDictionary<Key, Value>
    {
        [TableList]
        public List<KeyValue<Key, Value>> List;
        [HideInInspector]
        private Dictionary<Key, Value> dictionary;
        [HideInInspector]
        public Dictionary<Key, Value> Dictionary
        {
            get
            {
                if (dictionary == null)
                {
                    dictionary = new Dictionary<Key, Value>();
                    foreach (var item in List)
                    {
                        dictionary.TryAdd(item.key, item.value);
                    }
                }
                return dictionary;
            }
        }
    }
}
