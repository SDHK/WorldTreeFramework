﻿/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using System;

namespace WorldTree
{
	public static partial class TreeTweenRule
	{
		class StringTweenUpdateRule : TweenUpdateRule<TreeTween<string>>
		{
			protected override void Execute(TreeTween<string> self, TimeSpan deltaTime)
			{
				if (self.isRun)
				{
					if (self.time.TotalSeconds < self.clock)
					{
						//选取最长的值
						var longValue = self.startValue.Value.Length > self.endValue.Value.Length ? self.startValue.Value : self.endValue.Value;
						var shortValue = self.startValue.Value.Length > self.endValue.Value.Length ? self.endValue.Value : self.startValue.Value;

						//判断长包含短，否则清空开始值
						if (self.time == TimeSpan.Zero && self.startValue.Value.Length != 0 && longValue.Length != 0 && !longValue.StartsWith(shortValue))
						{
							self.startValue.Value = string.Empty;
						}

						//通过曲线获取长度变化
						int nowLength = (int)((self.endValue.Value.Length - self.startValue.Value.Length) * self.GetCurveEvaluate(deltaTime) + self.startValue.Value.Length);

						//当前长度不等于变化的长度时，进行拷贝赋值
						if (self.changeValue.Value.Length != nowLength)
						{
							self.changeValue.Value = longValue[..nowLength];
							self.Log(self.changeValue.Value);
						}
						//当前长度 等于 结果长度，但是对象不同，直接进行引用赋值
						else if (self.changeValue.Value.Length == self.endValue.Value.Length && self.endValue.Value != self.changeValue.Value)
						{
							self.changeValue.Value = self.endValue.Value;
							self.Log(self.changeValue.Value);
						}
						//当前长度 等于 当前裁剪长度时，但内容不同，进行拷贝赋值
						else if (!longValue.StartsWith(shortValue))
						{
							//可省去节省时间
							self.changeValue.Value = longValue[..nowLength];
							self.Log(self.changeValue.Value);
						}
					}
					else
					{
						self.isRun = false;
						self.OnCompleted.Send();
					}
				}
			}
		}
	}
}
