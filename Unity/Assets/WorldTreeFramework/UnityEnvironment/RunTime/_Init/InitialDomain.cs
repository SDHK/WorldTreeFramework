/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/26 0:23

* 描述： 初始域组件
* 
* 在 世界树 启动后挂载
* 
* 可用于初始化启动需要的功能组件
* 
* 然而框架还没完成，目前用于功能测试

*/

using System;
using UnityEngine;

namespace WorldTree
{
	/// <summary>
	/// 初始域
	/// </summary>
	public class InitialDomain : DynamicNodeListener, ComponentOf<INode>
		, AsRule<IAwakeRule>
		, AsRule<IFixedUpdateTimeRule>
		, AsRule<ILateUpdateTimeRule>
		, AsRule<IGuiUpdateTimeRule>
	{ }

	public static class InitialDomainRule
	{

		//测试框架功能
		class AddRule : AddRule<InitialDomain>
		{
			protected override void OnEvent(InitialDomain self)
			{
				self.Log($"初始域启动！！");
				self.Core.AddWorld(out WorldTreeCore core);//添加子世界

			}
		}

		class UpdateTimeRule : UpdateTimeRule<InitialDomain>
		{
			protected override void OnEvent(InitialDomain self, TimeSpan timeSpan)
			{
				//self.Log($"初始域更新！！{timeSpan.TotalSeconds}");
			}
		}

		class GuiUpdateTimeRule : GuiUpdateTimeRule<InitialDomain>
		{
			GUIStyle textStyle = new GUIStyle() { fontSize = 60 };

			protected override void OnEvent(InitialDomain self, TimeSpan timeSpan)
			{
				textStyle.normal.textColor = Color.red;
				GUILayout.Label($"初始域GUI更新！！{timeSpan.TotalMilliseconds}", textStyle);
			}
		}

		class RemoveRule : RemoveRule<InitialDomain>
		{
			protected override void OnEvent(InitialDomain self)
			{
				self.Log($"初始域关闭！！");
			}
		}
	}
}
