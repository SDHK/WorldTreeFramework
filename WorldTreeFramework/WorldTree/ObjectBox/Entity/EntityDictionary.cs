
/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/10 12:00

* 描述： 实体字典

*/

using System.Collections.Generic;

namespace WorldTree
{
    /// <summary>
    /// 实体字典泛型类
    /// </summary>
    /// <typeparam name="K">键</typeparam>
    /// <typeparam name="V">值</typeparam>
    public class EntityDictionary<K, V> : Node
    {
        public Dictionary<K, V> Value;
        public EntityDictionary() : base()
        {
            Value = new Dictionary<K, V>(); //初始化赋值
        }
        public override void OnDispose()
        {
            Value.Clear();
            base.OnDispose();
        }
    }

    public static class EntityDictionaryStaticRule
    {
        /// <summary>
        /// 获取或新建值
        /// </summary>
        /// <remarks>值为实体，则挂为实体字典子节点</remarks>
        public static V GetValueEntity<K, V>(this EntityDictionary<K, V> self, K key)
            where V : Node
        {
            if (!self.Value.TryGetValue(key, out V value))
            {
                value = self.AddChildren<V>();
                self.Value.Add(key, value);
            }
            return value;
        }
    }





}
