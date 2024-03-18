using System.Diagnostics;
using System.Runtime.InteropServices;

namespace HookTest
{
	class Program
	{
		private const int WH_KEYBOARD_LL = 13;
		private const int WM_KEYDOWN = 0x0100;
		private static LowLevelKeyboardProc _proc = HookCallback;
		private static IntPtr _hookID = IntPtr.Zero;
		private static Form1 form1 = new Form1();

		public static int a = 0;
		static void Main(string[] args)
		{
			_hookID = SetHook(_proc);

			Application.Run(form1);

			UnhookWindowsHookEx(_hookID);
		}

		private static IntPtr SetHook(LowLevelKeyboardProc proc)
		{
			using (Process curProcess = Process.GetCurrentProcess())
			using (ProcessModule curModule = curProcess.MainModule)
			{
				return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
			}
		}

		private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

		private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
		{
			if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
			{
				int vkCode = Marshal.ReadInt32(lParam);

				if ((Keys)vkCode == Keys.R && (GetKeyState((int)Keys.ControlKey) & 0x8000) != 0)
				{
					form1.labelT.Text = $"修改次数:{a++}";
					ModifyProjectFile();
				}
			}
			return CallNextHookEx(_hookID, nCode, wParam, lParam);
		}

		private static void ModifyProjectFile()
		{
			// 在这里添加修改配置文件的代码逻辑
			string ModelPath = @"E:\Project\Git\HeadMaster\DotNet\Model\DotNet.Model.csproj";
			string HotfixPath = @"E:\Project\Git\HeadMaster\DotNet\Model\DotNet.Model.csproj";

			string ModelString = File.ReadAllText(ModelPath);
			string HotfixString = File.ReadAllText(HotfixPath);
			ModelString.Replace(@"..\..\Unity\Assets\Scripts\Model\Server\**\*.cs", @"..\..\Unity\Assets\Scripts\Model\Server\**\**\*.cs");
			ModelString.Replace(@"..\..\Unity\Assets\Scripts\Model\Client\**\*.cs", @"..\..\Unity\Assets\Scripts\Model\Client\**\**\*.cs");
			ModelString.Replace(@"..\..\Unity\Assets\Scripts\Model\Share\**\*.cs", @"..\..\Unity\Assets\Scripts\Model\Share\**\**\*.cs");
			ModelString.Replace(@"..\..\Unity\Assets\Scripts\Model\Generate\Server\**\*.cs", @"..\..\Unity\Assets\Scripts\Model\Generate\Server\**\**\*.cs");

			File.WriteAllText(ModelPath, ModelString);
			HotfixString.Replace(@"..\..\Unity\Assets\Scripts\Hotfix\Client\**\*.cs", @"..\..\Unity\Assets\Scripts\Hotfix\Client\**\**\*.cs");
			HotfixString.Replace(@"..\..\Unity\Assets\Scripts\Hotfix\Server\**\*.cs", @"..\..\Unity\Assets\Scripts\Hotfix\Server\**\**\*.cs");
			HotfixString.Replace(@"..\..\Unity\Assets\Scripts\Hotfix\Share\**\*.cs", @"..\..\Unity\Assets\Scripts\Hotfix\Share\**\**\*.cs");
			File.WriteAllText(HotfixPath, HotfixString);

			Task.Run(() =>
			{
				Thread.Sleep(2000);

				string ModelString = File.ReadAllText(ModelPath);
				string HotfixString = File.ReadAllText(HotfixPath);
				ModelString.Replace(@"..\..\Unity\Assets\Scripts\Model\Server\**\**\*.cs", @"..\..\Unity\Assets\Scripts\Model\Server\**\*.cs");
				ModelString.Replace(@"..\..\Unity\Assets\Scripts\Model\Client\**\**\*.cs", @"..\..\Unity\Assets\Scripts\Model\Client\**\*.cs");
				ModelString.Replace(@"..\..\Unity\Assets\Scripts\Model\Share\**\**\*.cs", @"..\..\Unity\Assets\Scripts\Model\Share\**\*.cs");
				ModelString.Replace(@"..\..\Unity\Assets\Scripts\Model\Generate\Server\**\**\*.cs", @"..\..\Unity\Assets\Scripts\Model\Generate\Server\**\*.cs");

				File.WriteAllText(ModelPath, ModelString);
				HotfixString.Replace(@"..\..\Unity\Assets\Scripts\Hotfix\Client\**\**\*.cs", @"..\..\Unity\Assets\Scripts\Hotfix\Client\**\*.cs");
				HotfixString.Replace(@"..\..\Unity\Assets\Scripts\Hotfix\Server\**\**\*.cs", @"..\..\Unity\Assets\Scripts\Hotfix\Server\**\*.cs");
				HotfixString.Replace(@"..\..\Unity\Assets\Scripts\Hotfix\Share\**\**\*.cs", @"..\..\Unity\Assets\Scripts\Hotfix\Share\**\*.cs");

				File.WriteAllText(HotfixPath, HotfixString);
			});
		}



		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool UnhookWindowsHookEx(IntPtr hhk);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr GetModuleHandle(string lpModuleName);

		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern short GetKeyState(int keyCode);
	}
}