/****************************************

* 作者：闪电黑客
* 日期：2024/12/31 15:47

* 描述：

*/
namespace WorldTree
{
	public static class InputDeviceRule
	{

		class Awake : AwakeRule<InputDevice, InputDeviceType, byte>
		{
			protected override void Execute(InputDevice self, InputDeviceType deviceType, byte deviceId)
			{
				self.InputDeviceType = deviceType;
				self.DeviceId = deviceId;
				self.InputDatas = new InputData[256];
			}
		}

		private class Add : AddRule<InputDevice>
		{
			protected override void Execute(InputDevice self)
			{
				self.InputDatas = new InputData[256];
			}
		}

		/// <summary>
		/// 设置输入数据
		/// </summary>
		public static void SetData(this InputDevice self, byte inputCode, InputData inputData)
		{
			self.InputDatas[inputCode] = inputData;
		}


	}
}