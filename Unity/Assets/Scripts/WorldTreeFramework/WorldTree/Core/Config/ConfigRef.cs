﻿namespace WorldTree
{
	/// <summary>
	/// 配置引用
	/// </summary>
	public struct ConfigRef<C>
		where C : Config
	{
		/// <summary>
		/// 核心
		/// </summary>
		public WorldLine Core;

		/// <summary>
		/// 配置
		/// </summary>
		public C Config;

		/// <summary>
		/// 配置ID
		/// </summary>
		public long Id;
	}

}
