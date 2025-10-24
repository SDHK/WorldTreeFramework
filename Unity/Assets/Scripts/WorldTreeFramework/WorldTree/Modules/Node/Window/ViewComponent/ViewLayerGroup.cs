namespace WorldTree
{
	/// <summary>
	/// 视图层组 
	/// </summary>
	public class ViewLayerGroup : View<ViewLayerGroupBind>
	{
		/// <summary>
		/// 层级间隔
		/// </summary>
		public int Interval;
	}

	/// <summary>
	/// 视图层组绑定 
	/// </summary>
	public class ViewLayerGroupBind : ViewBind
		, AsGenericBranch<int>
	{

	}
}
