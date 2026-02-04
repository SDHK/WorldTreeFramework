using System;

namespace WorldTree
{
	/// <summary>
	/// 一元运算节点 (负号)
	/// </summary>
	public class StepMathParserUnaryOp : Node, IStepCodeGenerator
		, ChildOf<StepMathParser>
		, AsRule<Awake>
	{
		/// <summary>
		/// 操作数 
		/// </summary>
		public IStepCodeGenerator Operand { get; set; }

		/// <summary>
		/// 运算符 
		/// </summary>
		public char Operator { get; set; } // -

		/// <summary>
		/// 生成代码：先生成操作数，然后应用负号
		/// <para>负号实现为：0 - operand 或使用单独的 Neg 指令</para>
		/// </summary>
		public void GenerateCode(UnitList<StepAssemblyData> output)
		{
			if (Operator == '-')
			{
				// 方案1: 使用 0 - operand
				output.Add(new StepAssemblyData(StepOpCode.PushLiteral, (VarValue)0L));
				Operand.GenerateCode(output);
				output.Add(new StepAssemblyData(StepOpCode.Sub));
			}
			else
			{
				throw new InvalidOperationException($"未知一元运算符: {Operator}");
			}
		}

		public override string ToString() => $"{Operator}{Operand}";
	}
}
