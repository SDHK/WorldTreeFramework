using System;

namespace WorldTree
{
	/// <summary>
	/// 步骤数学表达式解析器
	/// </summary>
	public class StepMathParser : Node
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
		public IStepCodeGenerator ast;

		/// <summary>
		/// 解析数学表达式并生成步骤汇编数据
		/// </summary>
		public void Parse(string expression, UnitList<StepAssemblyData> output)
		{
			// 1. 初始化词法分析器
			codeTokenizer.SetCode(expression);

			// 2. 获取第一个有效令牌（跳过空白）
			Advance();

			// 3. 语法分析
			ast = ParseExpression();

			// 4. 生成代码
			ast.GenerateCode(output);
		}

		/// <summary>
		/// 解析表达式 (处理 + -)
		/// </summary>
		private IStepCodeGenerator ParseExpression()
		{
			var left = ParseTerm();

			while (currentToken.Type == CodeTokenType.Symbol)
			{
				char op = currentToken.Value.ToChar();
				if (op != '+' && op != '-') break;

				Advance();
				var right = ParseTerm();
				this.AddChild(out StepMathParserBinaryOp newLeft);
				newLeft.Left = left;
				newLeft.Operator = op;
				newLeft.Right = right;
				left = newLeft;
			}

			return left;
		}

		/// <summary>
		/// 解析项 (处理 * / %)
		/// </summary>
		private IStepCodeGenerator ParseTerm()
		{
			var left = ParseFactor();

			while (currentToken.Type == CodeTokenType.Symbol)
			{
				char op = currentToken.Value.ToChar();
				if (op != '*' && op != '/' && op != '%') break;

				Advance();
				var right = ParseFactor();
				this.AddChild(out StepMathParserBinaryOp newLeft);
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
		private IStepCodeGenerator ParseFactor()
		{
			// 处理负号
			if (currentToken.Type == CodeTokenType.Symbol && currentToken.Value.ToChar() == '-')
			{
				Advance();
				var operand = ParseFactor();
				this.AddChild(out StepMathParserUnaryOp unaryNode);
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
				VarValue value = currentToken.Value;
				Advance();
				this.AddChild(out StepMathParserNumber numberNode);
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

	public static class StepMathParserRule
	{
		class AwakeRule : AwakeRule<StepMathParser>
		{
			protected override void Execute(StepMathParser self)
			{
				self.AddChild(out self.codeTokenizer);
			}
		}
	}
}
