namespace WorldTree
{
	/// <summary>
	/// 一元运算节点 (负号)
	/// </summary>
	public class MathParserUnaryOp : Node, IMathParserSyntaxTree
		, ChildOf<MathParser>
		, AsRule<Awake>
	{
		/// <summary>
		/// 操作数 
		/// </summary>
		public IMathParserSyntaxTree Operand { get; set; }
		/// <summary>
		/// 运算符 
		/// </summary>
		public char Operator { get; set; } // -

		public double Evaluate()
		{
			return Operator == '-' ? -Operand.Evaluate() : Operand.Evaluate();
		}
		public override string ToString() => $"{Operator}{Operand}";
	}
}
