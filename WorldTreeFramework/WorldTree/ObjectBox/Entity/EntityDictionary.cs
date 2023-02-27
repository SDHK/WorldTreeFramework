
/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/10 12:00

* 描述： 实体字典

*/

using System.Collections;
using System.Collections.Generic;

namespace WorldTree
{
    /// <summary>
    /// 实体字典基类
    /// </summary>
    public abstract class EntityDictionary : Entity { public IDictionary m_Value; }

    /// <summary>
    /// 实体字典泛型类
    /// </summary>
    /// <typeparam name="K">键</typeparam>
    /// <typeparam name="V">值</typeparam>
    public class EntityDictionary<K, V> : EntityDictionary  //泛型箱
    {
        public EntityDictionary()
        {
            Type = typeof(EntityDictionary); //将这个泛型类的 匹配标签 改为基类
            m_Value = new Dictionary<K, V>(); //初始化赋值
        }
        public Dictionary<K, V> Value => m_Value as Dictionary<K, V>; //强转获取
      
    }

    class EntityDictionaryRemoveSystem : RemoveSystem<EntityDictionary>
    {
        public override void OnEvent(EntityDictionary self)
        {
            self.m_Value.Clear();
        }
    }
}
