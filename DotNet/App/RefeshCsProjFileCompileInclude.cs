using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
//using static WorldTree.RefeshCsProjFileCompileInclude;

namespace WorldTree
{
	public class RefeshCsProjFileCompileInclude : Node, ComponentOf<INode>
		, AsRule<IAwakeRule>
	{
		public const int WH_KEYBOARD_LL = 13;
		public const int WM_KEYDOWN = 0x0100;
		public static LowLevelKeyboardProc _proc = HookCallback;
		public static IntPtr _hookID = IntPtr.Zero;

		public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

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

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr SetWindowsHookEx(int idHook, RefeshCsProjFileCompileInclude.LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool UnhookWindowsHookEx(IntPtr hhk);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr GetModuleHandle(string lpModuleName);
	}
}