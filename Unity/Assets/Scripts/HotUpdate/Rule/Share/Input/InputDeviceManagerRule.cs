/****************************************

* 作者：闪电黑客
* 日期：2024/12/23 17:26

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
			}
		}

		private class Update : UpdateRule<InputDeviceManager>
		{
			protected override void Execute(InputDeviceManager self)
			{
				//while (self.DataQueue.TryDequeue(out var inputData))
				//{
				//	if (self.InputDeviceDict.TryGetValue(inputData.InputDeviceType, out var inputDeviceList))
				//	{
				//		inputDeviceList[inputData.InputDeviceId].InputDatas[inputData.InputCode] = inputData;
				//	}
				//}
			}
		}

		/// <summary>
		/// 添加输入数据
		/// </summary>
		public static void AddData(this InputDeviceManager self, InputData inputData)
		{
			self.Log($"{inputData.Device.InputDeviceType}:{inputData.Device.InputType}:{inputData.Device.InputCode}:{inputData.Info.InputState}:({inputData.Info.X},{inputData.Info.Y},{inputData.Info.Z})");
			//self.DataQueue.Enqueue(inputData);
		}
	}
}