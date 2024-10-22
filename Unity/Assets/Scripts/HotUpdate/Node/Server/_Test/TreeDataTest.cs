
using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// data
	/// </summary>
	[TreeDataSerializable]
	public partial class ADataBase
	{
		/// <summary>
		/// 测试int
		/// </summary>
		public float AInt = 10.1f;
	}

	/// <summary>
	/// data
	/// </summary>
	[TreeDataSerializable]
	public partial class AData : ADataBase
	{
		/// <summary>
		/// 测试int数组
		/// </summary>
		public int[][,,] Ints;

		public AData()
		{

		}
	}

	//public partial struct AData
	//{
	//	class TreeDataSerialize : TreeDataSerializeRule<TreeDataByteSequence, AData>
	//	{
	//		protected override void Execute(TreeDataByteSequence self, ref object value)
	//		{
	//			AData data = (AData)value;
	//			self.WriteType(typeof(AData));
	//			self.WriteUnmanaged(~2);
	//			if (!self.WriteCheckNameCode(-571700491)) self.AddNameCode(-571700491, nameof(data.AInt));
	//			self.WriteValue(data.AInt);
	//			if (!self.WriteCheckNameCode(-1413616393)) self.AddNameCode(-1413616393, nameof(data.Ints));
	//			self.WriteValue(data.Ints);
	//		}
	//	}
	//class TreeDataDeserialize : TreeDataDeserializeRule<TreeDataByteSequence, AData>
	//{
	//	protected override void Execute(TreeDataByteSequence self, ref object value)
	//	{
	//		self.TryReadType(out Type type);
	//		self.ReadUnmanaged(out int count);
	//		if (count < 0)
	//		{
	//			count = ~count;
	//		}
	//		else
	//		{
	//			self.ReadBack(4);
	//			self.SkipData(type);
	//			return;
	//		}
	//		if (typeof(AData) == type)
	//		{
	//			if (!(value is AData obj)) obj = new AData();
	//			for (int i = 0; i < count; i++)
	//			{
	//				self.ReadUnmanaged(out int nameCode);
	//				switch (nameCode)
	//				{
	//					case -571700491: obj.AInt = self.ReadValue(obj.AInt); break;
	//					case -1413616393: self.ReadValue(ref obj.Ints); break;
	//					default: self.SkipData(); break;
	//				}
	//			}
	//			value = obj;
	//		}
	//	}
	//}
	//}




	//以下代码需要由工具生成
	//public partial class AData
	//{
	//	public static class KeyValuePairFormatterRule
	//	{
	//		class Serialize : TreeDataSerializeRule<TreeDataByteSequence, AData>
	//		{
	//			protected override void Execute(TreeDataByteSequence self, ref object value)
	//			{
	//				//============ Data <=> Byte <=> Object ======


	//				AData data = (AData)value;
	//				//类型名称
	//				self.WriteType(typeof(AData));
	//				if (data == null)//空对象
	//				{
	//					self.WriteUnmanaged((long)ValueMarkCode.NULL_OBJECT);
	//					return;
	//				}

	//				//写入字段数量
	//				self.WriteUnmanaged(~2);

	//				//AData的字段名称1
	//				if (!self.WriteCheckNameCode(101)) self.AddNameCode(101, nameof(data.AInt));

	//				//value类型
	//				//类型名称
	//				//self.WriteType(data.AInt.GetType());
	//				//字段值
	//				//self.WriteUnmanaged(data.AInt);
	//				self.WriteValue(data.AInt);


	//				//AData的字段名称1
	//				if (!self.WriteCheckNameCode(102)) self.AddNameCode(102, nameof(data.Ints));

	//				self.WriteValue(data.Ints);
	//			}
	//		}

	//		class Deserialize : TreeDataDeserializeRule<TreeDataByteSequence, AData>
	//		{
	//			protected override void Execute(TreeDataByteSequence self, ref object value)
	//			{
	//				//通过类型码获取类型
	//				self.TryReadType(out Type type);
	//				//读取字段数量
	//				self.ReadUnmanaged(out int count);
	//				//空对象判断
	//				if (count == ValueMarkCode.NULL_OBJECT)
	//				{
	//					value = null;
	//					return;
	//				}
	//				//判断是否为负数，负数为数组
	//				if (count < 0)
	//				{
	//					count = ~count;
	//				}
	//				else
	//				{
	//					self.ReadBack(4);
	//					self.SkipData(type);
	//					return;
	//				}
	//				//是本身类型，正常读取流程
	//				if (typeof(AData) == type)
	//				{
	//					//类型新建和转换
	//					if (!(value is AData obj))
	//					{
	//						obj = new AData();
	//						value = obj;
	//					}
	//					//读取字段
	//					for (int i = 0; i < count; i++)
	//					{
	//						//读取字段名称
	//						self.ReadUnmanaged(out int nameCode);
	//						switch (nameCode)
	//						{
	//							case 101: self.ReadValue(ref obj.AInt); break;

	//							case 102: self.ReadValue(ref obj.Ints); break;

	//							default://不存在该字段，跳跃数据
	//								self.SkipData();
	//								break;
	//						}
	//					}
	//				}
	//				else
	//				{
	//					self.SubTypeReadValue(type, typeof(AData), ref value);
	//				}
	//			}
	//		}
	//	}

	//}





	/// <summary>
	/// 序列化测试
	/// </summary>
	public class TreeDataTest : Node
			, ComponentOf<INode>
			, AsAwake
	{ }
}

