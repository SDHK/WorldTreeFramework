

namespace WorldTree.Server
{
	/// <summary>
	/// 测试代码编译器
	/// </summary>
	public class CodeCompilerTest : Node
		, ComponentOf<DotNetInit>
		, AsComponentBranch
		, AsChildBranch
		, AsRule<Awake>
	{
	}
}