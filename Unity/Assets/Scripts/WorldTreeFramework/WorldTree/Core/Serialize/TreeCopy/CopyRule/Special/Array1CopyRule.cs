
using System;

namespace WorldTree.TreeCopys
{
	public static class Array1CopyRule
	{
		private class Copy<T> : TreeCopyRule<T[]>
		{
			protected override void Execute(TreeCopier self, ref object source, ref object target)
			{
				if (source == null)
				{
					target = null;
					return;
				}
				if (source is not T[] sourceArray) return;
				if (target is not T[] targetArray)
				{
					if (target is IDisposable disposable) disposable.Dispose();
					target = targetArray = new T[sourceArray.Length];
				}
				else if (targetArray.Length != sourceArray.Length)
				{
					foreach (var item in targetArray) if (item is IDisposable disposable) disposable.Dispose();
					target = targetArray = new T[sourceArray.Length];
				}
				for (int i = 0; i < sourceArray.Length; i++)
				{
					self.CopyTo(sourceArray[i], ref targetArray[i]);
				}
			}
		}
	}
}