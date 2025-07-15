/****************************************

* 作者：闪电黑客
* 日期：2024/12/21 18:09

* 描述：

*/
using System;

namespace WorldTree
{
	/// <summary>
	/// 输入数据
	/// </summary>
	public struct InputData
	{
		/// <summary>
		/// 输入设备信息
		/// </summary>
		public InputInfo Info;

		/// <summary>
		/// 输入状态信息
		/// </summary>
		public InputValue Value;

		/// <summary>
		/// 时间戳（毫秒）
		/// </summary>
		public DateTime TimeStamp;
	}


}