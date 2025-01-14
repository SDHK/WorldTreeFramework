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
	/// 输入映射管理器
	/// </summary>
	public class InputMapperManager : Node
	{
	}

	/// <summary>
	/// 输入映射组
	/// </summary>
	public class InputMapperGroup : Node
		, AsChildBranch
		, AsAwake
	{
	}

	/// <summary>
	/// 输入映射器
	/// </summary>
	public class InputMapper : Node
		, ChildOf<InputMapperGroup>
		, AsInputGlobal
	{
		/// <summary>
		/// 输入检测器配置
		/// </summary>
		public InputMapperConfig config;

		/// <summary>
		/// 输入信息队列
		/// </summary>
		public List<InputInfo> InfoList;


		/// <summary>
		/// 全局执行器Type
		/// </summary>
		public long InputRuleType;

		/// <summary>
		/// 全局输入事件法则
		/// </summary>
		public IRuleActuator<InputEvent> InputEvent;
	}


	/// <summary>
	/// 输入检测器配置
	/// </summary>
	public struct InputMapperConfig
	{
		/// <summary>
		/// 是否可改变
		/// </summary>
		public bool IsChange;

		/// <summary>
		/// 输入信息列表
		/// </summary>
		public List<InputInfo> InfoList;

	}
}