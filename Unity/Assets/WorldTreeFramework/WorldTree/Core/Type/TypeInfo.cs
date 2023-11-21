
/****************************************

* 作者： 闪电黑客
* 日期： 2023/8/9 14:57

* 描述： 类型信息：用于类型明确时的信息获取
* 

*/

using System;

namespace WorldTree
{

	/// <summary>
	/// 类型信息：用于类型的信息获取
	/// </summary>
	public static class TypeInfo<T>
	{
		/// <summary>
		/// 空值
		/// </summary>
		public readonly static T Default = default;

		/// <summary>
		/// 类型
		/// </summary>
		public readonly static Type Type = Init();

		/// <summary>
		/// 类型FullName 的 64位哈希码
		/// </summary>
		public readonly static long TypeCode = HashCore.GetHash64(typeof(T).FullName);
		/// <summary>
		/// 类型名称
		/// </summary>
		public readonly static string TypeName = typeof(T).Name;

		private static Type Init()
		{
			return TypeTable.Add(typeof(T));
		}
	}
}
