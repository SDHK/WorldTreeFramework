/****************************************

* 作者：闪电黑客
* 日期：2024/12/31 15:47

* 描述：

*/
namespace WorldTree
{
	public static class InputDeviceManagerRule
	{
		private class Add : AddRule<InputDeviceManager>
		{
			protected override void Execute(InputDeviceManager self)
			{
				self.Core.PoolGetUnit(out self.InputDeviceDict);
				self.Core.PoolGetUnit(out self.DataQueue);
				self.Core.GetRuleBroadcast(out self.InputDataEvent);
			}
		}

		private class Update : UpdateRule<InputDeviceManager>
		{
			protected override void Execute(InputDeviceManager self)
			{
				//输入数据塞入到设备
				while (self.DataQueue.TryDequeue(out var inputData))
				{
					//不存在设备类型则添加
					if (!self.InputDeviceDict.TryGetValue(inputData.Info.InputDeviceType, out var deviceList))
					{
						self.InputDeviceDict[inputData.Info.InputDeviceType] = self.Core.PoolGetUnit(out deviceList);
					}
					// 补全数量不够的设备
					while (deviceList.Count <= inputData.Info.InputDeviceId)
					{
						deviceList.Add(self.AddChild(out InputDevice _, inputData.Info.InputDeviceType, (byte)deviceList.Count));
					}
					deviceList[inputData.Info.InputDeviceId].SetData(inputData.Info.InputCode, inputData);
					self.InputDataEvent.Send(inputData);
				}
			}
		}

		/// <summary>	
		/// 添加输入数据
		/// </summary>
		public static void AddData(this InputDeviceManager self, InputData inputData)
		{
			//self.Log($"{inputData.Info.InputDeviceType}:{inputData.Info.InputType}:{inputData.Info.InputCode}:{inputData.Value.InputState}:({inputData.Value.X},{inputData.Value.Y},{inputData.Value.Z})");
			self.DataQueue.Enqueue(inputData);
		}
	}
}