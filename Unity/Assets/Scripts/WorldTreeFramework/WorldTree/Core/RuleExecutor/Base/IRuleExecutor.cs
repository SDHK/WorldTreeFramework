/****************************************

* 作者： 闪电黑客
* 日期： 2023/8/30 19:23

* 描述：
*

*/

namespace WorldTree
{
	/// <summary>
	/// 法则执行器操作方法接口
	/// </summary>
	public interface IRuleExecutor : INode
	{
		/// <summary>
		/// 移除节点
		/// </summary>
		public void Remove(long id);

		/// <summary>
		/// 移除节点
		/// </summary>
		public void Remove(INode node);

		/// <summary>
		/// 清除
		/// </summary>
		public void Clear();
	}


	/// <summary>
	/// 法则执行器调用接口，逆变泛型限制
	/// </summary>
	/// <typeparam name="R">法则类型</typeparam>
	/// <remarks>
	/// <para>主要作用是通过法则类型逆变提示可填写参数</para>
	/// </remarks>

	public interface IRuleExecutor<in R> : IRuleExecutor where R : IRule { }

	/// <summary>
	/// 法则执行器遍历接口
	/// </summary>
	/// <remarks>让执行器可以遍历</remarks>
	public interface IRuleExecutorEnumerable : INode
	{
		/// <summary>
		/// 动态的遍历数量
		/// </summary>
		/// <remarks>当遍历时移除后，在发生抵消的时候减少数量</remarks>
		public int TraversalCount { get; }

		/// <summary>
		/// 刷新遍历数量
		/// </summary>
		public int RefreshTraversalCount();

		/// <summary>
		/// 尝试出列
		/// </summary>
		public bool TryDequeue(out INode node, out RuleList ruleList);

		/// <summary>
		/// 尝试获取队顶
		/// </summary>
		public bool TryPeek(out INode node, out RuleList ruleList);

	}



	/// <summary>
	/// 格式化器 
	/// </summary>
	public static class IRuleExecutorFormatterRule
	{
		class TreeDataSerialize<R> : TreeDataSerializeRule<IRuleExecutor<R>>
			where R : IRule
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref SerializedTypeMode typeMode)
			{
				if (self.TryWriteDataHead(value, typeMode, 0, out IRuleExecutor<R> obj, false, false)) return;
			}
		}
		class TreeDataDeserialize<R> : TreeDataDeserializeRule<IRuleExecutor<R>>
			where R : IRule
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref int fieldNameCode)
			{
				int typePoint = self.ReadPoint;
				if (self.TryReadClassHead(typeof(IRuleExecutor<R>), ref value, out int count, out int objId, out int jumpReadPoint)) return;
				self.ReadJump(typePoint);
				self.SkipData();
				if (jumpReadPoint != TreeDataCode.NULL_OBJECT) self.ReadJump(jumpReadPoint);
			}
		}
	}


	//======================================

	/// <summary>
	/// 法则集合执行器基类接口
	/// </summary>
	public interface IRuleGroupExecutor : IRuleExecutor
	{
		/// <summary>
		/// 尝试添加节点
		/// </summary>
		public bool TryAdd(INode node);
	}

	/// <summary>
	/// 法则列表执行器基类接口
	/// </summary>
	public interface IRuleListExecutor : IRuleExecutor
	{
		/// <summary>
		/// 尝试添加节点与对应法则
		/// </summary>
		public bool TryAdd(INode node, RuleList ruleList);
	}
}