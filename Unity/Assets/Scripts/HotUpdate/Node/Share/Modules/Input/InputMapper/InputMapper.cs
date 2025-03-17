/****************************************

* 作者：闪电黑客
* 日期：2024/12/3 16:17

* 描述：

*/
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
		/// 输入检测器配置
		/// </summary>
		public InputMapperConfig Config;

		/// <summary>
		/// 输入信息队列
		/// </summary>
		[TreeDataIgnore]
		public UnitList<InputInfo> InfoList;

		/// <summary>
		/// 全局输入事件法则
		/// </summary>
		[TreeDataIgnore]
		public IRuleExecutor<InputEvent> InputEvent;
	}
}
