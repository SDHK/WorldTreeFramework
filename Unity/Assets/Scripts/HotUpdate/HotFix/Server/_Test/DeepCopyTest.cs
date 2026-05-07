/****************************************

* 作者：闪电黑客
* 日期：2025/5/21 15:45

* 描述：

*/

using System.Collections;
using System.Collections.Generic;

namespace WorldTree.Server
{
	/// <summary>
	/// 测试数据
	/// </summary>
	[TreeCopyable]
	public partial class CopyTest
	{

		/// <summary>
		/// 测试
		/// </summary>
		public CopyTestStruct ValuetStruct = default;

		/// <summary>
		/// 测试
		/// </summary>
		public IDictionary ValueDict = null;

		/// <summary>
		/// 测试
		/// </summary>
		public CopyTestA CopyA = null;
		/// <summary>
		/// 测试引用还原
		/// </summary>
		public CopyTestA CopyARef = null;

	}

	/// <summary>
	/// 测试
	/// </summary>
	[TreeCopyable]
	public partial class CopyTestA
	{
		/// <summary>
		/// 测试
		/// </summary>
		public CopyTestB CopyTestB = null;

	}

	/// <summary>
	/// 测试
	/// </summary>
	[TreeCopyable]
	public partial class CopyTestB
	{
		/// <summary>
		/// 字符串
		/// </summary>
		public string ValueString = "ABC";

	}


	/// <summary>
	/// 测试结构体
	/// </summary>
	[TreeCopyable]
	public partial struct CopyTestStruct
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
		/// 测试
		/// </summary>
		public string ValueString = "f";

		/// <summary>
		/// a
		/// </summary>
		public int Value11 { get => Value1; set => Value1 = value; }

		/// <summary>
		/// a
		/// </summary>
		public float Value21 { get => Value2; set => Value2 = value; }
		/// <summary>
		/// 测试
		/// </summary>
		public float Value3 = 1f;
		public CopyTestStruct()
		{
		}

	}

	/// <summary>
	/// 测试字典子类
	/// </summary>
	[TreeCopyable]
	public partial class CopyTestDict1 : Dictionary<int, int>
	{
		/// <summary>
		/// 测试
		/// </summary>
		public int Value1 = 1;

		/// <summary>
		/// 测试
		/// </summary>
		public string Value11 { get; set; }
	}






	/// <summary>
	/// 深拷贝测试
	/// </summary>
	public class DeepCopyTest : Node
		, ComponentOf<INode>
		, ChildOf<INode>
		, AsRule<Awake>
	{ }





}