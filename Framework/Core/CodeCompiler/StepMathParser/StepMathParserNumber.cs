namespace WorldTree
{
	/// <summary>
	/// 数字字面量节点
	/// </summary>
	public class StepMathParserNumber : Node, IStepCodeGenerator
		, ChildOf<StepMathParser>
		, AsRule<Awake>
	{
		/// <summary>
		/// 数字值 
		/// </summary>
		public VarValue Value { get; set; }

		/// <summary>
		/// 生成代码：压入字面量
		/// </summary>
		public void GenerateCode(UnitList<StepAssemblyData> output)
		{
			output.Add(new StepAssemblyData(StepOpCode.PushLiteral, Value));
		}

		public override string ToString() => Value.ToString();
	}
}
