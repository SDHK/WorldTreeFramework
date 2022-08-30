
/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/4 11:30

* 描述： 事件分组特性
* 
* 通过Type为键值分组
* 

*/

using System;

namespace WorldTree
{

    /// <summary>
    /// 事件分组
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class EventKeyAttribute: Attribute
    {
        public Type key { get; private set; }
        public EventKeyAttribute(Type key)
        {
            this.key = key;
        }

        public EventKeyAttribute()
        {
            this.key = typeof(EventDelegate);
        }
    }
}
