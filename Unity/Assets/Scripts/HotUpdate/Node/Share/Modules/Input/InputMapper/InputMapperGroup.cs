/****************************************

* 作者：闪电黑客
* 日期：2024/12/3 16:17

* 描述：

*/
namespace WorldTree
{
	/// <summary>
	/// 输入映射组
	/// </summary>
	[TreeDataSerializable]
	public partial class InputMapperGroup : NodeData
		, GenericOf<long, InputMapperManager>
		, GenericOf<long, InputMapperGroup>
		, AsGenericBranch<long>
		, AsChildBranch
		, AsAwake
	{
	}
}