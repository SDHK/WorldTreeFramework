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
		[NodeRule(nameof(AwakeRule<InitialDomain>))]
		private static void OnAwake(this InitialDomain self)
		{
			self.TestFloat = 1;

			self.Log($"初始域唤醒！！!");
		}

		[NodeRule(nameof(AddRule<InitialDomain>))]
		private static void OnAdd(this InitialDomain self)
		{
			self.Log($"初始域热更部分！！!");
			self.TypeToCode<InputDeviceManager>();
			self.World.AddComponent(out InputDeviceManager _);
			self.AddComponent(out InputMapperTest _);

			//添加宏测试成功，应该在编辑器下才能添加
			//string currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
			//if (!currentDefines.Contains("MY_NEW_DEFINE")) currentDefines += ";MY_NEW_DEFINE";
			//PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, currentDefines);
		}

		[NodeRule(nameof(UpdateRule<InitialDomain>))]
		private static void OnUpdate(this InitialDomain self)
		{
			if (Input.GetKeyDown(KeyCode.Q))
			{
				//self.Root.AddComponent(out CodeLoader _).HotReload();
				//self.AddComponent(out TreeDataTest _);

				//NodeRuleHelper.CallRule(self, default(TestEvent), 1.5f, 1, out string str);

			}
		}

		class TestEvent1 : TestEventRule<InitialDomain>
		{
			protected override string Execute(InitialDomain self, float arg1, int arg2)
			{
				return "返回字符串";
			}
		}

		[NodeRule(nameof(EnableRule<InitialDomain>))]
		private static void OnEnable(this InitialDomain self)
		{
			self.Log($"初始域激活！！");
		}
		[NodeRule(nameof(DisableRule<InitialDomain>))]
		private static void OnDisable(this InitialDomain self)
		{

			self.Log($"初始域失活！！");
		}

		[NodeRule(nameof(GuiUpdateRule<InitialDomain>))]
		private static void OnGuiUpdate(this InitialDomain self)
		{
			//GUILayout.Label($@"    {timeSpan.TotalMilliseconds} !  ", new GUIStyle() { fontSize = 60 });
		}


		[NodeRule(nameof(RemoveRule<InitialDomain>))]
		private static void OnRemove(this InitialDomain self)
		{
			self.Log($"初始域关闭！！");
		}
	}
}