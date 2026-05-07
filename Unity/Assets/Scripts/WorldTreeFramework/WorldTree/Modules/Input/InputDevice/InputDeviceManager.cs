/****************************************

* 作者：闪电黑客
* 日期：2024/12/3 16:17

* 描述：

*/
namespace WorldTree
{


	/// <summary>
	/// 全局输入事件法则
	/// </summary>
	public interface InputGlobal : ISendRule<InputData>, IGlobalRule { }


	/// <summary>
	/// 输入设备管理器
	/// </summary>
	public partial class InputDeviceManager : Node
		, ComponentOf<World>
		, AsComponentBranch
		, AsChildBranch
		, AsRule<Awake>
	{
		/// <summary>
		/// 输入设备集合
		/// </summary>
		public UnitDictionary<InputDeviceType, UnitList<InputDevice>> InputDeviceDict;

		/// <summary>
		/// 输入队列
		/// </summary>
		public UnitConcurrentQueue<InputData> DataQueue;

		/// <summary>
		/// 输入数据全局广播
		/// </summary>
		public RuleBroadcast<InputGlobal> InputDataEvent;

		[NodeRule(nameof(AddRule<InputDeviceManager>))]
		private static void OnAddRule(InputDeviceManager self)
		{
			self.World.PoolGetUnit(out self.InputDeviceDict);
			self.World.PoolGetUnit(out self.DataQueue);
			self.World.GetRuleBroadcast(out self.InputDataEvent);
		}

		[NodeRule(nameof(UpdateRule<InputDeviceManager>))]
		private static void OnUpdateRule(InputDeviceManager self)
		{
			//输入数据塞入到设备
			while (self.DataQueue.TryDequeue(out var inputData))
			{
				//不存在设备类型则添加
				if (!self.InputDeviceDict.TryGetValue(inputData.Info.InputDeviceType, out var deviceList))
				{
					self.InputDeviceDict[inputData.Info.InputDeviceType] = self.World.PoolGetUnit(out deviceList);
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

		/// <summary>	
		/// 添加输入数据
		/// </summary>
		public void AddData(InputData inputData)
		{
			//self.Log($"{inputData.Info.InputDeviceType}:{inputData.Info.InputType}:{inputData.Info.InputCode}:{inputData.Value.InputState}:({inputData.Value.X},{inputData.Value.Y},{inputData.Value.Z})");
			this.DataQueue.Enqueue(inputData);
		}
	}


}