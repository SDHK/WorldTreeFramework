namespace WorldTree
{
	/// <summary>
	/// 步骤处理器基类 
	/// </summary>
	public abstract class StepProcessor : Node
		, ComponentOf<StepMachine>
		, AsRule<Awake>
	{
		/// <summary>
		/// 步骤执行器 
		/// </summary>
		public StepMachine Machine;

		/// <summary>
		/// 压入参数  
		/// </summary>
		public int PushParam() => Machine.PushParam();

		/// <summary>
		/// 弹出参数 
		/// </summary>
		public int PopParam() => Machine.PopParam();

		/// <summary>
		/// 获取参数 
		/// </summary>
		public VarValue GetParam(int address) => Machine.GetParam(address);

		/// <summary>
		/// 设置参数 
		/// </summary>
		public void SetParam(int address, VarValue value) => Machine.SetParam(address, value);
		/// <summary>
		/// 添加步骤 
		/// </summary>
		public void AddStep(StepExecuteData codeData) => Machine.AddStep(codeData);
		/// <summary>
		/// 获取步骤数量
		/// </summary>
		public int GetStepCount() => Machine.StepList.Count;
	}

	public static class StepProcessorRule
	{
		class Awake : AwakeRule<StepProcessor>
		{
			protected override void Execute(StepProcessor self)
			{
				self.GetParent(out self.Machine);
			}
		}
	}

}
