/****************************************

* 作者：闪电黑客
* 日期：2024/6/24 15:41

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
	/// 枚举命名规范诊断器
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class EnumNamingDiagnostic : NamingDiagnosticBase
	{
		public override SyntaxKind DeclarationKind => SyntaxKind.EnumDeclaration;

		protected override void DiagnosticAction(SyntaxNodeAnalysisContext context)
		{
			// 获取语义模型
			SemanticModel semanticModel = context.SemanticModel;

			EnumDeclarationSyntax enumDeclaration = (EnumDeclarationSyntax)context.Node;

			if (AnalyzerSetting.ProjectDiagnostics.TryGetValue(context.Compilation.AssemblyName, out List<ObjectDiagnostic> objectDiagnostics))
			{
				foreach (ObjectDiagnostic objectDiagnostic in objectDiagnostics)
				{
					//获取当前枚举的类型
					INamedTypeSymbol? typeSymbol = semanticModel.GetDeclaredSymbol(enumDeclaration);
					if (!objectDiagnostic.Screen(typeSymbol)) continue;
					if (!objectDiagnostic.CodeDiagnostics.TryGetValue(DiagnosticKey.EnumNaming, out CodeDiagnosticConfig codeDiagnostic)) continue;
					// 需要的修饰符
					if (!TreeSyntaxHelper.SyntaxKindContains(enumDeclaration.Modifiers, codeDiagnostic.KeywordKinds)) continue;
					// 不需要检查的修饰符
					if (TreeSyntaxHelper.SyntaxKindContainsAny(enumDeclaration.Modifiers, codeDiagnostic.UnKeywordKinds, false)) continue;
					if (!codeDiagnostic.Check.Invoke(enumDeclaration.Identifier.Text) || (codeDiagnostic.NeedComment && !TreeSyntaxHelper.CheckSummaryComment(enumDeclaration)))
					{
						context.ReportDiagnostic(Diagnostic.Create(codeDiagnostic.Diagnostic, enumDeclaration.Identifier.GetLocation(), enumDeclaration.Identifier.Text));
					}
				}
			}
		}
	}

	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EnumNamingCodeFixProvider)), Shared]
	public class EnumNamingCodeFixProvider : NamingCodeFixProviderBase<EnumDeclarationSyntax>
	{
		public override SyntaxKind DeclarationKind => SyntaxKind.EnumDeclaration;
		protected override async Task<Document> CodeFix(CodeDiagnosticConfig codeDiagnostic, Document document, EnumDeclarationSyntax decl, CancellationToken cancellationToken)
		{
			var enumName = decl.Identifier.Text;
			enumName = codeDiagnostic.FixCode?.Invoke(enumName);

			// 创建新的枚举名并替换旧的枚举名
			var newEnumDecl = decl.WithIdentifier(SyntaxFactory.Identifier(enumName));
			var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
			var newRoot = root.ReplaceNode(decl, newEnumDecl);

			// 返回包含修改的文档
			return document.WithSyntaxRoot(newRoot);
		}
	}
}