/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/1 9:47

* 描述： 法则集合

*/

using System.Collections.Concurrent;

namespace WorldTree
{

	/// <summary>
	/// 法则集合 接口基类
	/// </summary>
	public interface IRuleGroup { }

	/// <summary>
	/// 法则集合 逆变泛型接口
	/// </summary>
	/// <typeparam name="T">法则类型</typeparam>
	/// <remarks>
	/// <para>主要通过法则类型逆变提示可填写参数</para>
	/// <para> RuleGroup 是没有泛型反射实例的，所以执行参数可能填错</para>
	/// </remarks>
	public interface IRuleGroup<in T> : IRuleGroup where T : IRule { }


	/// <summary>
	/// 法则集合
	/// </summary>
	/// <remarks>
	/// <para>Key是节点类型，Value是类型的法则列表</para>
	/// <para>法则集合储存了 不同节点类型 对应的 同种法则</para>
	/// </remarks>
	public class RuleGroup : ConcurrentDictionary<long, RuleList>, IRuleGroup<IRule>
	{
		/// <summary>
		/// 法则的类型
		/// </summary>
		public long RuleType;
	}
}
