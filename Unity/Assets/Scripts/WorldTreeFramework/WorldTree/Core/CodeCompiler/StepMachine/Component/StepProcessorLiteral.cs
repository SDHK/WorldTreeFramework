namespace WorldTree
{
	public partial class StepMachine
	{
		/// <summary> 步骤处理器：字面量  </summary>
		public StepProcessorLiteral ProcessorLiteral;
		/// <summary> 组装处理器：字面量  </summary>
		public void AddStepProcessorLiteral() => this.AddComponent(out ProcessorLiteral);
		/// <summary> 压入字面量 </summary>
		public void PushLiteral(VarValue value) => ProcessorLiteral.AddPushLiteral(value);
	}

	/// <summary>
	/// 步骤处理器：字面量 
	/// </summary>
	public class StepProcessorLiteral : StepProcessor
	{
		/// <summary>
		/// 字面量数据结构体 
		/// </summary>
		public struct StepDataLiteral
		{
			/// <summary> 字面量地址 </summary>
			public int LiteralAddress;
			/// <summary> 字面量值 </summary>
			public VarValue LiteralValue;
		}

		/// <summary>
		/// 字面量数据列表
		/// </summary>
		public UnitList<StepDataLiteral> dataList;

		/// <summary>
		///  执行步骤：压入字面量到参数栈
		/// </summary>
		private int ExecutePushLiteral(int pointer, int address)
		{
			StepDataLiteral data = dataList[address];
			SetParam(data.LiteralAddress, data.LiteralValue);
			return pointer + 1;
		}

		/// <summary>
		/// 添加压入字面量步骤 
		/// </summary>
		public void AddPushLiteral(VarValue value)
		{
			StepDataLiteral data = new()
			{
				LiteralAddress = PushParam(),
				LiteralValue = value
			};
			dataList.Add(data);
			AddStep(new(ExecutePushLiteral, dataList.Count - 1));
		}

	}
}
