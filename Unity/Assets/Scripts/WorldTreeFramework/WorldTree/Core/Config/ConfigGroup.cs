namespace WorldTree
{
	/// <summary>
	/// 配置集合
	/// </summary>
	public abstract class ConfigGroup : Node
		, AsAwake
		, ComponentOf<ConfigManager>
	{

	}

	/// <summary>
	/// 配置集合
	/// </summary>
	public abstract class ConfigGroup<C> : ConfigGroup
		where C : Config
	{

	}


	public static class ConfigGroupRule
	{

	}
}