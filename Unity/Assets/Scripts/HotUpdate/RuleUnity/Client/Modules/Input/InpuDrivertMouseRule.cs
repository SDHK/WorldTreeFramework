/****************************************

* 作者： 闪电黑客
* 日期： 2024/12/25 16:17

* 描述：

*/

using UnityEngine;

namespace WorldTree
{
	public static class InpuDrivertMouseRule
	{
		private class Awake : AwakeRule<InputDriverMouse, InputDeviceManager>
		{
			protected override void Execute(InputDriverMouse self, InputDeviceManager manager)
			{
				self.inputManager = manager;
				self.Core.PoolGetUnit(out self.InputInfosList);

				self.AddDevice<MouseKey>();
				self.SetInputType(MouseKey.Mouse, InputType.Axis2);
				self.SetInputType(MouseKey.MouseLeft, InputType.Press);
				self.SetInputType(MouseKey.MouseRight, InputType.Press);
				self.SetInputType(MouseKey.MouseMiddle, InputType.Press);
				self.SetInputType(MouseKey.MouseWheel, InputType.Delta);
				self.SetInputType(MouseKey.Mouse0, InputType.Press);
				self.SetInputType(MouseKey.Mouse1, InputType.Press);
				self.SetInputType(MouseKey.Mouse2, InputType.Press);
				self.SetInputType(MouseKey.Mouse3, InputType.Press);
				self.SetInputType(MouseKey.Mouse4, InputType.Press);
				self.SetInputType(MouseKey.Mouse5, InputType.Press);
				self.SetInputType(MouseKey.Mouse6, InputType.Press);
			}
		}

		private class Update : UpdateRule<InputDriverMouse>
		{
			protected override void Execute(InputDriverMouse self)
			{
				if (!Input.mousePresent) return;

				self.CreateData(0, (byte)MouseKey.Mouse, new()
				{
					InputState = InputState.Active,
					X = (int)Input.mousePosition.x,
					Y = (int)Input.mousePosition.y,
				});


				if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0) || Input.GetMouseButton(0))
				{
					self.CreateData(0, (byte)MouseKey.MouseLeft, new()
					{
						InputState = self.GetMouseState(0),
						X = (int)Input.mousePosition.x,
						Y = (int)Input.mousePosition.y,
					});
				}

				if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonUp(1) || Input.GetMouseButton(1))
				{
					self.CreateData(0, (byte)MouseKey.MouseRight, new()
					{
						InputState = self.GetMouseState(1),
						X = (int)Input.mousePosition.x,
						Y = (int)Input.mousePosition.y,
					});
				}

				if (Input.GetMouseButtonDown(2) || Input.GetMouseButtonUp(2) || Input.GetMouseButton(2))
				{
					self.CreateData(0, (byte)MouseKey.MouseMiddle, new()
					{
						InputState = self.GetMouseState(2),
						X = (int)Input.mousePosition.x,
						Y = (int)Input.mousePosition.y,
					});
				}

				if (Input.mouseScrollDelta.y != 0)
				{
					self.CreateData(0, (byte)MouseKey.MouseWheel, new()
					{
						InputState = InputState.Active,
						X = (int)Input.mouseScrollDelta.x,
						Y = (int)Input.mouseScrollDelta.y,
					});
				}


			}
		}


		/// <summary>
		/// 获取鼠标状态
		/// </summary>
		private static InputState GetMouseState(this InputDriverMouse self, int buttonId)
		{
			return Input.GetMouseButtonDown(buttonId) ? InputState.Start :
									   Input.GetMouseButtonUp(buttonId) ? InputState.End :
									   InputState.Active;
		}
	}
}