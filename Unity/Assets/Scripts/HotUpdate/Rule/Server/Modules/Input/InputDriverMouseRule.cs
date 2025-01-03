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
		/// <summary>
		/// 定义低级鼠标钩子的常量
		/// </summary>
		private const int WH_MOUSE_LL = 14;

		/// <summary>
		/// 定义鼠标消息的委托
		/// </summary>
		private static LowLevelMouseProc proc;

		/// <summary>
		/// 存储钩子的句柄
		/// </summary>
		private static IntPtr hookID = IntPtr.Zero;


		private class Awake : AwakeRule<InputDriverMouse, InputDeviceManager>
		{
			protected override void Execute(InputDriverMouse self, InputDeviceManager arg1)
			{

			}
		}

		/// <summary>
		/// 设置钩子
		/// </summary>
		/// <param name="proc"></param>
		/// <returns></returns>
		private static IntPtr SetHook(LowLevelMouseProc proc)
		{
			using (Process curProcess = Process.GetCurrentProcess())
			using (ProcessModule curModule = curProcess.MainModule)
			{
				// 调用 SetWindowsHookEx 设置钩子
				return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
			}
		}

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

	}

}