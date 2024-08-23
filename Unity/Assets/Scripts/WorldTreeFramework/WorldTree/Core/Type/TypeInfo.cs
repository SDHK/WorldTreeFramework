/****************************************

* 作者： 闪电黑客
* 日期： 2023/8/9 14:57

* 描述： 类型信息：用于类型明确时的信息获取
*

*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace WorldTree
{
	/// <summary>
	/// 类型信息：用于类型的信息获取
	/// </summary>
	public static class TypeInfo<T>
	{
		/// <summary>
		/// 类型码
		/// </summary>
		public static long TypeCode = 0;
	}


	/// <summary>
	/// 类型信息：用于类型的信息获取
	/// </summary>
	public class TypeInfo : Node
		, ComponentOf<WorldTreeCore>
		, AsAwake
	{

		/// <summary>
		/// 类型名称哈希码表
		/// </summary>
		private readonly ConcurrentDictionary<string, Type> nameTypeDict = new();

		/// <summary>
		/// 类型哈希码表
		/// </summary>
		private readonly ConcurrentDictionary<Type, long> typeHash64Dict = new();
		/// <summary>
		/// 64位哈希码类型表
		/// </summary>
		private readonly ConcurrentDictionary<long, Type> hash64TypeDict = new();

		public override void OnCreate() { }

		/// <summary>
		/// 清除
		/// </summary>
		public void Clear()
		{
			nameTypeDict.Clear();
			typeHash64Dict.Clear();
			hash64TypeDict.Clear();
		}

		/// <summary>
		/// 重新加载程序集类型
		/// </summary>
		public void ReLoadAssembly(Assembly[] assemblies)
		{
			Clear();
			foreach (Assembly ass in assemblies)
			{
				foreach (Type type in ass.GetTypes())
				{
					Add(type);
				}
			}
		}

		/// <summary>
		/// 类型注册到信息表
		/// </summary>
		public Type Add(Type type)
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
						nameTypeDict[type.FullName] = type;
						typeHash64Dict.TryRemove(oldType, out _);
						typeHash64Dict.TryAdd(type, hash64);
					}
					else
					{
						throw new InvalidOperationException($"64位哈希码冲突 {type} 与 {oldType} {type == oldType}");
					}
				}
				else
				{
					nameTypeDict.TryAdd(type.FullName, type);
					hash64TypeDict.TryAdd(hash64, type);
					typeHash64Dict.TryAdd(type, hash64);
				}
			}
			return type;
		}

		/// <summary>
		/// 类型转64位哈希码
		/// </summary>
		public long TypeToCode(Type type)
		{
			if (!typeHash64Dict.TryGetValue(type, out long hash64))
			{
				Add(type);
				typeHash64Dict.TryGetValue(type, out hash64);
			}
			return hash64;
		}

		/// <summary>
		/// 哈希码64转类型
		/// </summary>
		public Type CodeToType(long rcr) => hash64TypeDict[rcr];
	}
	public static class TypeInfoRule
	{
		/// <summary>
		/// 获取自身类型码
		/// </summary>
		public static long TypeToCode(this IWorldTreeBasic self) => self.Core.TypeInfo.TypeToCode(self.GetType());

		/// <summary>
		/// 获取泛型类型码
		/// </summary>
		public static long TypeToCode<T>(this IWorldTreeBasic self) => self.Core.TypeInfo.TypeToCode(typeof(T));

		/// <summary>
		/// 获取类型码
		/// </summary>
		public static long TypeToCode(this IWorldTreeBasic self, Type type) => self.Core.TypeInfo.TypeToCode(type);

		/// <summary>
		/// 获取类型
		/// </summary>
		public static Type CodeToType(this IWorldTreeBasic self, long typeCode) => self.Core.TypeInfo.CodeToType(typeCode);

	}
}