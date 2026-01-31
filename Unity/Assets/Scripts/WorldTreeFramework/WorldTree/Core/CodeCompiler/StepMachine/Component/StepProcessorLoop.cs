using System;

namespace WorldTree
{

	public partial class StepMachine
	{
		/// <summary> 步骤处理器：循环 </summary>
		public StepProcessorLoop ProcessorLoop;
		/// <summary> 组装处理器：循环 </summary>
		public void AddStepProcessorLoop() => this.AddComponent(out ProcessorLoop);

		/// <summary> 循环开始 </summary>
		public void LoopEnter() => ProcessorLoop.AddLoopEnter();
		/// <summary> 循环结束 </summary>
		public void LoopEnd() => ProcessorLoop.AddLoopEnd();
		/// <summary> 循环检测开始 </summary>
		public void LoopEnter(Func<bool> check) => ProcessorLoop.AddLoopEnter(check);
		/// <summary> 循环检测结束 </summary>
		public void LoopEnd(Func<bool> check) => ProcessorLoop.AddLoopEnd(check);
		/// <summary> Continue步骤 </summary>
		public void Continue() => ProcessorLoop.AddContinue();
		/// <summary> Break步骤 </summary>
		public void Break() => ProcessorLoop.AddBreak();
	}

	/// <summary>
	/// 步骤处理器：循环 
	/// </summary>
	public class StepProcessorLoop : StepProcessor
	{
		/// <summary>
		/// 循环结构体 
		/// </summary>
		public struct StepDataLoop
		{
			/// <summary>
			/// 循环判断事件 
			/// </summary>
			public Func<bool> Check;
			/// <summary>
			/// 循环开始地址   
			/// </summary>
			public int Enter;
			/// <summary>
			/// 循环结束地址 
			/// </summary>
			public int End;
		}

		/// <summary>
		/// 循环数据列表 
		/// </summary>
		public UnitList<StepDataLoop> dataList;

		/// <summary>
		///	循环地址栈 
		/// </summary>
		public UnitStack<int> AddressStack;


		/// <summary>
		/// 执行Loop步骤
		/// </summary>
		private int ExecuteLoopEnter(int pointer, int address)
		{
			StepDataLoop data = dataList[address];
			return (data.Check() ? pointer : data.End) + 1;
		}

		/// <summary>
		/// 执行LoopEnd步骤 
		/// </summary>
		private int ExecuteLoopEnd(int pointer, int address)
		{
			StepDataLoop data = dataList[address];
			return data.Check() ? data.Enter : pointer + 1;
		}

		/// <summary>
		/// 执行Continue步骤 
		/// </summary>
		private int ExecuteContinue(int pointer, int address) => dataList[address].Enter;

		/// <summary>
		/// 执行Break步骤 
		/// </summary>
		private int ExecuteBreak(int pointer, int address) => dataList[address].End + 1;


		/// <summary>
		/// 添加循环开始
		/// </summary>
		public void AddLoopEnter()
		{
			AddressStack.Push(dataList.Count);
			dataList.Add(new() { Enter = GetStepCount() });
		}

		/// <summary>
		/// 添加循环结束 
		/// </summary>
		public void AddLoopEnd()
		{
			int address = AddressStack.Pop();
			StepDataLoop data = dataList[address];
			data.End = GetStepCount();
			dataList[address] = data;
			AddStep(new(ExecuteLoopEnd, address));
		}

		/// <summary>
		/// 添加循环检测开始 
		/// </summary>
		public void AddLoopEnter(Func<bool> check)
		{
			// 地址栈添加当前任务地址
			AddressStack.Push(dataList.Count);
			// 新建循环结构数据
			dataList.Add(new()
			{
				Check = check,
				Enter = GetStepCount()
			});
			// 添加LoopEnter步骤
			AddStep(new(ExecuteLoopEnter, dataList.Count - 1));
		}

		/// <summary>
		/// 添加循环检测结束 
		/// </summary>
		public void AddLoopEnd(Func<bool> check)
		{
			// 获取LoopEnter地址
			int address = AddressStack.Pop();
			// 设置LoopEnter的End地址
			StepDataLoop data = dataList[address];
			data.Check = check;
			data.End = GetStepCount();
			dataList[address] = data;
			// 添加LoopEnd
			AddStep(new(ExecuteLoopEnd, address));
		}

		/// <summary>
		/// 添加Continue步骤 
		/// </summary>
		public void AddContinue()
		{
			if (AddressStack.Count == 0) this.LogError("Continue步骤获取错误，缺少对应的Loop步骤");
			// 获取Loop地址
			int loopAddress = AddressStack.Peek();
			// 添加Continue步骤
			AddStep(new(ExecuteContinue, loopAddress));
		}

		/// <summary>
		/// 添加Break步骤 
		/// </summary>
		public void AddBreak()
		{
			if (AddressStack.Count == 0) this.LogError("Break步骤获取错误，缺少对应的Loop步骤");
			// 获取Loop地址
			int loopAddress = AddressStack.Peek();
			// 添加Break
			AddStep(new(ExecuteBreak, loopAddress));
		}
	}

	public static class StepProcessorLoopRule
	{
		class Add : AddRule<StepProcessorLoop>
		{
			protected override void Execute(StepProcessorLoop self)
			{
				self.Core.PoolGetUnit(out self.dataList);
				self.Core.PoolGetUnit(out self.AddressStack);
			}
		}
		class Remove : RemoveRule<StepProcessorLoop>
		{
			protected override void Execute(StepProcessorLoop self)
			{
				self.dataList.Dispose();
				self.dataList = null;
				self.AddressStack.Dispose();
				self.AddressStack = null;
			}
		}
	}
}
