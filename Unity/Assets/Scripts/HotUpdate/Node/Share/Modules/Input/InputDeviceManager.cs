/****************************************

* 作者：闪电黑客
* 日期：2024/12/3 16:17

* 描述：

*/
using System.Collections.Generic;

namespace WorldTree
{
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
		public UnitDictionary<InputDeviceType, List<InputDevice>> InputDeviceDict;

		/// <summary>
		/// 输入队列
		/// </summary>
		public UnitConcurrentQueue<InputData> DataQueue;
	}
}