﻿/****************************************

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
	public partial class InputGroup : NodeData
		, GenericOf<long, InputLayer>
		, AsGenericBranch<long>
		, AsChildBranch
		, AsAwake
	{
	}
}