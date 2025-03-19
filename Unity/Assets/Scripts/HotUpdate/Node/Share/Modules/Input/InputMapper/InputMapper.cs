/****************************************

* 作者：闪电黑客
* 日期：2024/12/3 16:17

* 描述：

*/
using System.Collections.Generic;

namespace WorldTree
{

	/// <summary>
	/// 输入事件
	/// </summary>
	public interface InputEvent : ISendRule<InputData>, IGlobalRule { }

	/// <summary>
	/// 输入映射器
	/// </summary>
	[TreeDataSerializable]
	public partial class InputMapper : NodeData
		, ChildOf<InputMapperGroup>

		, AsInputGlobal
		, AsAwake
	{
		/// <summary>
		/// 是否可改变
		/// </summary>
		public bool IsChange;

		/// <summary>
		/// 输入信息列表
		/// </summary>
		public List<InputInfo> ConfigInfoList;

		/// <summary>
		/// 全局输入事件法则
		/// </summary>
		public IRuleExecutor<InputEvent> InputEvent;

		/// <summary>
		/// 输入信息队列
		/// </summary>
		[TreeDataIgnore]
		public UnitList<InputInfo> InfoList;
	}
}
