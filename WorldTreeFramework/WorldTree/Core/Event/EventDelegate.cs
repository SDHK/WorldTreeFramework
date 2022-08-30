
/******************************

 * 作者: 闪电黑客
 * 日期: 2021/10/18 6:31:30

 * 描述: 
 
    事件委托 ：继承了泛型对象池

    以委托的不同类型为键值，实现类似函数重载功能

    主要让"事件"有重载的功能（多播委托），
    可以有不同的类型参数，不同数量的参数，以及不同类型的返回值
    
*/
/****************************************

* 作者： 闪电黑客
* 日期： 2022/6/26 11:12

* 描述： 修改为继承 Entity 并入ECS树

*/


using System;


namespace WorldTree
{

    /// <summary>
    /// 事件委托 
    /// </summary>
    public class EventDelegate : Entity
    {
        private UnitDictionary<Type, UnitList<Delegate>> events;

        /// <summary>
        /// 添加一个委托
        /// </summary>
        public void AddDelegate(Delegate action)
        {
            if (events == null)
            {
                events = Root.ObjectPoolManager.Get<UnitDictionary<Type, UnitList<Delegate>>>();
            }

            Type key = action.GetType();

            if (events.ContainsKey(key))
            {
                events[key].Add(action);
            }
            else
            {
                UnitList<Delegate> delegates = Root.ObjectPoolManager.Get<UnitList<Delegate>>();
                delegates.Add(action);
                events.Add(key, delegates);
            }

        }

        /// <summary>
        /// 删除一个委托
        /// </summary>
        public void RemoveDelegate(Delegate action)
        {
            Type key = action.GetType();
            if (events.ContainsKey(key))
            {
                var delegates = events[key];
                delegates.Remove(action);
                if (delegates.Count == 0)
                {
                    delegates.Recycle();
                    events.Remove(key);
                    if (events.Count == 0)
                    {
                        events.Recycle();
                        events = null;
                    }
                }
            }
        }

        /// <summary>
        /// 删除一个类型的全部委托
        /// </summary>
        public void Remove(Type key)
        {
            if (events.ContainsKey(key))
            {
                events[key].Clear();
                events[key].Recycle();
                events.Remove(key);
                if (events.Count == 0)
                {
                    events.Recycle();
                    events = null;
                }
            }
        }

        /// <summary>
        ///  删除一个类型的全部委托
        /// </summary>
        public void Remove<T>()
        {
            events.Remove(typeof(T));
        }

        /// <summary>
        /// 获取一个类型的全部委托
        /// </summary>
        public UnitList<Delegate> Get(Type key)
        {
            events.TryGetValue(key, out UnitList<Delegate> delegates);
            return delegates;
        }

        /// <summary>
        /// 获取一个类型的全部委托
        /// </summary>
        public UnitList<Delegate> Get<T>() { return Get(typeof(T)); }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            foreach (var item in events)
            {
                var delegates = item.Value;
                delegates.Clear();
                delegates.Recycle();
            }
            events.Clear();
            events.Recycle();
            events = null;
        }

        class EventDelegateRecycleSystem : RecycleSystem<EventDelegate>
        {
            public override void OnRecycle(EventDelegate self)
            {
                self.Clear();
            }
        }

    }
}