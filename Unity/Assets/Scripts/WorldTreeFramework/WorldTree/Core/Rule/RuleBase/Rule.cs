/****************************************

* 创 建 者：  闪电黑客
* 创建时间：  2022/5/6 21:31
*
* 描    述: 法则基类
*
* 是全部法则的基类
*
* 设定法则只服务于Node节点，不服务于Unit单位。
* Unit会在框架启动时使用，这时候RuleManager还没有初始化，
* 所以Unit无法使用Rule
* 
* Rule的目标是节点INode而不是Object,
* 主要是为了能拿到节点树上的东西。

*/

using System;

namespace WorldTree
{
	/// <summary>
	/// 法则接口
	/// </summary>
	/// <remarks>
	/// <para>所有法则的最底层接口</para>
	/// <para>主要作用是可以让所有法则统一类型获取标记</para>
	/// </remarks>
	public interface IRule : IUnit
	{
		/// <summary>
		/// 节点类型标记
		/// </summary>
		long NodeType { get; set; }

		/// <summary>
		/// 法则类型标记
		/// </summary>
		long RuleType { get; set; }

		/// <summary>
		/// 法则执行顺序下标
		/// </summary>
		/// <remarks>由框架自动赋值，只读</remarks>
		int RuleIndex { get; set; }
		/// <summary>
		/// 法则执行总数
		/// </summary>
		/// <remarks>由框架自动赋值，只读</remarks>
		int RuleCount { get; set; }
	}

	/// <summary>
	/// 法则抽象基类
	/// </summary>
	/// <remarks>
	/// <para>法则的最底层基类</para>
	/// <para>主要作用是通过泛型给标记赋值</para>
	/// </remarks>
	public abstract class Rule<N, R> : Unit, IRule
	{
		public virtual long NodeType { get; set; }
		public virtual long RuleType { get; set; }

		public int RuleIndex { get; set; }
		public int RuleCount { get; set; }

		override public void OnCreate()
		{
			NodeType = Core.TypeToCode(typeof(N));
			RuleType = Core.TypeToCode(typeof(R));
		}
	}

	/// <summary>
	/// 法则约束
	/// </summary>
	/// <typeparam name="R">法则类型</typeparam>
	/// <remarks>节点拥有的法则，和Where约束搭配形成法则调用限制</remarks>
	public interface AsRule<in R> where R : IRule { }

	/// <summary>
	/// 全局法则
	/// </summary>
	public interface IGlobalRule : IRule { }

	/// <summary>
	/// 生命周期法则标记
	/// </summary>
	public interface ILifeCycleRule : IGlobalRule { }

	/// <summary>
	/// 代码生成器忽略标记
	/// </summary>
	public interface ISourceGeneratorIgnore { }

	/// <summary>
	/// 可调用方法法则标记
	/// </summary>
	public interface IMethodRule : IRule { }


	/// <summary>
	/// 节点法则
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class NodeRuleAttribute : Attribute
	{
		public NodeRuleAttribute(string rule, bool ruleType = false) { }
	}


	/// <summary>
	/// 法则分流
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class RuleSwitchAttribute : Attribute
	{
		public RuleSwitchAttribute(string ruleName, string switchValue, object caseKey) { }
	}

	/// <summary>
	/// 法则类型数组生成标记
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class RuleTypesGeneratorAttribute : Attribute { }



}