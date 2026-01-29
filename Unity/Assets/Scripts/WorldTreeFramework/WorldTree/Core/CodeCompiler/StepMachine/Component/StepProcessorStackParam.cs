namespace WorldTree
{
	//这个需要作用域支持，暂停实现
	/// <summary>
	/// 步骤处理器：栈参数 
	/// </summary>
	public class StepProcessorStackParam : StepProcessor
	{
		/// <summary>
		/// 压入字面量列表
		/// </summary>
		public UnitList<VarValue> PushParamList;

		/// <summary>
		/// 弹出变量地址列表
		/// </summary>
		public UnitList<int> PopVariableList;


		/// <summary>
		///  执行步骤：压入参数
		/// </summary>
		private int ExecutePushParam(int pointer, int address)
		{
			Push(PushParamList[address]);
			return pointer + 1;
		}

		/// <summary>
		/// 执行步骤：弹出变量 
		/// </summary>
		private int ExecutePopVariable(int pointer, int address)
		{
			var value = Pop();
			int varAddress = PopVariableList[address];
			//WorldTreeRuntime.currentRuntime.variableManager.SetVariableValue(address, value);
			return pointer + 1;
		}

		/// <summary>
		/// 添加压入参数步骤 
		/// </summary>
		public void AddPushParam(VarValue value)
		{
			PushParamList.Add(value);
			AddStep(new(ExecutePushParam, PushParamList.Count - 1));
		}



	}
}
