/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using System;
using System.Collections.Concurrent;

namespace WorldTree
{
	/// <summary>
	/// 类型信息表，用于不明确时，类型的信息查表
	/// </summary>
	public static class TypeTable
	{
		/// <summary>
		/// 类型哈希码表
		/// </summary>
		private static readonly ConcurrentDictionary<Type, long> typeHash64Dict = new();
		/// <summary>
		/// 64位哈希码类型表
		/// </summary>
		private static readonly ConcurrentDictionary<long, Type> hash64TypeDict = new();

		/// <summary>
		/// 类型注册到信息表
		/// </summary>
		public static Type Add(Type type)
		{
			if (!typeHash64Dict.ContainsKey(type))
			{
				type.GetHashCode();
				long hash64 = type.AssemblyQualifiedName.GetHash64();

				if (hash64TypeDict.TryGetValue(hash64, out Type oldType))
				{
					if (type.FullName == oldType.FullName)
					{
						hash64TypeDict[hash64] = type;
						typeHash64Dict.TryRemove(oldType, out _);
					}
					else
					{
						throw new InvalidOperationException($"64位哈希码冲突 {type} 与 {oldType} {type == oldType}");
					}
				}
				else
				{
					hash64TypeDict.TryAdd(hash64, type);
					typeHash64Dict.TryAdd(type, hash64);
				}
			}
			return type;
		}

		///// <summary>
		///// 类型转64位哈希码
		///// </summary>
		//public static long TypeToCode(this Type type)
		//{
		//	if (!typeHash64Dict.TryGetValue(type, out long hash64))
		//	{
		//		Add(type);
		//		typeHash64Dict.TryGetValue(type, out hash64);
		//	}
		//	return hash64;
		//}

		///// <summary>
		///// 哈希码64转类型
		///// </summary>
		//public static Type CodeToType(this long rcr) => hash64TypeDict[rcr];
	}
}