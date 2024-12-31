namespace WorldTree
{
	/// <summary>
	/// 树值测试
	/// </summary>
	public class TreeValueTest : Node, ComponentOf<InitialDomain>
		, AsChildBranch
		, AsAwake
	{
		/// <summary>
		/// 树值
		/// </summary>
		public TreeValue<float> valueFloat;
		/// <summary>
		/// 树值
		/// </summary>
		public TreeValue<int> valueInt;
		/// <summary>
		/// 树值
		/// </summary>
		public TreeValue<string> valueString;
		/// <summary>
		/// 树值
		/// </summary>
		public TreeTween<string> treeTween;
	}
}
