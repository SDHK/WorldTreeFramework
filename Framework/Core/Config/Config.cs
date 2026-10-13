using System;

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
		, AsRule<Awake>
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


	/// <summary>
	/// 树节点数据标记，默认为非恒定类型，恒定类型将不会记录类名到数据中，减少数据体积
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class ConfigMemberAttribute : Attribute
	{
		/// <summary>
		/// 是否为恒定类型
		/// </summary>
		public bool IsConstant = false;
		public ConfigMemberAttribute(Type type, string name, bool isServer = false)
		{
		}
	}
}
