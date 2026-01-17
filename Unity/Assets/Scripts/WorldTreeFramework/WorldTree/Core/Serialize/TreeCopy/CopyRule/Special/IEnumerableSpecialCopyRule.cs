

using System.Collections.Generic;

namespace WorldTree.TreeCopys
{
	public static class IEnumerableSpecialCopyRule
	{
		/// <summary>
		/// 迭代器类型特殊拷贝法则基类
		/// </summary>
		public abstract class CopyRuleBase<T, ItemT> : TreeCopyRule<T>
			where T : class, IEnumerable<ItemT>, new()
		{
			/// <summary>
			/// 遍历拷贝方法
			/// </summary>
			public abstract void ForeachCopy(TreeCopier self, T source, T target);

			protected override void Execute(TreeCopier self, ref object sourceObj, ref object targetObj)
			{
				if (sourceObj == null)
				{
					targetObj = null;
					return;
				}
				if (sourceObj is not T source) return;
				if (targetObj is not T target)
				{
					if (targetObj is System.IDisposable disposable) disposable.Dispose();
					targetObj = target = new T();
				}
				ForeachCopy(self, source, target);
			}
		}
	}
}