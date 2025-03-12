/****************************************

* 作者：闪电黑客
* 日期：2024/12/3 16:17

* 描述：

*/
namespace WorldTree
{


	/// <summary>
	/// 全局输入事件法则
	/// </summary>
	public interface InputGlobal : ISendRule<InputData>, IGlobalRule { }



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
		public IRuleActuator<InputGlobal> InputDataEvent;
	}


}