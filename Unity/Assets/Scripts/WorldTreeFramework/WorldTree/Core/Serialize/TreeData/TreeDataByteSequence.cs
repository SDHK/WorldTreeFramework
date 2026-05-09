/****************************************

* 作者：闪电黑客
* 日期：2024/11/15 10:10

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
		/// 空对象标记
		/// </summary>
		public const int NullObject = -1;

		/// <summary>
		/// 自动适配类型
		/// </summary>
		public const int AutoObject = 0;

		/// <summary>
		/// 反序列化自身类型模式
		/// </summary>
		public const int DeserializeSelfMode = -1;

		/// <summary>
		/// 非引用对象标记
		/// </summary>
		public const int UnRefObject = -1;
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
		/// 只写入值，不写入类型（用于数组类型的元素写入）
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
				self.World.PoolGetUnit(out self.TypeToTypeIdDict);
				self.World.PoolGetUnit(out self.TypeNameToTypeIdDict);
				self.World.PoolGetUnit(out self.codeToTypeNameDict);
				self.World.PoolGetUnit(out self.TypeIdToCodeList);
				self.World.PoolGetUnit(out self.IdToTypeIdList);
				self.World.PoolGetUnit(out self.ObjectToIdDict);
				self.World.PoolGetUnit(out self.IdToObjectDict);
				self.World.PoolGetUnit(out self.IdToDataPointList);
			}
		}

		class RemoveRule : RemoveRule<TreeDataByteSequence>
		{
			protected override void Execute(TreeDataByteSequence self)
			{
				self.Clear();
				self.TypeToTypeIdDict.Dispose();
				self.TypeNameToTypeIdDict.Dispose();
				self.codeToTypeNameDict.Dispose();
				self.TypeIdToCodeList.Dispose();
				self.IdToTypeIdList.Dispose();
				self.ObjectToIdDict.Dispose();
				self.IdToObjectDict.Dispose();
				self.IdToDataPointList.Dispose();
			}
		}
	}

	/// <summary>
	/// 树数据字节序列
	/// </summary>
	public partial class TreeDataByteSequence : ByteSequence
		, AsRule<ITreeDataSerialize>
		, AsRule<ITreeDataDeserialize>
	{
		#region 字段

		/// <summary>
		/// 递归层级
		/// </summary>
		public int Layer;

		/// <summary>
		/// 最大递归层级
		/// </summary>
		public int LayerMax = 1000;

		/// <summary>
		/// 类型对应类型Id字典
		/// </summary>
		public UnitDictionary<Type, int> TypeToTypeIdDict;

		/// <summary>
		/// 类型码对应类型名称字典，64哈希码对应
		/// </summary>
		public UnitDictionary<long, string> codeToTypeNameDict;

		/// <summary>
		/// 正数Id对应类型码
		/// </summary>
		public UnitList<long> TypeIdToCodeList;


		/// <summary>
		/// 对象实例Id对应类型Id
		/// </summary>
		public UnitList<int> IdToTypeIdList;
		/// <summary>
		/// 对象实例引用Id 对应在流中的起始位置
		/// </summary>
		public UnitList<int> IdToDataPointList;


		/// <summary>
		/// 对象实例对应Id
		/// </summary>
		public UnitDictionary<object, int> ObjectToIdDict;
		/// <summary>
		/// Id对应对象实例
		/// </summary>
		public UnitDictionary<int, object> IdToObjectDict;


		#endregion

		#region 映射表

		/// <summary>
		/// 获取或添加类型TypeId
		/// </summary>
		private int GetOrAddTypeId(Type type, bool isIgnoreName = false)
		{
			long typeCode;
			if (!TypeToTypeIdDict.TryGetValue(type, out int typeId))
			{
				if (TreeDataTypeHelper.TryGetTypeCode(type, out byte typeByteCode))
					typeCode = typeByteCode;
				else
					typeCode = this.TypeToCode(type);
				typeId = TypeIdToCodeList.Count;
				TypeIdToCodeList.Add(typeCode);
				TypeToTypeIdDict.Add(type, typeId);
			}

			// 忽略则不写入类型名称
			if (isIgnoreName) return typeId;
			// 如果类型是基础类型，则不写入类型名称
			if (TreeDataTypeHelper.ContainsType(type)) return typeId;
			// 拿到类型码，写入类型名称
			typeCode = TypeIdToCodeList[typeId];
			codeToTypeNameDict.TryAdd(typeCode, type.ToString());
			return typeId;
		}

		/// <summary>
		/// 尝试获取类型
		/// </summary>
		private bool TryIdGetType(int typeId, out Type type)
		{
			type = null;
			if (typeId >= TypeIdToCodeList.Count) return false;
			return TryCodeGetType(TypeIdToCodeList[typeId], out type);
		}

		/// <summary>
		/// 尝试获取类型
		/// </summary>
		private bool TryCodeGetType(long typeCode, out Type type)
		{
			// 先尝试获取基础类型
			if (TreeDataTypeHelper.TryGetType(typeCode, out type)) return true;
			// 再尝试获取已经存在的类型
			if (this.TryCodeToType(typeCode, out type)) return true;
			// 最后尝试通过类型名称获取类型，拿到类型后写入映射表
			if (!codeToTypeNameDict.TryGetValue(typeCode, out string typeName)) return false;
			type = System.Type.GetType(typeName);
			// 如果类型不存在，当前环境没有这个类型，无法反序列化，返回false
			if (type == null) return false;
			// 写入映射表
			this.TypeToCode(type);
			return true;
		}

		/// <summary>
		/// 尝试用对象实例Id获取类型Id
		/// </summary>
		private bool TryGetTypeId(int objId, out int typeId)
		{
			typeId = 0;
			if (objId >= IdToTypeIdList.Count) return false;
			typeId = IdToTypeIdList[objId];
			return true;
		}

		/// <summary>
		/// 尝试用类型Id获取类型码
		/// </summary>
		private bool TryGetTypeCode(int typeId, out long typeCode)
		{
			typeCode = 0;
			if (typeId >= TypeIdToCodeList.Count) return false;
			typeCode = TypeIdToCodeList[typeId];
			return true;
		}
		#endregion

		#region 序列化

		/// <summary>
		/// 序列化
		/// </summary>
		public void Serialize<T>(in T value)
		{
			Layer = 0;
			WriteValue(value, SerializedTypeMode.ObjectType);
			WriteDataInfo();
		}

		/// <summary>
		/// 反序列化
		/// </summary>
		public unsafe void Deserialize<T>(ref T value)
		{
			ReadDataInfo();
			Layer = 0;
			ReadValue(ref value);
		}

		/// <summary>
		/// 反序列化
		/// </summary>
		public unsafe void Deserialize(Type type, ref object value)
		{
			ReadDataInfo();
			Layer = 0;
			ReadValue(type, ref value);
		}

		/// <summary>
		/// 写入数据信息
		/// </summary>
		private void WriteDataInfo()
		{
			//记录映射表起始位置
			int startPoint = Length;
			//写入类型数量
			this.WriteDynamic(TypeIdToCodeList.Count);
			foreach (var code in TypeIdToCodeList)
			{
				//写入类型码
				this.WriteDynamic(code);
				//写入类型名称
				if (codeToTypeNameDict.TryGetValue(code, out string value))
				{
					WriteString(value);
				}
				else
				{
					WriteString(null);
				}
			}
			this.WriteDynamic(IdToTypeIdList.Count);
			for (int i = 0; i < IdToTypeIdList.Count; i++)
			{
				//写入类型Id
				this.WriteDynamic(IdToTypeIdList[i]);
				//写入读取位置
				this.WriteDynamic(IdToDataPointList[i]);
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
			this.ReadDynamic(out int typeCount);
			for (int i = 0; i < typeCount; i++)
			{
				//读取类型码
				this.ReadDynamic(out long typeCode);
				TypeIdToCodeList.Add(typeCode);

				//读取类型名称
				string typeName = ReadString();
				if (typeName != null) codeToTypeNameDict.Add(typeCode, typeName);
			}
			this.ReadDynamic(out int idCount);
			for (int i = 0; i < idCount; i++)
			{
				//读取类型Id
				this.ReadDynamic(out int typeId);
				//读取读取位置
				this.ReadDynamic(out int readPoint);
				IdToTypeIdList.Add(typeId);
				IdToDataPointList.Add(readPoint);
			}

			//读取指针定位到数据起始位置
			readPoint = 0;
			readBytePoint = 0;
			readSegmentPoint = 0;
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
		/// <param name="isRef">是否为可引用类型</param>
		/// <param name="writeType">写入类型，为null则写入泛型</param>
		/// <returns>是否为Null退出</returns>
		public bool TryWriteDataHead<T>(in object value, SerializedTypeMode typeMode, int count, out T obj, bool isIgnoreName = false, bool isRef = false, Type writeType = null)
		{
			if (typeMode == SerializedTypeMode.Value)
			{
				obj = default;
				this.WriteType(typeof(object));
				this.WriteDynamic(TreeDataCode.NullObject);
				this.LogError("错误，TryWriteDataHead不支持Value模式写入");
				return true;
			}

			//isRef 表示这个类型是可引用类型，并且不是抽象和接口。
			//否则即使是引用类型也当做值类型处理，不进行引用检测。
			if (!isRef || value == null)
			{
				switch (typeMode)
				{
					case SerializedTypeMode.ObjectType:
						this.WriteType(typeof(object));
						if (this.WriteCheckNull(value, count, out obj)) return true; break;
					case SerializedTypeMode.DataType:
						this.WriteType(writeType ?? typeof(T), isIgnoreName);
						if (this.WriteCheckNull(value, count, out obj)) return true; break;
				}
			}
			// 引用类型实例判断，检测是否已经写入过
			else if (ObjectToIdDict.TryGetValue(value, out int objId))
			{
				// 这个实例写入过，写入对象Id
				this.WriteDynamic(objId);
				obj = (T)value;
				// 如果写入类型模式为DataType，写入类型Id。
				if (typeMode == SerializedTypeMode.DataType)
				{
					// 可能先被写为Object，后出现多态字段引用需要覆盖写入真实类型Id。
					int newTypeId = GetOrAddTypeId(writeType ?? typeof(T), isIgnoreName);
					if (this.IdToTypeIdList[~objId] != newTypeId) IdToTypeIdList[~objId] = newTypeId;
				}
				return true;
			}
			// 没有写入过，写入对象Id和类型Id
			else
			{
				// 负数Id为新对象实例引用Id
				objId = ~IdToDataPointList.Count;
				switch (typeMode)
				{
					case SerializedTypeMode.ObjectType:
						AddNewObjectMap(GetOrAddTypeId(typeof(object)), value, objId);
						if (this.WriteCheckNull(value, count, out obj)) return true; break;
					case SerializedTypeMode.DataType:
						AddNewObjectMap(GetOrAddTypeId(writeType ?? typeof(T), isIgnoreName), value, objId);
						if (this.WriteCheckNull(value, count, out obj)) return true; break;
				}
			}
			obj = (T)value;
			return false;
		}

		/// <summary>
		/// 添加新对象映射
		/// </summary>
		private void AddNewObjectMap(int typeId, object value, int objId)
		{
			// 写入对象实例引用Id
			this.WriteDynamic(objId);
			// 记录类型Id
			IdToTypeIdList.Add(typeId);
			// 记录引用类型映射
			ObjectToIdDict.Add(value, objId);
			IdToObjectDict.Add(objId, value);
			// 记录实例数据读取位置
			IdToDataPointList.Add(Length);
		}


		/// <summary>
		/// 写入字段数量或value为空值标记
		/// </summary>
		/// <returns> value为Null返回True提前退出 </returns>
		private bool WriteCheckNull<T>(in object value, int count, out T obj)
		{
			// 判断如果对象不是Null，并且不等于默认值，则写入字段数量，否则写入Null标记
			if (value is T objValue && !EqualityComparer<T>.Default.Equals(objValue, default))
			{
				obj = objValue;
				this.WriteDynamic(count);
				return false;
			}
			obj = default;
			this.WriteDynamic(TreeDataCode.NullObject);
			return true;
		}

		/// <summary>
		/// 写入类型，默认写入类型名称
		/// </summary>
		public void WriteType(Type type, bool isIgnoreName = false) => this.WriteDynamic(GetOrAddTypeId(type, isIgnoreName));

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
			long typeCode = this.World.TypeToCode(type);
			this.World.Line.Core.RuleManager.SupportNodeRule(typeCode);

			//动态支持多维数组
			if (type.IsArray) this.World.Line.Core.RuleManager.SupportGenericParameterNodeRule(type.GetElementType(), typeof(TreeDataSerialize));

			//优化：可存住RuleGroup，减少查找次数
			if (this.World.Line.Core.RuleManager.TryGetRuleList<TreeDataSerialize>(typeCode, out RuleList ruleList) && ruleList.NodeType == typeCode)
			{
				((IRuleList<TreeDataSerialize>)ruleList).SendRef(this, ref Unsafe.AsRef<object>(value), ref typeMode);
			}
			else
			{
				//不支持的类型，写入空对象
				this.WriteType(typeof(object));
				this.WriteDynamic(TreeDataCode.NullObject);
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
			long typeCode = this.World.TypeToCode(type);
			this.World.Line.Core.RuleManager.SupportNodeRule(typeCode);

			//动态支持多维数组
			if (type.IsArray) this.World.Line.Core.RuleManager.SupportGenericParameterNodeRule(type.GetElementType(), typeof(TreeDataSerialize));

			if (this.World.Line.Core.RuleManager.TryGetRuleList<TreeDataSerialize>(typeCode, out RuleList ruleList) && ruleList.NodeType == typeCode)
			{
				((IRuleList<TreeDataSerialize>)ruleList).SendRef(this, ref Unsafe.AsRef<object>(value), ref typeMode);
			}
			else
			{
				//不支持的类型，写入空对象
				this.WriteType(typeof(object));
				this.WriteDynamic(TreeDataCode.NullObject);
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
		public void ReadValue(Type type, ref object value, int fieldNameCode = TreeDataCode.DeserializeSelfMode)
		{
			Layer++;
			if (Layer > LayerMax)
			{
				this.LogError("反序列化超出最大层级");
				return;
			}
			//假如是枚举类型，则获取枚举基础类型
			if (type.IsEnum) type = type.GetEnumUnderlyingType();
			long typeCode = this.World.TypeToCode(type);
			this.World.Line.Core.RuleManager.SupportNodeRule(typeCode);

			//动态支持多维数组
			if (type.IsArray) this.World.Line.Core.RuleManager.SupportGenericParameterNodeRule(type.GetElementType(), typeof(TreeDataDeserialize));
			if (this.World.Line.Core.RuleManager.TryGetRuleList<TreeDataDeserialize>(typeCode, out RuleList ruleList) && ruleList.NodeType == typeCode)
			{
				((IRuleList<TreeDataDeserialize>)ruleList).SendRef(this, ref value, ref fieldNameCode);
			}
			else//不支持的类型，跳跃数据
			{
				SkipData();
			}
			Layer--;
		}

		/// <summary>
		/// 尝试读取类型
		/// </summary>
		public bool TryReadType(out Type type)
		{
			if (this.ReadDynamic(out int typeId) < 0) typeId = IdToTypeIdList[~typeId];
			return TryIdGetType(typeId, out type);
		}

		/// <summary>
		/// 尝试读取类型数据头部
		/// </summary>
		public bool TryReadClassHead(Type targetType, ref object value, out int count, out int objId, out int jumpReadPoint)
		{
			if (TryReadDataHead(targetType, ref value, out count, out int countPoint, out objId, out jumpReadPoint)) return true;
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
		public bool TryReadArrayHead(Type targetType, ref object value, int targetCount, out int objId, out int jumpReadPoint)
		{
			if (TryReadDataHead(targetType, ref value, out int count, out int countPoint, out objId, out jumpReadPoint)) return true;
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
		private bool TryReadDataHead(Type targetType, ref object value, out int count, out int countPoint, out int objId, out int jumpReadPoint)
		{
			countPoint = readPoint;
			count = 0;
			this.ReadDynamic(out int typeId);
			jumpReadPoint = -1;//-f表示不需要跳跃
			if (typeId < 0)
			{
				// 为负数则为对象实例Id
				objId = ~typeId;
				int dataPoint = IdToDataPointList[objId];
				// 判断位置不一致，说明这里是引用地址
				if (dataPoint != readPoint)
				{
					// 判断如果这个引用Id已经被读取过，拿到实例返回，否则跳跃到数据位置读取
					if (IdToObjectDict.TryGetValue(objId, out value)) { return true; }
					// 记录当前数据起始点
					jumpReadPoint = readPoint;
					// 跳跃到数据位置读取（也就是ObjId后的Count数据）
					ReadJump(dataPoint);
				}
				// 拿到类型Id
				TryGetTypeId(objId, out typeId);
			}
			else
			{
				// 非引用实例标记，供上层判断是否登记 IdToObjectDict。
				// 同时防止结构体读为类型的情况，进行标记。
				objId = TreeDataCode.UnRefObject;
			}

			// 拿到类型Id尝试获取类型码
			TryGetTypeCode(typeId, out long typeCode);
			//判断如果是0,对应object的类型码，则为原类型
			if (typeCode == TreeDataCode.AutoObject)
			{
				countPoint = readPoint;
				this.ReadDynamic(out count);
				// 如果count是null标记那么直接返回null。
				if (count != TreeDataCode.NullObject) return false;
				value = default;
				return true;
			}
			//尝试获取类型
			if (TryCodeGetType(typeCode, out Type dataType))
			{
				//类型一样直接读取
				if (dataType == targetType)
				{
					countPoint = readPoint;
					this.ReadDynamic(out count);
					if (count != TreeDataCode.NullObject) return false;
					value = default;
					return true;
				}
				//不一样,判断多态类型，不是则尝试读取
				else if (!SubTypeReadValue(dataType, targetType, ref value, countPoint))
				{
					countPoint = readPoint;
					this.ReadDynamic(out count);
					if (count != TreeDataCode.NullObject) return false;
					value = default;
					return true;
				}
				else
				{
					return true;
				}
			}

			//数据类型不存在 ，判断目标类型是否非基础类型
			if (!TreeDataTypeHelper.CheckBasicsType(targetType))
			{
				countPoint = readPoint;
				//不是基础类型则尝试读取
				this.ReadDynamic(out count);
				if (count != TreeDataCode.NullObject) return false;
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
				if (TreeDataTypeHelper.CheckBasicsType(type))
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

			// 如果目标类型是接口或者类，判断类型是否为目标类型的子类型
			bool isSubType = false;
			if (targetType.IsInterface || targetType.IsClass)
			{
				if (targetType.IsAssignableFrom(type)) isSubType = true;
			}
			else //不是接口也不是类型，直接跳跃数据
			{
				SkipData(type);
				return true;
			}

			if (isSubType)// 是子类型
			{
				// 读取指针回退到类型码
				ReadJump(typePoint);
				// 子类型读取
				ReadValue(type, ref value);
				return true;
			}
			else // 不是子类型，返回去尝试读取。
			{
				return false;
			}
		}


		#endregion

		#region 跳跃

		/// <summary>
		/// 跳跃数据，跳跃前需要回退到类型
		/// </summary>
		public void SkipData()
		{
			if (this.ReadDynamic(out int typeId) < 0)
			{
				typeId = ~typeId;
				//判断位置不一致，说明这里是引用地址，后续没有数据，直接跳跃
				if (IdToDataPointList[typeId] != readPoint) return;
				typeId = IdToTypeIdList[typeId];
			}
			TryIdGetType(typeId, out Type type);
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
				if (TreeDataTypeHelper.TryGetUnmanagedTypeSize(type, out int byteCount))
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
			if (count == TreeDataCode.NullObject) return;

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
				if (type != null && type.IsArray && TreeDataTypeHelper.TryGetUnmanagedTypeSize(type.GetElementType(), out int arrayByteCount))
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
			if (this.ReadDynamic(out int length) == TreeDataCode.NullObject) return;
			else if (length == 0) return;
			this.ReadUnmanaged(out length);
			ReadSkip(length);
		}

		#endregion
	}
}
