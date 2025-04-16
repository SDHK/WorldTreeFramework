/****************************************

* 作者：闪电黑客
* 日期：2024/6/26 14:41

* 描述：

*/
using Microsoft.CodeAnalysis.CSharp;

namespace WorldTree.Analyzer
{
	/// <summary>
	/// 项目禁止类型诊断
	/// </summary>
	public class ProjectBanDiagnosticConfig : DiagnosticConfigGroup
	{
		public ProjectBanDiagnosticConfig()
		{
			Screen = (Compilation, Symbol) => true;

			SetConfig(DiagnosticKey.ClassNaming, new DiagnosticConfig()
			{
				Title = "项目禁止声明类型",
				MessageFormat = "项目禁止声明类型",
				DeclarationKind = SyntaxKind.ClassDeclaration,
				Check = (semanticModel, identifier) => false,
				NeedComment = false,
			});
			SetConfig(DiagnosticKey.StructNaming, new DiagnosticConfig()
			{
				Title = "项目禁止声明结构体",
				MessageFormat = "项目禁止声明结构体",
				DeclarationKind = SyntaxKind.StructDeclaration,
				Check = (semanticModel, identifier) => false,
				NeedComment = false,
			});
			SetConfig(DiagnosticKey.EnumNaming, new DiagnosticConfig()
			{
				Title = "项目禁止声明枚举",
				MessageFormat = "项目禁止声明枚举",
				DeclarationKind = SyntaxKind.EnumDeclaration,
				Check = (semanticModel, identifier) => false,
				NeedComment = false,
			});
		}
	}
}