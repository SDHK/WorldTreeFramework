
using System;

namespace WorldTree.TreeCopys
{
	public static class Array3CopyRule
	{
		private class Copy<T> : TreeCopyRule<T[,,]>
		{
			protected override void Execute(TreeCopier self, ref object source, ref object target)
			{
				if (source == null)
				{
					target = null;
					return;
				}
				if (source is not T[,,] sourceArray) return;
				int dim1 = sourceArray.GetLength(0);
				int dim2 = sourceArray.GetLength(1);
				int dim3 = sourceArray.GetLength(2);
				if (target is not T[,,] targetArray)
				{
					if (target is IDisposable disposable) disposable.Dispose();
					target = targetArray = new T[dim1, dim2, dim3];
				}
				else if (targetArray.Length != sourceArray.Length)
				{
					foreach (var item in targetArray) if (item is IDisposable disposable) disposable.Dispose();
					target = targetArray = new T[dim1, dim2, dim3];
				}
				for (int i = 0; i < dim1; i++)
				{
					for (int j = 0; j < dim2; j++)
					{
						self.CopyTo(sourceArray[dim1, dim2, dim3], ref targetArray[dim1, dim2, dim3]);
					}
				}
			}
		}
	}
}