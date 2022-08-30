
/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/1 9:47

* 描述： 添加事件系统
* 
* 在加入节点时触发事件

*/

namespace WorldTree
{
    /// <summary>
    /// 添加事件系统接口
    /// </summary>
    public interface IAddSystem : ISystem
    {
        void Add(Entity self);
    }

    /// <summary>
    /// 添加事件系统
    /// </summary>
    public abstract class AddSystem<T> : SystemBase<T,IAddSystem>, IAddSystem
        where T : Entity
    {
        public void Add(Entity self) => OnAdd(self as T);
        public abstract void OnAdd(T self);

    }

}
