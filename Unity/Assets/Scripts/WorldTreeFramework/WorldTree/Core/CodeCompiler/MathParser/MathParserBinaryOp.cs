using System;

namespace WorldTree
{
	/// <summary>
	/// 二元运算节点
	/// </summary>
	public class MathParserBinaryOp : Node, IMathParserSyntaxTree
		, ChildOf<MathParser>
		, AsRule<Awake>
	{
		/// <summary>
		/// 左操作数 
		/// </summary>
		public IMathParserSyntaxTree Left { get; set; }
		/// <summary>
		/// 右操作数 
		/// </summary>
		public IMathParserSyntaxTree Right { get; set; }
		/// <summary>
		/// 运算符 
		/// </summary>
		public char Operator { get; set; } // +, -, *, /

		public double Evaluate()
		{
			double left = Left.Evaluate();
			double right = Right.Evaluate();

			return Operator switch
			{
				'+' => left + right,
				'-' => left - right,
				'*' => left * right,
				'/' => right != 0 ? left / right : throw new DivideByZeroException(),
				_ => throw new InvalidOperationException($"未知运算符: {Operator}")
			};
		}

		public override string ToString() => $"({Left} {Operator} {Right})";
	}
}
