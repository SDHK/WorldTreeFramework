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
		public void Push(VarValue param) => Machine.Push(param);

		/// <summary>
		/// 弹出参数 
		/// </summary>
		public VarValue Pop() => Machine.Pop();

		/// <summary>
		/// 添加步骤 
		/// </summary>
		public void AddStep(StepData codeData)
		{
			Machine.AddStep(codeData);
		}
		/// <summary>
		/// 获取步骤数量
		/// </summary>
		public int GetStepCount()
		{
			return Machine.StepList.Count;
		}
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
