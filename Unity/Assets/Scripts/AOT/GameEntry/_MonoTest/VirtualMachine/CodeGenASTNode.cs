using System;
using System.Collections.Generic;

namespace VM
{

	#region 虚拟机指令生成的AST节点

	/// <summary>
	/// 可生成虚拟机指令的AST节点基类 : Abstract Syntax Tree 抽象语法树
	/// </summary>
	public abstract class CodeASTNode
	{
		/// <summary>
		/// 生成虚拟机指令
		/// </summary>
		/// <param name="generator">指令生成器</param>
		public abstract void GenerateCode(VMCodeGenerator generator);
	}

	/// <summary>
	/// 数字节点
	/// </summary>
	public class CodeNumberNode : CodeASTNode
	{
		public double Value { get; }

		public CodeNumberNode(double value)
		{
			Value = value;
		}

		public override void GenerateCode(VMCodeGenerator generator)
		{
			generator.Emit(VMInstruction.LOAD_CONST, Value);
		}
	}

	/// <summary>
	/// 变量节点
	/// </summary>
	public class CodeVariableNode : CodeASTNode
	{
		public string Name { get; }

		public CodeVariableNode(string name)
		{
			Name = name;
		}

		public override void GenerateCode(VMCodeGenerator generator)
		{
			generator.Emit(VMInstruction.LOAD_VAR, Name);
		}
	}

	/// <summary>
	/// 二元运算符节点
	/// </summary>
	public class CodeBinaryOpNode : CodeASTNode
	{
		public CodeASTNode Left { get; }
		public CodeASTNode Right { get; }
		public string Operator { get; }

		public CodeBinaryOpNode(CodeASTNode left, string op, CodeASTNode right)
		{
			Left = left;
			Operator = op;
			Right = right;
		}

		public override void GenerateCode(VMCodeGenerator generator)
		{
			// 生成左操作数指令
			Left.GenerateCode(generator);
			// 生成右操作数指令
			Right.GenerateCode(generator);

			// 生成运算指令
			VMInstruction instruction = Operator switch
			{
				"+" => VMInstruction.ADD,
				"-" => VMInstruction.SUB,
				"*" => VMInstruction.MUL,
				"/" => VMInstruction.DIV,
				"%" => VMInstruction.MOD,
				"^" => VMInstruction.POW,
				"==" => VMInstruction.EQ,
				"!=" => VMInstruction.NE,
				"<" => VMInstruction.LT,
				"<=" => VMInstruction.LE,
				">" => VMInstruction.GT,
				">=" => VMInstruction.GE,
				"&&" => VMInstruction.AND,
				"||" => VMInstruction.OR,
				_ => throw new InvalidOperationException($"未知运算符: {Operator}")
			};

			generator.Emit(instruction);
		}
	}

	/// <summary>
	/// 赋值节点（指令生成版）
	/// </summary>
	public class CodeAssignmentNode : CodeASTNode
	{
		public string VariableName { get; }
		public CodeASTNode Expression { get; }

		public CodeAssignmentNode(string variableName, CodeASTNode expression)
		{
			VariableName = variableName;
			Expression = expression;
		}

		public override void GenerateCode(VMCodeGenerator generator)
		{
			// 生成表达式指令
			Expression.GenerateCode(generator);
			// 生成存储指令
			generator.Emit(VMInstruction.STORE_VAR, VariableName);
		}
	}

	/// <summary>
	/// IF语句节点
	/// </summary>
	public class CodeIfNode : CodeASTNode
	{
		/// <summary>
		/// 条件表达式 
		/// </summary>
		public CodeASTNode Condition { get; }

		public CodeASTNode ThenBranch { get; }
		public CodeASTNode ElseBranch { get; }

		public CodeIfNode(CodeASTNode condition, CodeASTNode thenBranch, CodeASTNode elseBranch = null)
		{
			Condition = condition;
			ThenBranch = thenBranch;
			ElseBranch = elseBranch;
		}

		public override void GenerateCode(VMCodeGenerator generator)
		{
			// 生成条件表达式
			Condition.GenerateCode(generator);

			// 条件跳转：如果为假，跳转到else分支或结束
			int elseLabel = generator.AllocateLabel();
			generator.Emit(VMInstruction.JMP_IF_FALSE, elseLabel);

			// 生成then分支
			ThenBranch.GenerateCode(generator);

			if (ElseBranch != null)
			{
				// 跳过else分支
				int endLabel = generator.AllocateLabel();
				generator.Emit(VMInstruction.JMP, endLabel);

				// else分支开始
				generator.SetLabel(elseLabel);
				ElseBranch.GenerateCode(generator);

				// 设置结束标签
				generator.SetLabel(endLabel);
			}
			else
			{
				// 设置else标签（实际上是结束）
				generator.SetLabel(elseLabel);
			}
		}
	}

	/// <summary>
	/// 语句块节点
	/// </summary>
	public class CodeBlockNode : CodeASTNode
	{
		public List<CodeASTNode> Statements { get; } = new List<CodeASTNode>();

		public void AddStatement(CodeASTNode statement)
		{
			Statements.Add(statement);
		}

		public override void GenerateCode(VMCodeGenerator generator)
		{
			foreach (var statement in Statements)
			{
				statement.GenerateCode(generator);
			}
		}
	}

	/// <summary>
	/// 函数调用节点（支持多参数）
	/// </summary>
	public class CodeFunctionNode : CodeASTNode
	{
		public string FunctionName { get; }
		public List<CodeASTNode> Arguments { get; }

		public CodeFunctionNode(string functionName, List<CodeASTNode> arguments)
		{
			FunctionName = functionName;
			Arguments = arguments ?? new List<CodeASTNode>();
		}

		public override void GenerateCode(VMCodeGenerator generator)
		{
			// 先将参数推入栈（逆序，因为栈是后进先出）
			for (int i = Arguments.Count - 1; i >= 0; i--)
			{
				Arguments[i].GenerateCode(generator);
			}

			// 调用函数，参数个数作为操作数
			generator.Emit(VMInstruction.CALL_FUNC, new { Name = FunctionName, ArgCount = Arguments.Count });
		}
	}

	#endregion

	/// <summary>
	/// 代码解析器
	/// </summary>
	public class CodeParser
	{
		private CodeTokenizer tokenizer;
		private CodeToken _currentToken;


		public ASTNode Parse(string expression)
		{
			tokenizer = new CodeTokenizer(expression);
			_currentToken = tokenizer.GetNextToken();

			//ASTNode result = ProgramTokenizer();

			//if (_currentToken.Type != TokenType.EOF) throw new InvalidOperationException("表达式解析不完整");
			return null;
		}

	}


}
