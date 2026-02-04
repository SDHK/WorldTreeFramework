using System;

namespace WorldTree
{
	/// <summary>
	/// 二元运算节点
	/// </summary>
	public class StepMathParserBinaryOp : Node, IStepCodeGenerator
		, ChildOf<StepMathParser>
		, AsRule<Awake>
	{
		/// <summary>
		/// 左操作数 
		/// </summary>
		public IStepCodeGenerator Left { get; set; }

		/// <summary>
		/// 右操作数 
		/// </summary>
		public IStepCodeGenerator Right { get; set; }

		/// <summary>
		/// 运算符 
		/// </summary>
		public char Operator { get; set; } // +, -, *, /, %

		/// <summary>
		/// 生成代码：后序遍历，先左后右最后操作符
		/// </summary>
		public void GenerateCode(UnitList<StepAssemblyData> output)
		{
			// 1. 生成左操作数代码
			Left.GenerateCode(output);

			// 2. 生成右操作数代码
			Right.GenerateCode(output);

			// 3. 生成运算符代码
			StepOpCode opCode = Operator switch
			{
				'+' => StepOpCode.Add,
				'-' => StepOpCode.Sub,
				'*' => StepOpCode.Mul,
				'/' => StepOpCode.Div,
				'%' => StepOpCode.Mod,
				_ => throw new InvalidOperationException($"未知运算符: {Operator}")
			};

			output.Add(new StepAssemblyData(opCode));
		}

		public override string ToString() => $"({Left} {Operator} {Right})";
	}
}
