using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WorldTree
{

    [Serializable]
    public struct KeyValue<K, V>
    {
        [ShowInInspector]
        public K key;
        [ShowInInspector]
        public V value;
    }

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
                if (List.Count != Dictionary.Count)
                {
                    ToDictionary();
                }
                return dictionary;
            }
        }

        /// <summary>
        /// List转为字典
        /// </summary>
        public void ToDictionary()
        {
            foreach (var item in List)
            {
                dictionary.TryAdd(item.key, item.value);
            }
        }
    }
}
