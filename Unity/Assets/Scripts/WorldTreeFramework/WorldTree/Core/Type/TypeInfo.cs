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
using System.Security.Cryptography;

namespace WorldTree
{
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
		public readonly ConcurrentDictionary<string, Type> NameTypeDict = new();
		/// <summary>
		/// 类型哈希码表
		/// </summary>
		public readonly ConcurrentDictionary<Type, long> TypeHash64Dict = new();
		/// <summary>
		/// 64位哈希码类型表
		/// </summary>
		public readonly ConcurrentDictionary<long, Type> Hash64TypeDict = new();

		public override void OnCreate()
		{
		}

		/// <summary>
		/// 清除
		/// </summary>
		public void Clear()
		{
			NameTypeDict.Clear();
			TypeHash64Dict.Clear();
			Hash64TypeDict.Clear();
		}

		/// <summary>
		/// 重新加载程序集类型
		/// </summary>
		public void LoadAssembly(Assembly[] assemblies)
		{
			int typeCount = 0;
			foreach (Assembly assembly in assemblies)
			{
				var types = assembly.GetTypes();
				foreach (Type type in types) OverlayType(type);
				this.Log($"重载程序集 {assembly.FullName}, 类型数量 {types.Length}");
				typeCount += types.Length;
			}
			this.Log($"加载程序集数量 {assemblies.Length}, 类型数量 {typeCount}");
		}

		/// <summary>
		/// 覆盖或添加类型
		/// </summary>
		public void OverlayType(Type type)
		{
			if (NameTypeDict.Remove(type.FullName, out Type oldType))
			{
				if (TypeHash64Dict.Remove(oldType, out long typeCode))
				{
					Hash64TypeDict[typeCode] = type;
					TypeHash64Dict.TryAdd(type, typeCode);
				}
				NameTypeDict.TryAdd(type.FullName, type);
			}
			else
			{
				Add(type);
			}
		}

		/// <summary>
		/// 类型注册到信息表
		/// </summary>
		public Type Add(Type type)
		{
			if (!TypeHash64Dict.ContainsKey(type))
			{
				type.GetHashCode();
				long hash64 = type.AssemblyQualifiedName.GetHash64();

				if (Hash64TypeDict.TryGetValue(hash64, out Type oldType))
				{
					if (type.FullName == oldType.FullName)
					{
						Hash64TypeDict[hash64] = type;
						NameTypeDict[type.FullName] = type;
						TypeHash64Dict.TryRemove(oldType, out _);
						TypeHash64Dict.TryAdd(type, hash64);
					}
					else
					{
						throw new InvalidOperationException($"64位哈希码冲突 {type} 与 {oldType} {type == oldType}");
					}
				}
				else
				{
					NameTypeDict.TryAdd(type.FullName, type);
					Hash64TypeDict.TryAdd(hash64, type);
					TypeHash64Dict.TryAdd(type, hash64);
				}
			}
			return type;
		}

		/// <summary>
		/// 类型转64位哈希码
		/// </summary>
		public long TypeToCode(Type type)
		{
			if (!TypeHash64Dict.TryGetValue(type, out long hash64))
			{
				Add(type);
				TypeHash64Dict.TryGetValue(type, out hash64);
			}
			return hash64;
		}

		/// <summary>
		/// 哈希码64转类型
		/// </summary>
		public Type CodeToType(long rcr) => Hash64TypeDict[rcr];
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