using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldTree
{
	/// <summary>
	/// 序列化测试
	/// </summary>
	public class SerializeTest : Node
		,ComponentOf<INode>
		,AsAwake
	{
		/// <summary>
		/// 测试浮点
		/// </summary>
		public float TestFloat = 1.54321f;
		/// <summary>
		/// 测试整数
		/// </summary>
		public int TestInt = 123;
		/// <summary>
		/// 测试长整数
		/// </summary>
		public long TestLong = 456;
		/// <summary>
		/// 测试双精度
		/// </summary>
		public double TestDouble = 7.123456;
		/// <summary>
		/// 测试布尔
		/// </summary>
		public bool TestBool = true;
	}
}
