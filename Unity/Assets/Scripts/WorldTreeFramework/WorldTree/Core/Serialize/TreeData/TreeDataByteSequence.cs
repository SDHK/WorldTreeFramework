/****************************************

* 作者：闪电黑客
* 日期：2024/8/20 17:35

* 描述：

*/
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace WorldTree
{
	/// <summary>
	/// data
	/// </summary>
	public class AData
	{
		/// <summary>
		/// a
		/// </summary>
		public int AInt;
	}


	public static class KeyValuePairFormatterRule
	{
		class Serialize : TreeDataSerializeRule<TreeDataByteSequence, AData>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value)
			{
				//记录字段名称，类型名称，最后是数据
				//名称应该要转为数字码进行储存


				AData data = (AData)value;


				//============ Data <=> Byte <=> Object ======
				//a.GetType("AData<AData<int>,float>");

				//写入字段数量
				self.WriteUnmanaged(1);

				//object类型 
				//类型名称
				self.WriteUnmanaged(100); //假设数字是32位哈希码
				if (!self.ContainsTypeCode(100)) self.AddTypeCode(100, typeof(AData));

				//AData的字段名称1
				self.WriteUnmanaged(101);
				if (!self.ContainsNameCode(101)) self.AddNameCode(101, nameof(data.AInt));

				//value类型
				//类型名称
				self.WriteUnmanaged(1011);
				if (!self.ContainsTypeCode(1011)) self.AddTypeCode(1011, typeof(int));

				//字段值
				self.WriteUnmanaged(data.AInt);
			}
		}


		class Deserialize : TreeDataDeserializeRule<TreeDataByteSequence, AData>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value)
			{
				//假设数字是32位哈希码
				Type type = null;

				//读取类型码
				//self.ReadUnmanaged(out int typeCode); 

				//通过类型码获取类型
				//self.GetType(typeCode,out  type)

				//是本身类型
				if (typeof(AData) == type)
				{
					//正常读取流程
				}
				//不是本身类型，判断是否是子类型
				else if (type.BaseType == typeof(AData))
				{
					//读取指针回退，类型码
					self.ReadBack(Unsafe.SizeOf<int>());
					//子类型读取
					self.ReadValue(type, ref value);
				}
				else if (self != null)//是否为可转换类型
				{
					//读取指针回退，类型码
					self.ReadBack(Unsafe.SizeOf<int>());
					//可转换类型读取,例如int转long
					self.ReadValue(type, ref value);
				}
				else
				{
					//不是本身类型，也不是子类型，也不是可转换类型，跳跃数据，应该能写成一个通用方法

				}





				AData data = (AData)value;

				////object类型 
				////类型名称
				//self.ReadUnmanaged(out int typeCode); //假设数字是32位哈希码




				//AData的字段名称1
				self.ReadUnmanaged(out int FName);
				if (!self.ContainsNameCode(101)) self.AddNameCode(101, nameof(data.AInt));

				//value类型
				//类型名称
				self.WriteUnmanaged(1011);
				if (!self.ContainsTypeCode(1011)) self.AddTypeCode(1011, typeof(int));

				//字段值
				self.WriteUnmanaged(data.AInt);

				switch (FName)
				{
					case 101:
						self.ReadValue(ref data.AInt);
						break;
					default:
						break;
				}
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
			}
		}
	}

	/// <summary>
	/// 树数据类型
	/// </summary>
	public static class TreeDataType
	{
		/// <summary>
		/// 基础值类型
		/// </summary>
		public static HashSet<Type> TypeHash = new()
		{
			typeof(int)

		};

	}


	/// <summary>
	/// 树数据字节序列
	/// </summary>
	public class TreeDataByteSequence : ByteSequence
		, AsRule<ITreeDataSerialize>
		, AsRule<ITreeDataDeserialize>
	{
		/// <summary>
		/// 类型码对应名称字典，32哈希码对应
		/// </summary>
		public UnitDictionary<int, Type> CodeToTypeDict;

		/// <summary>
		/// 泛型类型码
		/// </summary>
		public HashSet<int> GenericsTypeCodeHash;

		/// <summary>
		/// 字段码对应名称字典，32哈希码对应，代码生成直接使用int对比
		/// </summary>
		public UnitDictionary<int, string> codeToNameDict;


		/// <summary>
		/// 类型码判断
		/// </summary>
		public bool ContainsTypeCode(int typeCode)
			=> CodeToTypeDict.ContainsKey(typeCode);

		/// <summary>
		/// 名称码判断
		/// </summary>
		public bool ContainsNameCode(int nameCode)
			=> codeToNameDict.ContainsKey(nameCode);

		/// <summary>
		/// 添加类型码
		/// </summary>
		public void AddTypeCode(int typeCode, Type type)
		{
			CodeToTypeDict.Add(typeCode, type);
		}

		/// <summary>
		/// 添加名称码
		/// </summary>
		public void AddNameCode(int nameCode, string name)
		{
			codeToNameDict.Add(nameCode, name);
		}




		/// <summary>
		/// 写入值
		/// </summary>
		public void WriteValue<T>(in T value)
		{
			WriteValue(typeof(T), ref Unsafe.AsRef<object>(value));
		}

		/// <summary>
		/// 写入值
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValue(Type type, ref object value)
		{
			long typeCode = this.Core.TypeToCode(type);
			this.Core.RuleManager.SupportNodeRule(typeCode);
			if (this.Core.RuleManager.TryGetRuleList<ITreeDataSerialize>(typeCode, out RuleList ruleList))
			{
				((IRuleList<TreeDataSerialize>)ruleList).SendRef(this, ref value);
			}
		}


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
		/// 读取值
		/// </summary>
		public void ReadValue(Type type, ref object value)
		{
			long typeCode = this.Core.TypeToCode(type);
			this.Core.RuleManager.SupportNodeRule(typeCode);
			if (this.Core.RuleManager.TryGetRuleList<ITreeDataSerialize>(typeCode, out RuleList ruleList))
			{
				((IRuleList<TreeDataDeserialize>)ruleList).SendRef(this, ref value);
			}
		}
	}
}
