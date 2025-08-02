namespace WorldTree
{
	/// <summary>
	/// 配置管理器
	/// </summary>
	public class ConfigManager : Node
		, CoreManagerOf<WorldLine>
		, AsRule<Awake>
		, AsComponentBranch
	{ }

}
