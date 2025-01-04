/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
namespace WorldTree
{
	/// <summary>
	/// LiteDB测试
	/// </summary>
	public class LiteDBTestProxy : DataBaseProxy
		, AsAwake
	{

	}


	/// <summary>
	/// 测试
	/// </summary>
	public class LiteDBTest : Node
	, AsComponentBranch
	, AsChildBranch
	, ComponentOf<InitialDomain>
	, AsAwake
	{

	}

	/// <summary>
	/// 测试数据类
	/// </summary>
	[TreeDataSerializable]
	public partial class TestClass : NodeData
		,ChildOf<INode>
		,AsAwake
	{
		/// <summary>
		/// 名称
		/// </summary>
		public string Name;
	}
}