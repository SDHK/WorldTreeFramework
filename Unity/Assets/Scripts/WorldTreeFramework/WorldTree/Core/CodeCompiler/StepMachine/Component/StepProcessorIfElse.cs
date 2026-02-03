namespace WorldTree
{
	public partial class StepMachine
	{
		/// <summary> 步骤处理器：条件分支 </summary>
		public StepProcessorIfElse ProcessorIfElse;
		/// <summary> 组装处理器：条件分支 </summary>
		public void AddStepProcessorIfElse() => this.AddComponent(out ProcessorIfElse);
		/// <summary> If步骤 </summary>
		public void IfPop() => ProcessorIfElse.AddIfPop();
		/// <summary> Else步骤 </summary>
		public void Else() => ProcessorIfElse.AddElse();
		/// <summary> IfEnd步骤 </summary>
		public void IfEnd() => ProcessorIfElse.AddIFEnd();
	}

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
			/// 判断地址
			/// </summary>
			public int CheckAddress;
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
		private int ExecuteIf(int address, int pointer)
		{
			StepDataIfElse data = dataList[address];

			VarValue check = GetParam(data.CheckAddress);
			if (check.ToBool())
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
		private int ExecuteElse(int address, int pointer) => dataList[address].End;


		/// <summary>
		/// 获取IF代码数据 
		/// </summary>
		public void AddIfPop()
		{
			StepDataIfElse data = new()
			{
				CheckAddress = PopParam(),
				Else = 0,
				End = 0,
			};
			dataList.Add(data);
			AddressStack.Push(dataList.Count - 1);
			AddStep(new StepExecuteData()
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
			AddStep(new StepExecuteData()
			{
				Execute = ExecuteElse,
				Address = ifAddress,
			});
		}

		/// <summary>
		/// 添加End步骤 
		/// </summary>
		public void AddIFEnd()
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
