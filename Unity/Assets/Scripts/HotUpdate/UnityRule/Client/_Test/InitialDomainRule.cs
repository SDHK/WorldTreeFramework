/****************************************

* 作者：闪电黑客
* 日期：2024/8/21 19:56

* 描述：

*/
using UnityEngine;

namespace WorldTree
{


	public static partial class InitialDomainRule
	{
		static OnAwake<InitialDomain> OnAwake = (self) =>
		{
			self.TestFloat = 1;

			self.Log($"初始域唤醒！！!");
		};

		static OnAdd<InitialDomain> OnAdd = (self) =>
		{
			self.Log($"初始域热更部分！！!");
			self.TypeToCode<InputDeviceManager>();
			self.World.AddComponent(out InputDeviceManager _);
			//self.AddComponent(out InputMapperTest _);

			//添加宏测试成功，应该在编辑器下才能添加
			//string currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
			//if (!currentDefines.Contains("MY_NEW_DEFINE")) currentDefines += ";MY_NEW_DEFINE";
			//PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, currentDefines);
		};

		static OnUpdate<InitialDomain> OnUpdate = (self) =>
		{
			if (Input.GetKeyDown(KeyCode.Q))
			{
				//self.Root.AddComponent(out CodeLoader _).HotReload();
				//self.AddComponent(out TreeDataTest _);

				//NodeRuleHelper.CallRule(self, default(TestEvent), 1.5f, 1, out string str);

			}

		};

		class TestEvent1 : TestEventRule<InitialDomain>
		{
			protected override string Execute(InitialDomain self, float arg1, int arg2)
			{
				return "返回字符串";
			}
		}

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

		static OnGuiUpdate<InitialDomain> OnGuiUpdate = (self) =>
		{

		};

	}
}