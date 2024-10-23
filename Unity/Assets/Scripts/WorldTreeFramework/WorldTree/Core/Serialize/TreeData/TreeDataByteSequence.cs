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
		/// 基础值类型,字节长度
		/// </summary>
		public static Dictionary<Type, int> TypeDict = new()
		{
			[typeof(bool)] = 1,
			[typeof(byte)] = 1,
			[typeof(sbyte)] = 1,
			[typeof(short)] = 2,
			[typeof(ushort)] = 2,
			[typeof(int)] = 4,
			[typeof(uint)] = 4,
			[typeof(long)] = 8,
			[typeof(ulong)] = 8,
			[typeof(float)] = 4,
			[typeof(double)] = 8,
			[typeof(char)] = 4,
			[typeof(decimal)] = 16,
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
				self.Core.PoolGetUnit(out self.codeToTypeNameDict);
				self.Core.PoolGetUnit(out self.codeToNameDict);
			}
		}

		class RemoveRule : RemoveRule<TreeDataByteSequence>
		{
			protected override void Execute(TreeDataByteSequence self)
			{
				self.TypeToCodeDict.Dispose();
				self.codeToTypeNameDict.Dispose();
				self.codeToNameDict.Dispose();
			}
		}
	}

	/// <summary>
	/// 树数据字节序列
	/// </summary>
	public class TreeDataByteSequence : ByteSequence
		, AsRule<ITreeDataSerialize>
		, AsRule<ITreeDataDeserialize>
	{
		/// <summary>
		/// 短类型名称正则表达式
		/// </summary>
		public static Regex ShortTypeNameRegex = new Regex(@", Version=\d+.\d+.\d+.\d+, Culture=[\w-]+, PublicKeyToken=(?:null|[a-f0-9]{16})", RegexOptions.Compiled);

		/// <summary>
		/// 类型对应类型码字典，64哈希码对应
		/// </summary>
		public UnitDictionary<Type, long> TypeToCodeDict;

		/// <summary>
		/// 类型码对应类型名称字典，64哈希码对应
		/// </summary>
		public UnitDictionary<long, string> codeToTypeNameDict;

		/// <summary>
		/// 字段码对应名称字典，32哈希码对应，代码生成直接使用int对比
		/// </summary>
		public UnitDictionary<int, string> codeToNameDict;

		#region 映射表
		/// <summary>
		/// 写入名称码并判断是否存在
		/// </summary>
		public bool WriteCheckNameCode(int nameCode)
		{
			WriteUnmanaged(nameCode);
			return codeToNameDict.ContainsKey(nameCode);
		}

		/// <summary>
		/// 添加类型
		/// </summary>
		private long AddTypeCode(Type type)
		{
			if (!TypeToCodeDict.TryGetValue(type, out long typeCode))
			{
				typeCode = this.TypeToCode(type);
				TypeToCodeDict.Add(type, typeCode);
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
		private bool TryGetType(long typeCode, out Type type)
		{
			if (this.TryCodeToType(typeCode, out type)) return true;
			if (codeToTypeNameDict.TryGetValue(typeCode, out string typeName))
			{
				type = System.Type.GetType(typeName);
				if (type != null)
				{
					this.TypeToCode(type);
					return true;
				}
			}
			return false;
		}

		#endregion



		/// <summary>
		/// 序列化
		/// </summary>
		public void Serialize<T>(in T value)
		{
			//写入数据
			WriteValue(value);

			//记录映射表起始位置
			int startPoint = Length;
			//写入类型数量
			WriteUnmanaged(TypeToCodeDict.Count);
			foreach (var item in TypeToCodeDict)
			{
				//写入类型码
				WriteUnmanaged(item.Value);
				//写入类型名称
				WriteString(ShortTypeNameRegex.Replace(item.Key.AssemblyQualifiedName, ""));
			}
			//写入字段数量
			WriteUnmanaged(codeToNameDict.Count);
			foreach (var item in codeToNameDict)
			{
				//写入字段码
				WriteUnmanaged(item.Key);
				//写入字段名称
				WriteString(item.Value);
			}
			//写入映射表起始位置偏差距离
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
				//读取类型码
				ReadUnmanaged(out long typeCode);
				//读取类型名称
				string typeName = ReadString();
				codeToTypeNameDict.Add(typeCode, typeName);
			}
			//读取字段数量
			ReadUnmanaged(out int nameCount);
			for (int i = 0; i < nameCount; i++)
			{
				//读取字段码
				ReadUnmanaged(out int nameCode);
				//读取字段名称
				string name = ReadString();
				codeToNameDict.Add(nameCode, name);
			}

			//读取指针定位到数据起始位置
			readPoint = 0;
			readBytePoint = 0;
			readSegmentPoint = 0;

			//读取数据
			ReadValue(ref value);
		}

		#region 写入

		/// <summary>
		/// 写入类型
		/// </summary>
		public void WriteType(Type type)
		{
			WriteUnmanaged(AddTypeCode(type));
		}

		/// <summary>
		/// 写入值
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValue(in object value)
		{
			Type type = value.GetType();
			long typeCode = this.Core.TypeToCode(type);
			this.Core.RuleManager.SupportNodeRule(typeCode);

			//动态支持多维数组
			if (type.IsArray) this.Core.RuleManager.SupportGenericParameterNodeRule(type.GetElementType(), typeof(TreeDataSerialize));

			if (this.Core.RuleManager.TryGetRuleList<TreeDataSerialize>(typeCode, out RuleList ruleList))
			{
				((IRuleList<TreeDataSerialize>)ruleList).SendRef(this, ref Unsafe.AsRef(value));
			}
		}

		#endregion

		#region 读取

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
		/// 读取值，方便属性读取
		/// </summary>
		public T ReadValue<T>()
		{
			object obj = default;
			ReadValue(typeof(T), ref obj);
			return (T)obj;
		}

		/// <summary>
		/// 读取值
		/// </summary>
		public void ReadValue(Type type, ref object value)
		{
			long typeCode = this.Core.TypeToCode(type);
			this.Core.RuleManager.SupportNodeRule(typeCode);

			//动态支持多维数组
			if (type.IsArray) this.Core.RuleManager.SupportGenericParameterNodeRule(type.GetElementType(), typeof(TreeDataDeserialize));

			if (this.Core.RuleManager.TryGetRuleList<TreeDataDeserialize>(typeCode, out RuleList ruleList))
			{
				((IRuleList<TreeDataDeserialize>)ruleList).SendRef(this, ref value);
			}
			else
			{
				//不支持的类型，跳跃数据
				SkipData(type);
			}
		}

		/// <summary>
		/// 读取字段名称
		/// </summary>
		public bool TryReadName(out string name)
		{
			ReadUnmanaged(out int nameCode);
			return codeToNameDict.TryGetValue(nameCode, out name);
		}

		/// <summary>
		/// 读取类型
		/// </summary>
		public bool TryReadType(out Type type)
		{
			ReadUnmanaged(out long typeCode);
			return TryGetType(typeCode, out type);
		}

		/// <summary>
		/// 尝试以子类型读取
		/// </summary>
		public void SubTypeReadValue(Type type, Type targetType, ref object value)
		{
			bool isSubType = false;
			Type baseType = type.BaseType;
			if (targetType.IsInterface)
			{
				Type[] interfaces = type.GetInterfaces();
				foreach (var interfaceType in interfaces)
				{
					if (interfaceType == targetType)
					{
						isSubType = true;
						break;
					}
				}
			}
			else
			{
				while (baseType != null && baseType != typeof(object))
				{
					if (baseType == targetType)
					{
						isSubType = true;
						break;
					}
					baseType = type.BaseType;
				}
			}
			if (isSubType)//是子类型
			{
				//读取指针回退，类型码
				ReadBack(8);
				//子类型读取
				ReadValue(type, ref value);
			}
			else //不是本身类型，也不是子类型，也不是可转换类型，跳跃数据。
			{
				//跳跃数据
				SkipData(type);
			}
		}

		#endregion


		/// <summary>
		/// 跳跃数据，跳跃前需要回退到类型
		/// </summary>
		public void SkipData()
		{
			//读取类型，是基础类型直接跳跃。
			if (TryReadType(out Type type) && TreeDataType.TypeDict.TryGetValue(type, out int byteCount))
			{
				ReadSkip(byteCount);
				return;
			}
			SkipData(type);
		}

		/// <summary>
		/// 跳跃数据
		/// </summary>
		public void SkipData(Type type)
		{
			//读取字段数量
			ReadUnmanaged(out int count);
			//空对象判断
			if (count == ValueMarkCode.NULL_OBJECT) return;

			//Type不存在的情况下，负数为数组类型
			if (count >= 0)
			{
				for (int i = 0; i < count; i++)
				{
					//读取字段名称码
					ReadSkip(4);
					SkipData();
				}
			}
			else
			{
				count = ~count;
				//此时Count是维度，直接累乘计算总长度，一般来说数量不会超过int极限。
				int totalLength = 0;
				for (int i = 0; i < count; i++)
				{
					ReadUnmanaged(out int length);
					totalLength *= length;
				}
				//为0的情况下，是数组，但是数组长度为0
				if (totalLength == 0) return;
				if (type != null && TreeDataType.TypeDict.TryGetValue(type.GetElementType(), out int arrayByteCount))
				{
					//基础数组类型，直接跳跃
					ReadSkip(arrayByteCount * totalLength);
				}
				else
				{
					//非基础数组类型，递归跳跃
					for (int i = 0; i < totalLength; i++) SkipData();
				}
			}
		}





		/// <summary>
		/// 获取TreeData
		/// </summary>
		public TreeData GetTreeData()
		{
			TreeData data = null;
			ReadUnmanaged(out long typeCode);
			//判断是否是基础类型，或是字符串
			if (this.TryGetType(typeCode, out Type type) && (type == typeof(string) || TreeDataType.TypeDict.TryGetValue(type, out _)))
			{
				data = this.Parent.AddTemp(out TreeValue treeValue);
				codeToTypeNameDict.TryGetValue(typeCode, out treeValue.TypeName);
				//取值????
				return data;
			}
			//不是基础类型获取类型名称
			if (codeToTypeNameDict.TryGetValue(typeCode, out string typeName))
			{
				this.Parent.AddTemp(out data);
				data.TypeName = typeName;
			}

			//读取字段数量
			ReadUnmanaged(out int count);
			//空对象判断
			if (count == ValueMarkCode.NULL_OBJECT) return data;
			data.IsDefault = false;

			//Type不存在的情况下，负数为数组类型
			if (count >= 0)
			{
				for (int i = 0; i < count; i++)
				{
					//读取字段名称码
					if (TryReadName(out string name))
					{
						//????
						data.AddStringNode(name, out TreeData node);
					}
				}
			}
			else
			{
				count = ~count;
				//此时Count是维度，直接累乘计算总长度，一般来说数量不会超过int极限。
				int totalLength = 0;
				for (int i = 0; i < count; i++)
				{
					ReadUnmanaged(out int length);
					totalLength *= length;
				}
				//为0的情况下，是数组，但是数组长度为0
				if (totalLength == 0) return data;
				if (type != null && TreeDataType.TypeDict.TryGetValue(type.GetElementType(), out int arrayByteCount))
				{
					//基础数组类型，直接跳跃
					ReadSkip(arrayByteCount * totalLength);
				}
				else
				{
					//非基础数组类型，递归跳跃
					for (int i = 0; i < totalLength; i++) SkipData();
				}
			}

			return null;
		}

		/// <summary>
		/// 添加数组项
		/// </summary>
		public void AddItem(TreeData parentData, int index)
		{
			TreeData self;
			ReadUnmanaged(out long typeCode);
			//判断是否是基础类型，或是字符串
			if (this.TryGetType(typeCode, out Type type) && (type == typeof(string) || TreeDataType.TypeDict.TryGetValue(type, out _)))
			{
				parentData.AddNumberNode(index, out TreeValue treeValue);
				codeToTypeNameDict.TryGetValue(typeCode, out treeValue.TypeName);
				//取值????
				return;
			}
			//不是基础类型获取类型名称
			if (codeToTypeNameDict.TryGetValue(typeCode, out string typeName))
			{
				parentData.AddNumberNode(index, out self);
				self.TypeName = typeName;
			}
			else
			{
				return;
			}

		}

		/// <summary>
		/// 设置数据
		/// </summary>
		public void AddField(TreeData parentData, string fieldName)
		{
			TreeData self;
			ReadUnmanaged(out long typeCode);
			//判断是否是基础类型，或是字符串
			if (this.TryGetType(typeCode, out Type type) && (type == typeof(string) || TreeDataType.TypeDict.TryGetValue(type, out _)))
			{
				parentData.AddStringNode(fieldName, out TreeValue treeValue);
				codeToTypeNameDict.TryGetValue(typeCode, out treeValue.TypeName);
				//取值????
				return;
			}
			//不是基础类型获取类型名称
			if (codeToTypeNameDict.TryGetValue(typeCode, out string typeName))
			{
				parentData.AddStringNode(fieldName, out self);
				self.TypeName = typeName;
			}
			else
			{
				return;
			}

			//读取字段数量
			ReadUnmanaged(out int count);
			//空对象判断
			if (count == ValueMarkCode.NULL_OBJECT) return;
			parentData.IsDefault = false;

			//Type不存在的情况下，负数为数组类型
			if (count >= 0)
			{
				for (int i = 0; i < count; i++)
				{
					//读取字段名称码
					if (TryReadName(out string name))
					{
						AddField(self, name);
					}
				}
			}
			else
			{
				count = ~count;
				//此时Count是维度，直接累乘计算总长度，一般来说数量不会超过int极限。
				int totalLength = 0;
				for (int i = 0; i < count; i++)
				{
					ReadUnmanaged(out int length);
					totalLength *= length;
				}
				//为0的情况下，是数组，但是数组长度为0
				if (totalLength == 0) return;
				if (type != null && TreeDataType.TypeDict.TryGetValue(type.GetElementType(), out int arrayByteCount))
				{
					//基础数组类型，直接跳跃
					ReadSkip(arrayByteCount * totalLength);
				}
				else
				{
					//非基础数组类型，递归跳跃
					for (int i = 0; i < totalLength; i++) SkipData();
				}
			}
		}

	}



}
