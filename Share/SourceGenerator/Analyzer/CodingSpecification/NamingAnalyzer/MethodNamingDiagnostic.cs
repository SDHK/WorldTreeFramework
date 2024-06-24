/****************************************

* 作者：闪电黑客
* 日期：2024/6/21 17:27

* 描述：方法命名规范诊断器

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
	/// 方法命名规范诊断器
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class MethodNamingDiagnostic : NamingDiagnosticBase
	{
		public override SyntaxKind DeclarationKind => SyntaxKind.MethodDeclaration;

		protected override void DiagnosticAction(SyntaxNodeAnalysisContext context)
		{
			// 获取语义模型
			SemanticModel semanticModel = context.SemanticModel;
			MethodDeclarationSyntax? methodDeclaration = context.Node as MethodDeclarationSyntax;

			if (AnalyzerSetting.ProjectDiagnostics.TryGetValue(context.Compilation.AssemblyName, out List<DiagnosticGroupConfig> objectDiagnostics))
			{
				foreach (DiagnosticGroupConfig objectDiagnostic in objectDiagnostics)
				{
					BaseTypeDeclarationSyntax parentType = TreeSyntaxHelper.GetParentType(methodDeclaration);
					INamedTypeSymbol? typeSymbol = semanticModel.GetDeclaredSymbol(parentType);
					if (!objectDiagnostic.Screen(typeSymbol)) continue;
					if (!objectDiagnostic.CodeDiagnostics.TryGetValue(DiagnosticKey.MethodNaming, out DiagnosticConfig codeDiagnostic)) continue;
					// 需要的修饰符
					if (!TreeSyntaxHelper.SyntaxKindContains(methodDeclaration.Modifiers, codeDiagnostic.KeywordKinds)) continue;
					// 不需要检查的修饰符
					if (TreeSyntaxHelper.SyntaxKindContainsAny(methodDeclaration.Modifiers, codeDiagnostic.UnKeywordKinds, false)) continue;
					if (!codeDiagnostic.Check.Invoke(methodDeclaration.Identifier.Text) || (codeDiagnostic.NeedComment && !TreeSyntaxHelper.CheckSummaryComment(methodDeclaration)))
					{
						context.ReportDiagnostic(Diagnostic.Create(codeDiagnostic.Diagnostic, methodDeclaration.GetLocation(), methodDeclaration.Identifier.Text));
					}
					return;
				}
			}
		}
	}

	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MethodNamingCodeFixProvider)), Shared]
	public class MethodNamingCodeFixProvider : NamingCodeFixProviderBase<MethodDeclarationSyntax>
	{
		public override SyntaxKind DeclarationKind => SyntaxKind.MethodDeclaration;

		protected override async Task<Document> CodeFix(DiagnosticConfig codeDiagnostic, Document document, MethodDeclarationSyntax decl, CancellationToken cancellationToken)
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