namespace WorldTree
{
	/// <summary>
	/// 步骤代码生成器接口
	/// </summary>
	public interface IStepCodeGenerator
	{
		/// <summary>
		/// 生成步骤汇编数据
		/// </summary>
		void GenerateCode(UnitList<StepAssemblyData> output);
	}
}
