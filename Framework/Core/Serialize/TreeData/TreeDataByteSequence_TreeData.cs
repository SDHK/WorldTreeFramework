using System;
using System.Runtime.CompilerServices;

namespace WorldTree
{
	public partial class TreeDataByteSequence
	{
		/// <summary>
		/// 类型名称对应类型Id字典
		/// </summary>
		public UnitDictionary<string, int> TypeNameToTypeIdDict;

		/// <summary>
		/// 添加类型并获取TypeId
		/// </summary>
		private int GetOrAddTypeId(string typeName, bool isIgnoreName = false)
		{
			Type type = System.Type.GetType(typeName);
			long typeCode;
			if (!TypeNameToTypeIdDict.TryGetValue(typeName, out int typeId))
			{
				if (type != null)
				{
					// 如果类型是基础类型，则直接写入类型码
					if (TreeDataTypeHelper.TypeCodeDict.TryGetValue(type, out byte typeByteCode))
						typeCode = typeByteCode;
					else
						typeCode = this.TypeToCode(type);
				}
				else
				{
					typeCode = typeName.GetHash64();
				}

				typeId = TypeIdToCodeList.Count;
				TypeIdToCodeList.Add(typeCode);
				TypeNameToTypeIdDict.Add(typeName, typeId);
			}

			// 忽略则不写入类型名称
			if (isIgnoreName) return typeId;
			// 如果类型存在且是基础类型，则不写入类型名称
			if (type != null && TreeDataTypeHelper.TypeCodeDict.ContainsKey(type)) return typeId;
			// 拿到类型码，写入类型名称
			typeCode = TypeIdToCodeList[typeId];
			if (!codeToTypeNameDict.ContainsKey(typeCode))
			{
				codeToTypeNameDict.Add(typeCode, type.ToString());
			}
			return typeId;
		}


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
			//判断是否是引用
			if (treeData is TreeDataRef treeDataRef)
			{
				ObjectToIdDict.TryGetValue(treeDataRef.Data, out int objId);
				this.WriteDynamic(objId);
			}
			else if (treeData is TreeDataArray treeDataArray)
			{
				//写入类型码
				long typeCode = treeData.TypeName.GetHash64();

				//判断是否为基础类型
				if (World.TryCodeToType(typeCode, out Type type) && TreeDataTypeHelper.TypeCodeDict.TryGetValue(type, out byte value))
				{
					typeCode = value;
				}
				else if (!(type != null && type.IsArray && TreeDataTypeHelper.TypeCodeDict.ContainsKey(type.GetElementType())))
				//非基础类型，写入名称
				{
					codeToTypeNameDict.TryAdd(typeCode, treeData.TypeName);
				}

				if (treeData.IsRef && !treeData.IsDefault)
				{
					//负数Id为新对象
					int id = ~IdToObjectDict.Count;
					ObjectToIdDict.Add(treeData, id);
					IdToObjectDict.Add(id, treeData);
					IdToTypeIdList.Add(GetOrAddTypeId(treeData.TypeName));
					this.WriteDynamic(id);
					IdToDataPointList.Add(Length);//记录数据读取位置
				}
				else
				{
					this.WriteDynamic(typeCode);
				}

				//空对象判断
				if (treeData.IsDefault)
				{
					this.WriteDynamic(TreeDataCode.NullObject);
					return;
				}

				//写入数组维度
				this.WriteDynamic(~treeDataArray.LengthList.Count);

				//判断这个类型是否是基础数组类型
				if (type != null && type.IsArray && TreeDataTypeHelper.CheckUnmanagedType(type.GetElementType()))
				{
					this.World.Line.Core.RuleManager.SupportGenericParameterNodeRule(type.GetElementType(), typeof(TreeDataSerialize));

					int count = 1;
					//写入数组长度
					foreach (var item in treeDataArray.LengthList)
					{
						count *= item;
						this.WriteDynamic(item);
					}

					long elementTypeCodeHash = this.World.TypeToCode(type.GetElementType());
					//基础数组类型取值
					if (this.World.Line.Core.RuleManager.TryGetRuleList<TreeDataSerialize>(elementTypeCodeHash, out RuleList ruleList))
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
				if (this.World.Line.Core.RuleManager.TryGetRuleList<TreeDataSerialize>(typeCodeHash, out RuleList ruleList) && ruleList.NodeType == typeCodeHash)
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
				if (World.TryCodeToType(typeCode, out Type type) && TreeDataTypeHelper.TypeCodeDict.TryGetValue(type, out byte value))
				{
					typeCode = value;
				}
				else //非基础类型，写入名称
				{
					codeToTypeNameDict.TryAdd(typeCode, treeData.TypeName);
				}

				if (treeData.IsRef && !treeData.IsDefault)
				{
					//负数Id为新对象
					int id = ~IdToObjectDict.Count;
					ObjectToIdDict.Add(treeData, id);
					IdToObjectDict.Add(id, treeData);
					IdToTypeIdList.Add(GetOrAddTypeId(treeData.TypeName));
					this.WriteDynamic(id);
					IdToDataPointList.Add(Length);//记录数据读取位置
				}
				else
				{
					this.WriteDynamic(typeCode);
				}

				//空对象判断
				if (treeData.IsDefault)
				{
					this.WriteDynamic(TreeDataCode.NullObject);
					return;
				}

				//写入字段数量
				var branch = treeData.GenericBranch<long, TreeData>();
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

			bool isRef = false;
			int objId = 0;

			//读取类型Id
			if (this.ReadDynamic(out int typeId) < 0)
			{
				objId = ~typeId;
				//判断位置不一致，说明这里是引用地址，后续没有数据。
				if (IdToDataPointList[objId] != readPoint)
				{
					IdToObjectDict.TryGetValue(objId, out object obj);
					data = AddTreeData(node, out TreeDataRef treeRef, number, isArray);
					treeRef.Data = obj as TreeData;
					return data;
				}
				typeId = IdToTypeIdList[objId];
				isRef = true;
			}
			//拿到类型码
			this.TryGetTypeCode(typeId, out long typeCode);
			if (this.TryCodeGetType(typeCode, out Type type))
			{
				//获取真实类型码
				if (TreeDataTypeHelper.TypeCodeDict.ContainsKey(type)) typeCode = this.World.TypeToCode(type);
				//判断是否是基础类型
				if (TreeDataTypeHelper.CheckBasicsType(type))
				{
					data = AddTreeData(node, out TreeDataValue treeValue, number, isArray, isRef, objId);
					data.IsRef = isRef;
					data.TypeName = type.ToString();
					//基础类型取值
					if (this.World.Line.Core.RuleManager.TryGetRuleList<TreeDataDeserialize>(typeCode, out RuleList ruleList))
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
			if (count == TreeDataCode.NullObject)
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
				if (type?.GetElementType() != null && TreeDataTypeHelper.CheckUnmanagedType(type.GetElementType()))
				{
					//跳跃回类型开头
					this.ReadJump(startPoint);
					//动态支持多维数组
					if (type.IsArray) this.World.Line.Core.RuleManager.SupportGenericParameterNodeRule(type.GetElementType(), typeof(TreeDataDeserialize));
					//基础数组类型取值
					if (this.World.Line.Core.RuleManager.TryGetRuleList<TreeDataDeserialize>(typeCode, out RuleList ruleList) && ruleList.NodeType == typeCode)
					{
						int fieldNameCode = TreeDataCode.DeserializeSelfMode;
						object obj = null;
						((IRuleList<TreeDataDeserialize>)ruleList).SendRef(this, ref obj, ref fieldNameCode);
						if (isRef)
						{
							data.IsRef = true;
							ObjectToIdDict.Add(data, objId);
							IdToObjectDict[objId] = data;
						}

						Array arrayList = (obj as Array);
						int i = 0;
						foreach (var item in arrayList)
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
				data = AddTreeData(node, out TreeData _, number, isArray, isRef, objId);
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
		private T AddTreeData<T>(INode node, out T treeValue, int number, bool isArray, bool isRef = false, int id = 0)
			where T : TreeData
		{
			if (node is TreeData treeData)
			{
				if (isArray) treeData.AddListNode(number, out treeValue);
				else treeData.AddGeneric((long)number, out treeValue);
			}
			else
			{
				node.AddTemp(out treeValue);
			}
			if (isRef)
			{
				treeValue.IsRef = true;
				ObjectToIdDict.Add(treeValue, id);
				IdToObjectDict.Add(id, treeValue);
			}
			return treeValue;
		}
		#endregion

	}
}
