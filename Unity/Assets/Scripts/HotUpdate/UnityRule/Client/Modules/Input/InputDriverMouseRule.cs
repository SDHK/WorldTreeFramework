/****************************************

* 作者：闪电黑客
* 日期：2024/12/25 16:17

* 描述：

*/
using UnityEngine;

namespace WorldTree
{
	public static class InputDriverMouseRule

	{
		private class Awake : AwakeRule<InputDriverMouse>
		{
			protected override void Execute(InputDriverMouse self)
			{
				self.GetParent(out self.inputManager);
				self.Core.PoolGetUnit(out self.InputInfosList);

				self.RegisterDevice(1, 12);
			}
		}

		private class Update : UpdateRule<InputDriverMouse>
		{
			protected override void Execute(InputDriverMouse self)
			{
				self.InputDriver(0);
			}
		}

		/// <summary>
		/// 鼠标控件检测
		/// </summary>
		private static void InputDriver(this InputDriverMouse self, byte deviceId)
		{
			self.IsExists[deviceId] = Input.mousePresent;
			self.InputData(deviceId, (byte)InputMouseKey.Mouse, GetAxis2(Input.mousePosition));
			self.InputData(deviceId, (byte)InputMouseKey.MouseLeft, GetPress(Input.GetMouseButton(0)));
			self.InputData(deviceId, (byte)InputMouseKey.MouseRight, GetPress(Input.GetMouseButton(1)));
			self.InputData(deviceId, (byte)InputMouseKey.MouseMiddle, GetPress(Input.GetMouseButton(2)));
			self.InputData(deviceId, (byte)InputMouseKey.MouseWheel, GetDelta2(Input.mouseScrollDelta));
			self.InputData(deviceId, (byte)InputMouseKey.Mouse0, GetPress(Input.GetMouseButton(3)));
			self.InputData(deviceId, (byte)InputMouseKey.Mouse1, GetPress(Input.GetMouseButton(4)));
		}

		/// <summary>
		/// 获取鼠标按键
		/// </summary>
		private static InputDriverInfo GetPress(bool isPress)
			=> Input.mousePresent && isPress ? new(InputType.Press, true, 1) : new(InputType.Press, false, 0);

		/// <summary>
		/// 获取鼠标位置
		/// </summary>
		private static InputDriverInfo GetAxis2(Vector2 vector2)
			=> Input.mousePresent ? new(InputType.Axis2, true, (int)vector2.x, (int)vector2.y) : new(InputType.Axis2, false, (int)vector2.x, (int)vector2.y);

		/// <summary>
		/// 获取滚轮数据
		/// </summary>
		private static InputDriverInfo GetDelta2(Vector2 vector2)
			=> Input.mousePresent && vector2 != Vector2.zero ? new(InputType.Delta2, true, (int)vector2.x, (int)vector2.y) : new(InputType.Delta2, false, 0, 0);

	}
}