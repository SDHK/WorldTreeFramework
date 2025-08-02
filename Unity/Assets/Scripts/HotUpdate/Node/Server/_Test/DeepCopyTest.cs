/****************************************

* ���ߣ�����ڿ�
* ���ڣ�2025/5/21 15:45

* ������

*/

using System.Collections;
using System.Collections.Generic;

namespace WorldTree.Server
{
	/// <summary>
	/// ��������
	/// </summary>
	[TreeCopyable]
	public partial class CopyTest
	{

		/// <summary>
		/// ����
		/// </summary>
		public CopyTestStruct ValuetStruct = default;

		/// <summary>
		/// ����
		/// </summary>
		public IDictionary ValueDict = null;

		/// <summary>
		/// ����
		/// </summary>
		public CopyTestA CopyA = null;
		/// <summary>
		/// �������û�ԭ
		/// </summary>
		public CopyTestA CopyARef = null;

	}

	/// <summary>
	/// ����
	/// </summary>
	[TreeCopyable]
	public partial class CopyTestA
	{
		/// <summary>
		/// ����
		/// </summary>
		public CopyTestB CopyTestB = null;

	}

	/// <summary>
	/// ����
	/// </summary>
	[TreeCopyable]
	public partial class CopyTestB
	{
		/// <summary>
		/// �ַ���
		/// </summary>
		public string ValueString = "ABC";
	}


	/// <summary>
	/// ���Խṹ��
	/// </summary>
	[TreeCopyable]
	public partial struct CopyTestStruct
	{
		/// <summary>
		/// ����
		/// </summary>
		public int Value1 = 1;
		/// <summary>
		/// ����
		/// </summary>
		public float Value2 = 1f;
		/// <summary>
		/// ����
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

		public CopyTestStruct()
		{
		}

	}

	/// <summary>
	/// �����ֵ�����
	/// </summary>
	[TreeCopyable]
	public partial class CopyTestDict1 : Dictionary<int, int>
	{
		/// <summary>
		/// ����
		/// </summary>
		public int Value1 = 1;

		/// <summary>
		/// ����
		/// </summary>
		public string Value11 { get; set; }
	}






	/// <summary>
	/// �������
	/// </summary>
	public class DeepCopyTest : Node
		, ComponentOf<INode>
		, ChildOf<INode>
		, AsRule<Awake>
	{ }





}