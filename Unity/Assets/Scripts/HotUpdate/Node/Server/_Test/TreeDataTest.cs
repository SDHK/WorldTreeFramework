
using System.Runtime.CompilerServices;
using System;

namespace WorldTree
{
	/// <summary>
	/// data
	/// </summary>
	public partial class AData
	{
		/// <summary>
		/// 测试int
		/// </summary>
		public int AInt = 10;
	}

	//以下代码需要由工具生成
	public partial class AData
	{
		public static class KeyValuePairFormatterRule
		{
			class Serialize : TreeDataSerializeRule<TreeDataByteSequence, AData>
			{
				protected override void Execute(TreeDataByteSequence self, ref object value)
				{
					//============ Data <=> Byte <=> Object ======

					AData data = (AData)value;
					//类型名称
					self.WriteUnmanaged(self.AddTypeCode(typeof(AData)));

					//写入字段数量
					self.WriteUnmanaged(1);

					//AData的字段名称1
					self.WriteUnmanaged(101);
					if (!self.ContainsNameCode(101)) self.AddNameCode(101, nameof(data.AInt));

					//value类型
					//类型名称
					self.WriteUnmanaged(self.AddTypeCode(data.AInt.GetType()));

					//字段值
					self.WriteUnmanaged(data.AInt);
				}
			}

			class Deserialize : TreeDataDeserializeRule<TreeDataByteSequence, AData>
			{
				protected override void Execute(TreeDataByteSequence self, ref object value)
				{
					//读取类型码
					self.ReadUnmanaged(out long typeCode);

					//通过类型码获取类型
					self.TryGetType(typeCode, out Type type);

					//是本身类型，正常读取流程
					if (typeof(AData) == type)
					{
						//类型新建和转换
						if (!(value is AData obj))
						{
							obj = new AData();
							value = obj;
						}

						//读取字段数量
						self.ReadUnmanaged(out int count);

						//读取字段
						for (int i = 0; i < count; i++)
						{
							//读取字段名称
							self.ReadUnmanaged(out int nameCode);
							switch (nameCode)
							{
								case 101: self.ReadValue(ref obj.AInt); break;

								default:
									SkipData(self);//跳跃数据
									break;
							}
						}
					}
					//不是本身类型，判断是否是子类型 
					else
					{
						bool isSubType = false;
						Type baseType = type.BaseType;
						if (typeof(AData).IsInterface)
						{
							while (baseType != null && baseType != typeof(object))
							{
								if (baseType == typeof(AData))
								{
									isSubType = true;
									break;
								}
								baseType = type.BaseType;
							}
						}
						else //接口
						{
							Type[] interfaces = type.GetInterfaces();
							foreach (var interfaceType in interfaces)
							{
								if (interfaceType == typeof(AData))
								{
									isSubType = true;
									break;
								}
							}
						}
						if (isSubType)//是子类型
						{
							//读取指针回退，类型码
							self.ReadBack(Unsafe.SizeOf<int>());
							//子类型读取
							self.ReadValue(type, ref value);
						}
						else
						{
							//不是本身类型，也不是子类型，也不是可转换类型，跳跃数据。

							//读取指针回退，类型码
							self.ReadBack(4);
							//跳跃数据
							SkipData(self);
						}
					}

					//跳跃数据
					void SkipData(TreeDataByteSequence self)
					{
						//读取类型码
						self.ReadUnmanaged(out long typeCode);

						//通过类型码获取类型
						if (self.TryGetType(typeCode, out type))
						{
							//判断是否是基础类型，直接跳跃
							if (TreeDataType.TypeDict.TryGetValue(type, out int byteCount))
							{
								self.ReadSkip(byteCount);
								return;
							}
						}

						//读取字段数量
						self.ReadUnmanaged(out int count);

						//读取字段
						for (int i = 0; i < count; i++)
						{
							//读取字段名称
							self.ReadUnmanaged(out int nameCode);
							SkipData(self);
						}
					}
				}
			}
		}

	}



	/// <summary>
	/// 序列化测试
	/// </summary>
	public class TreeDataTest : Node
		, ComponentOf<INode>
		, AsAwake
	{ }
}

