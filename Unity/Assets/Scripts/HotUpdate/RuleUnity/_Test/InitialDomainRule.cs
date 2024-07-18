using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.TextCore.Text;
using WorldTree.Internal;

namespace WorldTree
{
	public static partial class InitialDomainRule
	{
		static OnAdd<InitialDomain> OnAdd = (self) =>
		{
			self.Log($"初始域热更部分！！!");
			//self.Core.AddWorld(out WorldTreeCore core);//添加子世界
			//self.AddComponent(out TaskTest _);



			DateTime dateTime = self.Core.RealTimeManager.UtcNow;

			self.Log($"网络时间：{dateTime}");
			self.Log($"本地机器Utc时间：{DateTime.UtcNow}");
			self.Log($"本地机器时间：{DateTime.Now}");

			//添加宏测试成功，应该在编辑器下才能添加
			//string currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
			//if (!currentDefines.Contains("MY_NEW_DEFINE")) currentDefines += ";MY_NEW_DEFINE";
			//PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, currentDefines);
		};

		static OnUpdate<InitialDomain> OnUpdate = (self) =>
		{
			//self.Log($"初始域更新！！");

			if (Input.GetKeyDown(KeyCode.Q))
			{
				self.AddComponent(out SerializeTest _);
			}
		};

		static OnEnable<InitialDomain> OnEnable = (self) =>
		{
			self.Log($"初始域激活！！");
		};

		static OnDisable<InitialDomain> OnDisable = (self) =>
		{
			self.Log($"初始域失活！！");
		};


		static OnGuiUpdateTime<InitialDomain> OnGui = (self, timeSpan) =>
		{
			//GUILayout.Label($@"    {timeSpan.TotalMilliseconds} !  ", new GUIStyle() { fontSize = 60 });
		};

		static OnRemove<InitialDomain> OnRemove = (self) =>
		{
			self.Log($"初始域关闭！！");
		};
	}
}