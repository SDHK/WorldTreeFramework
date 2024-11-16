/****************************************

* 作者：闪电黑客
* 日期：2024/8/20 17:35

* 描述：

*/
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace WorldTree
{

	/// <summary>
	/// 树数据标记编码
	/// </summary>
	public static class TreeDataCode
	{
		/// <summary>
		/// 空对象
		/// </summary>
		public const short NULL_OBJECT = -1;



		/// <summary>
		/// 自动
		/// </summary>
		public const short AUTO = 0;


	}

	/// <summary>
	/// 树数据类型
	/// </summary>
	public static class TreeDataType
	{
		/// <summary>
		/// 基础值类型,字节长度
		/// </summary>
		public static Dictionary<Type, int> TypeSizeDict = new()
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

		/// <summary>
		/// 默认类型码，类型不会记录到数据里，以下标为键，用于最小化类型码，128个内为 1 Btye 长度
		/// </summary>
		public static Type[] TypeCodes = new Type[]
		{
			typeof(object),
			typeof(bool),
			typeof(byte),
			typeof(sbyte),
			typeof(short),
			typeof(ushort),
			typeof(int),
			typeof(uint),
			typeof(long),
			typeof(ulong),
			typeof(float),
			typeof(double),
			typeof(char),
			typeof(decimal),
			typeof(string),
		};


		/// <summary>
		/// 基础值类型哈希表，这些类型序列化是直接写入数据的，用于跳跃数据过滤
		/// </summary>
		public static HashSet<Type> BasicsTypeHash = new()
		{
			typeof(bool),
			typeof(byte),
			typeof(sbyte),
			typeof(short),
			typeof(ushort),
			typeof(int),
			typeof(uint),
			typeof(long),
			typeof(ulong),
			typeof(float),
			typeof(double),
			typeof(char),
			typeof(decimal),
			typeof(string),//string有特别判断，所以可以加入
		};



		/// <summary>
		/// 基础类型对应类型码
		/// </summary>
		private static Dictionary<Type, byte> typeCodeDict;

		/// <summary>
		/// 基础类型对应类型码
		/// </summary>
		public static Dictionary<Type, byte> TypeCodeDict
		{
			get
			{
				if (typeCodeDict == null)
				{
					typeCodeDict = new();
					for (int i = 0; i < TypeCodes.Length; i++)
						typeCodeDict.Add(TypeCodes[i], (byte)i);
				}
				return typeCodeDict;
			}
		}

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
			}
		}

		class RemoveRule : RemoveRule<TreeDataByteSequence>
		{
			protected override void Execute(TreeDataByteSequence self)
			{
				self.Clear();
				self.TypeToCodeDict.Dispose();
				self.codeToTypeNameDict.Dispose();
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
		/// 类型对应类型码字典，64哈希码对应
		/// </summary>
		public UnitDictionary<Type, long> TypeToCodeDict;

		/// <summary>
		/// 类型码对应类型名称字典，64哈希码对应
		/// </summary>
		public UnitDictionary<long, string> codeToTypeNameDict;


		#region 映射表


		/// <summary>
		/// 添加类型
		/// </summary>
		private long GetTypeCode(Type type, bool isWriteName = true)
		{
			if (TreeDataType.TypeCodeDict.TryGetValue(type, out byte typeByteCode)) return typeByteCode;
			if (!TypeToCodeDict.TryGetValue(type, out long typeCode))
			{
				typeCode = this.TypeToCode(type);
				if (isWriteName) TypeToCodeDict.Add(type, typeCode);
			}
			return typeCode;
		}


		/// <summary>
		/// 尝试获取类型
		/// </summary>
		private bool TryGetType(long typeCode, out Type type)
		{
			if (TreeDataType.TypeCodes.Length > typeCode && typeCode >= 0)
			{
				type = TreeDataType.TypeCodes[typeCode];
				return true;
			}

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
				WriteString(item.Key.ToString());
			}
			WriteUnmanaged(startPoint);
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
			ReadJump(readPoint - 4);
			//读取映射表起始位置距离
			ReadUnmanaged(out int startPoint);
			//回退到映射表起始位置
			ReadJump(startPoint);

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

			//读取指针定位到数据起始位置
			readPoint = 0;
			readBytePoint = 0;
			readSegmentPoint = 0;

			this.Log($"TypeCount: {codeToTypeNameDict.Count}");
			foreach (var item in this.codeToTypeNameDict)
			{
				this.Log($"Type: {item.Value}");
			}


			//读取数据
			ReadValue(ref value);
		}

		/*
		 	this.Log($"TypeCount: {codeToTypeNameDict.Count}");
			foreach (var item in this.codeToTypeNameDict)
			{
				this.Log($"Type: {item.Value}");
			}
		 */

		#region 写入


		/// <summary>
		/// 写入字段数量或空标记
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value">类型</param>
		/// <param name="nameCode">字段码</param>
		/// <param name="count">字段数或数组维度</param>
		/// <param name="obj">返回对象</param>
		/// <returns>是否为Null退出</returns>
		public bool TryWriteDataHead<T>(in object value, int nameCode, int count, out T obj)
		{
			switch (nameCode)
			{
				case -2:
					this.WriteType(typeof(object));
					if (this.WriteCheckNull(value, count, out obj)) return true;
					break;
				case -1:
					this.WriteType(typeof(T));
					if (this.WriteCheckNull(value, count, out obj)) return true;
					break;
			}
			obj = (T)value;
			return false;
		}




		/// <summary>
		/// 写入字段数量或空标记
		/// </summary>
		private bool WriteCheckNull<T>(in object value, int count, out T obj)
		{
			if (value is T objValue)
			{
				if (!objValue.Equals(default))
				{
					obj = objValue;
					this.WriteUnmanaged(count);
					return false;
				}
			}
			obj = default;
			this.WriteUnmanaged((int)ValueMarkCode.NULL_OBJECT);
			return true;
		}

		/// <summary>
		/// 写入类型，默认写入类型名称
		/// </summary>
		public void WriteType(Type type, bool isWriteName = true)
		{
			this.WriteDynamic(GetTypeCode(type, isWriteName));
		}

		/// <summary>
		/// 写入值
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValue<T>(in T value, int nameCode = -1)
		{
			Type originalType = typeof(T);
			Type type = value?.GetType();
			//如果类型为空，或者类型和原始类型一致，则标记为0，不写入类型。
			if (type == null)
			{
				type = originalType;
				nameCode = -2;
			}
			else if (type == originalType)
			{
				nameCode = -2;
			}

			long typeCode = this.Core.TypeToCode(type);
			this.Core.RuleManager.SupportNodeRule(typeCode);

			//动态支持多维数组
			if (type.IsArray) this.Core.RuleManager.SupportGenericParameterNodeRule(type.GetElementType(), typeof(TreeDataSerialize));

			if (this.Core.RuleManager.TryGetRuleList<TreeDataSerialize>(typeCode, out RuleList ruleList) && ruleList.NodeType == typeCode)
			{
				((IRuleList<TreeDataSerialize>)ruleList).SendRef(this, ref Unsafe.AsRef<object>(value), ref nameCode);
			}
			else
			{
				//不支持的类型，写入空对象
				this.WriteType(typeof(object), false);
				this.WriteUnmanaged((int)ValueMarkCode.NULL_OBJECT);
			}
		}


		/// <summary>
		/// 指定类型写入值
		/// </summary>
		public void WriteValue(Type type, in object value, int nameCode = -1)
		{
			long typeCode = this.Core.TypeToCode(type);
			this.Core.RuleManager.SupportNodeRule(typeCode);

			//动态支持多维数组
			if (type.IsArray) this.Core.RuleManager.SupportGenericParameterNodeRule(type.GetElementType(), typeof(TreeDataSerialize));

			if (this.Core.RuleManager.TryGetRuleList<TreeDataSerialize>(typeCode, out RuleList ruleList) && ruleList.NodeType == typeCode)
			{
				((IRuleList<TreeDataSerialize>)ruleList).SendRef(this, ref Unsafe.AsRef<object>(value), ref nameCode);
			}
			else
			{
				//不支持的类型，写入空对象
				this.WriteType(typeof(object));
				this.WriteUnmanaged((int)ValueMarkCode.NULL_OBJECT);
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
		/// 指定类型读取值
		/// </summary>
		public void ReadValue(Type type, ref object value, int nameCode = -1)
		{
			long typeCode = this.Core.TypeToCode(type);
			this.Core.RuleManager.SupportNodeRule(typeCode);

			//动态支持多维数组
			if (type.IsArray) this.Core.RuleManager.SupportGenericParameterNodeRule(type.GetElementType(), typeof(TreeDataDeserialize));

			if (this.Core.RuleManager.TryGetRuleList<TreeDataDeserialize>(typeCode, out RuleList ruleList) && ruleList.NodeType == typeCode)
			{
				((IRuleList<TreeDataDeserialize>)ruleList).SendRef(this, ref value, ref nameCode);
			}
			else
			{
				//不支持的类型，跳跃数据
				SkipData();
			}
		}

		/// <summary>
		/// 尝试读取类型
		/// </summary>
		public bool TryReadType(out Type type)
		{
			this.ReadDynamic(out long typeCode);
			return TryGetType(typeCode, out type);
		}

		/// <summary>
		/// 尝试读取类型数据头部
		/// </summary>
		public bool TryReadClassHead(Type targetType, ref object value, out int count)
		{
			if (TryReadDataHead(targetType, ref value, out count, out int countPoint)) return true;
			if (count < 0)
			{
				//能读取到count，说名这里的Type绝对不是基础类型，所以传null跳跃
				ReadJump(countPoint);
				SkipData(null);
				return true;
			}
			return false;
		}

		/// <summary>
		/// 尝试读取数组数据头部
		/// </summary>
		public bool TryReadArrayHead(Type targetType, ref object value, int targetCount)
		{

			if (TryReadDataHead(targetType, ref value, out int count, out int countPoint)) return true;
			count = ~count;
			if (count != targetCount)
			{
				//能读取到count，说名这里的Type绝对不是基础类型，所以传null跳跃
				ReadJump(countPoint);
				SkipData(null);
				return true;
			}
			return false;
		}

		/// <summary>
		/// 尝试读取类型数据头部
		/// </summary>
		private bool TryReadDataHead(Type targetType, ref object value, out int count, out int countPoint)
		{
			countPoint = readPoint;
			count = 0;
			this.ReadDynamic(out long typeCode);
			if (typeCode == 0)//判断如果是0，则为原类型
			{
				countPoint = readPoint;
				this.ReadUnmanaged(out count);
				if (count != ValueMarkCode.NULL_OBJECT) return false;
				value = default;
				return true;
			}

			//尝试获取类型
			if (TryGetType(typeCode, out Type dataType))
			{
				//类型一样直接读取
				if (dataType == targetType)
				{
					countPoint = readPoint;
					this.ReadUnmanaged(out count);
					if (count != ValueMarkCode.NULL_OBJECT) return false;
					value = default;
					return true;
				}
				//不一样,判断多态类型，不是则尝试读取
				else if (!SubTypeReadValue(dataType, targetType, ref value, countPoint))
				{
					countPoint = readPoint;
					this.ReadUnmanaged(out count);
					if (count != ValueMarkCode.NULL_OBJECT) return false;
					value = default;
					return true;
				}
				else
				{
					return true;
				}
			}

			//数据类型不存在 ，判断目标类型是否非基础类型
			if (!TreeDataType.BasicsTypeHash.Contains(targetType))
			{
				countPoint = readPoint;
				//不是基础类型则尝试读取
				this.ReadUnmanaged(out count);
				if (count != ValueMarkCode.NULL_OBJECT) return false;
			}
			//数据跳跃
			SkipData(dataType);
			return true;
		}


		/// <summary>
		/// 尝试以子类型读取
		/// </summary>
		private bool SubTypeReadValue(Type type, Type targetType, ref object value, int typePoint)
		{
			if (type != null)
			{
				//判断是否为基础类型，直接跳跃数据。
				if (TreeDataType.BasicsTypeHash.Contains(type))
				{
					SkipData(type);
					return true;
				}
			}
			else
			{
				//类型不存在直接跳跃数据
				SkipData(type);
				return true;
			}

			bool isSubType = false;
			Type baseType = type?.BaseType;
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
			else if (targetType.IsClass)
			{
				while (baseType != null && baseType != typeof(object))
				{
					if (baseType == targetType)
					{
						isSubType = true;
						break;
					}
					baseType = baseType.BaseType;
				}
			}
			else //不是接口也不是类型，直接跳跃数据
			{
				SkipData(type);
				return true;
			}

			if (isSubType)//是子类型
			{
				//读取指针回退到类型码
				ReadJump(typePoint);
				//子类型读取
				ReadValue(type, ref value);
				return true;
			}
			else //不是子类型，返回去尝试读取。
			{
				return false;
			}
		}

		#endregion


		/// <summary>
		/// 跳跃数据，跳跃前需要回退到类型
		/// </summary>
		public void SkipData()
		{
			TryReadType(out Type type);
			SkipData(type);
		}

		/// <summary>
		/// 跳跃数据
		/// </summary>
		public void SkipData(Type type)
		{
			//是基础类型直接跳跃
			if (type != null)
			{
				if (TreeDataType.TypeSizeDict.TryGetValue(type, out int byteCount))
				{
					ReadSkip(byteCount);
					return;
				}
				//string类型需要特殊处理
				else if (type == typeof(string))
				{
					SkipString();
					return;
				}
			}

			//读取字段数量
			this.ReadUnmanaged(out int count);
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
				int totalLength = 1;
				for (int i = 0; i < count; i++)
				{
					this.ReadUnmanaged(out int length);
					totalLength *= length;
				}
				//为0的情况下，是数组，但是数组长度为0
				if (totalLength == 0) return;
				if (type != null && type.IsArray && TreeDataType.TypeSizeDict.TryGetValue(type.GetElementType(), out int arrayByteCount))
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





		#region TreeData测试

		/// <summary>
		/// 获取TreeData
		/// </summary>
		public TreeData GetTreeData()
		{
			TreeData data = null;
			ReadUnmanaged(out long typeCode);
			//判断是否是基础类型，或是字符串
			if (this.TryGetType(typeCode, out Type type) && (type == typeof(string) || TreeDataType.TypeSizeDict.TryGetValue(type, out _)))
			{
				data = this.Parent.AddTemp(out TreeValue treeValue);
				codeToTypeNameDict.TryGetValue(typeCode, out treeValue.TypeName);
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
					////读取字段名称码
					//if (TryReadName(out string name))
					//{
					//	//????
					//	data.AddStringNode(name, out TreeData node);
					//}
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
				if (type != null && TreeDataType.TypeSizeDict.TryGetValue(type.GetElementType(), out int arrayByteCount))
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
			if (this.TryGetType(typeCode, out Type type) && (type == typeof(string) || TreeDataType.TypeSizeDict.TryGetValue(type, out _)))
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
			if (this.TryGetType(typeCode, out Type type) && (type == typeof(string) || TreeDataType.TypeSizeDict.TryGetValue(type, out _)))
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
					////读取字段名称码
					//if (TryReadName(out string name))
					//{
					//	AddField(self, name);
					//}
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
				if (type != null && TreeDataType.TypeSizeDict.TryGetValue(type.GetElementType(), out int arrayByteCount))
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
		#endregion

	}

}
