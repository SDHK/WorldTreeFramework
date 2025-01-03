/****************************************

* 作者：闪电黑客
* 日期：2025/1/2 17:06

* 描述：

*/
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WorldTree.Server
{
	public static class InpuDriverMouseRule
	{
		private class Awake : AwakeRule<InputDriverMouse, InputDeviceManager>
		{
			protected override void Execute(InputDriverMouse self, InputDeviceManager manager)
			{
				self.mouseProc = self.HookMouseCallback;
				self.mouseHookID = self.SetHook(self.mouseProc);

				self.inputManager = manager;
				self.Core.PoolGetUnit(out self.InputInfosList);

				self.IsExists = new bool[256];

				self.RegisterDevice<MouseKey>(1);

				self.SetInputType(MouseKey.Mouse, InputType.Axis2);
				self.SetInputType(MouseKey.MouseLeft, InputType.Press);
				self.SetInputType(MouseKey.MouseRight, InputType.Press);
				self.SetInputType(MouseKey.MouseMiddle, InputType.Press);
				self.SetInputType(MouseKey.MouseWheel, InputType.Delta2);
				self.SetInputType(MouseKey.Mouse0, InputType.Press);
				self.SetInputType(MouseKey.Mouse1, InputType.Press);
				self.SetInputType(MouseKey.Mouse2, InputType.Press);
				self.SetInputType(MouseKey.Mouse3, InputType.Press);
				self.SetInputType(MouseKey.Mouse4, InputType.Press);
				self.SetInputType(MouseKey.Mouse5, InputType.Press);
				self.SetInputType(MouseKey.Mouse6, InputType.Press);
			}
		}
		class Remove : RemoveRule<InputDriverMouse>
		{
			protected override void Execute(InputDriverMouse self)
			{
				UnhookWindowsHookEx(self.mouseHookID);
				self.mouseProc = null;
				self.mouseHookID = default;
			}
		}


		/// <summary>
		/// 鼠标钩子的回调函数
		/// </summary>
		/// <param name="nCode"></param>
		/// <param name="wParam"></param>
		/// <param name="lParam"></param>
		/// <returns></returns>
		private static IntPtr HookMouseCallback(this InputDriverMouse self, int nCode, IntPtr wParam, IntPtr lParam)
		{
			if (nCode < 0) return CallNextHookEx(self.mouseHookID, nCode, wParam, lParam);
			switch (wParam)
			{
				case (IntPtr)WindowMouseKey.LeftButtonDown:
					self.Log("111");
					//self.InputData(0, (byte)MouseKey.MouseLeft, GetPress(true));
					break;
				case (IntPtr)WindowMouseKey.LeftButtonUp:
					//self.InputData(0, (byte)MouseKey.MouseLeft, GetPress(false));
					break;
				case (IntPtr)WindowMouseKey.RightButtonDown:
					//self.InputData(0, (byte)MouseKey.MouseRight, GetPress(true));
					break;
				case (IntPtr)WindowMouseKey.RightButtonUp:
					//self.InputData(0, (byte)MouseKey.MouseRight, GetPress(false));
					break;
				case (IntPtr)WindowMouseKey.MiddleButtonDown:
					//self.InputData(0, (byte)MouseKey.MouseMiddle, GetPress(true));
					break;
				case (IntPtr)WindowMouseKey.MiddleButtonUp:
					//self.InputData(0, (byte)MouseKey.MouseMiddle, GetPress(false));
					break;
			}

			// 调用下一个钩子
			return CallNextHookEx(self.mouseHookID, nCode, wParam, lParam);
		}

		/// <summary>
		/// 获取鼠标按键
		/// </summary>
		private static InputDriverInfo GetPress(bool isPress) => isPress ? new(true, 1) : default;

		/// <summary>
		/// 设置钩子
		/// </summary>
		private static IntPtr SetHook(this InputDriverMouse self, LowLevelMouseProc proc)
		{
			using (Process curProcess = Process.GetCurrentProcess())
			using (ProcessModule curModule = curProcess.MainModule)
			{
				// 设置全局鼠标钩子
				return SetWindowsHookEx(self.WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
			}
		}


		#region 外部函数

		/// <summary>
		/// 声明外部函数 SetWindowsHookEx
		/// </summary>
		/// <param name="idHook"></param>
		/// <param name="lpfn"></param>
		/// <param name="hMod"></param>
		/// <param name="dwThreadId"></param>
		/// <returns></returns>
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

		/// <summary>
		/// 声明外部函数 UnhookWindowsHookEx
		/// </summary>
		/// <param name="hhk"></param>
		/// <returns></returns>
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool UnhookWindowsHookEx(IntPtr hhk);

		/// <summary>
		/// 声明外部函数 CallNextHookEx
		/// </summary>
		/// <param name="hhk"></param>
		/// <param name="nCode"></param>
		/// <param name="wParam"></param>
		/// <param name="lParam"></param>
		/// <returns></returns>
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

		/// <summary>
		/// 声明外部函数 GetModuleHandle
		/// </summary>
		/// <param name="lpModuleName"></param>
		/// <returns></returns>
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr GetModuleHandle(string lpModuleName);

		/// <summary>
		/// 声明外部函数 GetCursorPos
		/// </summary>
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetCursorPos(out POINT lpPoint);

		#endregion
	}

}