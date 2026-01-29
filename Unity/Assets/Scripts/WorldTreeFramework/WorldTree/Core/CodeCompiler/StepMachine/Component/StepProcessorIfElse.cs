using System;

namespace WorldTree
{

	/// <summary>
	/// 步骤处理器：条件分支 
	/// </summary>
	public class StepProcessorIfElse : StepProcessor
		, AsRule<Awake>
	{
		/// <summary>
		/// 条件结构体 
		/// </summary>
		public struct StepDataIfElse
		{
			/// <summary>
			/// 条件判断事件 
			/// </summary>
			public Func<bool> Check;
			/// <summary>
			/// Else地址 
			/// </summary>
			public int Else;
			/// <summary>
			/// End地址 
			/// </summary>
			public int End;
		}
		/// <summary>
		/// 条件数据列表
		/// </summary>
		public UnitList<StepDataIfElse> dataList;

		/// <summary>
		/// IF地址栈 
		/// </summary>
		public UnitStack<int> AddressStack;

		/// <summary>
		/// 执行If步骤 
		/// </summary>
		private int ExecuteIf(int pointer, int address)
		{
			StepDataIfElse data = dataList[address];
			if (data.Check.Invoke())
				return pointer + 1;
			else if (data.Else != 0)
				return data.Else + 1;
			else if (data.End != 0)
				return data.End;
			else
				this.LogError("If步骤执行错误，缺少Else或End地址");
			return pointer + 1;
		}

		/// <summary>
		/// 执行Else步骤 
		/// </summary>
		private int ExecuteElse(int pointer, int address) => dataList[address].End;//跳转到End地址


		/// <summary>
		/// 获取IF代码数据 
		/// </summary>
		public void AddIF(Func<bool> check)
		{
			StepDataIfElse data = new()
			{
				Check = check,
				Else = 0,
				End = 0,
			};
			dataList.Add(data);
			AddressStack.Push(dataList.Count - 1);
			AddStep(new StepData()
			{
				Execute = ExecuteIf,
				Address = dataList.Count - 1,
			});
		}
		/// <summary>
		/// 添加Else步骤 
		/// </summary>
		public void AddElse()
		{
			if (AddressStack.Count == 0)
				this.LogError("Else步骤获取错误，缺少对应的If步骤");
			// 获取If地址
			int ifAddress = AddressStack.Peek();
			// 设置If的Else地址
			StepDataIfElse data = dataList[ifAddress];
			data.Else = GetStepCount() - 1;
			dataList[ifAddress] = data;
			AddStep(new StepData()
			{
				Execute = ExecuteElse,
				Address = ifAddress,
			});
		}

		/// <summary>
		/// 添加End步骤 
		/// </summary>
		public void AddEnd()
		{
			if (AddressStack.Count == 0)
				this.LogError("End步骤获取错误，缺少对应的If步骤");
			// 获取If地址
			int ifAddress = AddressStack.Pop();
			// 设置If的End地址
			StepDataIfElse data = dataList[ifAddress];
			data.End = GetStepCount();
			dataList[ifAddress] = data;
		}
	}

	public static class StepProcessorIfElseRule
	{
		class Add : AddRule<StepProcessorIfElse>
		{
			protected override void Execute(StepProcessorIfElse self)
			{
				self.Core.PoolGetUnit(out self.dataList);
				self.Core.PoolGetUnit(out self.AddressStack);
			}
		}
		class Remove : RemoveRule<StepProcessorIfElse>
		{
			protected override void Execute(StepProcessorIfElse self)
			{
				self.dataList.Dispose();
				self.dataList = null;
				self.AddressStack.Dispose();
				self.AddressStack = null;
			}
		}
	}
}
