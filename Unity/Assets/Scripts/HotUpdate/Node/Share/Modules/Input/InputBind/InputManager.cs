/****************************************

* 作者：闪电黑客
* 日期：2024/12/3 16:17

* 描述：

*/
namespace WorldTree
{
	/// <summary>
	/// 输入管理器
	/// </summary>
	public partial class InputManager : NodeData
		, AsGenericBranch<long>
		, ComponentOf<World>
		, AsAwake
	{

	}
}