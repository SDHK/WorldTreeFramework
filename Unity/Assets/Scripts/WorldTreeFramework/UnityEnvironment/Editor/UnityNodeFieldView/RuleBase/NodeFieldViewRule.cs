using System.Reflection;

namespace WorldTree
{
	/// <summary>
	/// 世界树节点字段信息可视化法则接口
	/// </summary>
	public interface INodeFieldViewRule : ISendRule<INode, FieldInfo> { }

	public interface AsINodeFieldViewRule : AsRule<INodeFieldViewRule> { }

	/// <summary>
	/// 世界树节点字段信息可视化法则
	/// </summary>
	/// <typeparam name="N"></typeparam>
	public abstract class NodeFieldViewRule<N> : SendRule<N, INodeFieldViewRule, INode, FieldInfo>
		where N : class, INode, AsRule<INodeFieldViewRule>
	{ }
}
