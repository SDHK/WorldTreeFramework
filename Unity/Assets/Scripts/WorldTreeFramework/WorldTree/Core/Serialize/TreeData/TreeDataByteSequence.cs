/****************************************

* 作者：闪电黑客
* 日期：2024/8/20 17:35

* 描述：

*/
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
		//public static Regex ShortTypeNameRegex = new Regex(@", Version=\d+.\d+.\d+.\d+, Culture=[\w-]+, PublicKeyToken=(?:null|[a-f0-9]{16})", RegexOptions.Compiled);
		public static Regex ShortTypeNameRegex = new Regex(@"(?<=[\w.]+),.*?(?=(\]|$))", RegexOptions.Compiled);
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
				if (!TreeDataType.TypeDict.ContainsKey(type) && type != typeof(string) && type != typeof(object))
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
				//WriteString(ShortTypeNameRegex.Replace(item.Key.AssemblyQualifiedName, ""));
				WriteString(ShortTypeNameRegex.Replace(item.Key.FullName, ""));
			}
			////写入字段数量
			//WriteUnmanaged(codeToNameDict.Count);
			//foreach (var item in codeToNameDict)
			//{
			//	//写入字段码
			//	WriteUnmanaged(item.Key);
			//	//写入字段名称
			//	WriteString(item.Value);
			//}
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
			////读取字段数量
			//ReadUnmanaged(out int nameCount);
			//for (int i = 0; i < nameCount; i++)
			//{
			//	//读取字段码
			//	ReadUnmanaged(out int nameCode);
			//	//读取字段名称
			//	string name = ReadString();
			//	codeToNameDict.Add(nameCode, name);
			//}

			//读取指针定位到数据起始位置
			readPoint = 0;
			readBytePoint = 0;
			readSegmentPoint = 0;

			this.Log($"TypeCount: {codeToTypeNameDict.Count}");
			foreach (var item in this.codeToTypeNameDict)
			{
				this.Log($"Type: {item.Value}");
			}
			this.Log($"NameCount: {codeToNameDict.Count}");
			foreach (var item in this.codeToNameDict)
			{
				this.Log($"Name: {item.Value}");
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
			this.Log($"NameCount: {codeToNameDict.Count}");
			foreach (var item in this.codeToNameDict)
			{
				this.Log($"Name: {item.Value}");
			}
		 
		 */

		#region 写入
		/// <summary>
		/// 读取字段数量或空标记
		/// </summary>
		public bool ReadCheckNull(out int count)
		{
			this.ReadUnmanaged(out count);
			if (count == ValueMarkCode.NULL_OBJECT) return true;
			return false;
		}

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
					this.WriteUnmanaged(0L);
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
		/// 检测类型字段数量是否为0
		/// </summary>
		public bool CheckClassCount(int count)
		{
			if (count < 0)
			{
				//能读取到count，说名这里的Type绝对不是基础类型，所以传null跳跃
				ReadBack(4);
				SkipData(null);
				return true;
			}
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
		/// 写入类型
		/// </summary>
		public void WriteType(Type type)
		{
			this.WriteUnmanaged(AddTypeCode(type));
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
				this.WriteType(typeof(object));
				this.WriteUnmanaged((int)ValueMarkCode.NULL_OBJECT);
			}
		}


		/// <summary>
		/// 写入值
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
		/// 读取值
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
			this.ReadUnmanaged(out long typeCode);
			return TryGetType(typeCode, out type);
		}

		/// <summary>
		/// 读取类型
		/// </summary>
		public bool TryReadDataHead(Type targetType, ref object value, out int count)
		{
			bool isSkip = true;
			Type dataType = null;
			count = 0;
			this.ReadUnmanaged(out long typeCode);
			if (typeCode == 0)//判断如果是0，则为原类型，尝试读取
			{
				//dataType = targetType;
				this.ReadUnmanaged(out count);//直接尝试读取
				if (count == ValueMarkCode.NULL_OBJECT)
				{
					value = default;
					return true;
				}
				else
				{
					return false;
				}
			}
			else if (TryGetType(typeCode, out dataType))
			{
				//能读取到类型，说明是多态类型，判断一样则直接读取
				isSkip = false;
				if (dataType == targetType)//如果直接相等
				{
					this.ReadUnmanaged(out count);
					if (count == ValueMarkCode.NULL_OBJECT)
					{
						value = default;
						return true;
					}
					else
					{
						return false;
					}
				}
			}

			if (isSkip)
			{
				SkipData(dataType);
				return true;
			}

			//不一致或不存在的类型，尝试判断为子类读取或跳跃
			if (!SubTypeReadValue(dataType, targetType, ref value))
			{
				this.ReadUnmanaged(out count);
				if (count == ValueMarkCode.NULL_OBJECT)
				{
					value = default;
					return true;
				}
				else
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// 尝试以子类型读取
		/// </summary>
		private bool SubTypeReadValue(Type type, Type targetType, ref object value)
		{
			if (type != null)
			{
				//判断是否为基础类型，直接跳跃数据。
				if (TreeDataType.TypeDict.ContainsKey(type) || type == typeof(string))
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
				//读取指针回退，类型码
				ReadBack(8);
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
				if (TreeDataType.TypeDict.TryGetValue(type, out int byteCount))
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
				if (type != null && type.IsArray && TreeDataType.TypeDict.TryGetValue(type.GetElementType(), out int arrayByteCount))
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


	/// <summary>
	/// a
	/// </summary>
	public class TypeHashGenerator1
	{
		/// <summary>
		/// 储存类型名称的字典
		/// </summary>
		public Dictionary<long, string> TypeNameDict = new();
		/// <summary>
		/// 储存泛型参数的字典
		/// </summary>
		public Dictionary<long, List<long>> GenericParamsDict = new();

		/// <summary>
		/// 获取类型的哈希码
		/// </summary>
		public long GetTypeHash(Type type)
		{
			// 处理数组类型
			//if (type.IsArray)
			//{
			//	var elementHash = GetTypeHash(type.GetElementType());
			//	var arrayString = $"Array[{type.GetArrayRank()}]";
			//	var arrayHash = HashCodeHelper.GetHash64(arrayString);
			//	TypeNameDict[arrayHash] = arrayString;

			//	var paramList = new List<long> { elementHash };
			//	GenericParamsDict[arrayHash] = paramList;
			//	return arrayHash;
			//}

			if (type.IsArray)
			{
				var arrayHash = -type.GetArrayRank();
				var elementHash = GetTypeHash(type.GetElementType());
				// 将维度编码为负数，这样-1表示一维数组，-2表示二维数组，以此类推

				var paramList = new List<long> { elementHash };
				GenericParamsDict[arrayHash] = paramList;
				return -type.GetArrayRank() * elementHash;
			}

			// 处理泛型类型
			if (type.IsGenericType)
			{
				// 获取泛型类型定义
				Type genericTypeDef = type.GetGenericTypeDefinition();
				string typeName = TreeDataByteSequence.ShortTypeNameRegex.Replace(genericTypeDef.AssemblyQualifiedName, "");
				long typeHash = HashCodeHelper.GetHash64(typeName);

				// 储存类型定义名称
				TypeNameDict[typeHash] = typeName;

				// 递归处理所有泛型参数
				var paramHasheList = new List<long>();
				foreach (var argType in type.GetGenericArguments())
				{
					paramHasheList.Add(GetTypeHash(argType));
				}

				// 储存泛型参数哈希列表
				GenericParamsDict[typeHash] = paramHasheList;

				return typeHash;
			}

			// 处理普通类型
			string fullName = TreeDataByteSequence.ShortTypeNameRegex.Replace(type.AssemblyQualifiedName, "");
			long hash = HashCodeHelper.GetHash64(fullName);
			TypeNameDict[hash] = fullName;
			return hash;
		}

		/// <summary>
		/// a
		/// </summary>
		/// <param name="hash"></param>
		/// <returns></returns>
		private long GetBasicHash(long hash)
		{
			if (hash < 0) hash = -hash;
			return hash;
		}

		/// <summary>
		/// 从哈希码还原类型
		/// </summary>
		public Type RestoreType(long hash)
		{
			// 处理数组类型
			if (hash < 0 && hash > -100)
			{
				// 获取数组维度（负数）
				int rank = -(int)hash;
				// 还原元素类型的哈希
				var elementType = RestoreType(rank);
				// 根据维度创建数组类型
				return rank == -1 ? elementType.MakeArrayType() : elementType.MakeArrayType(-rank);
			}


			// 获取类型名称
			if (!TypeNameDict.TryGetValue(hash, out string typeName))
			{
				throw new KeyNotFoundException($"Type hash {hash} not found");
			}

			//// 处理数组类型
			//if (typeName.StartsWith("Array["))
			//{
			//	var paramList = GenericParamsDict[hash];
			//	var elementType = RestoreType(paramList[0]);
			//	int rank = int.Parse(typeName.Substring(6, typeName.Length - 7));
			//	return rank == 1 ? elementType.MakeArrayType() : elementType.MakeArrayType(rank);
			//}

			// 检查是否是泛型类型
			if (GenericParamsDict.TryGetValue(hash, out List<long> paramHashes))
			{
				// 获取基础类型
				Type baseType = Type.GetType(typeName);
				if (baseType == null)
				{
					throw new TypeLoadException($"Cannot load type {typeName}");
				}

				// 还原所有泛型参数
				Type[] genericArgs = paramHashes.Select(h => RestoreType(h)).ToArray();

				// 构造完整的泛型类型
				return baseType.MakeGenericType(genericArgs);
			}

			// 普通类型直接加载
			return Type.GetType(typeName);
		}

	}




	public static class TypeHashGeneratorRule
	{
		/// <summary>
		/// 使用示例：
		/// </summary>
		public static void Example()
		{
			var generator = new TypeHashGenerator1();

			// 测试简单类型
			Type intType = typeof(int);
			long intHash = generator.GetTypeHash(intType);

			// 测试泛型类型
			Type listType = typeof(List<int>);
			long listHash = generator.GetTypeHash(listType);

			// 测试嵌套泛型
			Type dictType = typeof(Dictionary<string, List<int>>);
			long dictHash = generator.GetTypeHash(dictType);

			// 测试数组
			Type arrayType = typeof(int[,]);
			long arrayHash = generator.GetTypeHash(arrayType);

			// 还原类型测试
			Type restoredType = generator.RestoreType(listHash);
			Console.WriteLine(restoredType.FullName);

			foreach (var item in generator.TypeNameDict)
			{
				Console.WriteLine(item.Value);

			}

		}
	}


}
