/****************************************

* 作者：闪电黑客
* 日期：2024/8/20 17:35

* 描述：

*/
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace WorldTree
{

	/// <summary>
	/// 树数据类型
	/// </summary>
	public static class TreeDataType
	{
		/// <summary>
		/// 基础值类型
		/// </summary>
		public static Dictionary<Type, int> TypeDict = new()
		{
			[typeof(bool)] = 1,
			[typeof(byte)] = 1,
			[typeof(sbyte)] = 1,
			[typeof(short)] = 2,
			[typeof(ushort)] = 2,
			[typeof(uint)] = 4,
			[typeof(int)] = 4,
			[typeof(long)] = 8,
			[typeof(ulong)] = 8,
			[typeof(float)] = 4,
			[typeof(double)] = 8,
			[typeof(decimal)] = 16,
			[typeof(char)] = 4,
		};
	}


	public static class TreeDataByteSequenceRule
	{
		class AddRule : AddRule<TreeDataByteSequence>
		{
			protected override void Execute(TreeDataByteSequence self)
			{
				self.GetBaseRule<TreeDataByteSequence, ByteSequence, Add>().Send(self);

				self.Core.PoolGetUnit(out self.TypeToCodeDict);
				self.Core.PoolGetUnit(out self.CodeToTypeDict);
				self.Core.PoolGetUnit(out self.codeToTypeNameDict);
				self.Core.PoolGetUnit(out self.codeToNameDict);
			}
		}

		class RemoveRule : RemoveRule<TreeDataByteSequence>
		{
			protected override void Execute(TreeDataByteSequence self)
			{
				self.indexCode = 0;

				self.TypeToCodeDict.Dispose();
				self.CodeToTypeDict.Dispose();
				self.codeToTypeNameDict.Dispose();
				self.codeToNameDict.Dispose();

			}
		}
	}

	/// <summary>
	/// 树数据字节序列
	/// </summary>
	public class TreeDataByteSequence : ByteSequence
		, AsRule<TreeDataSerialize>
		, AsRule<TreeDataDeserialize>
	{
		/// <summary>
		/// 短类型名称正则表达式
		/// </summary>
		public static Regex ShortTypeNameRegex = new Regex(@", Version=\d+.\d+.\d+.\d+, Culture=[\w-]+, PublicKeyToken=(?:null|[a-f0-9]{16})", RegexOptions.Compiled);

		/// <summary>
		/// 累计类型码
		/// </summary>
		public int indexCode;

		/// <summary>
		/// 类型对应类型码字典，32哈希码对应
		/// </summary>
		public UnitDictionary<Type, int> TypeToCodeDict;

		/// <summary>
		/// 类型码对应名称字典，32哈希码对应
		/// </summary>
		public UnitDictionary<int, Type> CodeToTypeDict;

		/// <summary>
		/// 类型码对应类型名称字典，32哈希码对应
		/// </summary>
		public UnitDictionary<int, string> codeToTypeNameDict;

		/// <summary>
		/// 字段码对应名称字典，32哈希码对应，代码生成直接使用int对比
		/// </summary>
		public UnitDictionary<int, string> codeToNameDict;

		/// <summary>
		/// 类型码判断
		/// </summary>
		public bool ContainsTypeCode(int typeCode)
			=> CodeToTypeDict.ContainsKey(typeCode);

		/// <summary>
		/// 名称码判断
		/// </summary>
		public bool ContainsNameCode(int nameCode)
			=> codeToNameDict.ContainsKey(nameCode);


		/// <summary>
		/// 序列化
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		public void Serialize<T>(in T value)
		{
			WriteValue(typeof(T), ref Unsafe.AsRef<object>(value));
			int startPoint = Length;
			//写入类型码

			//写入类型数量
			WriteUnmanaged(TypeToCodeDict.Count);
			foreach (var item in TypeToCodeDict)
			{
				WriteUnmanaged(item.Value);
				WriteString(ShortTypeNameRegex.Replace(item.Key.AssemblyQualifiedName, ""));
			}
			//写入字段数量
			WriteUnmanaged(codeToNameDict.Count);
			foreach (var item in codeToNameDict)
			{
				WriteUnmanaged(item.Key);
				WriteString(item.Value);
			}
			//写入映射表起始位置
			WriteUnmanaged(length - startPoint);
		}

		/// <summary>
		/// 反序列化
		/// </summary>
		public unsafe void Deserialize<T>(ref T value)
		{
			//读取指针定位到最后
			readPoint = length;
			readBytePoint = 0;
			readSegmentPoint = segmentList.Count;
			//回退4位
			ReadBack(4);
			//读取映射表起始位置距离
			ReadUnmanaged(out int offset);
			//回退到映射表起始位置
			ReadBack(offset + 4);

			//读取类型数量
			ReadUnmanaged(out int typeCount);
			for (int i = 0; i < typeCount; i++)
			{
				ReadUnmanaged(out int typeCode);
				string typeName = ReadString();
				codeToTypeNameDict.Add(typeCode, typeName);
			}
			//读取字段数量
			ReadUnmanaged(out int nameCount);
			for (int i = 0; i < nameCount; i++)
			{
				ReadUnmanaged(out int nameCode);
				string name = ReadString();
				codeToNameDict.Add(nameCode, name);
			}

			//读取指针定位到数据起始位置
			readPoint = 0;
			readBytePoint = 0;
			readSegmentPoint = 0;
			//读取数据
			ReadValue(typeof(T), ref Unsafe.AsRef<object>(Unsafe.AsPointer(ref value)));
		}

		/// <summary>
		/// 添加类型
		/// </summary>
		public int AddTypeCode(Type type)
		{
			if (!TypeToCodeDict.TryGetValue(type, out int typeCode))
			{
				typeCode = indexCode;
				TypeToCodeDict.Add(type, indexCode++);
			}
			return typeCode;
		}

		/// <summary>
		/// 添加名称码
		/// </summary>
		public void AddNameCode(int nameCode, string name)
		{
			codeToNameDict.Add(nameCode, name);
		}

		/// <summary>
		/// 尝试获取字段名称
		/// </summary>
		public void TryGetName(int nameCode, out string name)
		{
			codeToNameDict.TryGetValue(nameCode, out name);
		}

		/// <summary>
		/// 尝试获取类型
		/// </summary>
		public bool TryGetType(int typeCode, out Type type)
		{
			if (!CodeToTypeDict.TryGetValue(typeCode, out type))
			{
				if (codeToTypeNameDict.TryGetValue(typeCode, out string typeName))
				{
					type = System.Type.GetType(typeName);
					CodeToTypeDict.Add(typeCode, type);
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 写入值
		/// </summary>
		public void WriteValue<T>(in T value)
		{
			WriteValue(typeof(T), ref Unsafe.AsRef<object>(value));
		}

		/// <summary>
		/// 写入值
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValue(Type type, ref object value)
		{
			long typeCode = this.Core.TypeToCode(type);
			this.Core.RuleManager.SupportNodeRule(typeCode);
			if (this.Core.RuleManager.TryGetRuleList<TreeDataSerialize>(typeCode, out RuleList ruleList))
			{
				((IRuleList<TreeDataSerialize>)ruleList).SendRef(this, ref value);
			}
		}


		/// <summary>
		/// 读取值
		/// </summary>
		public void ReadValue<T>(ref T value)
		{
			object obj = value;
			ReadValue(typeof(T), ref obj);
			value = (T)obj;
		}

		/// <summary>
		/// 读取值
		/// </summary>
		public void ReadValue(Type type, ref object value)
		{
			long typeCode = this.Core.TypeToCode(type);
			this.Core.RuleManager.SupportNodeRule(typeCode);
			if (this.Core.RuleManager.TryGetRuleList<TreeDataDeserialize>(typeCode, out RuleList ruleList))
			{
				((IRuleList<TreeDataDeserialize>)ruleList).SendRef(this, ref value);
			}
		}
	}
}
