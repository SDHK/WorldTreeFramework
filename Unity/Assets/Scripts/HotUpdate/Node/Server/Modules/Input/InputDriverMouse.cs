/****************************************

* 作者：闪电黑客
* 日期：2025/1/2 17:04

* 描述：

*/
using System;

namespace WorldTree.Server
{
	/// <summary>
	/// 鼠标输入驱动器
	/// </summary>
	public class InputDriverMouse : InputDriver
		, AsAwake<InputDeviceManager>
		, AsUpdate
	{



	}

	/// <summary>
	/// 定义鼠标消息的委托
	/// </summary>
	/// <param name="nCode"></param>
	/// <param name="wParam"></param>
	/// <param name="lParam"></param>
	/// <returns></returns>
	public delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

}