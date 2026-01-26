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
		public ExprNode ast;

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
		private ExprNode ParseExpression()
		{
			var left = ParseTerm();

			while (currentToken.Type == CodeTokenType.Symbol)
			{
				char op = currentToken.Value.ToChar();
				if (op != '+' && op != '-') break;

				Advance();
				var right = ParseTerm();
				left = new BinaryOpNode { Left = left, Operator = op, Right = right };
			}

			return left;
		}

		/// <summary>
		/// 解析项 (处理 * /)
		/// </summary>
		private ExprNode ParseTerm()
		{
			var left = ParseFactor();

			while (currentToken.Type == CodeTokenType.Symbol)
			{
				char op = currentToken.Value.ToChar();
				if (op != '*' && op != '/') break;

				Advance();
				var right = ParseFactor();
				left = new BinaryOpNode { Left = left, Operator = op, Right = right };
			}

			return left;
		}

		/// <summary>
		/// 解析因子 (处理数字、括号、负号)
		/// </summary>
		private ExprNode ParseFactor()
		{
			// 处理负号
			if (currentToken.Type == CodeTokenType.Symbol && currentToken.Value.ToChar() == '-')
			{
				Advance();
				var operand = ParseFactor();
				return new UnaryOpNode { Operator = '-', Operand = operand };
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
				return new NumberNode { Value = value };
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
				//self.AddChild(out self.ast);
			}
		}
	}


	/// <summary>
	/// 表达式节点基类
	/// </summary>
	public abstract class ExprNode
	{
		/// <summary>
		/// 计算表达式值 
		/// </summary>
		public abstract double Evaluate();
	}

	/// <summary>
	/// 数字字面量节点
	/// </summary>
	public class NumberNode : ExprNode
	{
		/// <summary>
		/// 数字值 
		/// </summary>
		public double Value { get; set; }

		public override double Evaluate() => Value;

		public override string ToString() => Value.ToString();
	}

	/// <summary>
	/// 二元运算节点
	/// </summary>
	public class BinaryOpNode : ExprNode
	{
		/// <summary>
		/// 左操作数 
		/// </summary>
		public ExprNode Left { get; set; }
		/// <summary>
		/// 右操作数 
		/// </summary>
		public ExprNode Right { get; set; }
		/// <summary>
		/// 运算符 
		/// </summary>
		public char Operator { get; set; } // +, -, *, /

		public override double Evaluate()
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

	/// <summary>
	/// 一元运算节点 (负号)
	/// </summary>
	public class UnaryOpNode : ExprNode
	{
		/// <summary>
		/// 操作数 
		/// </summary>
		public ExprNode Operand { get; set; }
		/// <summary>
		/// 运算符 
		/// </summary>
		public char Operator { get; set; } // -

		public override double Evaluate()
		{
			return Operator == '-' ? -Operand.Evaluate() : Operand.Evaluate();
		}
		public override string ToString() => $"{Operator}{Operand}";
	}
}
