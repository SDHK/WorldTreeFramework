/****************************************

* 作者：闪电黑客
* 日期：2024/12/3 16:17

* 描述：

*/
namespace WorldTree
{
	/// <summary>
	/// 输入映射存档
	/// </summary>
	[TreeDataSerializable]
	public partial class InputArchive : NodeData
		, GenericOf<string, InputManager>
		, AsGenericBranch<string>
		, AsAwake
	{
	}
}