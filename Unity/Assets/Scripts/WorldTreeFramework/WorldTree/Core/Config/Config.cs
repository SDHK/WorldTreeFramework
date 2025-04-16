namespace WorldTree
{
	/// <summary>
	/// 配置
	/// </summary>
	public abstract class Config : Node
	{

	}

	/// <summary>
	/// 配置
	/// </summary>
	public abstract class Config<K> : Config
		, AsAwake
		, GenericOf<K, ConfigGroup>
	{
		/// <summary>
		/// 配置ID
		/// </summary>
		public K ConfigId;
	}

	public static class ConfigRule
	{
		class Awake<K> : AwakeRule<Config<K>>
		{
			protected override void Execute(Config<K> self)
			{
				self.ConfigId = self.GetKey<K>();
			}
		}
	}
}
