/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/21 21:12

* 描述： 初始化法则
* 
* 用于节点Add添加到世界树上时的构造参数传递
* 
* 在OnGet与OnAdd之间执行

*/

namespace WorldTree
{
    /// <summary>
    /// 初始化法则接口
    /// </summary>
    public interface IAwakeRule : ISendRule { }
    /// <summary>
    /// 初始化法则接口
    /// </summary>
    public interface IAwakeRule<T1> : ISendRule<T1> { }
    /// <summary>
    /// 初始化法则接口
    /// </summary>
    public interface IAwakeRule<T1, T2> : ISendRule<T1, T2> { }
    /// <summary>
    /// 初始化法则接口
    /// </summary>
    public interface IAwakeRule<T1, T2, T3> : ISendRule<T1, T2, T3> { }
    /// <summary>
    /// 初始化法则接口
    /// </summary>
    public interface IAwakeRule<T1, T2, T3, T4> : ISendRule<T1, T2, T3, T4> { }
    /// <summary>
    /// 初始化法则接口
    /// </summary>
    public interface IAwakeRule<T1, T2, T3, T4, T5> : ISendRule<T1, T2, T3, T4, T5> { }


    /// <summary>
    /// 初始化法则
    /// </summary>
    public abstract class AwakeRule<N> : SendRuleBase<IAwakeRule, N> where N : Node { }
    /// <summary>
    /// 初始化法则
    /// </summary>
    public abstract class AwakeRule<N, T1> : SendRuleBase<IAwakeRule<T1>, N, T1> where N : Node { }
    /// <summary>
    /// 初始化法则
    /// </summary>
    public abstract class AwakeRule<N, T1, T2> : SendRuleBase<IAwakeRule<T1, T2>, N, T1, T2> where N : Node { }
    /// <summary>
    /// 初始化法则
    /// </summary>
    public abstract class AwakeRule<N, T1, T2, T3> : SendRuleBase<IAwakeRule<T1, T2, T3>, N, T1, T2, T3> where N : Node { }
    /// <summary>
    /// 初始化法则
    /// </summary>
    public abstract class AwakeRule<N, T1, T2, T3, T4> : SendRuleBase<IAwakeRule<T1, T2, T3, T4>, N, T1, T2, T3, T4> where N : Node { }
    /// <summary>
    /// 初始化法则
    /// </summary>
    public abstract class AwakeRule<N, T1, T2, T3, T4, T5> : SendRuleBase<IAwakeRule<T1, T2, T3, T4, T5>, N, T1, T2, T3, T4, T5> where N : Node { }


}
