namespace WorldTree
{
	/// <summary>
	/// 表达式节点接口
	/// </summary>
	public interface IMathParserSyntaxTree
	{
		/// <summary>
		/// 计算表达式值 
		/// </summary>
		public abstract double Evaluate();
	}
}
