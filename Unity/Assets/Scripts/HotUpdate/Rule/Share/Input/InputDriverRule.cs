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
		/// 添加一个设备
		/// </summary>
		public static void AddDevice<T>(this InputDriver self)
			where T : Enum
		{
			int count = Enum.GetNames(typeof(T)).Length;
			self.InputInfosList.Add(new InputInfo[count]);
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
		public static void CreateData(this InputDriver self, byte deviceId, byte keyCode, InputInfo info)
		{
			var oldInfo = self.InputInfosList[deviceId][keyCode];
			if (oldInfo == info) return;

			//info.InputState = info.InputState switch
			//{
			//	InputState.Active => oldInfo.InputState == InputState.Active ? InputState.Active : InputState.Down,
			//	InputState.Down => oldInfo.InputState == InputState.Active ? InputState.Up : InputState.Down,
			//	InputState.Up => oldInfo.InputState == InputState.Up ? InputState.Up : InputState.Down,
			//	_ => throw new NotImplementedException(),
			//};

			if (self.InputInfosList[deviceId][keyCode] == info) return;



			var data = new InputData
			{
				Device = new InpuDeviceInfo
				{
					InputDeviceType = self.DeviceType,
					InputDeviceId = deviceId,
					InputType = self.InputTypes[keyCode],
					InputCode = keyCode,
				},
				Info = info,
				TimeStamp = self.Core.RealTimeManager.GetUtcNow(),
			};

			self.inputManager.AddData(data);
		}
	}
}
