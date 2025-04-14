namespace WorldTree
{
	/// <summary>
	/// 配置集合
	/// </summary>
	public abstract class ConfigGroup : Node
		, AsAwake
		, ComponentOf<ConfigManager>
		, AsNumberNodeBranch
	{

	}

}
