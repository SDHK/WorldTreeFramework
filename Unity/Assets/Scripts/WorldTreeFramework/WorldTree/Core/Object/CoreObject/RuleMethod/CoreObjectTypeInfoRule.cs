using System;

namespace WorldTree
{
	public static class CoreObjectTypeInfoRule
	{
		/// <summary>
		/// 获取自身类型码
		/// </summary>
		public static long TypeToCode(this ICoreObject self) => self.Core.TypeInfo.TypeToCode(self.GetType());

		/// <summary>
		/// 注册并获取泛型类型码
		/// </summary>
		public static long TypeToCode<T>(this ICoreObject self) => self.Core.TypeInfo.TypeToCode(typeof(T));

		/// <summary>
		/// 获取类型码
		/// </summary>	
		public static long TypeToCode(this ICoreObject self, Type type) => self.Core.TypeInfo.TypeToCode(type);

		/// <summary>
		/// 获取类型
		/// </summary>
		public static Type CodeToType(this ICoreObject self, long typeCode) => self.Core.TypeInfo.CodeToType(typeCode);

		/// <summary>
		/// 尝试获取类型
		/// </summary>
		public static bool TryCodeToType(this ICoreObject self, long typeCode, out Type type) => self.Core.TypeInfo.TryCodeToType(typeCode, out type);

	}
}
