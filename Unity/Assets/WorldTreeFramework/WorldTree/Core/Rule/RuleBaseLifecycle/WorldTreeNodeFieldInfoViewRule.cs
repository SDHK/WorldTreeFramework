using System.Reflection;

namespace WorldTree
{
	/// <summary>
	/// 世界树节点字段信息可视化法则接口
	/// </summary>
	public interface IWorldTreeNodeFieldInfoViewRule : ISendRuleBase<INode, FieldInfo> { }

	/// <summary>
	/// 世界树节点字段信息可视化法则
	/// </summary>
	/// <typeparam name="N"></typeparam>
	public abstract class WorldTreeNodeFieldInfoViewRule<N> : SendRuleBase<N, IWorldTreeNodeFieldInfoViewRule, INode, FieldInfo>
		where N : class, INode, AsRule<IWorldTreeNodeFieldInfoViewRule>
	{ }
}
