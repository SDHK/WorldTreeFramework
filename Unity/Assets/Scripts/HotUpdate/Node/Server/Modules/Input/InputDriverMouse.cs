/****************************************

* 作者：闪电黑客
* 日期：2025/1/2 17:04

* 描述：

*/
using System;
using System.Runtime.InteropServices;

namespace WorldTree.Server
{
	/// <summary>
	/// Windows鼠标码
	/// </summary>
	public enum WindowMouseKey : uint
	{
		/// <summary>
		/// 鼠标左键按下事件码
		/// </summary>
		LeftButtonDown = 0x0201,
		/// <summary>
		/// 鼠标左键抬起事件码
		/// </summary>
		LeftButtonUp = 0x0202,
		/// <summary>
		/// 鼠标右键按下事件码
		/// </summary>
		RightButtonDown = 0x0204,
		/// <summary>
		/// 鼠标右键抬起事件码
		/// </summary>
		RightButtonUp = 0x0205,
		/// <summary>
		/// 鼠标中键按下事件码
		/// </summary>
		MiddleButtonDown = 0x0207,
		/// <summary>
		/// 鼠标中键抬起事件码
		/// </summary>
		MiddleButtonUp = 0x0208
	}



	/// <summary>
	/// 鼠标输入驱动器
	/// </summary>
	public class InputDriverMouse : InputDriver
		, AsAwake<InputDeviceManager>
		, AsUpdate
	{

		/// <summary>
		/// 定义低级鼠标钩子的常量
		/// </summary>
		public int WH_MOUSE_LL = 14;


		/// <summary>
		/// 定义鼠标消息的委托
		/// </summary>
		public LowLevelMouseProc mouseProc;

		/// <summary>
		/// 存储钩子的句柄
		/// </summary>
		public IntPtr mouseHookID = IntPtr.Zero;

	}

	/// <summary>
	/// 定义鼠标消息的委托
	/// </summary>
	/// <param name="nCode"></param>
	/// <param name="wParam"></param>
	/// <param name="lParam"></param>
	/// <returns></returns>
	public delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);


	/// <summary>
	/// 定义 POINT 结构体
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct POINT
	{
		/// <summary>
		/// X 坐标
		/// </summary>
		public int X;
		/// <summary>
		/// Y 坐标
		/// </summary>
		public int Y;
	}
}