using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WorldTree
{
	public class TreeValueTest : Node, ComponentOf<InitialDomain>
		, AsRule<IAwakeRule>
	{
		public TreeValue<float> valueFloat;
		public TreeValue<int> valueInt;

		public TreeValue<string> valueString;
		public TreeTween<string> treeTween;
	}

	public static class TreeValueTestRule
	{
		class AddRule : AddRule<TreeValueTest>
		{
			protected override void OnEvent(TreeValueTest self)
			{
				self.AddChild(out self.valueFloat);
				self.AddChild(out self.valueInt);
				self.AddChild(out self.valueString, "Hello world! 你好世界！");
				self.valueFloat.Bind(self.valueInt);

				//数值组件Float 单向绑定 数值组件String ,当float值发生变化时，会通知 valueString 并转为字符串
				self.valueFloat.Bind(self.valueString);
			}
		}

		class UpdateRule : UpdateRule<TreeValueTest>
		{
			protected override void OnEvent(TreeValueTest self)
			{
				if (Input.GetKeyDown(KeyCode.A))
				{
					//对 字符串数值组件 启动一个渐变动画，从 "Hello world! 你好世界！" 变成 "Hello" 动画时间为10秒
					self.valueString.GetTween("Hello", 10f).Run();
				}
				if (Input.GetKeyDown(KeyCode.S))
				{
					self.valueString.GetTween("Hello world! 你好世界！", 10f).Run();
				}

				if (Input.GetKeyDown(KeyCode.D))
				{
					//对 Float数值组件 启动一个渐变动画，变成 3f 动画时间为 3 秒
					self.valueFloat.GetTween(3f, 3f).Run();
				}
				if (Input.GetKeyDown(KeyCode.F))
				{
					//对 Float数值组件 启动一个渐变动画，变成 10f 动画时间为 3 秒
					self.valueFloat.GetTween(10f, 3f).Run();
				}
			}
		}
	}
}
