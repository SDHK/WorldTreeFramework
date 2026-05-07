namespace WorldTree
{
	/// <summary>
	/// 视图层级元素接口 
	/// </summary>
	public interface IViewLayerElement : IViewElement
	{

	}

	/// <summary>
	/// 视图层绑定 
	/// </summary>
	public class ViewLayerBind : ViewBind
		, AsComponentBranch
	{
		/// 节点列表 
		/// </summary>
		public UnitList<NodeRef<View>> ViewList;

		/// <summary>
		/// ID索引 
		/// </summary>
		public UnitDictionary<long, int> IdIndexDict;
	}

	/// <summary>
	/// 视图层 
	/// </summary>
	public class ViewLayer : View<ViewLayerBind>
		, GenericOf<int, ViewLayerGroupBind>
		, AsComponentBranch
		, AsRule<SubViewClose>
		, AsRule<Awake>

	{
		/// <summary>
		/// 层级数量 
		/// </summary>
		public byte LayerCount => (byte)(ViewBind == null ? 0 : ViewBind.ViewList.Count);
	}
}
