/****************************************

* 作者：闪电黑客
* 日期：2024/6/24 15:53

* 描述：

*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Composition;
using WorldTree.SourceGenerator;

namespace WorldTree.Analyzer
{
	/// <summary>
	/// 枚举成员命名规范诊断器
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class EnumMemberNamingDiagnostic : NamingDiagnosticBase
	{
		public override SyntaxKind DeclarationKind => SyntaxKind.EnumMemberDeclaration;

		protected override void DiagnosticAction(SyntaxNodeAnalysisContext context)
		{
			// 获取语义模型
			SemanticModel semanticModel = context.SemanticModel;

			EnumMemberDeclarationSyntax enumMemberDeclaration = (EnumMemberDeclarationSyntax)context.Node;

			if (AnalyzerSetting.ProjectDiagnostics.TryGetValue(context.Compilation.AssemblyName, out List<DiagnosticConfigGroup> objectDiagnostics))
			{
				foreach (DiagnosticConfigGroup objectDiagnostic in objectDiagnostics)
				{
					//获取当前枚举成员的类型
					ISymbol? symbol = semanticModel.GetDeclaredSymbol(enumMemberDeclaration);
					if (!objectDiagnostic.Screen(symbol)) continue;
					if (!objectDiagnostic.Diagnostics.TryGetValue(DiagnosticKey.EnumMemberNaming, out DiagnosticConfig codeDiagnostic)) continue;
					// 需要的修饰符
					if (!TreeSyntaxHelper.SyntaxKindContains(enumMemberDeclaration.Modifiers, codeDiagnostic.KeywordKinds)) continue;
					// 不需要检查的修饰符
					if (TreeSyntaxHelper.SyntaxKindContainsAny(enumMemberDeclaration.Modifiers, codeDiagnostic.UnKeywordKinds, false)) continue;
					if (!codeDiagnostic.Check.Invoke(enumMemberDeclaration.Identifier.Text) || (codeDiagnostic.NeedComment && !TreeSyntaxHelper.CheckSummaryComment(enumMemberDeclaration)))
					{
						context.ReportDiagnostic(Diagnostic.Create(codeDiagnostic.Diagnostic, enumMemberDeclaration.Identifier.GetLocation(), enumMemberDeclaration.Identifier.Text));
					}
				}
			}
		}
	}
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EnumMemberNamingCodeFixProvider)), Shared]
	public class EnumMemberNamingCodeFixProvider : NamingCodeFixProviderBase<EnumMemberDeclarationSyntax>
	{
		public override SyntaxKind DeclarationKind => SyntaxKind.EnumMemberDeclaration;
		protected override async Task<Document> CodeFix(DiagnosticConfig codeDiagnostic, Document document, EnumMemberDeclarationSyntax decl, CancellationToken cancellationToken)
		{
			var enumMemberName = decl.Identifier.Text;
			enumMemberName = codeDiagnostic.FixCode?.Invoke(enumMemberName);

			// 创建新的枚举成员名并替换旧的枚举成员名
			var newEnumMemberDecl = decl.WithIdentifier(SyntaxFactory.Identifier(enumMemberName));
			var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
			var newRoot = root.ReplaceNode(decl, newEnumMemberDecl);

			// 返回包含修改的文档
			return document.WithSyntaxRoot(newRoot);
		}
	}
}