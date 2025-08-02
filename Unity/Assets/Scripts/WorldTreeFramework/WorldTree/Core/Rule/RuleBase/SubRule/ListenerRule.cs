/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/18 9:35

* 描述： 节点监听法则基类
* 
* 主要作用： 给管理器用的节点添加和移除时的事件监听
* 
* 这样就不需要手动将节点添加到管理器，
* 在而是在添加或移除节点的时候，
* 管理器就能监听到指定节点类型的添加移除事件，并且拿到实例。
* 
* 而且监听移除事件，也能防止节点被移除后，管理器忘记手动移除的情况。
* 
* 设定：
* 1.静态指定节点类型。 
*   泛型填写目标节点类型，节点指定后，指定法则是无效的。
*   
* 2.静态指定法则类型。 
*   泛型填写目标节点必须为 INode，才生效
* 
* 3.动态指定： (此功能已删除)
*   节点必须指定为 INode，法则必须指定为 IRule
*   可在运行时随意切换指定目标
*   
* 
*/

namespace WorldTree
{

	/// <summary>
	/// 节点监听法则接口
	/// </summary>
	public interface IListenerRule : ISendRule<INode>, ILifeCycleRule, ISourceGeneratorIgnore
	{
		/// <summary>
		/// 监听目标:节点类型
		/// </summary>
		long TargetNodeType { get; set; }
		/// <summary>
		/// 监听目标:节点法则
		/// </summary>
		long TargetRuleType { get; set; }
	}


	/// <summary>
	/// 节点监听法则抽象基类
	/// </summary>
	/// <remarks>目标为INode和IRule时为动态监听</remarks>
	public abstract class ListenerRule<N, R, TN, TR> : Rule<N, R>, IListenerRule
	where N : class, INode, AsRule<R>
	where TN : class, INode
	where R : IListenerRule
	where TR : IRule
	{
		public virtual long TargetNodeType { get; set; }
		public virtual long TargetRuleType { get; set; }

		override public void OnCreate()
		{
			NodeType = Core.TypeToCode(typeof(N));
			RuleType = Core.TypeToCode(typeof(R));
			TargetNodeType = Core.TypeToCode(typeof(TN));
			TargetRuleType = Core.TypeToCode(typeof(TR));
		}

		public virtual void Invoke(INode self, INode node) => Execute(self as N, node as TN);
		/// <summary>
		/// 执行
		/// </summary>
		/// <param name="self">自身</param>
		/// <param name="node">监听目标</param>
		protected abstract void Execute(N self, TN node);
	}
}
