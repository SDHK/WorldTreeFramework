namespace WorldTree
{
	public static partial class ViewLayerRule
	{

		/// <summary>
		/// 添加视图数据
		/// </summary>
		public static void AddViewData<V>(this ViewLayer self, V subView)
			where V : View
		{
			self.NodeList.Add(new(subView));
		}




		[NodeRule(nameof(OpenRule<ViewLayer>))]
		private static void OnOpenRule(this ViewLayer self)
		{

		}


	}
}
