/****************************************

* 作者：闪电黑客
* 日期：2025/5/21 15:45

* 描述：

*/

namespace WorldTree.Server
{
	/// <summary>
	/// 测试数据
	/// </summary>
	[TreeCopyable]
	public class CopyTest1
	{
		/// <summary>
		/// 测试
		/// </summary>
		public int Value1 = 1;
		/// <summary>
		/// 测试
		/// </summary>
		public float Value2 = 1f;

		/// <summary>
		/// a
		/// </summary>
		public int Value11 { get => Value1; set => Value1 = value; }

		/// <summary>
		/// a
		/// </summary>
		public float Value21 { get => Value2; set => Value2 = value; }
	}

	/// <summary>
	/// 测试数据
	/// </summary>
	public class CopyTest1Sub
	{
		/// <summary>
		/// 测试
		/// </summary>
		public int Value1 = 1;
	}



	/// <summary>
	/// 深拷贝测试
	/// </summary>
	public class DeepCopyTest : Node
		, ComponentOf<INode>
		, AsAwake
	{ }

}