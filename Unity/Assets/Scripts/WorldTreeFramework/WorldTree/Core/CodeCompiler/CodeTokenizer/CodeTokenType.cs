/****************************************

* 作者： 闪电黑客
* 日期： 2026/1/24 14:45

* 描述： 

*/
namespace WorldTree
{
	/// <summary>
	/// 代码词法令牌类型
	/// </summary>
	public enum CodeTokenType
	{
		/// <summary>
		/// 数字字面量
		/// </summary>
		Number,
		/// <summary>
		/// 标识符（包括所有文本内容：变量、关键字、字符串等）
		/// </summary>
		Identifier,
		/// <summary>
		/// 符号（所有非字母数字的符号）
		/// </summary>
		Symbol,
		/// <summary>
		/// 空白符（可能某些语言需要）
		/// </summary>
		Whitespace,
		/// <summary>
		/// 换行符（可能某些语言需要）
		/// </summary>
		LineBreak,
		/// <summary>
		/// 结束符
		/// </summary>
		EOF,
	}
}
