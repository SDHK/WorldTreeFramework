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
		
			self.AddComponent(out YooAssetTest _);


			//添加宏测试成功，应该在编辑器下才能添加
			//string currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
			//if (!currentDefines.Contains("MY_NEW_DEFINE")) currentDefines += ";MY_NEW_DEFINE";
			//PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, currentDefines);
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
