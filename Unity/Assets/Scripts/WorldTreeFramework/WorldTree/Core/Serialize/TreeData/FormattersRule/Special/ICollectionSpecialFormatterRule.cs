using System.Collections.Generic;

namespace WorldTree.TreeDataFormatters
{
	/// <summary>
	/// 收集类型特殊格式化器
	/// </summary>
	public static class ICollectionSpecialFormatterRule
	{
		/// <summary>
		/// 收集类型特殊序列化法则基类
		/// </summary>
		public abstract class SerializeBase<T, ItemT> : IEnumerableSpecialFormatterRule.SerializeBase<T, ItemT>
			where T : class, ICollection<ItemT>, new()
		{
			public override void ForeachWrite(TreeDataByteSequence self, T obj)
			{
				self.WriteUnmanaged(obj.Count);
				if (obj.Count == 0) return;
				//写入数组数据
				foreach (var item in obj) self.WriteValue(item);
			}
		}

		/// <summary>
		/// 收集类型特殊反序列化法则基类
		/// </summary>
		public abstract class DeserializeBase<T, ItemT> : IEnumerableSpecialFormatterRule.DeserializeBase<T, ItemT>
			where T : class, ICollection<ItemT>, new()
		{
			public override void ForeachRead(TreeDataByteSequence self, T obj)
			{
				//假如有数据，则清空数据
				if (obj.Count != 0) obj.Clear();
				self.ReadUnmanaged(out int length);
				//数据长度为0，直接返回
				if (length == 0) return;
				//读取数组数据
				for (int i = 0; i < length; i++) obj.Add(self.ReadValue<ItemT>());
			}
		}
	}

}