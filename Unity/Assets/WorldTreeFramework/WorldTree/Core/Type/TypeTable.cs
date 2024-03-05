﻿/****************************************

* 作者： 闪电黑客
* 日期： 2023/8/9 14:58

* 描述： 类型信息表，用于不明确时类型的信息查表

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
		private static readonly ConcurrentDictionary<Type, long> TypeHash64 = new();
		private static readonly ConcurrentDictionary<long, Type> Hash64Type = new();

		/// <summary>
		/// 类型注册到信息表
		/// </summary>
		public static Type Add(this Type type)
		{
			if (!TypeHash64.ContainsKey(type))
			{
				type.GetHashCode();
				long hash64 = type.AssemblyQualifiedName.GetHash64();

				if (Hash64Type.TryGetValue(hash64, out Type oldType))
				{
					if (type.FullName == oldType.FullName)
					{
						Hash64Type[hash64] = type;
						TypeHash64.TryRemove(oldType, out _);
					}
					else
					{
						throw new InvalidOperationException($"64位哈希码冲突 {type} 与 {oldType} {type == oldType}");
					}
				}
				else
				{
					Hash64Type.TryAdd(hash64, type);
					TypeHash64.TryAdd(type, hash64);
				}
			}
			return type;
		}

		/// <summary>
		/// 类型转64位哈希码
		/// </summary>
		public static long TypeToCore(this Type type)
		{
			if (!TypeHash64.TryGetValue(type, out long hash64))
			{
				Add(type);
				TypeHash64.TryGetValue(type, out hash64);
			}
			return hash64;
		}

		/// <summary>
		/// 哈希码64转类型
		/// </summary>
		public static Type CoreToType(this long rcr) => Hash64Type[rcr];
	}
}