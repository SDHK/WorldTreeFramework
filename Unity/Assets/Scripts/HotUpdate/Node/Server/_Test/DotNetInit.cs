using System;

namespace WorldTree
{
	/// <summary>
	/// ²âÊÔ½Úµã
	/// </summary>
	public partial class DotNetInit : Node, ComponentOf<INode>
		, AsComponentBranch
		, AsAwake
	{ 
		public Action action;
	}
}