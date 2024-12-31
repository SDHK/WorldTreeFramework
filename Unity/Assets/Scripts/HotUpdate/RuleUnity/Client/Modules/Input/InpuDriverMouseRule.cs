/****************************************

* 作者： 闪电黑客
* 日期： 2024/12/25 16:17

* 描述：

*/

using UnityEngine;

namespace WorldTree
{
	public static class InpuDriverMouseRule
	{
		private class Awake : AwakeRule<InputDriverMouse, InputDeviceManager>
		{
			protected override void Execute(InputDriverMouse self, InputDeviceManager manager)
			{
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

		private class Update : UpdateRule<InputDriverMouse>
		{
			protected override void Execute(InputDriverMouse self)
			{
				self.IsExists[0] = Input.mousePresent;
				self.InputData(0, (byte)MouseKey.Mouse, GetAxis2(Input.mousePosition));
				self.InputData(0, (byte)MouseKey.MouseLeft, GetPress(Input.GetMouseButton(0)));
				self.InputData(0, (byte)MouseKey.MouseRight, GetPress(Input.GetMouseButton(1)));
				self.InputData(0, (byte)MouseKey.MouseMiddle, GetPress(Input.GetMouseButton(2)));
				self.InputData(0, (byte)MouseKey.Mouse0, GetPress(Input.GetMouseButton(3)));
				self.InputData(0, (byte)MouseKey.Mouse1, GetPress(Input.GetMouseButton(4)));
				self.InputData(0, (byte)MouseKey.Mouse2, GetPress(Input.GetMouseButton(5)));
				self.InputData(0, (byte)MouseKey.Mouse3, GetPress(Input.GetMouseButton(6)));
				self.InputData(0, (byte)MouseKey.Mouse4, GetPress(Input.GetMouseButton(7)));
				self.InputData(0, (byte)MouseKey.Mouse5, GetPress(Input.GetMouseButton(8)));
				self.InputData(0, (byte)MouseKey.Mouse6, GetPress(Input.GetMouseButton(9)));
				self.InputData(0, (byte)MouseKey.MouseWheel, GetDelta2(Input.mouseScrollDelta));
			}
		}

		/// <summary>
		/// 获取鼠标按键
		/// </summary>
		private static InputDriverInfo GetPress(bool isPress) => Input.mousePresent && isPress ? new(1) : default;

		/// <summary>
		/// 获取鼠标位置
		/// </summary>
		private static InputDriverInfo GetAxis2(Vector2 vector2) => Input.mousePresent ? new((int)vector2.x, (int)vector2.y) : default;

		/// <summary>
		/// 获取滚轮数据
		/// </summary>
		private static InputDriverInfo GetDelta2(Vector2 vector2) => Input.mousePresent && vector2 != Vector2.zero ? new((int)vector2.x, (int)vector2.y) : default;

	}
}