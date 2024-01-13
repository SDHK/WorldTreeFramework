namespace WorldTree
{
	/// <summary>
	/// 世界树节点字段信息可视化泛型组件
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class WorldTreeNodeFieldInfoUnityView<T> : Node, IWorldTreeNodeView
		, ComponentOf<WorldTreeRoot>
		, AsRule<IAwakeRule>
		, AsRule<IWorldTreeNodeFieldInfoViewRule>
	{ }

	
}
