﻿/****************************************

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
	public interface TestEvent : ICallRule<float, int, string> { }

	/// <summary>
	/// 初始域
	/// </summary>
	public class InitialDomain : Node, ComponentOf<INode>
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
}