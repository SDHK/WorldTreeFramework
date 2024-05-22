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
	public interface ISendData
	{
		public long nodeId { get; set; }

		public void Send(INode node);
	}

	/// <summary>
	/// 可序列化的发送数据
	/// </summary>
	/// <typeparam name="R">事件类型</typeparam>
	public class SendData<R, T1, T2, T3> : Node
		where R : ISendRule<T1, T2, T3>
	{
		/// <summary>
		/// 目标类型id
		/// </summary>
		public long nodeId { get; set; }
		
		private T1 arg1;
		private T2 arg2;
		private T3 arg3;

		/// <summary>
		/// 调用将参数塞入目标节点
		/// </summary>
		public void Send(INode node)
		{
			//node.TrySend(TypeInfo<R>.Default, arg1, arg2, arg3);
		}

		public void SetData(byte[] dataBytes)
		{
		}
	}

	/// <summary>
	/// 初始域
	/// </summary>
	public class InitialDomain : Node, ComponentOf<INode>
		, AsAwake
		, AsFixedUpdateTime
		, AsLateUpdateTime
		, AsGuiUpdateTime
	{
		public AnimationCurve AnimationCurve = new AnimationCurve();
		public float TestFloat = 1f;
		public double TestDouble = 1;
		public int TestInt = 1;
		public long TestLong = 1;
		public bool TestBool = true;
		public string TestString = "1";
		public char TestChar = '1';
		public Bounds Bounds = new Bounds(Vector3.one, Vector3.one);
		public DateTime TestDateTime = DateTime.Now;
		public Rect Rect = new Rect(0, 0, 100, 100);
		public Color TestColor = Color.red;
		public Vector2 TestVector2 = Vector2.one;
		public Vector3 TestVector3 = Vector3.one;
		public Vector4 TestVector4 = Vector4.one;

		public TreeList<int> values;
	}

	public static class InitialDomainRule_
	{
		//测试框架功能
		private class AddRule : AddRule<InitialDomain>
		{
			protected override void Execute(InitialDomain self)
			{
				self.Log($"初始域启动！");
			}
		}
	}
}