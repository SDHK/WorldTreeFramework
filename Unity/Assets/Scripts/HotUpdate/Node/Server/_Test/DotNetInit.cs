/****************************************

* ���ߣ�����ڿ�
* ���ڣ�2024/8/27 11:53

* ������

*/
using System;
using System.Collections.Generic;

namespace WorldTree.Server
{

	/// <summary>
	/// a
	/// </summary>
	[INodeProxy]

	public partial class TestList<T> : List<T>, INode
	{

	}

	/// <summary>
	/// ���Խڵ�
	/// </summary>
	public partial class DotNetInit : Node
		, ComponentOf<MainWorld>
		//, AsBranch<IBranch>
		//, AsComponentBranch
		//, AsChildBranch
		, AsRule<Awake>
	{
		/// <summary>
		/// ����
		/// </summary>
		public int ConfigId;
		/// <summary>
		/// ����
		/// </summary>
		public Action Action;

		/// <summary>
		/// a
		/// </summary>
		public List<int> intsList;


	}




	/// <summary>
	/// ����
	/// </summary>
	public partial class Test<T> : Node
	, ComponentOf<DotNetInit>
	, AsRule<TestNodeEvent<DotNetInit>>
	, AsRule<TestNodeEvent<Type>>
	{
		/// <summary>
		/// �ֶ�
		/// </summary>
		public int ConfigId;

		/// <summary>
		/// ����
		/// </summary>
		public long ConfigName => ConfigId;
	}
}