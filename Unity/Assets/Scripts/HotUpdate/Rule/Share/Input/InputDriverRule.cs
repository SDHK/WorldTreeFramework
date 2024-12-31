/****************************************

* 作者： 闪电黑客
* 日期： 2024/12/26 17:31

* 描述： 

*/
using System;

namespace WorldTree
{
	public static class InputDriverRule
	{
		/// <summary>
		/// 注册设备
		/// </summary>
		public static void RegisterDevice<T>(this InputDriver self, int deviceCount)
			where T : Enum
		{
			int count = Enum.GetNames(typeof(T)).Length;
			for (int i = 0; i < deviceCount; i++)
			{
				self.InputInfosList.Add(new InputDriverInfo[count]);
			}
			self.InputTypes = new InputType[count];
		}

		/// <summary>
		/// 设置输入类型
		/// </summary>
		public static void SetInputType<T>(this InputDriver self, T key, InputType type)
			where T : Enum
		{
			self.InputTypes[Convert.ToInt32(key)] = type;
		}


		/// <summary>
		/// 创建数据
		/// </summary>
		public static void InputData(this InputDriver self, byte deviceId, byte keyCode, InputDriverInfo info)
		{
			InputDriverInfo oldInfo = self.InputInfosList[deviceId][keyCode];
			if (oldInfo.IsInput == false && info.IsInput == false) return;
			if (oldInfo == info) return;

			InputState inputState = oldInfo.IsInput
			? (info.IsInput ? InputState.Active : InputState.End)
			: (info.IsInput ? InputState.Start : InputState.None);

			self.InputInfosList[deviceId][keyCode] = info;

			InputData data = new()
			{
				Device = new()
				{
					InputDeviceType = self.DeviceType,
					InputDeviceId = deviceId,
					InputType = self.InputTypes[keyCode],
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
