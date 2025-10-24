namespace WorldTree
{
	/// <summary>
	/// 视图层绑定 
	/// </summary>
	public class ViewLayerBind : ViewBind
		, AsListNodeBranch
	{
	}

	/// <summary>
	/// 视图层 
	/// </summary>
	public class ViewLayer : View<ViewLayerBind>
		, GenericOf<int, ViewLayerGroupBind>
		, AsRule<Awake>
	{
		/// <summary>
		/// 节点列表 
		/// </summary>
		public UnitList<NodeRef<INode>> NodeList;



	}

}
