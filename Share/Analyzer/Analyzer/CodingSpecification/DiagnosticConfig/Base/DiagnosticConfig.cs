/****************************************

* 作者：闪电黑客
* 日期：2024/6/20 15:18

* 描述：

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace WorldTree.Analyzer
{
	/// <summary>
	/// 代码规范诊断配置
	/// </summary>
	public struct DiagnosticConfig
	{

		/// <summary>
		/// 诊断键值
		/// </summary>
		public DiagnosticKey DiagnosticKey;

		/// <summary>
		/// 诊断标题
		/// </summary>
		public string Title = "命名规范诊断";

		/// <summary>
		/// 诊断消息格式
		/// </summary>
		public string MessageFormat = " {0} 命名不规范";

		/// <summary>
		/// 诊断描述
		/// </summary>
		public string Description = "命名不规范.";

		/// <summary>
		/// 修复标题
		/// </summary>
		public string CodeFixTitle = "【修复为规范命名】";

		/// <summary>
		/// 诊断名称
		/// </summary>
		public string Key = "Name";

		/// <summary>
		/// 是否需要注释
		/// </summary>
		public Func<SemanticModel, SyntaxToken, bool> NeedComment = (semanticModel, identifier) => true;

		/// <summary>
		/// 声明语法形式筛选
		/// </summary>
		public SyntaxKind DeclarationKind = SyntaxKind.None;

		/// <summary>
		/// 有修饰符语的法形式筛选
		/// </summary>
		public List<SyntaxKind> KeywordKinds = new();
		/// <summary>
		/// 没有修饰符的语法形式筛选
		/// </summary>
		public List<SyntaxKind> UnKeywordKinds = new();

		/// <summary>
		/// 检查规则
		/// </summary>
		public Func<SemanticModel, SyntaxToken, bool> Check = (semanticModel, identifier) => Regex.IsMatch(identifier.Text, "^[A-Z].*$");

		/// <summary>
		/// 修复规则
		/// </summary>
		public Func<string, string> FixCode = s => char.ToUpper(s[0]) + s.Substring(1);

		/// <summary>
		/// 诊断规则
		/// </summary>
		public DiagnosticDescriptor Diagnostic = null;

		public DiagnosticConfig() { }

		/// <summary>
		/// 初始化配置
		/// </summary>
		/// <param name="key">键值</param>
		public DiagnosticConfig Init(string key)
		{
			//if (NeedComment) MessageFormat += "【需要注释】";

			// 生成唯一诊断名称
			this.Key = key;
			this.Key += DeclarationKind.ToString().Replace("Declaration", "");
			foreach (SyntaxKind item in KeywordKinds) this.Key += item.ToString().Replace("Keyword", "");
			foreach (SyntaxKind item in UnKeywordKinds) this.Key += item.ToString().Replace("Keyword", "");

			Diagnostic = new DiagnosticDescriptor(this.Key, Title, MessageFormat, "CodingSpecification", DiagnosticSeverity.Error, true, Description);
			return this;
		}
	}
}