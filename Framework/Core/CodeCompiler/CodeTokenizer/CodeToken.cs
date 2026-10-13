/****************************************

* 作者： 闪电黑客
* 日期： 2026/1/24 14:48

* 描述： 

*/
namespace WorldTree
{
	/// <summary>
	/// 代码令牌
	/// </summary>
	public struct CodeToken
	{
		/// <summary>
		/// 代码令牌类型 
		/// </summary>
		public CodeTokenType Type { get; set; }
		/// <summary>
		/// 代码令牌值 
		/// </summary>
		public VarValue Value { get; set; }
		/// <summary>
		/// 代码令牌所在行号 
		/// </summary>
		public int Line { get; set; }
		/// <summary>
		/// 代码令牌所在列号 
		/// </summary>
		public int Column { get; set; }

		public CodeToken(CodeTokenType type, VarValue value, int line = 0, int column = 0)
		{
			Type = type;
			Line = line;
			Column = column;
			Value = value;
		}
	}
}
