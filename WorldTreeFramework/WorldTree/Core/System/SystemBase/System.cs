/****************************************

* 创 建 者：  闪电黑客
* 创建时间：  2022/5/6 21:31
* 
* 描    述: 系统基类
* 
* 是全部系统的基类

*/

using System;


namespace WorldTree
{

    /// <summary>
    /// 系统接口
    /// </summary>
    public interface ISystem
    {
        Type EntityType { get; }
        Type SystemType { get; }
    }

    /// <summary>
    /// 系统基类
    /// </summary>
    public abstract class SystemBase<T, S> : ISystem
    {
        public virtual Type EntityType => typeof(T);
        public virtual Type SystemType => typeof(S);
    }




    public abstract class SendSystem<E, T1, T2> : ISystem
    where E : class
    {
        public virtual Type EntityType => typeof(E);
        public virtual Type SystemType => GetType();

        public void Invoke(object self, T1 arg1, T2 arg2) => Event(self as E, arg1, arg2);
        public abstract void Event(E self, T1 arg1, T2 arg2);
    }

    public abstract class SendSystem<E, T1, T2, T3> : ISystem
    where E : class
    {
        public virtual Type EntityType => typeof(E);
        public virtual Type SystemType => GetType();

        public void Invoke(object self, T1 arg1, T2 arg2, T3 arg3) => Event(self as E, arg1, arg2, arg3);
        public abstract void Event(E self, T1 arg1, T2 arg2, T3 arg3);
    }



    //接口实现的作用,调用的还是 统一转换成SendSystem基类

    //思路为 e.send<IEsystem>(1); 
    //标签继承的是抽象类，然后以类名当接口 ，这样就算没实现也能调用，oop没实现是写不了这句的
    //考虑直接用泛型接口？
    //统一最底层系统

    // void send<sys,T,T2>(this T self,T2 t2){}
    //fore root.systemManager.GetSystem<sys>()

    // as SendSystem<T,T2> sys.Send(self,t2);

    // self.Call(1,2,3).Value<int>()




}
