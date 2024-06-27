using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
//using static WorldTree.RefeshCsProjFileCompileInclude;

namespace WorldTree
{
	/// <summary>
	/// 刷新
	/// </summary>
	public class RefeshCsProjFileCompileInclude : Node, ComponentOf<INode>
		, AsAwake
	{
		/// <summary>
		/// 窗口
		/// </summary>
		public const int WH_KEYBOARD_LL = 13;
		/// <summary>
		/// 窗口
		/// </summary>
		public const int WM_KEYDOWN = 0x0100;
		/// <summary>
		/// 回调
		/// </summary>
		public static LowLevelKeyboardProc _proc = HookCallback;
		/// <summary>
		/// id
		/// </summary>
		public static IntPtr _hookID = IntPtr.Zero;

		/// <summary>
		/// 委托
		/// </summary>
		public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

		/// <summary>
		/// 钩子回调
		/// </summary>
		public static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
		{
			if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
			{
				int vkCode = Marshal.ReadInt32(lParam);

				//// 在这里添加你的快捷键处理逻辑
				//if ((Keys)vkCode == Keys.F5)
				//{
				//	// 执行修改配置文件的操作
				//	ModifyProjectFile();
				//}
			}
			return RefeshCsProjFileCompileIncludeRule.CallNextHookEx(_hookID, nCode, wParam, lParam);
		}

		/// <summary>
		/// 修改
		/// </summary>
		public static void ModifyProjectFile()
		{
			// 在这里添加修改配置文件的代码逻辑
			Console.WriteLine("修改配置文件");
		}
	}

	public static partial class RefeshCsProjFileCompileIncludeRule
	{
		private class AddRule : AddRule<RefeshCsProjFileCompileInclude>
		{
			protected override void Execute(RefeshCsProjFileCompileInclude self)
			{
				// 设置钩子
				Console.WriteLine("监听启动");

				RefeshCsProjFileCompileInclude._hookID = SetHook(RefeshCsProjFileCompileInclude._proc);
			}
		}

		private class RemoveRule : RemoveRule<RefeshCsProjFileCompileInclude>
		{
			protected override void Execute(RefeshCsProjFileCompileInclude self)
			{
				// 移除钩子
				UnhookWindowsHookEx(RefeshCsProjFileCompileInclude._hookID);
			}
		}

		/// <summary>
		/// 设置钩子
		/// </summary>
		/// <param name="proc"></param>
		/// <returns></returns>
		public static IntPtr SetHook(RefeshCsProjFileCompileInclude.LowLevelKeyboardProc proc)
		{
			using (Process curProcess = Process.GetCurrentProcess())
			using (ProcessModule curModule = curProcess.MainModule)
			{
				return SetWindowsHookEx(RefeshCsProjFileCompileInclude.WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
			}
		}
		/// <summary>
		/// dll
		/// </summary>
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr SetWindowsHookEx(int idHook, RefeshCsProjFileCompileInclude.LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

		/// <summary>
		/// dll
		/// </summary>
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool UnhookWindowsHookEx(IntPtr hhk);

		/// <summary>
		/// dll
		/// </summary>
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

		/// <summary>
		/// dll
		/// </summary>
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr GetModuleHandle(string lpModuleName);
	}
}