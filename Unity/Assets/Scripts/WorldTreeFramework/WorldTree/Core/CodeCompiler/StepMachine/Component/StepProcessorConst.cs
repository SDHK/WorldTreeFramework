namespace WorldTree
{
	public partial class StepMachine
	{
		/// <summary> 步骤处理器：常量参数  </summary>
		public StepProcessorConst ProcessorConst;
		/// <summary> 组装处理器：常量参数  </summary>
		public void AddStepProcessorConst() => this.AddComponent(out ProcessorConst);
		/// <summary> 压入常量 </summary>
		public void PushConst(VarValue value) => ProcessorConst.AddPushConst(value);
	}

	/// <summary>
	/// 步骤处理器：常量参数 
	/// </summary>
	public class StepProcessorConst : StepProcessor
	{
		/// <summary>
		/// 常量参数数据结构体 
		/// </summary>
		public struct StepDataConst
		{
			/// <summary> 常量地址 </summary>
			public int ConstAddress;
			/// <summary> 常量值 </summary>
			public VarValue ConstValue;
		}

		/// <summary>
		/// 常量数据列表
		/// </summary>
		public UnitList<StepDataConst> dataList;

		/// <summary>
		///  执行步骤：压入常量到参数栈
		/// </summary>
		private int ExecutePushConst(int pointer, int address)
		{
			StepDataConst data = dataList[address];
			SetParam(data.ConstAddress, data.ConstValue);
			return pointer + 1;
		}

		/// <summary>
		/// 添加压入常量步骤 
		/// </summary>
		public void AddPushConst(VarValue value)
		{
			StepDataConst data = new()
			{
				ConstAddress = PushParam(),
				ConstValue = value
			};
			dataList.Add(data);
			AddStep(new(ExecutePushConst, dataList.Count - 1));
		}
	}
}
