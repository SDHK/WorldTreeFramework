/****************************************

* 作者：闪电黑客
* 日期：2024/12/31 15:47

* 描述：

*/
namespace WorldTree
{
	public static class InputDriverRule
	{
		/// <summary>
		/// 注册设备
		/// </summary>
		/// <param name="deviceCount">设备数量</param>
		/// <param name="keyCount">每个设备的控件数量</param>
		public static void RegisterDevice(this InputDriver self, int deviceCount, int keyCount)
		{
			self.IsExists = new bool[deviceCount];
			for (int i = 0; i < deviceCount; i++)
			{
				self.InputInfosList.Add(new InputDriverInfo[keyCount]);
			}
		}

		/// <summary>
		/// 创建数据发送到管理器
		/// </summary>
		public static void InputData(this InputDriver self, byte deviceId, byte keyCode, InputDriverInfo info)
		{
			InputDriverInfo oldInfo = self.InputInfosList[deviceId][keyCode];
			if (oldInfo.IsInput == false && info.IsInput == false) return;
			if (oldInfo == info) return;

			// 计算组合状态
			InputState inputState = oldInfo.IsInput
				? (info.IsInput ? InputState.Active : InputState.End)
				: (info.IsInput ? (InputState.Active | InputState.Start) : InputState.End);

			self.InputInfosList[deviceId][keyCode] = info;

			InputData data = new()
			{
				Device = new()
				{
					InputDeviceType = self.DeviceType,
					InputDeviceId = deviceId,
					InputType = info.InputType,
					InputCode = keyCode,
				},
				Info = new()
				{
					InputState = inputState,
					X = info.X,
					Y = info.Y,
					Z = info.Z,
				},
				TimeStamp = self.Core.RealTimeManager.GetUtcNow(),
			};

			self.inputManager.AddData(data);
		}
	}
}
