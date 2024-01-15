using System.Reflection;

namespace WorldTree
{
	/// <summary>
	/// 世界树节点字段信息可视化法则接口
	/// </summary>
	public interface INodeFieldViewRule : ISendRuleBase<INode, FieldInfo> { }

	/// <summary>
	/// 世界树节点字段信息可视化法则
	/// </summary>
	/// <typeparam name="N"></typeparam>
	public abstract class NodeFieldViewRule<N> : SendRuleBase<N, INodeFieldViewRule, INode, FieldInfo>
		where N : class, INode, AsRule<INodeFieldViewRule>
	{ }
}
