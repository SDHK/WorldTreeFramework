/****************************************

* 作者：闪电黑客
* 日期：2024/11/15 10:10

* 描述：

*/
using System;
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
		/// 自动适配类型
		/// </summary>
		public const short AUTO = 0;
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

		/// <summary>
		/// 递归层级
		/// </summary>
		public int Layer;

		/// <summary>
		/// 最大递归层级
		/// </summary>
		public int LayerMax = 1000;


		#region 映射表


		/// <summary>
		/// 添加类型
		/// </summary>
		private long GetTypeCode(Type type, bool isIgnoreName = false)
		{
			if (TreeDataTypeHelper.TypeCodeDict.TryGetValue(type, out byte typeByteCode)) return typeByteCode;

			if (!TypeToCodeDict.TryGetValue(type, out long typeCode))
			{
				typeCode = this.TypeToCode(type);
				if (!isIgnoreName) TypeToCodeDict.Add(type, typeCode);
			}
			return typeCode;
		}


		/// <summary>
		/// 尝试获取类型
		/// </summary>
		private bool TryGetType(long typeCode, out Type type)
		{
			if (TreeDataTypeHelper.TypeCodes.Length > typeCode && typeCode >= 0)
			{
				type = TreeDataTypeHelper.TypeCodes[typeCode];
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
			Layer = 0;
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
			Layer = 0;
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
		public bool TryWriteDataHead<T>(in object value, int nameCode, int count, out T obj, bool isIgnoreName = false)
		{
			switch (nameCode)
			{
				case -2:
					this.WriteType(typeof(object));
					if (this.WriteCheckNull(value, count, out obj)) return true;
					break;
				case -1:
					this.WriteType(typeof(T), isIgnoreName);
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
					this.WriteDynamic(count);
					return false;
				}
			}
			obj = default;
			this.WriteDynamic((int)ValueMarkCode.NULL_OBJECT);
			return true;
		}

		/// <summary>
		/// 写入类型，默认写入类型名称
		/// </summary>
		public void WriteType(Type type, bool isIgnoreName = false)
		{
			this.WriteDynamic(GetTypeCode(type, isIgnoreName));
		}

		/// <summary>
		/// 写入值
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValue<T>(in T value, int nameCode = -1)
		{
			Layer++;
			if (Layer > LayerMax)
			{
				this.LogError("序列化超出最大层级");
				return;
			}
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
				this.WriteDynamic((int)ValueMarkCode.NULL_OBJECT);
			}
			Layer--;
		}


		/// <summary>
		/// 指定类型写入值
		/// </summary>
		public void WriteValue(Type type, in object value, int nameCode = -1)
		{
			Layer++;
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
				this.WriteDynamic((int)ValueMarkCode.NULL_OBJECT);
			}
			Layer--;
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
			Layer++;
			if (Layer > LayerMax)
			{
				this.LogError("反序列化超出最大层级");
				return;
			}
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
			Layer--;
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
				this.ReadDynamic(out count);
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
					this.ReadDynamic(out count);
					if (count != ValueMarkCode.NULL_OBJECT) return false;
					value = default;
					return true;
				}
				//不一样,判断多态类型，不是则尝试读取
				else if (!SubTypeReadValue(dataType, targetType, ref value, countPoint))
				{
					countPoint = readPoint;
					this.ReadDynamic(out count);
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
			if (!TreeDataTypeHelper.BasicsTypeHash.Contains(targetType))
			{
				countPoint = readPoint;
				//不是基础类型则尝试读取
				this.ReadDynamic(out count);
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
				if (TreeDataTypeHelper.BasicsTypeHash.Contains(type))
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
				if (TreeDataTypeHelper.TypeSizeDict.TryGetValue(type, out int byteCount))
				{
					ReadSkip(byteCount);
					return;
				}
				else if (type == typeof(string))
				{
					SkipString();
					return;
				}
			}
			//读取字段数量
			this.ReadDynamic(out int count);
			//空对象判断
			if (count == ValueMarkCode.NULL_OBJECT) return;

			//Type可能不存在的情况下，负数为数组类型
			if (count < 0)
			{
				count = ~count;
				//此时Count是维度，直接累乘计算总长度，一般来说数量不会超过int极限。
				int totalLength = 1;
				for (int i = 0; i < count; i++)
				{
					this.ReadDynamic(out int length);
					totalLength *= length;
				}
				//为0的情况下，是数组，但是数组长度为0
				if (totalLength == 0) return;
				if (type != null && type.IsArray && TreeDataTypeHelper.TypeSizeDict.TryGetValue(type.GetElementType(), out int arrayByteCount))
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
			else
			{
				for (int i = 0; i < count; i++)
				{
					//读取字段名称码
					ReadSkip(4);
					SkipData();
				}
			}
		}



		#region TreeData

		/// <summary>
		/// 设置
		/// </summary>
		public void SetTreeData(TreeData treeData)
		{
			Layer = 0;
			//判断是否是数组
			if (treeData is TreeDataArray treeDataArray)
			{
				//int count = NodeBranchHelper.GetBranch<NumberNodeBranch>(treeData)?.Count ?? 0;
				//写入类型码
				//this.WriteString(treeDataArray.TypeName);//特别处理！！！

				//写入数组维度
				this.WriteDynamic(~treeDataArray.LengthList.Count);
				foreach (var item in treeDataArray.LengthList)
				{
					this.WriteDynamic(item);
				}
				//判断这个类型是否是基础数组类型
				//if (treeData.TypeName != null && TreeDataTypeHelper.TypeSizeDict.ContainsKey(treeData.TypeName.GetHash64()))
				//{
				//	//基础数组类型取值
				//	if (this.Core.RuleManager.TryGetRuleList<TreeDataSerialize>(this.Core.TypeToCode(Type.GetType(treeData.TypeName)), out RuleList ruleList))
				//	{
				//		((IRuleList<TreeDataSerialize>)ruleList).SendRef(this, ref treeData.Value, ref treeData.NameCode);
				//	}
				//}
				//else //非基础数组类型，递归
				//{
				//	foreach (var item in NodeBranchHelper.GetBranch<NumberNodeBranch>(treeData))
				//	{
				//		SetTreeData(item as TreeData);
				//	}
				//}
			}
			else
			{
				//写入类型码
				//this.WriteDynamic(this.Core.TypeToCode(Type.GetType(treeData.TypeName)));
				//写入字段数量
				var branch = NodeBranchHelper.GetBranch<NumberNodeBranch>(treeData);
				this.WriteDynamic(branch.Count);
				foreach (var item in branch.GetEnumerable())
				{
					//写入字段名称码
					this.WriteUnmanaged(item.Key);
					SetTreeData(item.Value as TreeData);
				}
			}
		}

		/// <summary>
		/// 获取
		/// </summary>
		public TreeData GetTreeData() => GetTreeData(this.Parent);

		/// <summary>
		/// 获取TreeData
		/// </summary>
		private TreeData GetTreeData(INode node, int number = 0)
		{
			TreeData data;
			int startPoint = this.readPoint;
			//读取类型码
			this.ReadDynamic(out long typeCode);
			//判断是否是基础类型
			if (this.TryGetType(typeCode, out Type type) && TreeDataTypeHelper.BasicsTypeHash.Contains(type))
			{
				data = node is TreeData treeData ? treeData.AddNumberNode(number, out TreeDataValue treeValue) : node.AddTemp(out treeValue);
				data.TypeName = type.ToString();
				//获取真实类型码
				long typeHashCode = this.Core.TypeToCode(type);
				//基础类型取值
				if (this.Core.RuleManager.TryGetRuleList<TreeDataDeserialize>(typeHashCode, out RuleList ruleList))
				{
					int nameCode = 0;
					((IRuleList<TreeDataDeserialize>)ruleList).SendRef(this, ref treeValue.Value, ref nameCode);
				}
				return data;
			}

			//读取字段数量
			this.ReadDynamic(out int count);
			//空对象判断
			if (count == ValueMarkCode.NULL_OBJECT)
			{
				data = node is TreeData treeData ? treeData.AddNumberNode(number, out TreeData _) : node.AddTemp(out TreeData _);
				data.TypeName = type?.ToString();
				data.IsDefault = true;
				return data;
			}

			//Type可能不存在的情况下，负数为数组类型
			if (count < 0)
			{
				TreeDataArray treeArray;
				if (node is TreeData treeData)
				{
					data = treeData.AddNumberNode(number, out treeArray);
				}
				else
				{
					data = node.AddTemp(out treeArray);
				}
				data.TypeName = type?.ToString();


				count = ~count;
				//此时Count是维度，直接累乘计算总长度，一般来说数量不会超过int极限。
				int totalLength = 1;
				for (int i = 0; i < count; i++)
				{
					this.ReadDynamic(out int length);
					treeArray.LengthList.Add(length);
					totalLength *= length;
				}
				//为0的情况下，是数组，但是数组长度为0
				if (totalLength == 0) return data;
				//判断这个类型是否是基础数组类型
				if (type?.GetElementType() != null && TreeDataTypeHelper.TypeSizeDict.ContainsKey(type.GetElementType()))
				{
					//跳跃回类型开头
					this.ReadJump(startPoint);
					//动态支持多维数组
					if (type.IsArray) this.Core.RuleManager.SupportGenericParameterNodeRule(type.GetElementType(), typeof(TreeDataDeserialize));
					//基础数组类型取值
					if (this.Core.RuleManager.TryGetRuleList<TreeDataDeserialize>(typeCode, out RuleList ruleList) && ruleList.NodeType == typeCode)
					{
						int namecode = -1;
						object obj = null;
						((IRuleList<TreeDataDeserialize>)ruleList).SendRef(this, ref obj, ref namecode);
						Array array = (obj as Array);
						int i = 0;
						foreach (var item in array)
						{
							data.AddNumberNode(i++, out TreeDataValue treeValue);
							treeValue.TypeName = type.GetElementType().ToString();
							treeValue.Value = item;
						}
					}
				}
				else //非基础数组类型，递归
				{
					for (int i = 0; i < totalLength; i++) GetTreeData(data, i);
				}
			}
			else
			{
				data = node is TreeData treeData ? treeData.AddNumberNode(number, out TreeData _) : node.AddTemp(out TreeData _);
				data.TypeName = type?.ToString();

				for (int i = 0; i < count; i++)
				{
					//读取字段名称码
					this.ReadUnmanaged(out int nameCode);
					GetTreeData(data, nameCode);
				}
			}
			return data;
		}
		#endregion
	}

}
