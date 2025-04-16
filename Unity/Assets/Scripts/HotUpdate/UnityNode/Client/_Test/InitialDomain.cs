/****************************************

* 作者：闪电黑客
* 日期：2024/8/27 15:07

* 描述：

*/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WorldTree
{
	/// <summary>
	/// 测试事件法则
	/// </summary>
	public interface TestEvent : ICallRule<float, int, string> { }

	/// <summary>
	/// 初始域
	/// </summary>
	public class InitialDomain : Node, ComponentOf<INode>
		, AsComponentBranch
		, AsChildBranch
		, AsAwake
		, AsFixedUpdateTime
		, AsUpdateTime
		, AsLateUpdateTime
		, AsGuiUpdateTime
		, AsRule<IRule>
		, AsCurveEvaluate
		, AsTestEvent
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
		/// </summary>
		public Vector4 TestVector4 = Vector4.one;
		/// <summary>
		/// 测试
		/// </summary>
		public TreeList<int> ValueList;
		/// <summary>
		/// 测试
		/// </summary>
		public TreeDictionary<int, int> ValueDict;
	}



	//泛型分支

	/// <summary>
	/// 测试数据
	/// </summary>
	public class StartWorldConfigGroup : ConfigGroup<StartWorldConfig>
		, AsTypeNodeBranch<string>
	{
		/// <summary>
		/// 数据
		/// </summary>
		public List<StartWorldConfig> ConfigList = new();
	}

	public static class StartWorldConfigGroupRule
	{
		class Add : AddRule<StartWorldConfigGroup>
		{
			protected override void Execute(StartWorldConfigGroup self)
			{
				self.AddTypeNode("", out StartWorldConfig cs, 1L);

				self.AddTypeNode(1L, out StartWorldConfig c, 1L);
				self.TryGetTypeNode(1L, out StartWorldConfig c1);
			}
		}
	}

	/// <summary>
	/// 测试数据
	/// </summary>
	public class StartWorldConfig : Config<long>
		, TypeNodeOf<string, StartWorldConfigGroup>
		, TypeNodeOf<int, StartWorldConfigGroup>
	{
		/// <summary>
		/// 测试整数
		/// </summary>
		public int TestInt = 1;

		/// <summary>
		/// 数据
		/// </summary>
		public List<string> stringList = new();
	}
}
