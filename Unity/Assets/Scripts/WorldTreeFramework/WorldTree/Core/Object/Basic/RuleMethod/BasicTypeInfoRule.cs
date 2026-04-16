/****************************************

* 作者：闪电黑客
* 日期：2024/11/15 10:10

* 描述：

*/
using System;

namespace WorldTree
{
	public static class BasicTypeInfoRule
	{
		/// <summary>
		/// 获取自身类型码
		/// </summary>
		public static long TypeToCode(this IBasic self) => self.Core.WorldLineManager.TypeInfo.TypeToCode(self.GetType());

		/// <summary>
		/// 注册并获取泛型类型码
		/// </summary>
		public static long TypeToCode<T>(this IBasic self) => self.Core.WorldLineManager.TypeInfo.TypeToCode(typeof(T));

		/// <summary>
		/// 获取类型码
		/// </summary>	
		public static long TypeToCode(this IBasic self, Type type) => self.Core.WorldLineManager.TypeInfo.TypeToCode(type);

		/// <summary>
		/// 获取类型
		/// </summary>
		public static Type CodeToType(this IBasic self, long typeCode) => self.Core.WorldLineManager.TypeInfo.CodeToType(typeCode);

		/// <summary>
		/// 尝试获取类型
		/// </summary>
		public static bool TryCodeToType(this IBasic self, long typeCode, out Type type) => self.Core.WorldLineManager.TypeInfo.TryCodeToType(typeCode, out type);

	}
}