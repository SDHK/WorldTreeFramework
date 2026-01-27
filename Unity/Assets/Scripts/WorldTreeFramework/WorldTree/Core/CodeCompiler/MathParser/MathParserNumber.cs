namespace WorldTree
{
	/// <summary>
	/// 数字字面量节点
	/// </summary>
	public class MathParserNumber : Node, IMathParserSyntaxTree
		, ChildOf<MathParser>
		, AsRule<Awake>
	{
		/// <summary>
		/// 数字值 
		/// </summary>
		public double Value { get; set; }

		public double Evaluate() => Value;

		public override string ToString() => Value.ToString();
	}
}
