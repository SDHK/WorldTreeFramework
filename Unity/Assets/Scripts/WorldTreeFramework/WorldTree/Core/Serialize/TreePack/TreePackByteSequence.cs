/****************************************

* 作者：闪电黑客
* 日期：2024/10/15 21:07

* 描述：

*/
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace WorldTree
{

	public static partial class TreePackByteSequenceRule
	{
		/// <summary>
		/// 序列化写入类型
		/// </summary>
		public static void Serialize<T>(this TreePackByteSequence self, in T value)
		{
			self.WriteValue(value);
		}

		/// <summary>
		/// 反序列化读取类型
		/// </summary>
		public static void Deserialize<T>(this TreePackByteSequence self, ref T value)
		{
			self.ReadValue(ref value);
		}

		class AddRule : AddRule<TreePackByteSequence>
		{
			protected override void Execute(TreePackByteSequence self)
			{
				self.GetBaseRule<TreePackByteSequence, ByteSequence, Add>().Send(self);

				// 获取节点的法则集
				self.Core.RuleManager.NodeTypeRulesDict.TryGetValue(self.Type, out self.unmanagedRuleDict);
				self.Core.RuleManager.TryGetRuleGroup<ITreePackSerialize>(out self.serializeRuleDict);
				self.Core.RuleManager.TryGetRuleGroup<ITreePackDeserialize>(out self.deserializeRuleDict);
			}
		}
	}

	/// <summary>
	/// 树包字节序列
	/// </summary>
	public class TreePackByteSequence : ByteSequence
		, AsRule<ITreePackSerialize>
		, AsRule<ITreePackDeserialize>
	{
		/// <summary>
		/// 自身拥有的非委托类型法则列表集合
		/// </summary>
		/// <remarks> RuleTypeCode, 法则列表 </remarks>
		public Dictionary<long, RuleList> unmanagedRuleDict;

		/// <summary>
		/// 不同类型序列化法则列表集合
		/// </summary>
		public RuleGroup serializeRuleDict;

		/// <summary>
		/// 不同类型反序列化法则列表集合
		/// </summary>
		public RuleGroup deserializeRuleDict;


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
		/// 写入值
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValue<T>(in T value)
		{
			Type type = typeof(T);
			long typeCode = Core.TypeToCode<T>();
			Core.RuleManager.SupportNodeRule(typeCode);//数组不需要支持
			if (type.IsArray)
				this.Core.RuleManager.SupportGenericParameterNodeRule(type.GetElementType(), typeof(ITreePackSerialize));
			if (serializeRuleDict.TryGetValue(typeCode, out RuleList ruleList))
			{
				foreach (IRule rule in ruleList) Unsafe.As<TreePackSerializeRule<T>>(rule).Invoke(this, ref Unsafe.AsRef(value));
				return;
			}
			Core.RuleManager.SupportGenericRule<T>(typeof(TreePackSerializeUnmanaged<>));
			if (unmanagedRuleDict.TryGetValue(Core.TypeToCode<TreePackSerializeUnmanaged<T>>(), out ruleList))
				((IRuleList<TreePackSerializeUnmanaged<T>>)ruleList).SendRef(this, ref Unsafe.AsRef(value));
		}

		//类型码读取，子类转换

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
				WriteUnmanaged(ValueMarkCode.NULL_OBJECT);
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
				WriteUnmanaged(ValueMarkCode.NULL_OBJECT);
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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValue<T>(ref T value)
		{
			Type type = typeof(T);
			long typeCode = Core.TypeToCode<T>();
			Core.RuleManager.SupportNodeRule(typeCode);
			if (type.IsArray)
				this.Core.RuleManager.SupportGenericParameterNodeRule(type.GetElementType(), typeof(ITreePackDeserialize));
			if (deserializeRuleDict.TryGetValue(typeCode, out RuleList ruleList))
			{
				foreach (IRule rule in ruleList) Unsafe.As<TreePackDeserializeRule<T>>(rule).Invoke(this, ref value);
				return;
			}

			Core.RuleManager.SupportGenericRule<T>(typeof(TreePackDeserializeUnmanaged<>));
			if (unmanagedRuleDict.TryGetValue(Core.TypeToCode<TreePackDeserializeUnmanaged<T>>(), out ruleList))
				((IRuleList<TreePackDeserializeUnmanaged<T>>)ruleList).SendRef(this, ref value);
		}

		/// <summary>
		/// 读取值
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T ReadValue<T>()
		{
			T value = default;
			ReadValue(ref value);
			return value;
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
					//SkipData(type);
					return true;
				}
			}
			else
			{
				//类型不存在直接跳跃数据
				//SkipData(type);
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
				//SkipData(type);
				return true;
			}

			if (isSubType)//是子类型
			{
				//读取指针回退到类型码
				ReadJump(typePoint);
				//子类型读取
				//ReadValue(type, ref value);
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
			if (ReadUnmanaged(out int length) == ValueMarkCode.NULL_OBJECT) return null;
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
	}

}
