using UnityEngine;

namespace WorldTree
{


	public static partial class InitialDomainRule
	{
		static OnAdd<InitialDomain> OnAdd = (self) =>
		{
			self.Log($"初始域热更部分！！!");

			//添加宏测试成功，应该在编辑器下才能添加
			//string currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
			//if (!currentDefines.Contains("MY_NEW_DEFINE")) currentDefines += ";MY_NEW_DEFINE";
			//PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, currentDefines);
		};

		static OnUpdate<InitialDomain> OnUpdate = (self) =>
		{
			//self.Log($"初始域更新！！!");

			if (Input.GetKeyDown(KeyCode.Q))
			{

				//self.Root.AddComponent(out CodeLoader _).HotReload();
				self.AddComponent(out TreeDataTest _);

				//for (int i = 0; i < 100000; i++)
				//{
				//	self.AddChild(out TestNode _);
				//}
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