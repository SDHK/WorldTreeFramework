namespace WorldTree
{
	/// <summary>
	/// 配置
	/// </summary>
	public abstract class Config : Node
		, AsAwake
		, NumberNodeOf<ConfigGroup>
		, AsNumberNodeBranch
	{
		/// <summary>
		/// 配置ID
		/// </summary>
		public long ConfigId => this.GetKey<long>();
	}

}
