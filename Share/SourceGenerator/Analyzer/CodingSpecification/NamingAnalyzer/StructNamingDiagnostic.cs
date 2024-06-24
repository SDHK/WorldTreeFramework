/****************************************

* 作者：闪电黑客
* 日期：2024/6/24 10:53

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
	/// 结构体命名规范诊断器
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class StructNamingDiagnostic : NamingDiagnosticBase
	{
		public override SyntaxKind DeclarationKind => SyntaxKind.StructDeclaration;

		protected override void DiagnosticAction(SyntaxNodeAnalysisContext context)
		{
			// 获取语义模型
			SemanticModel semanticModel = context.SemanticModel;
			StructDeclarationSyntax structDeclaration = (StructDeclarationSyntax)context.Node;

			if (AnalyzerSetting.ProjectDiagnostics.TryGetValue(context.Compilation.AssemblyName, out List<DiagnosticGroupConfig> objectDiagnostics))
			{
				foreach (DiagnosticGroupConfig objectDiagnostic in objectDiagnostics)
				{
					//获取当前类的类型
					INamedTypeSymbol? typeSymbol = semanticModel.GetDeclaredSymbol(structDeclaration);
					if (!objectDiagnostic.Screen(typeSymbol)) continue;
					if (!objectDiagnostic.CodeDiagnostics.TryGetValue(DiagnosticKey.StructNaming, out DiagnosticConfig codeDiagnostic)) continue;
					// 需要的修饰符
					if (!TreeSyntaxHelper.SyntaxKindContains(structDeclaration.Modifiers, codeDiagnostic.KeywordKinds)) continue;
					// 不需要检查的修饰符
					if (TreeSyntaxHelper.SyntaxKindContainsAny(structDeclaration.Modifiers, codeDiagnostic.UnKeywordKinds, false)) continue;
					if (!codeDiagnostic.Check.Invoke(structDeclaration.Identifier.Text) || (codeDiagnostic.NeedComment && !TreeSyntaxHelper.CheckSummaryComment(structDeclaration)))
					{
						context.ReportDiagnostic(Diagnostic.Create(codeDiagnostic.Diagnostic, structDeclaration.GetLocation(), structDeclaration.Identifier.Text));
					}
					return;
				}
			}
		}
	}

	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StructNamingCodeFixProvider)), Shared]
	public class StructNamingCodeFixProvider : NamingCodeFixProviderBase<StructDeclarationSyntax>
	{
		public override SyntaxKind DeclarationKind => SyntaxKind.StructDeclaration;
		protected override async Task<Document> CodeFix(DiagnosticConfig codeDiagnostic, Document document, StructDeclarationSyntax decl, CancellationToken cancellationToken)
		{

			var fieldName = decl.Identifier.Text;

			fieldName = codeDiagnostic.FixCode?.Invoke(fieldName);

			// 创建新的字段名并替换旧的字段名
			var newFieldDecl = decl.WithIdentifier(SyntaxFactory.Identifier(fieldName));
			var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
			var newRoot = root.ReplaceNode(decl, newFieldDecl);

			// 返回包含修改的文档
			return document.WithSyntaxRoot(newRoot);
		}
	}
}