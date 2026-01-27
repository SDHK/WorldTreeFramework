using System;

namespace WorldTree
{
	/// <summary>
	/// 数学表达式解析器
	/// </summary>
	public class MathParser : Node
		, ChildOf<INode>
		, AsChildBranch
		, AsRule<Awake>
	{
		/// <summary>
		/// 当前令牌 
		/// </summary>
		private CodeToken currentToken;

		/// <summary>
		/// 代码词法分析器 
		/// </summary>
		public CodeTokenizer codeTokenizer;

		/// <summary>
		/// 抽象语法树根节点 
		/// </summary>
		public IMathParserSyntaxTree ast;

		/// <summary>
		/// 解析并计算数学表达式的值 
		/// </summary>
		public double Parse(string expression)
		{
			// 1. 初始化词法分析器
			codeTokenizer.SetCode(expression);

			// 2. 获取第一个有效令牌（跳过空白）
			Advance();

			// 3. 语法分析 + 求值
			ast = ParseExpression();

			// 4. 执行求值
			return ast.Evaluate();
		}

		/// <summary>
		/// 解析表达式 (处理 + -)
		/// </summary>
		private IMathParserSyntaxTree ParseExpression()
		{
			var left = ParseTerm();

			while (currentToken.Type == CodeTokenType.Symbol)
			{
				char op = currentToken.Value.ToChar();
				if (op != '+' && op != '-') break;

				Advance();
				var right = ParseTerm();
				this.AddChild(out MathParserBinaryOp newLeft);
				newLeft.Left = left;
				newLeft.Operator = op;
				newLeft.Right = right;
				left = newLeft;
			}

			return left;
		}

		/// <summary>
		/// 解析项 (处理 * /)
		/// </summary>
		private IMathParserSyntaxTree ParseTerm()
		{
			var left = ParseFactor();

			while (currentToken.Type == CodeTokenType.Symbol)
			{
				char op = currentToken.Value.ToChar();
				if (op != '*' && op != '/') break;

				Advance();
				var right = ParseFactor();
				this.AddChild(out MathParserBinaryOp newLeft);
				newLeft.Left = left;
				newLeft.Operator = op;
				newLeft.Right = right;
				left = newLeft;
			}

			return left;
		}

		/// <summary>
		/// 解析因子 (处理数字、括号、负号)
		/// </summary>
		private IMathParserSyntaxTree ParseFactor()
		{
			// 处理负号
			if (currentToken.Type == CodeTokenType.Symbol && currentToken.Value.ToChar() == '-')
			{
				Advance();
				var operand = ParseFactor();
				this.AddChild(out MathParserUnaryOp unaryNode);
				unaryNode.Operator = '-';
				unaryNode.Operand = operand;
				return unaryNode;
			}

			// 处理括号
			if (currentToken.Type == CodeTokenType.Symbol && currentToken.Value.ToChar() == '(')
			{
				Advance(); // 消费 '('
				var expr = ParseExpression();

				if (currentToken.Type != CodeTokenType.Symbol || currentToken.Value.ToChar() != ')')
					throw new InvalidOperationException("缺少右括号");

				Advance(); // 消费 ')'
				return expr;
			}

			// 处理数字
			if (currentToken.Type == CodeTokenType.Number)
			{
				double value = currentToken.Value.Type switch
				{
					VarType.Long => (long)currentToken.Value,
					VarType.Double => (double)currentToken.Value,
					_ => throw new InvalidOperationException("不支持的数字类型")
				};

				Advance();
				this.AddChild(out MathParserNumber numberNode);
				numberNode.Value = value;
				return numberNode;
			}

			throw new InvalidOperationException($"意外的令牌: {currentToken.Type}");
		}
		/// <summary>
		/// 推进到下一个令牌（跳过空白符）
		/// </summary>
		private void Advance()
		{
			do
			{
				currentToken = codeTokenizer.GetNextToken();
			}
			while (currentToken.Type == CodeTokenType.Whitespace);
		}
	}

	public static class MathParserRule
	{
		class AwakeRule : AwakeRule<MathParser>
		{
			protected override void Execute(MathParser self)
			{
				self.AddChild(out self.codeTokenizer);
			}
		}
	}
}
