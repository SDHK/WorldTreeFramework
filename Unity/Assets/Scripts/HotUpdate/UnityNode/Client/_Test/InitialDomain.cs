/****************************************

* 作者：闪电黑客
* 日期：2022/8/26 0:23

* 描述：初始域组件
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
	/// 测试节点
	/// </summary>
	public class TestNode : Node, ChildOf<InitialDomain>
		, AsAwake
	{
		/// <summary>
		/// 测试数据
		/// </summary>
		public Color color = Color.red;
	}

	/// <summary>
	/// 初始域
	/// </summary>
	public class InitialDomain : Node, ComponentOf<INode>
		, AsComponentBranch
		, AsChildBranch
		, AsAwake
		, AsFixedUpdateTime
		, AsLateUpdateTime
		, AsGuiUpdateTime
		, AsRule<IRule>
		, AsCurveEvaluate
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
	}
}