/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/21 21:12

* 描述： 初始化法则
* 
* 用于节点Add添加到世界树上时的构造参数传递
* 
* 在OnGet与OnAdd之间执行，在全局之前广播前执行。
* 

*/

namespace WorldTree
{
	/// <summary>
	/// 初始化法则接口
	/// </summary>
	public interface IAwakeRule : ITypeInfo<IAwakeRule>, ISendRuleBase { }
	/// <summary>
	/// 初始化法则接口
	/// </summary>
	public interface IAwakeRule<T1> : ITypeInfo<IAwakeRule<T1>>, ISendRuleBase<T1> { }
	/// <summary>
	/// 初始化法则接口
	/// </summary>
	public interface IAwakeRule<T1, T2> : ITypeInfo<IAwakeRule<T1, T2>>, ISendRuleBase<T1, T2> { }
	/// <summary>
	/// 初始化法则接口
	/// </summary>
	public interface IAwakeRule<T1, T2, T3> : ITypeInfo<IAwakeRule<T1, T2, T3>>, ISendRuleBase<T1, T2, T3> { }
	/// <summary>
	/// 初始化法则接口
	/// </summary>
	public interface IAwakeRule<T1, T2, T3, T4> : ITypeInfo<IAwakeRule<T1, T2, T3, T4>>, ISendRuleBase<T1, T2, T3, T4> { }
	/// <summary>
	/// 初始化法则接口
	/// </summary>
	public interface IAwakeRule<T1, T2, T3, T4, T5> : ITypeInfo<IAwakeRule<T1, T2, T3, T4, T5>>, ISendRuleBase<T1, T2, T3, T4, T5> { }


	/// <summary>
	/// 初始化法则
	/// </summary>
	public abstract class AwakeRule<N> : SendRuleBase<N, IAwakeRule> where N : class, INode, AsRule<IAwakeRule> { }
	/// <summary>
	/// 初始化法则
	/// </summary>
	public abstract class AwakeRule<N, T1> : SendRuleBase<N, IAwakeRule<T1>, T1> where N : class, INode, AsRule<IAwakeRule<T1>> { }
	/// <summary>
	/// 初始化法则
	/// </summary>
	public abstract class AwakeRule<N, T1, T2> : SendRuleBase<N, IAwakeRule<T1, T2>, T1, T2> where N : class, INode, AsRule<IAwakeRule<T1, T2>> { }
	/// <summary>
	/// 初始化法则
	/// </summary>
	public abstract class AwakeRule<N, T1, T2, T3> : SendRuleBase<N, IAwakeRule<T1, T2, T3>, T1, T2, T3> where N : class, INode, AsRule<IAwakeRule<T1, T2, T3>> { }
	/// <summary>
	/// 初始化法则
	/// </summary>
	public abstract class AwakeRule<N, T1, T2, T3, T4> : SendRuleBase<N, IAwakeRule<T1, T2, T3, T4>, T1, T2, T3, T4> where N : class, INode, AsRule<IAwakeRule<T1, T2, T3, T4>> { }
	/// <summary>
	/// 初始化法则
	/// </summary>
	public abstract class AwakeRule<N, T1, T2, T3, T4, T5> : SendRuleBase<N, IAwakeRule<T1, T2, T3, T4, T5>, T1, T2, T3, T4, T5> where N : class, INode, AsRule<IAwakeRule<T1, T2, T3, T4, T5>> { }

}
