/****************************************

* 作者：闪电黑客
* 日期：2024/12/3 16:17

* 描述：

*/
namespace WorldTree
{


	/// <summary>
	/// 输入映射层
	/// </summary>
	[TreeDataSerializable]
	public partial class InputLayer : NodeData
		, GenericOf<long, InputArchive>
		, AsGenericBranch<long>
		, AsAwake
	{
	}
}