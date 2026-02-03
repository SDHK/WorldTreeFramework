using System;

namespace WorldTree
{
	public partial class StepMachine
	{
		/// <summary> 步骤处理器：执行 </summary>
		public StepProcessorEvent ProcessorEvent;
		/// <summary> 组装处理器：执行 </summary>
		public void AddStepProcessorEvent() => this.AddComponent(out ProcessorEvent);

		/// <summary> 执行步骤 </summary>
		public void Event(Action callBack) => ProcessorEvent.AddEvent(callBack);
	}

	/// <summary>
	/// 步骤处理器：执行
	/// </summary>
	public class StepProcessorEvent : StepProcessor
	{
		/// <summary>
		/// 执行列表 
		/// </summary>
		public UnitList<Action> dataList;

		/// <summary>
		/// 执行
		/// </summary>
		private int ExecuteEvent(int address, int pointer)
		{
			dataList[address].Invoke();
			return pointer + 1;
		}

		/// <summary>
		/// 添加执行 
		/// </summary>
		public void AddEvent(Action callback)
		{
			dataList.Add(callback);
			AddStep(new()
			{
				Execute = ExecuteEvent,
				Address = dataList.Count - 1,
			});
		}
	}

	public static class StepProcessorEventRule
	{
		class AddRule : AddRule<StepProcessorEvent>
		{
			protected override void Execute(StepProcessorEvent self)
			{
				self.Core.PoolGetUnit(out self.dataList);
			}
		}

		class RemoveRule : RemoveRule<StepProcessorEvent>
		{
			protected override void Execute(StepProcessorEvent self)
			{
				self.dataList.Dispose();
				self.dataList = null;
			}
		}
	}
}
