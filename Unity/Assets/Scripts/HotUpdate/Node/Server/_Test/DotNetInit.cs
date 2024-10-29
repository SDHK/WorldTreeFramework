using System;

namespace WorldTree
{
	/// <summary>
	/// ≤‚ ‘∑®‘Ú123,≤‚ ‘◊¢ Õ
	/// </summary>
	public interface TestRule : ISendRule<float, string>, IMethodRule { }



	/// <summary>
	/// ≤‚ ‘Ω⁄µ„
	/// </summary>
	public partial class DotNetInit : Node, ComponentOf<INode>
		, AsComponentBranch
		, AsAwake
		, AsTestRule
	{
		/// <summary>
		/// ≤‚ ‘
		/// </summary>
		public Action Action;
	}
}