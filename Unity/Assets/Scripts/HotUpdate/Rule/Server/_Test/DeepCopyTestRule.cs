

using System;

namespace WorldTree.Server
{

	public static partial class DeepCopyTestRule
	{
		public class TreeDeepCopyRule1 : TreeCopyRule<CopyTest1>
		{
			protected override void Execute(TreeCopyExecutor self, ref object source, ref object target)
			{
				if (source == null)
				{
					target = null;
					return;
				}

				//类型对不上
				if (source is not CopyTest1 sourceValue) return;

				if (target is not CopyTest1 targetValue)
				{
					//目标类型不对，尝试释放，并重新创建
					if (target is IDisposable disposable) disposable.Dispose();
					target = targetValue = new CopyTest1();
				}

				//值类型直接赋值
				targetValue.Value1 = sourceValue.Value1;
				targetValue.Value11 = sourceValue.Value11;

				self.CopyTo(sourceValue.Value1, ref targetValue.Value1);
				self.CopyTo(sourceValue.Value2, ref targetValue.Value2);

				//引用类型属性

				//self.CopyTo(sourceValue.Value11, ref targetValue.Value11);
			}
		}
	}
}