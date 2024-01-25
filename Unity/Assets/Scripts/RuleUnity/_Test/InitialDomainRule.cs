using System;
using UnityEngine;
using WorldTree;

public static partial class InitialDomainRule
{
	//测试框架功能
	class AddRule : AddRule<InitialDomain>
	{
		protected override void OnEvent(InitialDomain self)
		{
			self.Log($"初始域启动！！!");
			//self.Core.AddWorld(out WorldTreeCore core);//添加子世界

			TreeTask.ExceptionHandler = (e) => self.LogError(e.ToString());
			self.AddComponent(out YooAssetTest _);

		}
	}

	class EnableRule : EnableRule<InitialDomain>
	{
		protected override void OnEvent(InitialDomain self)
		{
			self.Log($"初始域激活！！");
		}
	}

	class DisableRule : DisableRule<InitialDomain>
	{
		protected override void OnEvent(InitialDomain self)
		{
			self.Log($"初始域禁用！！");
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
