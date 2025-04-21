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
	public abstract class ConfigGroup<K, C> : ConfigGroup
		, AsGenericBranch<K>

		where C : Config<K>
	{


	}


	public static class ConfigGroupRule
	{

	}
}