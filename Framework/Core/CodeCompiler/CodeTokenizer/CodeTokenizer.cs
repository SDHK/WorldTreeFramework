/****************************************

* 作者： 闪电黑客
* 日期： 2026/1/24 14:45

* 描述： 

*/
using System;
using System.Globalization;

namespace WorldTree
{
	/// <summary>
	/// 代码词法分析器
	/// </summary>
	public class CodeTokenizer : Node
		, ChildOf<INode>
		, AsRule<Awake>
	{
		/// <summary>
		/// 源代码 
		/// </summary>
		private string sourceCode;
		/// <summary>
		/// 当前字符位置 
		/// </summary>
		private int chatPoint;
		/// <summary>
		/// 当前行号 
		/// </summary>
		private int linePoint;
		/// <summary>
		/// 当前列号 
		/// </summary>
		private int columnPoint;
		/// <summary>
		/// 当前字符 
		/// </summary>
		private char currentChar;

		/// <summary>
		/// 设置源代码 
		/// </summary>
		public void SetCode(string source)
		{
			sourceCode = source;
			chatPoint = 0;
			linePoint = 1;
			columnPoint = 1;
			currentChar = chatPoint < sourceCode.Length ? sourceCode[chatPoint] : '\0';
		}
		/// <summary>
		/// 进位
		/// </summary>
		private void Advance()
		{
			if (currentChar == '\n')
			{
				linePoint++;//行数+1
				columnPoint = 1;//列数重置
			}
			else
			{
				columnPoint++;
			}
			chatPoint++;
			currentChar = chatPoint < sourceCode.Length ? sourceCode[chatPoint] : '\0';
		}
		/// <summary>
		/// 尝试读取数字
		/// </summary>
		private bool TryReadNumber(out VarValue number)
		{
			number = default;
			if (!char.IsDigit(currentChar)) return false;

			string numStr = "";
			bool isFloat = false;

			while (char.IsDigit(currentChar) || currentChar == '.')
			{
				if (currentChar == '.')
				{
					if (isFloat) break; // 避免多个小数点
					isFloat = true;
				}
				numStr += currentChar;
				Advance();
			}
			if (isFloat)
			{
				if (double.TryParse(numStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double doubleValue))
				{
					number = doubleValue; // 使用隐式转换
					return true;
				}
			}
			else
			{
				if (long.TryParse(numStr, NumberStyles.Any, CultureInfo.InvariantCulture, out long longValue))
				{
					number = longValue; // 使用隐式转换
					return true;
				}
			}
			throw new InvalidOperationException($"无效数字格式: {numStr} at line {linePoint}, column {columnPoint}");
		}
		/// <summary>
		/// 尝试读取标识符（变量名或关键字）
		/// </summary>
		private bool TryReadIdentifier(out string identifier)
		{
			identifier = "";
			if (!char.IsLetter(currentChar) && currentChar != '_') return false;
			while (char.IsLetterOrDigit(currentChar) || currentChar == '_')
			{
				identifier += currentChar;
				Advance();
			}
			return true;
		}
		/// <summary>
		/// 尝试读取符号
		/// </summary>
		private bool TryReadSymbol(out char symbol)
		{
			symbol = '\0';
			if (!char.IsSymbol(currentChar) && !char.IsPunctuation(currentChar)) return false;
			symbol = currentChar;
			Advance();
			return true;
		}
		/// <summary>
		/// 尝试读取空白符（空格、制表符等，排除换行符）
		/// </summary>
		private bool TryReadWhitespace(out string whitespace)
		{
			whitespace = "";
			if (currentChar != ' ' && currentChar != '\t' && currentChar != '\v' && currentChar != '\f')
				return false;

			while (currentChar == ' ' || currentChar == '\t' || currentChar == '\v' || currentChar == '\f')
			{
				whitespace += currentChar;
				Advance();
			}
			return true;
		}
		/// <summary>
		/// 尝试读取换行符
		/// </summary>
		private bool TryReadLineBreak(out string lineBreak)
		{
			lineBreak = "";
			if (currentChar != '\n' && currentChar != '\r') return false;
			while (currentChar == '\n' || currentChar == '\r')
			{
				lineBreak += currentChar;
				Advance();
			}
			return true;
		}

		/// <summary>
		/// 获取下一个代码令牌
		/// </summary>
		/// <returns></returns>
		public CodeToken GetNextToken()
		{
			while (currentChar != '\0')
			{
				// 判断结束符
				if (currentChar == '\0') break;
				// 常量数字
				if (TryReadNumber(out VarValue number)) return new CodeToken(CodeTokenType.Number, number, linePoint, columnPoint);
				// 标识符
				if (TryReadIdentifier(out string identifier)) return new CodeToken(CodeTokenType.Identifier, identifier, linePoint, columnPoint);
				// 符号
				if (TryReadSymbol(out char symbol)) return new CodeToken(CodeTokenType.Symbol, (long)symbol, linePoint, columnPoint);
				// 空白符
				if (TryReadWhitespace(out string whitespace)) return new CodeToken(CodeTokenType.Whitespace, whitespace, linePoint, columnPoint);
				// 换行符
				if (TryReadLineBreak(out string lineBreak)) return new CodeToken(CodeTokenType.LineBreak, lineBreak, linePoint, columnPoint);
			}
			return new CodeToken(CodeTokenType.EOF, "", linePoint, columnPoint);
		}
	}

	public static class CodeTokenizerRule
	{
		class Awake : AwakeRule<CodeTokenizer>
		{
			protected override void Execute(CodeTokenizer self)
			{
				//默认空代码
				self.SetCode(string.Empty);
			}
		}
	}
}
