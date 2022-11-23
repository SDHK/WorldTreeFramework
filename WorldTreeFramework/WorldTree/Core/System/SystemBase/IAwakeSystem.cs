/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/21 21:12

* 描述： 初始化系统
* 
* 用于Add添加到实体树上时的构造参数传递
* 
* 在OnGet与OnAdd之间执行

*/

namespace WorldTree
{
    public interface IAwakeSystem : ISendSystem { }

    public interface IAwakeSystem<T1> : ISendSystem<T1> { }

    public interface IAwakeSystem<T1, T2> : ISendSystem<T1, T2> { }

    public interface IAwakeSystem<T1, T2, T3> : ISendSystem<T1, T2, T3> { }

    public interface IAwakeSystem<T1, T2, T3, T4> : ISendSystem<T1, T2, T3, T4> { }

    public interface IAwakeSystem<T1, T2, T3, T4, T5> : ISendSystem<T1, T2, T3, T4, T5> { }

    /// <summary>
    /// 初始化系统
    /// </summary>
    public abstract class AwakeSystem<T> : SystemBase<T, IAwakeSystem>, IAwakeSystem
      where T : Entity
    {
        public void Invoke(Entity self) => OnAwake(self as T);
        public abstract void OnAwake(T self);
    }
    /// <summary>
    /// 初始化系统
    /// </summary>
    public abstract class AwakeSystem<T, T1> : SystemBase<T, IAwakeSystem<T1>>, IAwakeSystem<T1>
       where T : Entity
    {
        public void Invoke(Entity self, T1 arg1) => OnAwake(self as T, arg1);
        public abstract void OnAwake(T self, T1 arg1);
    }
    /// <summary>
    /// 初始化系统
    /// </summary>
    public abstract class AwakeSystem<T, T1, T2> : SystemBase<T, IAwakeSystem<T1, T2>>, IAwakeSystem<T1, T2>
       where T : Entity
    {
        public void Invoke(Entity self, T1 arg1, T2 arg2) => OnAwake(self as T, arg1, arg2);
        public abstract void OnAwake(T self, T1 arg1, T2 arg2);
    }
    /// <summary>
    /// 初始化系统
    /// </summary>
    public abstract class AwakeSystem<T, T1, T2, T3> : SystemBase<T, IAwakeSystem<T1, T2, T3>>, IAwakeSystem<T1, T2, T3>
      where T : Entity
    {
        public void Invoke(Entity self, T1 arg1, T2 arg2, T3 arg3) => OnAwake(self as T, arg1, arg2, arg3);
        public abstract void OnAwake(T self, T1 arg1, T2 arg2, T3 arg3);
    }
    /// <summary>
    /// 初始化系统
    /// </summary>
    public abstract class AwakeSystem<T, T1, T2, T3, T4> : SystemBase<T, IAwakeSystem<T1, T2, T3, T4>>, IAwakeSystem<T1, T2, T3, T4>
     where T : Entity
    {
        public void Invoke(Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4) => OnAwake(self as T, arg1, arg2, arg3, arg4);
        public abstract void OnAwake(T self, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }
    /// <summary>
    /// 初始化系统
    /// </summary>
    public abstract class AwakeSystem<T, T1, T2, T3, T4, T5> : SystemBase<T, IAwakeSystem<T1, T2, T3, T4, T5>>, IAwakeSystem<T1, T2, T3, T4, T5>
    where T : Entity
    {
        public void Invoke(Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => OnAwake(self as T, arg1, arg2, arg3, arg4, arg5);
        public abstract void OnAwake(T self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    }
}
