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
						case "VAR":
							ParserVar();
							break;
						case "MOVE":
							ParserMove();
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

						case "PUSH":
							ParserPushValue();
							break;
						case "PUSH_VAR":
							ParserPushVariable();
							break;
						case "POP_VAR":
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

						case "PRINT":
							ParserPrint();
							break;
						default:
							throw new InvalidOperationException($"未知指令: {instruction}");
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

				// 否则作为普通标识符处理
				ConsumeToken(CodeTokenType.Identifier);
				return identifierValue;
			}
			throw new InvalidOperationException($"期望数字或字符串字面量，但得到 {currentToken.Type}");
		}

		/// <summary>
		/// 解析 Var 指令 
		/// </summary>
		public void ParserVar()
		{
			if (currentToken.Type == CodeTokenType.Identifier)
			{
				string varName = (string)currentToken.Value;
				ConsumeToken(CodeTokenType.Identifier);
				var var = ParserGetVarValue();
				executor.SetVariableEvent(varName, var);
			}
			else
			{
				throw new InvalidOperationException("VAR 需要一个变量名");
			}
		}
		/// <summary>
		/// 解析 Move 指令  
		/// </summary>
		public void ParserMove()
		{
			if (currentToken.Type == CodeTokenType.Identifier)
			{
				string varName = (string)currentToken.Value;
				ConsumeToken(CodeTokenType.Identifier);
				JumpWhiteSpaceAndLineBreak();
				if (currentToken.Type == CodeTokenType.Identifier)
				{
					string var = (string)currentToken.Value;
					executor.MoveVariableEvent(varName, var);
					ConsumeToken(CodeTokenType.Identifier);
				}
				else
				{
					throw new InvalidOperationException("Move 需要一个目标变量名");
				}
			}
			else
			{
				throw new InvalidOperationException("Move 需要一个变量名");
			}
		}


		/// <summary>
		/// 解析 IF 指令 
		/// </summary>
		public void ParserIFEvent()
		{
			if (currentToken.Type == CodeTokenType.Identifier)
			{
				string varName = (string)currentToken.Value;
				ConsumeToken(CodeTokenType.Identifier);
				executor.IF_Event(() => (bool)executor.GetVariable(varName));
			}
			else
			{
				throw new InvalidOperationException("IF 需要一个变量名");
			}
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
		/// <exception cref="InvalidOperationException"></exception>
		public void ParserLoopEnter()
		{
			if (currentToken.Type == CodeTokenType.Identifier)
			{
				string varName = (string)currentToken.Value;
				ConsumeToken(CodeTokenType.Identifier);
				executor.Loop_Enter(() => (bool)executor.GetVariable(varName));
			}
			else
			{
				throw new InvalidOperationException("LOOP 需要一个变量名");
			}
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

			if (currentToken.Type == CodeTokenType.Identifier)
			{
				string varName = (string)currentToken.Value;
				ConsumeToken(CodeTokenType.Identifier);
				executor.Loop_End(() => (bool)executor.GetVariable(varName));
			}
			else
			{
				throw new InvalidOperationException("LOOP_END_DO 需要一个变量名");
			}
		}

		/// <summary>
		/// 解析 DELAY 指令 
		/// </summary>
		public void ParserDelay()
		{
			var delay = ParserGetVarValue();
			if (delay.Type != VarType.Long)
			{
				throw new InvalidOperationException("DELAY 需要一个数字");
			}
			executor.Delay((int)delay);
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
		/// 解析 Push 值指令 
		/// </summary>
		public void ParserPushValue()
		{
			var var = ParserGetVarValue();
			executor.PushValue(var);
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


		/// <summary>
		/// 获取三个字符串变量名 
		/// </summary>
		public void GetString3(out string varName, out string var1, out string var2)
		{
			if (currentToken.Type == CodeTokenType.Identifier)
			{
				varName = (string)currentToken.Value;
				ConsumeToken(CodeTokenType.Identifier);
				JumpWhiteSpaceAndLineBreak();
				if (currentToken.Type == CodeTokenType.Identifier)
				{
					var1 = (string)currentToken.Value;
					ConsumeToken(CodeTokenType.Identifier);
					JumpWhiteSpaceAndLineBreak();
					if (currentToken.Type == CodeTokenType.Identifier)
					{
						var2 = (string)currentToken.Value;
						ConsumeToken(CodeTokenType.Identifier);
					}
					else
					{
						throw new InvalidOperationException("需要一个变量名2");
					}
				}
				else
				{
					throw new InvalidOperationException("需要一个变量名1");
				}
			}
			else
			{
				throw new InvalidOperationException("需要一个目标变量名");
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
				executor.SetVariable(varName, executor.GetVariable(var1) + executor.GetVariable(var2));
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
				executor.SetVariable(varName, executor.GetVariable(var1) - executor.GetVariable(var2));
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
				executor.SetVariable(varName, executor.GetVariable(var1) * executor.GetVariable(var2));
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
				executor.SetVariable(varName, executor.GetVariable(var1) / executor.GetVariable(var2));
			});
		}

		/// <summary>
		/// 解析 PRINT 指令，打印变量值 
		/// </summary>
		public void ParserPrint()
		{
			if (currentToken.Type == CodeTokenType.Identifier)
			{
				string varName = (string)currentToken.Value;
				ConsumeToken(CodeTokenType.Identifier);
				executor.Event(() =>
				{
					Debug.Log(executor.GetVariable(varName));
				});
			}
			else
			{
				throw new InvalidOperationException("PRINT 需要一个变量名");
			}
		}

		#endregion
	}
}
