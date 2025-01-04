/****************************************

* 作者：闪电黑客
* 日期：2024/10/31 20:53

* 描述：

*/
using System.Collections;
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
				self.WriteDynamic(obj.Count);
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
				if (obj.Count != 0) obj.Clear();
				self.ReadDynamic(out int length);
				for (int i = 0; i < length; i++) obj.Add(self.ReadValue<ItemT>());
			}
		}
	}

}