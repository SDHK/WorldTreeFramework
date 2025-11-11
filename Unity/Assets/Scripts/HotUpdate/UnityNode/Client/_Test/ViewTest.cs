namespace WorldTree
{
	/// <summary>
	/// 视图测试节点 
	/// </summary>
	public class ViewTest : Node
		, ComponentOf<InitialDomain>
		, AsComponentBranch
		, AsRule<Awake>
	{

	}
}
