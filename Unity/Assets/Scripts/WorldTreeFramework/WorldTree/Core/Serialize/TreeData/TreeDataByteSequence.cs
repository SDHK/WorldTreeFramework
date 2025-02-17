﻿/****************************************

* 作者：闪电黑客
* 日期：2024/11/15 10:10

* 描述：

*/
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace WorldTree
{

	/// <summary>
	/// 树数据标记编码
	/// </summary>
	public static class TreeDataCode
	{
		/// <summary>
		/// 空对象标记
		/// </summary>
		public const int NULL_OBJECT = -1;

		/// <summary>
		/// 自动适配类型
		/// </summary>
		public const int AUTO_OBJECT = 0;

		/// <summary>
		/// 反序列化自身类型模式
		/// </summary>
		public const int DESERIALIZE_SELF_MODE = -1;
	}

	/// <summary>
	/// 数据写入类型模式
	/// </summary>
	public enum SerializedTypeMode
	{
		/// <summary>
		/// 写入真实类型
		/// </summary>
		DataType = -1,

		/// <summary>
		/// 写入Object类型
		/// </summary>
		ObjectType = -2,

		/// <summary>
		/// 只写入值，不写入类型
		/// </summary>
		Value = -3,
	}

	public static class TreeDataByteSequenceRule
	{
		class AddRule : AddRule<TreeDataByteSequence>
		{
			protected override void Execute(TreeDataByteSequence self)
			{
				TreeDataTypeHelper.InitTypes(self);


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

		#region 序列化

		/// <summary>
		/// 序列化
		/// </summary>
		public void Serialize<T>(in T value)
		{
			Layer = 0;
			//写入数据
			WriteValue(value);
			WriteDataInfo();
		}

		/// <summary>
		/// 反序列化
		/// </summary>
		public unsafe void Deserialize<T>(ref T value)
		{
			ReadDataInfo();
			Layer = 0;
			//读取数据
			ReadValue(ref value);
		}

		/// <summary>
		/// 写入数据信息
		/// </summary>
		private void WriteDataInfo()
		{
			//记录映射表起始位置
			int startPoint = Length;
			//写入类型数量
			WriteUnmanaged(codeToTypeNameDict.Count);
			foreach (var item in codeToTypeNameDict)
			{
				//写入类型码
				WriteUnmanaged(item.Key);
				//写入类型名称
				WriteString(item.Value);
			}
			WriteUnmanaged(startPoint);
		}

		/// <summary>
		/// 读取数据信息
		/// </summary>
		private void ReadDataInfo()
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

			//this.Log($"TypeCount: {codeToTypeNameDict.Count}");
			//foreach (var item in this.codeToTypeNameDict)
			//{
			//	this.Log($"Type: {item.Value}");
			//}
		}

		#endregion

		#region 写入

		/// <summary>
		/// 写入字段数量或空标记
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value">类型</param>
		/// <param name="typeMode">字段码</param>
		/// <param name="count">字段数或数组维度>/param>
		/// <param name="obj">希望返回的对象类型</param>
		/// <param name="isIgnoreName">是否忽略写入名称</param>
		/// <param name="writeType">写入类型，为null则写入泛型</param>
		/// <returns>是否为Null退出</returns>
		public bool TryWriteDataHead<T>(in object value, SerializedTypeMode typeMode, int count, out T obj, bool isIgnoreName = false, Type writeType = null)
		{
			switch (typeMode)
			{
				case SerializedTypeMode.ObjectType:
					this.WriteType(typeof(object));
					if (this.WriteCheckNull(value, count, out obj)) return true;
					break;
				case SerializedTypeMode.DataType:
					this.WriteType(writeType ?? typeof(T), isIgnoreName);
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
			this.WriteDynamic(TreeDataCode.NULL_OBJECT);
			return true;
		}

		/// <summary>
		/// 写入类型，默认写入类型名称
		/// </summary>
		public void WriteType(Type type, bool isIgnoreName = false) => this.WriteDynamic(GetTypeCode(type, isIgnoreName));

		/// <summary>
		/// 写入值
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValue<T>(in T value, SerializedTypeMode typeMode = SerializedTypeMode.DataType)
		{
			Layer++;
			if (Layer > LayerMax)
			{
				this.LogError("序列化超出最大层级");
				return;
			}
			Type originalType = typeof(T);
			Type type = value?.GetType();
			//如果类型为空，或者类型和原始类型一致，写入Object类型。
			if (type == null)
			{
				type = originalType;
				typeMode = SerializedTypeMode.ObjectType;
			}
			else if (type == originalType)
			{
				typeMode = SerializedTypeMode.ObjectType;
			}
			//假如是枚举类型，则获取枚举基础类型
			if (type.IsEnum) type = type.GetEnumUnderlyingType();
			long typeCode = this.Core.TypeToCode(type);
			this.Core.RuleManager.SupportNodeRule(typeCode);

			//动态支持多维数组
			if (type.IsArray) this.Core.RuleManager.SupportGenericParameterNodeRule(type.GetElementType(), typeof(TreeDataSerialize));

			if (this.Core.RuleManager.TryGetRuleList<TreeDataSerialize>(typeCode, out RuleList ruleList) && ruleList.NodeType == typeCode)
			{
				((IRuleList<TreeDataSerialize>)ruleList).SendRef(this, ref Unsafe.AsRef<object>(value), ref typeMode);
			}
			else
			{
				//不支持的类型，写入空对象
				this.WriteType(typeof(object));
				this.WriteDynamic(TreeDataCode.NULL_OBJECT);
			}
			Layer--;
		}

		/// <summary>
		/// 指定类型写入值
		/// </summary>
		public void WriteValue(Type type, in object value, SerializedTypeMode typeMode = SerializedTypeMode.DataType)
		{
			Layer++;
			//假如是枚举类型，则获取枚举基础类型
			if (type.IsEnum) type = type.GetEnumUnderlyingType();
			long typeCode = this.Core.TypeToCode(type);
			this.Core.RuleManager.SupportNodeRule(typeCode);

			//动态支持多维数组
			if (type.IsArray) this.Core.RuleManager.SupportGenericParameterNodeRule(type.GetElementType(), typeof(TreeDataSerialize));

			if (this.Core.RuleManager.TryGetRuleList<TreeDataSerialize>(typeCode, out RuleList ruleList) && ruleList.NodeType == typeCode)
			{
				((IRuleList<TreeDataSerialize>)ruleList).SendRef(this, ref Unsafe.AsRef<object>(value), ref typeMode);
			}
			else
			{
				//不支持的类型，写入空对象
				this.WriteType(typeof(object));
				this.WriteDynamic(TreeDataCode.NULL_OBJECT);
			}
			Layer--;
		}

		/// <summary>
		/// 写入字符串
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteString(string value)
		{
			if (Utf8)
				WriteUtf8(value);
			else
				WriteUtf16(value);
		}

		/// <summary>
		/// 写入字符串
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteUtf16(string value)
		{
			if (value == null)
			{
				WriteUnmanaged(TreeDataCode.NULL_OBJECT);
				return;
			}
			if (value.Length == 0)
			{
				WriteUnmanaged(0);
				return;
			}

			//获取字符串长度,因为 UTF-16 编码的每个字符占用 2 个字节，checked 防止溢出int值
			var copyByteCount = checked(value.Length * 2);
			//这行代码获取一个引用，指向一个足够大的缓冲区，以容纳字符串的字节数和额外的 4 个字节。
			ref byte dest = ref GetWriteRefByte(copyByteCount + 4);
			//这行代码将字符串的长度（以字节为单位）写入缓冲区的前 4 个字节
			Unsafe.WriteUnaligned(ref dest, value.Length * 2);
			//这行代码将字符串的实际字节数据复制到缓冲区中，跳过前 4 个字节
			MemoryMarshal.AsBytes(value.AsSpan()).CopyTo(MemoryMarshal.CreateSpan(ref Unsafe.Add(ref dest, 4), copyByteCount));
		}

		/// <summary>
		/// 写入字符串
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteUtf8(string value)
		{
			if (value == null)
			{
				WriteUnmanaged(TreeDataCode.NULL_OBJECT);
				return;
			}
			if (value.Length == 0)
			{
				WriteUnmanaged(0);
				return;
			}

			// (int utf16-length, int utf8-byte-count, utf8-bytes)
			ReadOnlySpan<char> source = value.AsSpan();

			// 由于不知道空间大小，所以字符数*3只是获取一个可能的最大空间，字符最小可能是只占1个字节
			int maxByteCount = (source.Length + 1) * 3;

			//申请总空间，包含utf8长度和数据

			// 头部需要写入byte真实长度，int长度偏移+4
			ref byte destPointer = ref GetWriteRefByte(maxByteCount + 4);

			//申请数据空间，byte长度int要写到头部，所以要偏移4
			Span<byte> dest = MemoryMarshal.CreateSpan(ref Unsafe.Add(ref destPointer, 4), maxByteCount);

			// 数据写入到dest，此时拿到了byte的真实长度
			int bytesWritten = Encoding.UTF8.GetBytes(value, dest);

			//~0 的结果是 -1，但前面if挡住了，所以不会出现-1，所以可以用来区分8位和16位
			Unsafe.WriteUnaligned(ref destPointer, bytesWritten);

			// 重新定位指针，裁剪空间
			WriteBack(maxByteCount - bytesWritten);
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
		public void ReadValue(Type type, ref object value, int fieldNameCode = TreeDataCode.DESERIALIZE_SELF_MODE)
		{
			Layer++;
			if (Layer > LayerMax)
			{
				this.LogError("反序列化超出最大层级");
				return;
			}
			//假如是枚举类型，则获取枚举基础类型
			if (type.IsEnum) type = type.GetEnumUnderlyingType();
			long typeCode = this.Core.TypeToCode(type);
			this.Core.RuleManager.SupportNodeRule(typeCode);

			//动态支持多维数组
			if (type.IsArray) this.Core.RuleManager.SupportGenericParameterNodeRule(type.GetElementType(), typeof(TreeDataDeserialize));

			if (this.Core.RuleManager.TryGetRuleList<TreeDataDeserialize>(typeCode, out RuleList ruleList) && ruleList.NodeType == typeCode)
			{
				((IRuleList<TreeDataDeserialize>)ruleList).SendRef(this, ref value, ref fieldNameCode);
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
		public bool TryReadType(out Type type) => TryGetType(this.ReadDynamic(out long _), out type);

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
				if (count != TreeDataCode.NULL_OBJECT) return false;
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
					if (count != TreeDataCode.NULL_OBJECT) return false;
					value = default;
					return true;
				}
				//不一样,判断多态类型，不是则尝试读取
				else if (!SubTypeReadValue(dataType, targetType, ref value, countPoint))
				{
					countPoint = readPoint;
					this.ReadDynamic(out count);
					if (count != TreeDataCode.NULL_OBJECT) return false;
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
				if (count != TreeDataCode.NULL_OBJECT) return false;
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

		/// <summary>
		/// 读取字符串
		/// </summary>
		public string ReadString()
		{
			if (ReadUnmanaged(out int length) == TreeDataCode.NULL_OBJECT) return null;
			else if (length == 0) return string.Empty;
			if (Utf8)
				return ReadUtf8(length);
			else
				return ReadUtf16(length);
		}

		/// <summary>
		/// 读取字符串
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ReadUtf16(int length)
		{
			if (ReadRemain < length)
			{
				this.LogError($"字符串长度超出数据长度: {length}.");
				return null;
			}
			ref byte src = ref GetReadRefByte(length);
			return new string(MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<byte, char>(ref src), (int)(length * 0.5f)));
		}

		/// <summary>
		/// 读取字符串
		/// </summary>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public string ReadUtf8(int length)
		{
			if (ReadRemain < length)
			{
				this.LogError($"字符串长度超出数据长度: {length}.");
				return null;
			}
			ref var spanRef = ref GetReadRefByte(length);
			return Encoding.UTF8.GetString(MemoryMarshal.CreateReadOnlySpan(ref spanRef, length));
		}

		#endregion

		#region 跳跃

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
			if (count == TreeDataCode.NULL_OBJECT) return;

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

		/// <summary>
		/// 跳过字符串
		/// </summary>
		public void SkipString()
		{
			if (ReadUnmanaged(out int length) == TreeDataCode.NULL_OBJECT) return;
			else if (length == 0) return;
			ReadSkip(length);
		}

		#endregion

		#region TreeData

		/// <summary>
		/// 序列化TreeData
		/// </summary>
		public void SerializeTreeData(TreeData treeData)
		{

			Layer = 0;
			SetTreeData(treeData);
			WriteDataInfo();
		}
		/// <summary>
		/// 序列化TreeData
		/// </summary>
		private void SetTreeData(TreeData treeData)
		{
			//判断是否是数组
			if (treeData is TreeDataArray treeDataArray)
			{
				//写入类型码
				long typeCode = treeDataArray.TypeName.GetHash64();

				//判断是否为基础类型
				if (Core.TryCodeToType(typeCode, out Type type) && TreeDataTypeHelper.TypeCodeDict.TryGetValue(type, out byte value))
				{
					typeCode = value;
				}
				else if (!(type != null && type.IsArray && TreeDataTypeHelper.TypeCodeDict.ContainsKey(type.GetElementType())))
				//非基础类型，写入名称
				{
					codeToTypeNameDict.TryAdd(typeCode, treeData.TypeName);
				}
				this.WriteDynamic(typeCode);

				//写入数组维度
				this.WriteDynamic(~treeDataArray.LengthList.Count);

				//判断这个类型是否是基础数组类型
				if (type != null && type.IsArray && TreeDataTypeHelper.TypeSizeDict.ContainsKey(type.GetElementType()))
				{
					Core.RuleManager.SupportGenericParameterNodeRule(type.GetElementType(), typeof(TreeDataSerialize));

					int count = 1;
					//写入数组长度
					foreach (var item in treeDataArray.LengthList)
					{
						count *= item;
						this.WriteDynamic(item);
					}

					long elementTypeCodeHash = this.Core.TypeToCode(type.GetElementType());
					//基础数组类型取值
					if (this.Core.RuleManager.TryGetRuleList<TreeDataSerialize>(elementTypeCodeHash, out RuleList ruleList))
					{
						SerializedTypeMode typeMode = SerializedTypeMode.Value;
						//一个个往里写
						foreach (var item in treeDataArray.ListNodeBranch().GetEnumerable())
						{
							((IRuleList<TreeDataSerialize>)ruleList).SendRef(this, ref Unsafe.AsRef<object>((item.Value as TreeDataValue).Value), ref typeMode);
						}
					}
				}
				else //非基础数组类型，递归
				{
					//写入数组长度
					foreach (var item in treeDataArray.LengthList) this.WriteDynamic(item);

					foreach (var item in treeData.ListNodeBranch())
					{
						SetTreeData(item as TreeData);
					}
				}
			}
			else if (treeData is TreeDataValue treeDataValue)
			{
				long typeCodeHash = treeDataValue.TypeName.GetHash64();
				//写入数值
				if (this.Core.RuleManager.TryGetRuleList<TreeDataSerialize>(typeCodeHash, out RuleList ruleList) && ruleList.NodeType == typeCodeHash)
				{
					SerializedTypeMode typeMode = SerializedTypeMode.DataType;
					((IRuleList<TreeDataSerialize>)ruleList).SendRef(this, ref Unsafe.AsRef<object>(treeDataValue.Value), ref typeMode);
				}
			}
			else
			{
				//写入类型码
				long typeCode = treeData.TypeName.GetHash64();
				//判断是否为基础类型
				if (Core.TryCodeToType(typeCode, out Type type) && TreeDataTypeHelper.TypeCodeDict.TryGetValue(type, out byte value))
				{
					typeCode = value;
				}
				else //非基础类型，写入名称
				{
					codeToTypeNameDict.TryAdd(typeCode, treeData.TypeName);
				}
				this.WriteDynamic(typeCode);

				//空对象判断
				if (treeData.IsDefault)
				{
					this.WriteDynamic(TreeDataCode.NULL_OBJECT);
					return;
				}

				//写入字段数量
				NumberNodeBranch branch = treeData.NumberNodeBranch();
				this.WriteDynamic(branch.Count);
				foreach (var item in branch.GetEnumerable())
				{
					//写入字段名称码
					this.WriteUnmanaged((int)item.Key);
					SetTreeData(item.Value as TreeData);
				}
			}
		}

		/// <summary>
		/// 反序列化出TreeData
		/// </summary>
		public TreeData DeserializeTreeData()
		{
			ReadDataInfo();
			Layer = 0;
			return GetTreeData(this.Parent);
		}

		/// <summary>
		/// 反序列化出TreeData
		/// </summary>
		private TreeData GetTreeData(INode node, int number = 0, bool isArray = false)
		{
			TreeData data;
			int startPoint = this.readPoint;
			//读取类型码
			this.ReadDynamic(out long typeCode);
			if (this.TryGetType(typeCode, out Type type))
			{
				//获取真实类型码
				if (TreeDataTypeHelper.TypeCodeDict.ContainsKey(type)) typeCode = this.Core.TypeToCode(type);
				//判断是否是基础类型
				if (TreeDataTypeHelper.BasicsTypeHash.Contains(type))
				{
					data = AddTreeData(node, out TreeDataValue treeValue, number, isArray);
					data.TypeName = type.ToString();
					//基础类型取值
					if (this.Core.RuleManager.TryGetRuleList<TreeDataDeserialize>(typeCode, out RuleList ruleList))
					{
						int fieldNameCode = 0;
						((IRuleList<TreeDataDeserialize>)ruleList).SendRef(this, ref treeValue.Value, ref fieldNameCode);
					}
					return data;
				}
			}

			//读取字段数量
			this.ReadDynamic(out int count);
			//空对象判断
			if (count == TreeDataCode.NULL_OBJECT)
			{
				data = AddTreeData(node, out TreeData _, number, isArray);
				data.TypeName = type?.ToString();
				data.IsDefault = true;
				return data;
			}

			//Type可能不存在的情况下，负数为数组类型
			if (count < 0)
			{
				data = AddTreeData(node, out TreeDataArray treeArray, number, isArray);
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
						int fieldNameCode = TreeDataCode.DESERIALIZE_SELF_MODE;
						object obj = null;
						((IRuleList<TreeDataDeserialize>)ruleList).SendRef(this, ref obj, ref fieldNameCode);
						Array array = (obj as Array);
						int i = 0;
						foreach (var item in array)
						{
							data.AddListNode(i++, out TreeDataValue treeValue);
							treeValue.TypeName = type.GetElementType().ToString();
							treeValue.Value = item;
						}
					}
				}
				else //非基础数组类型，递归
				{
					for (int i = 0; i < totalLength; i++) GetTreeData(data, i, true);
				}
			}
			else
			{
				data = AddTreeData(node, out TreeData _, number, isArray);
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

		/// <summary>
		/// 添加TreeData结构
		/// </summary>
		private T AddTreeData<T>(INode node, out T treeValue, int number, bool isArray)
			where T : TreeData
		{
			if (node is TreeData treeData)
			{
				if (isArray) treeData.AddListNode(number, out treeValue);
				else treeData.AddNumberNode(number, out treeValue);
			}
			else
			{
				node.AddTemp(out treeValue);
			}
			return treeValue;
		}
		#endregion
	}

}
