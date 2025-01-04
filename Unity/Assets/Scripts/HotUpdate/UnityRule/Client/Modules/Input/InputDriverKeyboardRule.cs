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

		private class Awake : AwakeRule<InputDriverKeyboard>
		{
			protected override void Execute(InputDriverKeyboard self)
			{
				self.GetParent(out self.inputManager);
				self.Core.PoolGetUnit(out self.InputInfosList);
				self.RegisterDevice(1, 104);
			}
		}

		private class Update : UpdateRule<InputDriverKeyboard>
		{
			protected override void Execute(InputDriverKeyboard self)
			{
				Input.GetKey(KeyCode.A);
			}
		}
	}
}