/****************************************

* 作者：闪电黑客
* 日期：2024/8/27 15:07

* 描述：

*/
using System;
using UnityEngine;

namespace WorldTree
{
	/// <summary>
	/// 测试事件法则
	/// </summary>
	public interface TestEvent : ICallRule<float, int, string>, IGlobalRule { }

	///// <summary>
	///// 测试
	///// </summary>
	//public partial class InitialTest<T> : Node
	//	, ComponentOf<INode>
	//	, AsComponentBranch
	//	, AsChildBranch
	//	, AsRule<Awake>
	//	, AsRule<GuiUpdateTime>
	//{

	//}

	/// <summary>
	/// 初始域
	/// </summary>
	public partial class InitialDomain : Node, ComponentOf<INode>
		, AsComponentBranch
		, AsChildBranch
		, AsRule<Awake>
		, AsRule<FixedUpdateTime>
		, AsRule<LateUpdateTime>
		, AsRule<GuiUpdateTime>
		, AsRule<IRule>
		, AsRule<CurveEvaluate>
		, AsRule<TestEvent>
	{
		/// <summary>
		/// 测试动画曲线
		/// </summary>
		public AnimationCurve AnimationCurve = new AnimationCurve();
		/// <summary>
		/// 测试浮点
		/// </summary>
		public float TestFloat = 1f;
		/// <summary>
		/// 测试双精度
		/// </summary>
		public double TestDouble = 1;
		/// <summary>
		/// 测试整数
		/// </summary>
		public int TestInt = 1;
		/// <summary>
		/// 测试长整数
		/// </summary>
		public long TestLong = 1;
		/// <summary>
		/// 测试布尔
		/// </summary>
		public bool TestBool = true;
		/// <summary>
		/// 测试字符串
		/// </summary>
		public string TestString = "1";
		/// <summary>
		/// 测试字符
		/// </summary>
		public char TestChar = '1';
		/// <summary>
		/// 测试边界
		/// </summary>
		public Bounds Bounds = new Bounds(Vector3.one, Vector3.one);
		/// <summary>
		/// 测试日期时间
		/// </summary>
		public DateTime TestDateTime = default;
		/// <summary>
		/// 测试矩阵
		/// </summary>
		public Rect Rect = new Rect(0, 0, 100, 100);
		/// <summary>
		/// 测试颜色
		/// </summary>
		public Color TestColor = Color.red;
		/// <summary>
		/// 测试颜色
		/// </summary>
		public Color TestColor1 => Color.red;

		/// <summary>
		/// 测试向量
		/// </summary>
		public Vector2 TestVector2 = Vector2.one;
		/// <summary>
		/// 测试向量
		/// </summary>
		public Vector3 TestVector3 = Vector3.one;
		/// <summary>
		/// 测试向量
		public Vector4 TestVector4 = Vector4.one;
		/// <summary>
		/// 测试
		/// </summary>
		public TreeList<int> ValueList;
		/// <summary>
		/// 测试
		/// </summary>
		public TreeDictionary<int, int> ValueDict;

		[NodeRule(nameof(AwakeRule<InitialDomain>))]
		private static void OnAwakeRule(InitialDomain self)
		{
			self.TestFloat = 1;

			self.Log($"初始域唤醒123！！!");
		}

		[NodeRule(nameof(AddRule<InitialDomain>))]
		private static void OnAddRule(InitialDomain self)
		{
			self.Log($"初始域热更部分！！!");

			//self.TypeToCode<InputDeviceManager>();
			//self.World.AddComponent(out InputDeviceManager _);
			//self.AddComponent(out InputMapperTest _);

			//添加宏测试成功，应该在编辑器下才能添加
			//string currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
			//if (!currentDefines.Contains("MY_NEW_DEFINE")) currentDefines += ";MY_NEW_DEFINE";
			//PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, currentDefines);
			//self.AddComponent(out CascadeTickerTest _);
			//self.AddComponent(out StepMachineTest _);
			self.AddComponent(out ViewTest _);
		}

		[NodeRule(nameof(UpdateRule<InitialDomain>))]
		private static void OnUpdateRule(InitialDomain self)
		{
			//self.Log($"测试Update！！!");

			if (Input.GetKeyDown(KeyCode.Q))
			{
				//self.Root.AddComponent(out CodeLoader _).HotReload();
				//self.AddComponent(out TreeDataTest _);
				//self.AddComponent(out ViewTest _);

				//NodeRuleHelper.CallRule(self, default(TestEvent), 1.5f, 1, out string str);

				////获取一个ICallRule通用委托unicast
				//self.AddTemp(out RuleUnicast<ICallRule<float, int, string>> unicast);

				////unicat是委托
				//unicast.Add(self, default(TestEvent));

				////调用委托
				//unicast.Call(2.5f, 2, out string str2);

				//全局广播调用
				//self.Core.GetRuleBroadcast(out RuleBroadcast<TestEvent> ruleBroadcast);
				//ruleBroadcast.Call(2.5f, 2, out string str2);
			}
		}

		[NodeRule(nameof(TestEventRule<InitialDomain>))]
		private static string OnTestEventRule(InitialDomain self, float arg1, int arg2)
		{
			return "返回字符串";
		}

		[NodeRule(nameof(RemoveRule<InitialDomain>))]
		private static void OnRemoveRule(InitialDomain self)
		{
			self.Log($"初始域关闭！！");
		}

		[NodeRule(nameof(EnableRule<InitialDomain>))]
		private static void OnEnableRule(InitialDomain self)
		{
			self.Log($"初始域激活！！");
		}
		[NodeRule(nameof(DisableRule<InitialDomain>))]
		private static void OnDisableRule(InitialDomain self)
		{

			self.Log($"初始域失活！！");
		}


		[NodeRule(nameof(GuiUpdateRule<InitialDomain>))]
		private static void OnGuiUpdateRule(InitialDomain self)
		{
			//self.Log($"测试OnGuiUpdateRule！！!");

			//GUILayout.Label($@"    {timeSpan.TotalMilliseconds} !  ", new GUIStyle() { fontSize = 60 });
		}

		[NodeRule(nameof(GuiUpdateTimeRule<InitialDomain>))]
		private static void OnGuiUpdateTimeRule(InitialDomain self, TimeSpan timeSpan)
		{
			GUILayout.Label($@"    {timeSpan.Ticks} !  ", new GUIStyle() { fontSize = 60 });
		}

	}
}