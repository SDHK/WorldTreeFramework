namespace WorldTree
{
	/// <summary>
	/// 配置
	/// </summary>
	public abstract class Config : Node
		, TypeNodeOf<int, ConfigGroup>
	{

	}

	/// <summary>
	/// 配置
	/// </summary>
	public abstract class Config<K> : Config
		, AsAwake<K>
		, TypeNodeOf<long, ConfigGroup>
	{
		/// <summary>
		/// 配置ID
		/// </summary>
		public K ConfigId;
	}

	public static class ConfigRule
	{
		class Awake<K> : AwakeRule<Config<K>, K>
		{
			protected override void Execute(Config<K> self, K key)
			{
				self.ConfigId = key;
			}
		}
	}
}
