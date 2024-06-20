/****************************************

* 作者：闪电黑客
* 日期：2024/6/20 20:46

* 描述：

*/
using Microsoft.CodeAnalysis.CSharp;
using System.Text.RegularExpressions;

namespace WorldTree.Analyzer
{
	/// <summary>
	/// List类型诊断
	/// </summary>
	public class ListDiagnostic : ObjectDiagnostic
	{
		public override ObjectDiagnostic Init()
		{
			SyntaxKinds = new() { SyntaxKind.ClassDeclaration, };
			CheckClassName = (s) => Regex.IsMatch(s, "^System.Collections.Generic.List<.*>$");
			SetNamingRule(DiagnosticKey.ClassFieldNaming, new CodeDiagnosticConfig()
			{
				Title = "List类型字段命名规范诊断",
				MessageFormat = "List类型字段：{0} 命名要加List后戳",
				DeclarationSyntaxKinds = new() { SyntaxKind.FieldDeclaration },
				Check = s => Regex.IsMatch(s, ".*List$"),
				Fix = s => s + "List",
			});
			return this;
		}
	}
}