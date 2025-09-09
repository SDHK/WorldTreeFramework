/****************************************

* 作者： 闪电黑客
* 日期： 2025/9/3 11:10

* 描述： 虚拟机汇编语法解析器
* 
* 作用是将汇编代码字符串解析为一系列的指令和操作数，供虚拟机执行

*/
using System;
using System.Collections.Generic;
using UnityEngine;


namespace VM
{
	/// <summary>
	/// 虚拟机汇编语法解析器
	/// </summary>
	public class AssemblySyntaxParser
	{
		/// <summary>
		/// 虚拟机
		/// </summary>
		private TaskExecutor executor;

		/// <summary>
		/// 词法令牌分析器
		/// </summary>
		public CodeTokenizer Tokenizer;

		/// <summary>
		/// 当前令牌 
		/// </summary>
		private CodeToken currentToken;

		/// <summary>
		/// 解析汇编代码
		/// </summary>
		public void Parse(TaskExecutor executor, string expression)
		{
			this.executor = executor;
			Tokenizer = new CodeTokenizer(expression);
			currentToken = Tokenizer.GetNextToken();
			JumpWhiteSpaceAndLineBreak();
			while (currentToken.Type != CodeTokenType.EOF)
			{
				if (currentToken.Type == CodeTokenType.Identifier)
				{
					string instruction = (string)currentToken.Value;
					ConsumeToken(CodeTokenType.Identifier);
					JumpWhiteSpaceAndLineBreak();
					switch (instruction.ToUpper())
					{
						case "SET":
							ParserSet();
							break;
						case "IF":
							ParserIFEvent();
							break;
						case "ELSE":
							ParserIFElse();
							break;
						case "IF_END":
							ParserIFEnd();
							break;

						case "LOOP":
							ParserLoopEnter();
							break;
						case "LOOP_END":
							ParserLoopEnd();
							break;
						case "LOOP_END_DO":
							ParserLoopEndDo();
							break;
						case "DELAY":
							ParserDelay();
							break;

						case "FUNC_DEF":
							ParserMethodDefine();
							break;
						case "FUNC_END":
							ParserMethodEnd();
							break;
						case "FUNC_CALL":
							ParserMethodCall();
							break;
						case "FUNC_RUN":
							ParserMethodRun();
							break;

						case "PUSH":
							ParserPushValue();
							break;
						case "POP":
							ParserPopVariable();
							break;

						case "ADD":
							ParserAdd();
							break;
						case "SUB":
							ParserSub();
							break;
						case "MUL":
							ParserMul();
							break;
						case "DIV":
							ParserDiv();
							break;

						case "AND":
							ParserAnd();
							break;
						case "OR":
							ParserOr();
							break;
						case "NOT":
							ParserNot();
							break;
						case "XOR":
							ParserXor();
							break;
						case "EQUAL":
							ParserEqual();
							break;
						case "NOT_EQUAL":
							ParserNotEqual();
							break;
						case "GREATER":
							ParserGreaterThan();
							break;
						case "LESS":
							ParserLessThan();
							break;
						case "GREATER_EQUAL":
							ParserGreaterEqual();
							break;
						case "LESS_EQUAL":
							ParserLessEqual();
							break;

						case "PRINT":
							ParserPrint();
							break;
						default:
							throw new InvalidOperationException($"未知指令: {instruction}");
					}
				}
				// 支持行内注释，以 # 开头和结尾的内容视为注释
				else if (currentToken.Type == CodeTokenType.Symbol)
				{
					string instruction = (string)currentToken.Value;
					ConsumeToken(CodeTokenType.Symbol);
					if (instruction == "#")
					{
						while (currentToken.Type != CodeTokenType.EOF)
						{
							if (currentToken.Type == CodeTokenType.Symbol && currentToken.Value.ToString() == "#")
							{
								ConsumeToken(CodeTokenType.Symbol);
								break;
							}
							currentToken = Tokenizer.GetNextToken();
						}
					}
					else if (instruction == "/")
					{
						instruction = (string)currentToken.Value;
						ConsumeToken(CodeTokenType.Symbol);
						if (instruction == "/")
						{
							while (currentToken.Type != CodeTokenType.EOF && currentToken.Type != CodeTokenType.LineBreak)
							{
								currentToken = Tokenizer.GetNextToken();
							}
						}
						else
						{
							throw new InvalidOperationException($"意外的符号: {instruction}");
						}
					}
					else
					{
						throw new InvalidOperationException($"意外的符号: {instruction}");
					}
				}
				else
				{
					throw new InvalidOperationException($"意外的令牌: {currentToken.Type}");
				}
				JumpWhiteSpaceAndLineBreak();
			}
		}

		/// <summary>
		/// 消费一个期望的令牌
		/// </summary>
		private void ConsumeToken(CodeTokenType expectedType)
		{
			if (currentToken.Type == expectedType)
			{
				currentToken = Tokenizer.GetNextToken();
			}
			else
			{
				throw new InvalidOperationException($"期望 {expectedType}，但得到 {currentToken.Type}");
			}
		}

		/// <summary>
		/// 跳过所有空白符和换行符 
		/// </summary>
		private void JumpWhiteSpaceAndLineBreak()
		{
			while (currentToken.Type is CodeTokenType.Whitespace or CodeTokenType.LineBreak)
			{
				currentToken = Tokenizer.GetNextToken();
			}
		}

		/// <summary>
		/// 跳过所有空白符 
		/// </summary>
		private void JumpWhiteSpace()
		{
			while (currentToken.Type == CodeTokenType.Whitespace)
			{
				currentToken = Tokenizer.GetNextToken();
			}
		}


		#region 指令解析

		/// <summary>
		/// 获取一个变量值（数字或字符串） 
		/// </summary>
		public VarValue ParserGetVarValue()
		{
			JumpWhiteSpace();
			// 解析字符串字面量（用双引号包裹）
			if (currentToken.Type == CodeTokenType.Symbol && currentToken.Value.ToString() == "\"")
			{
				ConsumeToken(CodeTokenType.Symbol); // 消费开始的双引号

				string stringValue = "";
				while (currentToken.Type != CodeTokenType.EOF)
				{
					if (currentToken.Type == CodeTokenType.Symbol && currentToken.Value.ToString() == "\"")
					{
						ConsumeToken(CodeTokenType.Symbol); // 消费结束的双引号
						return new VarValue { Type = VarType.String, ObjectValue = stringValue };
					}

					// 收集字符串内容（可能包含标识符、数字、符号等）
					stringValue += currentToken.Value.ToString();
					currentToken = Tokenizer.GetNextToken();
				}

				throw new InvalidOperationException("字符串字面量缺少结束的双引号");
			}

			// 解析数字字面量
			if (currentToken.Type == CodeTokenType.Number)
			{
				var numberValue = currentToken.Value;
				ConsumeToken(CodeTokenType.Number);
				return numberValue;
			}

			// 解析标识符（可能是变量名、布尔值或其他字符串内容）
			if (currentToken.Type == CodeTokenType.Identifier)
			{
				var identifierValue = currentToken.Value;
				string identifierString = identifierValue.ToString().ToLower();

				// 检查是否为布尔值字面量
				if (identifierString == "true")
				{
					ConsumeToken(CodeTokenType.Identifier);
					return new VarValue { Type = VarType.Bool, BoolValue = true };
				}
				else if (identifierString == "false")
				{
					ConsumeToken(CodeTokenType.Identifier);
					return new VarValue { Type = VarType.Bool, BoolValue = false };
				}

				// 否则作为变量名称处理
				ConsumeToken(CodeTokenType.Identifier);
				identifierValue.Type = VarType.Object;
				return identifierValue;
			}
			throw new InvalidOperationException($"期望数字或字符串字面量，但得到 {currentToken.Type}");
		}

		/// <summary>
		/// 解析 Move 指令  
		/// </summary>
		public void ParserSet()
		{
			GetString2(out string varName, out var var1);
			executor.Event(() =>
			{
				SetResult(varName, ResolveOperand(var1));
			});
		}

		/// <summary>
		/// 解析 IF 指令 
		/// </summary>
		public void ParserIFEvent()
		{
			var var1 = ParserGetVarValue();
			executor.IF_Event(() => (bool)ResolveOperand(var1));
		}

		/// <summary>
		/// 解析 ELSE 指令 
		/// </summary>
		public void ParserIFElse() => executor.IF_Else();

		/// <summary>
		/// 解析 ENDIF 指令 
		/// </summary>
		public void ParserIFEnd() => executor.IF_End();

		/// <summary>
		/// 解析 LOOP 指令 
		/// </summary>
		public void ParserLoopEnter()
		{
			var var1 = ParserGetVarValue();
			executor.Loop_Enter(() => (bool)ResolveOperand(var1));
		}

		/// <summary>
		/// 解析 LOOP_END 指令 
		/// </summary>
		public void ParserLoopEnd() => executor.Loop_End();

		/// <summary>
		/// 解析 LOOP_END_DO 指令 
		/// </summary>
		public void ParserLoopEndDo()
		{
			var var1 = ParserGetVarValue();
			executor.Loop_End(() => (bool)ResolveOperand(var1));
		}

		/// <summary>
		/// 解析 DELAY 指令 
		/// </summary>
		public void ParserDelay()
		{
			executor.Delay((int)ResolveOperand(ParserGetVarValue()));
		}

		/// <summary>
		/// 解析 Method 定义指令 
		/// </summary>
		public void ParserMethodDefine()
		{
			if (currentToken.Type == CodeTokenType.Identifier)
			{
				string methodName = (string)currentToken.Value;
				ConsumeToken(CodeTokenType.Identifier);

				List<string> parameters = new List<string>();
				while (currentToken.Type != CodeTokenType.LineBreak)
				{
					JumpWhiteSpace();
					if (currentToken.Type == CodeTokenType.Identifier)
					{
						string paramName = (string)currentToken.Value;
						parameters.Add(paramName);
						ConsumeToken(CodeTokenType.Identifier);
					}
					else if (currentToken.Type != CodeTokenType.LineBreak &&
					currentToken.Type != CodeTokenType.EOF)
					{
						throw new InvalidOperationException("参数列表中缺少参数名");
					}
				}
				executor.MethodDefine(methodName, parameters.ToArray());
			}
		}

		/// <summary>
		/// 解析 Method 结束指令 
		/// </summary>
		public void ParserMethodEnd()
		{
			executor.MethodEnd();
		}

		/// <summary>
		/// 解析 Method 调用指令 
		/// </summary>
		public void ParserMethodCall()
		{
			if (currentToken.Type == CodeTokenType.Identifier)
			{
				string methodName = (string)currentToken.Value;
				ConsumeToken(CodeTokenType.Identifier);
				executor.MethodCall(methodName);
			}
			else
			{
				throw new InvalidOperationException("FUNC_CALL 需要一个函数名");
			}
		}

		/// <summary>
		/// 解析 Method 运行指令 
		/// </summary>
		public void ParserMethodRun()
		{
			if (currentToken.Type == CodeTokenType.Identifier)
			{
				string methodName = (string)currentToken.Value;
				ConsumeToken(CodeTokenType.Identifier);
				executor.MethodRun(methodName);
			}
			else
			{
				throw new InvalidOperationException("FUNC_RUN 需要一个函数名");
			}
		}

		/// <summary>
		/// 解析 Push 值指令 
		/// </summary>
		public void ParserPushValue()
		{
			var value = ParserGetVarValue();
			if (value.Type == VarType.Object)
			{
				executor.PushVariable((string)value);
			}
			else
			{
				executor.PushValue(value);
			}
		}

		/// <summary>
		/// 解析 Push 变量指令 
		/// </summary>
		public void ParserPushVariable()
		{
			if (currentToken.Type == CodeTokenType.Identifier)
			{
				string varName = (string)currentToken.Value;
				ConsumeToken(CodeTokenType.Identifier);
				executor.PushVariable(varName);
			}
			else
			{
				throw new InvalidOperationException("PUSH 需要一个变量名");
			}
		}

		/// <summary>
		/// 解析 Pop 变量指令 
		/// </summary>
		public void ParserPopVariable()
		{
			if (currentToken.Type == CodeTokenType.Identifier)
			{
				string varName = (string)currentToken.Value;
				ConsumeToken(CodeTokenType.Identifier);
				executor.PopVariable(varName);
			}
			else
			{
				throw new InvalidOperationException("POP 需要一个变量名");
			}
		}

		#region 算术运算


		/// <summary>
		/// 获取三个值
		/// </summary>
		public void GetString3(out string varName, out VarValue var1, out VarValue var2)
		{
			if (currentToken.Type == CodeTokenType.Identifier)
			{
				varName = (string)currentToken.Value;
				ConsumeToken(CodeTokenType.Identifier);
				JumpWhiteSpaceAndLineBreak();
				var1 = ParserGetVarValue();
				var2 = ParserGetVarValue();
			}
			else
			{
				throw new InvalidOperationException("需要一个目标变量名");
			}
		}

		/// <summary>
		/// 获取两个值
		/// </summary>
		public void GetString2(out string varName, out VarValue var1)
		{
			if (currentToken.Type == CodeTokenType.Identifier)
			{
				varName = (string)currentToken.Value;
				ConsumeToken(CodeTokenType.Identifier);
				JumpWhiteSpaceAndLineBreak();
				var1 = ParserGetVarValue();
			}
			else
			{
				throw new InvalidOperationException("需要一个目标变量名");
			}
		}

		/// <summary>
		/// 解析操作数值（支持字面量、变量、POP）
		/// </summary>
		private VarValue ResolveOperand(VarValue operand)
		{
			if (operand.Type == VarType.Object)
			{
				string operandStr = (string)operand;
				return operandStr == "POP" ? executor.Pop() : executor.GetVariable(operandStr);
			}
			return operand;
		}

		/// <summary>
		/// 设置结果（支持变量或PUSH）
		/// </summary>
		private void SetResult(string target, VarValue value)
		{
			if (target == "PUSH")
			{
				executor.Push(value);
			}
			else
			{
				executor.SetVariable(target, value);
			}
		}


		/// <summary>
		/// 解析 ADD 指令，变量相加 
		/// </summary>
		public void ParserAdd()
		{
			GetString3(out var varName, out var var1, out var var2);
			executor.Event(() =>
			{
				SetResult(varName, ResolveOperand(var1) + ResolveOperand(var2));
			});
		}

		/// <summary>
		/// 解析 SUB 指令，变量相减 
		/// </summary>
		public void ParserSub()
		{
			GetString3(out var varName, out var var1, out var var2);
			executor.Event(() =>
			{
				SetResult(varName, ResolveOperand(var1) - ResolveOperand(var2));
			});
		}

		/// <summary>
		/// 解析 MUL 指令，变量相乘 
		/// </summary>
		public void ParserMul()
		{
			GetString3(out var varName, out var var1, out var var2);
			executor.Event(() =>
			{
				SetResult(varName, ResolveOperand(var1) * ResolveOperand(var2));
			});
		}

		/// <summary>
		/// 解析 DIV 指令，变量相除 
		/// </summary>
		public void ParserDiv()
		{
			GetString3(out var varName, out var var1, out var var2);
			executor.Event(() =>
			{
				SetResult(varName, ResolveOperand(var1) / ResolveOperand(var2));
			});
		}

		#endregion

		#region 逻辑运算


		/// <summary>
		/// 解析 AND 指令，逻辑与运算
		/// </summary>
		public void ParserAnd()
		{
			GetString3(out var varName, out var var1, out var var2);
			executor.Event(() =>
			{
				SetResult(varName, (ResolveOperand(var1).ToBool() && ResolveOperand(var2).ToBool()));
			});
		}

		/// <summary>
		/// 解析 OR 指令，逻辑或运算
		/// </summary>
		public void ParserOr()
		{
			GetString3(out var varName, out var var1, out var var2);
			executor.Event(() =>
			{
				SetResult(varName, (ResolveOperand(var1).ToBool() || ResolveOperand(var2).ToBool()));
			});
		}

		public void ParserXor()
		{
			GetString3(out var varName, out var var1, out var var2);
			executor.Event(() =>
			{
				SetResult(varName, (ResolveOperand(var1).ToBool() ^ ResolveOperand(var2).ToBool()));
			});
		}

		/// <summary>
		/// 解析 NOT 指令，逻辑非运算
		/// </summary>
		public void ParserNot()
		{
			if (currentToken.Type == CodeTokenType.Identifier)
			{
				string varName = (string)currentToken.Value;
				ConsumeToken(CodeTokenType.Identifier);
				JumpWhiteSpaceAndLineBreak();
				var var1 = ParserGetVarValue();
				executor.Event(() =>
				{
					SetResult(varName, !ResolveOperand(var1).ToBool());
				});
			}
			else
			{
				throw new InvalidOperationException("NOT 需要一个结果变量名");
			}
		}
		#endregion

		#region 比较运算
		/// <summary>
		/// 解析 EQ 指令，等于比较
		/// </summary>
		public void ParserEqual()
		{
			GetString3(out var varName, out var var1, out var var2);
			executor.Event(() =>
			{
				SetResult(varName, ResolveOperand(var1) == ResolveOperand(var2));
			});
		}

		public void ParserNotEqual()
		{
			GetString3(out var varName, out var var1, out var var2);
			executor.Event(() =>
			{
				SetResult(varName, ResolveOperand(var1) != ResolveOperand(var2));
			});
		}

		public void ParserGreaterThan()
		{
			GetString3(out var varName, out var var1, out var var2);
			executor.Event(() =>
			{
				SetResult(varName, ResolveOperand(var1) > ResolveOperand(var2));
			});
		}

		public void ParserLessThan()
		{
			GetString3(out var varName, out var var1, out var var2);
			executor.Event(() =>
			{
				SetResult(varName, ResolveOperand(var1) < ResolveOperand(var2));
			});
		}

		public void ParserGreaterEqual()
		{
			GetString3(out var varName, out var var1, out var var2);
			executor.Event(() =>
			{
				SetResult(varName, ResolveOperand(var1) >= ResolveOperand(var2));
			});
		}

		public void ParserLessEqual()
		{
			GetString3(out var varName, out var var1, out var var2);
			executor.Event(() =>
			{
				SetResult(varName, ResolveOperand(var1) <= ResolveOperand(var2));
			});
		}
		#endregion

		/// <summary>
		/// 解析 PRINT 指令，打印变量值或字面量 
		/// </summary>
		public void ParserPrint()
		{
			var var = ParserGetVarValue();

			executor.Event(() =>
			{
				Debug.Log(ResolveOperand(var));
			});
		}

		#endregion
	}
}
