
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using VM;

/// <summary>
/// 简化版委托任务虚拟机
/// </summary>
public class SimpleTaskVM
{
	private static SimpleTaskVM _instance;
}


/// <summary>
/// 译码器 - 负责将高级任务转换为低级指令
/// </summary>
public class Compiler
{
	/// <summary>
	/// 源码
	/// </summary>
	public string Codes;



	/// <summary>
	/// 类型实例地址，字段名称，值
	/// </summary>
	Dictionary<int, Dictionary<string, ITuple>> ClassData = new();

	/// <summary>
	/// 地址名称
	/// </summary>
	Dictionary<int, string> ClassNames = new();


	/// <summary>
	/// 编译
	/// </summary>
	public void Compiled()
	{
		ITuple a = new Tuple<List<int>>(new());
	}

	public bool C(string code)
	{


		return false;
	}


	/// <summary>
	/// 执行器
	/// </summary>
	TaskExecutor taskExecutor;
}



#region 词法分析器

/// <summary>
/// 数学表达式令牌类型
/// </summary>
public enum TokenType
{
	/// <summary>
	/// 数字
	/// </summary>
	Number,
	/// <summary>
	/// 运算符 (+, -, *, /, ^)
	/// </summary>
	Operator,
	/// <summary>
	/// 左括号 (
	/// </summary>
	LeftParen,
	/// <summary>
	/// 右括号 )
	/// </summary>
	RightParen,
	/// <summary>
	/// 变量
	/// </summary>
	Variable,
	/// <summary>
	/// 函数 (sin, cos, sqrt等)
	/// </summary>
	Function,
	/// <summary>
	/// 结束符
	/// </summary>
	EOF
}


/// <summary>
/// 数学表达式令牌
/// </summary>
public class Token
{
	public TokenType Type { get; set; }
	public string Value { get; set; }
	public double NumericValue { get; set; }
	public Token(TokenType type, string value, double numericValue = 0)
	{
		Type = type;
		Value = value;
		NumericValue = numericValue;
	}
}

/// <summary>
/// 词法分析器
/// </summary>
public class MathTokenizer
{
	private readonly string _expression;
	private int _position;
	private char _currentChar;

	public MathTokenizer(string expression)
	{
		_expression = expression.Replace(" ", ""); // 移除空格
		_position = 0;
		_currentChar = _position < _expression.Length ? _expression[_position] : '\0';
	}

	/// <summary>
	/// 指针前进
	/// </summary>
	private void Advance()
	{
		_position++;
		_currentChar = _position < _expression.Length ? _expression[_position] : '\0';
	}

	/// <summary>
	/// 读取数字
	/// </summary>
	private double ReadNumber()
	{
		string number = "";
		while (char.IsDigit(_currentChar) || _currentChar == '.')
		{
			number += _currentChar;
			Advance();
		}
		return double.Parse(number, CultureInfo.InvariantCulture);
	}
	/// <summary>
	/// 读取标识符（变量或函数名）
	/// </summary>
	private string ReadIdentifier()
	{
		string identifier = "";
		while (char.IsLetter(_currentChar) || char.IsDigit(_currentChar) || _currentChar == '_' || _currentChar == '@')
		{
			identifier += _currentChar;
			Advance();
		}
		return identifier;
	}

	public Token GetNextToken()
	{
		while (_currentChar != '\0')
		{
			// 判断字符是否为数字
			if (char.IsDigit(_currentChar))
			{
				double number = ReadNumber();
				return new Token(TokenType.Number, number.ToString(CultureInfo.InvariantCulture), number);
			}
			// 判断字符是否为字母（变量或函数）
			if (char.IsLetter(_currentChar))
			{
				string identifier = ReadIdentifier();
				// 检查是否为函数
				if (IsMathFunction(identifier))
				{
					if (identifier.StartsWith("@")) identifier = identifier.Substring(1);
					return new Token(TokenType.Function, identifier);
				}
				// 否则视为变量
				else
				{
					return new Token(TokenType.Variable, identifier);
				}
			}

			switch (_currentChar)
			{
				case '+':
				case '-':
				case '*':
				case '/':
				case '^':
				case '%':
					char op = _currentChar;
					Advance();
					return new Token(TokenType.Operator, op.ToString());

				case '(':
					Advance();
					return new Token(TokenType.LeftParen, "(");

				case ')':
					Advance();
					return new Token(TokenType.RightParen, ")");

				default:
					throw new InvalidOperationException($"无效字符: {_currentChar}");
			}
		}

		return new Token(TokenType.EOF, "");
	}

	/// <summary>
	/// 判断是否为函数
	/// </summary>
	private bool IsMathFunction(string identifier) => identifier.StartsWith("@");
}


/// <summary>
/// 抽象语法树节点基类
/// </summary>
public abstract class ASTNode
{
	/// <summary>
	/// 计算节点值
	/// </summary>
	public abstract double Evaluate(Dictionary<string, double> variables = null);
}

/// <summary>
/// 数字节点
/// </summary>
public class NumberNode : ASTNode
{
	public double Value { get; }

	public NumberNode(double value)
	{
		Value = value;
	}

	public override double Evaluate(Dictionary<string, double> variables = null)
	{
		return Value;
	}
}

/// <summary>
/// 变量节点
/// </summary>
public class VariableNode : ASTNode
{
	public string Name { get; }

	public VariableNode(string name)
	{
		Name = name;
	}

	public override double Evaluate(Dictionary<string, double> variables = null)
	{
		if (variables != null && variables.TryGetValue(Name, out double value))
		{
			return value;
		}
		throw new InvalidOperationException($"未定义的变量: {Name}");
	}
}

/// <summary>
/// 二元运算符节点：例如1 + 2
/// </summary>
public class BinaryOperatorNode : ASTNode
{
	public ASTNode Left { get; }
	public ASTNode Right { get; }
	public string Operator { get; }

	public BinaryOperatorNode(ASTNode left, string op, ASTNode right)
	{
		Left = left;
		Operator = op;
		Right = right;
	}

	public override double Evaluate(Dictionary<string, double> variables = null)
	{
		double leftValue = Left.Evaluate(variables);
		double rightValue = Right.Evaluate(variables);

		return Operator switch
		{
			"+" => leftValue + rightValue,
			"-" => leftValue - rightValue,
			"*" => leftValue * rightValue,
			"/" => rightValue != 0 ? leftValue / rightValue : throw new DivideByZeroException(),
			"^" => Math.Pow(leftValue, rightValue),
			"%" => leftValue % rightValue,
			_ => throw new InvalidOperationException($"未知运算符: {Operator}")
		};
	}
}

/// <summary>
/// 一元运算符节点：例如:-1
/// </summary>
public class UnaryOperatorNode : ASTNode
{
	public ASTNode Operand { get; }
	public string Operator { get; }

	public UnaryOperatorNode(string op, ASTNode operand)
	{
		Operator = op;
		Operand = operand;
	}

	public override double Evaluate(Dictionary<string, double> variables = null)
	{
		double value = Operand.Evaluate(variables);

		return Operator switch
		{
			"+" => value,
			"-" => -value,
			_ => throw new InvalidOperationException($"未知一元运算符: {Operator}")
		};
	}
}

/// <summary>
/// 函数调用节点
/// </summary>
public class FunctionNode : ASTNode
{
	public string FunctionName { get; }
	public ASTNode Argument { get; }

	public FunctionNode(string functionName, ASTNode argument)
	{
		FunctionName = functionName;
		Argument = argument;
	}

	public override double Evaluate(Dictionary<string, double> variables = null)
	{
		double argValue = Argument.Evaluate(variables);

		return FunctionName.ToLower() switch
		{
			"sin" => Math.Sin(argValue),
			"cos" => Math.Cos(argValue),
			"tan" => Math.Tan(argValue),
			"sqrt" => Math.Sqrt(argValue),
			"log" => Math.Log10(argValue),
			"ln" => Math.Log(argValue),
			"abs" => Math.Abs(argValue),
			"floor" => Math.Floor(argValue),
			"ceil" => Math.Ceiling(argValue),
			"round" => Math.Round(argValue),
			_ => throw new InvalidOperationException($"未知函数: {FunctionName}")
		};
	}
}


#region 解析器

/// <summary>
/// 数学表达式解析器（递归下降解析）
/// </summary>
public class MathExpressionParser
{
	private MathTokenizer _tokenizer;
	private Token _currentToken;

	public ASTNode Parse(string expression)
	{
		_tokenizer = new MathTokenizer(expression);
		_currentToken = _tokenizer.GetNextToken();

		ASTNode result = ParseExpression();

		if (_currentToken.Type != TokenType.EOF) throw new InvalidOperationException("表达式解析不完整");
		return result;
	}

	/// <summary>
	/// 消费一个期望的令牌
	/// </summary>
	private void ConsumeToken(TokenType expectedType)
	{
		if (_currentToken.Type == expectedType)
		{
			_currentToken = _tokenizer.GetNextToken();
		}
		else
		{
			throw new InvalidOperationException($"期望 {expectedType}，但得到 {_currentToken.Type}");
		}
	}

	/// <summary>
	/// 解析表达式
	/// </summary>
	private ASTNode ParseExpression()
	{
		ASTNode node = ParseTerm();

		while (_currentToken.Type == TokenType.Operator &&
			   (_currentToken.Value == "+" || _currentToken.Value == "-"))
		{
			string op = _currentToken.Value;
			ConsumeToken(TokenType.Operator);
			ASTNode right = ParseTerm();
			node = new BinaryOperatorNode(node, op, right);
		}

		return node;
	}

	/// <summary>
	/// 解析乘除法和取模
	/// </summary>
	private ASTNode ParseTerm()
	{
		ASTNode node = ParseFactor();

		while (_currentToken.Type == TokenType.Operator &&
			   (_currentToken.Value == "*" || _currentToken.Value == "/" || _currentToken.Value == "%"))
		{
			string op = _currentToken.Value;
			ConsumeToken(TokenType.Operator);
			ASTNode right = ParseFactor();
			node = new BinaryOperatorNode(node, op, right);
		}

		return node;
	}

	/// <summary>
	/// 解析指数运算
	/// </summary>
	private ASTNode ParseFactor()
	{
		ASTNode node = ParsePower();

		while (_currentToken.Type == TokenType.Operator && _currentToken.Value == "^")
		{
			string op = _currentToken.Value;
			ConsumeToken(TokenType.Operator);
			ASTNode right = ParsePower(); // 右结合
			node = new BinaryOperatorNode(node, op, right);
		}

		return node;
	}

	/// <summary>
	/// 解析幂运算、一元运算符、函数调用、括号、数字和变量
	/// </summary>
	private ASTNode ParsePower()
	{
		// 处理一元运算符
		if (_currentToken.Type == TokenType.Operator &&
			(_currentToken.Value == "+" || _currentToken.Value == "-"))
		{
			string op = _currentToken.Value;
			ConsumeToken(TokenType.Operator);
			ASTNode operand = ParsePower();
			return new UnaryOperatorNode(op, operand);
		}

		// 处理函数调用，多参数函数考虑！！！！！！！！，多参数解析难度+++
		if (_currentToken.Type == TokenType.Function)
		{
			string functionName = _currentToken.Value;
			ConsumeToken(TokenType.Function);
			ConsumeToken(TokenType.LeftParen);
			ASTNode argument = ParseExpression();
			ConsumeToken(TokenType.RightParen);
			return new FunctionNode(functionName, argument);
		}

		// 处理括号
		if (_currentToken.Type == TokenType.LeftParen)
		{
			ConsumeToken(TokenType.LeftParen);
			ASTNode node = ParseExpression();
			ConsumeToken(TokenType.RightParen);
			return node;
		}

		// 处理数字
		if (_currentToken.Type == TokenType.Number)
		{
			double value = _currentToken.NumericValue;
			ConsumeToken(TokenType.Number);
			return new NumberNode(value);
		}

		// 处理变量
		if (_currentToken.Type == TokenType.Variable)
		{
			string name = _currentToken.Value;
			ConsumeToken(TokenType.Variable);
			return new VariableNode(name);
		}

		throw new InvalidOperationException($"意外的令牌: {_currentToken.Value}");
	}
}

#endregion

#endregion