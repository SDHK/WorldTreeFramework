/****************************************

* 作者：闪电黑客
* 日期：2025/1/4 18:04

* 描述：

*/
using UnityEngine;

namespace WorldTree
{
	public static class InputDriverKeyboardRule
	{
		private class ManagerAdd : NodeAddRule<InputDeviceManager, InputDriverKeyboard> { }

		private class Awake : AwakeRule<InputDriverKeyboard>
		{
			protected override void Execute(InputDriverKeyboard self)
			{
				self.GetParent(out self.inputManager);
				self.Core.PoolGetUnit(out self.InputInfosList);
				self.DeviceType = InputDeviceType.Keyboard;
				self.RegisterDevice(1, 104);
			}
		}
#if ENABLE_INPUT_SYSTEM
		// 新输入系统
#elif ENABLE_LEGACY_INPUT_MANAGER
		// 旧输入系统
		private class Update : UpdateRule<InputDriverKeyboard>
		{
			protected override void Execute(InputDriverKeyboard self)
			{
				self.CheckPress(KeyCode.Escape, InputBoardKey.Escape);
				self.CheckPress(KeyCode.F1, InputBoardKey.F1, InputBoardKey.F12);
				self.CheckPress(KeyCode.Tab, InputBoardKey.Tab);

				self.CheckPress(KeyCode.LeftShift, InputBoardKey.ShiftLeft, InputBoardKey.ShiftRight);
				self.CheckPress(KeyCode.LeftControl, InputBoardKey.CtrlLeft, InputBoardKey.CtrlLeft);
				self.CheckPress(KeyCode.LeftAlt, InputBoardKey.AltLeft, InputBoardKey.AltRight);

				self.CheckPress(KeyCode.Return, InputBoardKey.Enter);
				self.CheckPress(KeyCode.Space, InputBoardKey.Space);
				self.CheckPress(KeyCode.Backspace, InputBoardKey.Backspace);
				self.CheckPress(KeyCode.KeypadEnter, InputBoardKey.NumEnter);
				self.CheckPress(KeyCode.KeypadPlus, InputBoardKey.NumPlus);
				self.CheckPress(KeyCode.KeypadMinus, InputBoardKey.KeyMinus);
				self.CheckPress(KeyCode.KeypadMultiply, InputBoardKey.NumMultiply);
				self.CheckPress(KeyCode.KeypadDivide, InputBoardKey.NumDivide);
				self.CheckPress(KeyCode.KeypadPeriod, InputBoardKey.NumPeriod);
				self.CheckPress(KeyCode.Numlock, InputBoardKey.NumLock);
				self.CheckPress(KeyCode.Keypad0, InputBoardKey.Num0, InputBoardKey.Num9);
				self.CheckPress(KeyCode.Alpha0, InputBoardKey.Digit0, InputBoardKey.Digit9);
				self.CheckPress(KeyCode.UpArrow, InputBoardKey.Up);
				self.CheckPress(KeyCode.DownArrow, InputBoardKey.Down);
				self.CheckPress(KeyCode.LeftArrow, InputBoardKey.Left);
				self.CheckPress(KeyCode.RightArrow, InputBoardKey.Right);
				self.CheckPress(KeyCode.BackQuote, InputBoardKey.BackQuote);
				self.CheckPress(KeyCode.Minus, InputBoardKey.Minus);
				self.CheckPress(KeyCode.Equals, InputBoardKey.Equal);
				self.CheckPress(KeyCode.LeftBracket, InputBoardKey.LeftBracket);
				self.CheckPress(KeyCode.RightBracket, InputBoardKey.RightBracket);
				self.CheckPress(KeyCode.Backslash, InputBoardKey.Backslash);
				self.CheckPress(KeyCode.Semicolon, InputBoardKey.Semicolon);
				self.CheckPress(KeyCode.Quote, InputBoardKey.Quote);
				self.CheckPress(KeyCode.Comma, InputBoardKey.Comma);
				self.CheckPress(KeyCode.Period, InputBoardKey.Period);
				self.CheckPress(KeyCode.Slash, InputBoardKey.Slash);
				self.CheckPress(KeyCode.A, InputBoardKey.A, InputBoardKey.Z);

				self.CheckPress(KeyCode.LeftWindows, InputBoardKey.SystemKeyLeft, InputBoardKey.SystemKeyRight);
				self.CheckPress(KeyCode.CapsLock, InputBoardKey.CapsLock);
				self.CheckPress(KeyCode.Menu, InputBoardKey.MenuKey);
				self.CheckPress(KeyCode.Print, InputBoardKey.Print);
				self.CheckPress(KeyCode.ScrollLock, InputBoardKey.ScrollLock);
				self.CheckPress(KeyCode.Pause, InputBoardKey.Pause);

				self.CheckPress(KeyCode.Insert, InputBoardKey.Insert);
				self.CheckPress(KeyCode.Delete, InputBoardKey.Delete);
				self.CheckPress(KeyCode.Home, InputBoardKey.Home);
				self.CheckPress(KeyCode.End, InputBoardKey.End);
				self.CheckPress(KeyCode.PageUp, InputBoardKey.PageUp);
				self.CheckPress(KeyCode.PageDown, InputBoardKey.PageDown);
			}
		}


		/// <summary>
		/// 检测按键
		/// </summary>
		private static void CheckPress(this InputDriverKeyboard self, KeyCode keyCode, InputBoardKey keyboardKey)
			=> self.InputData(0, (byte)keyboardKey, Input.GetKey(keyCode) ? new(InputType.Press, true, 1) : new(InputType.Press, false, 0));

		/// <summary>
		/// 检测按键区间
		/// </summary>
		private static void CheckPress(this InputDriverKeyboard self, KeyCode unityKeyCode, InputBoardKey keyCode, InputBoardKey keyCodeEnd)
		{
			int count = keyCodeEnd - keyCode + 1;
			for (int i = 0; i < count; i++)
			{
				self.CheckPress(unityKeyCode + i, (InputBoardKey)((int)keyCode + i));
			}
		}

#endif
	}
}