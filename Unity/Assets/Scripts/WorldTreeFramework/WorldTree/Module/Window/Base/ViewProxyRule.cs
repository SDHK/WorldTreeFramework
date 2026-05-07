namespace WorldTree
{
	public static class ViewProxyRule
	{

		/// <summary>
		/// 视图层级变更时调用 
		/// </summary>
		public static void LayerChanged(View self)
		{
			//节点释放前序遍历处理,节点释放后续遍历处理
			NodeBranchTraversalHelper.TraversalPreorder(self, OnLayerChanged, false);
		}
		/// <summary>
		/// 视图层级变更时调用 
		/// </summary>
		public static void OnLayerChanged(INode self)
		{
			if (self is View) NodeRuleHelper.TrySendRule(self, default(LayerChange));
		}



	}

}
