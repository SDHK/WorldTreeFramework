/****************************************

* 作者：闪电黑客
* 日期：2024/12/3 16:17

* 描述：

*/
using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 输入事件法则
	/// </summary>
	public interface InputDataEvent : ISendRule<InputData> { }


	/// <summary>
	/// 输入设备管理器
	/// </summary>
	public class InputDeviceManager : Node
		, ComponentOf<WorldTreeRoot>
		, AsComponentBranch
		, AsChildBranch
		, AsAwake
		, AsUpdate
	{
		/// <summary>
		/// 输入设备集合
		/// </summary>
		public UnitDictionary<InputDeviceType, UnitList<InputDevice>> InputDeviceDict;

		/// <summary>
		/// 输入队列
		/// </summary>
		public UnitConcurrentQueue<InputData> DataQueue;

		/// <summary>
		/// 输入数据全局广播
		/// </summary>
		public GlobalRuleActuator<InputDataEvent> InputDataEvent;
	}

	//===============
	/// <summary>
	/// 输入适配器配置
	/// </summary>
	public struct InputAdapterConfig
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

	/// <summary>
	/// 输入适配器
	/// </summary>
	public class InputAdapter : Node
	{
		/// <summary>
		/// 输入适配器配置
		/// </summary>
		public InputAdapterConfig config;

		/// <summary>
		/// 输入信息列表
		/// </summary>
		public List<InputInfo> InfoList;
	}

	/// <summary>
	/// 区间按键适配器
	/// </summary>
	public class InputAxisPress2Adapter : InputAdapter
	{


	}




	/// <summary>
	/// 输入控件
	/// </summary>
	public class InputControl : Node
		, AsAwake
		, AsInputDataEvent
	{
		/// <summary>
		/// 输入适配器
		/// </summary>
		public InputAdapter InputAdapter;

		/// <summary>
		/// 行为事件
		/// </summary>
		public long InputType;

	}

}