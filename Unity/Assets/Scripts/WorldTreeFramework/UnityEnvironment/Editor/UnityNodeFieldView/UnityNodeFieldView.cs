namespace WorldTree
{
	/// <summary>
	/// 世界树节点字段信息可视化泛型组件
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class UnityNodeFieldView<T> : Node, IWorldTreeNodeViewBuilder
		, ComponentOf<World>
		, AsRule<Awake>
		, AsINodeFieldViewRule
	{ }


}
