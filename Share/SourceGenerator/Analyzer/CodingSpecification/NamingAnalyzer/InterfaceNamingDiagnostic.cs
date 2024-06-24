/****************************************

* 作者：闪电黑客
* 日期：2024/6/24 14:38

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
	/// 接口命名规范诊断器
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class InterfaceNamingDiagnostic : NamingDiagnosticBase
	{
		public override SyntaxKind DeclarationKind => SyntaxKind.InterfaceDeclaration;

		protected override void DiagnosticAction(SyntaxNodeAnalysisContext context)
		{
			// 获取语义模型
			SemanticModel semanticModel = context.SemanticModel;

			InterfaceDeclarationSyntax interfaceDeclaration = (InterfaceDeclarationSyntax)context.Node;

			if (AnalyzerSetting.ProjectDiagnostics.TryGetValue(context.Compilation.AssemblyName, out List<DiagnosticConfigGroup> objectDiagnostics))
			{
				foreach (DiagnosticConfigGroup objectDiagnostic in objectDiagnostics)
				{
					//获取当前接口的类型
					INamedTypeSymbol? typeSymbol = semanticModel.GetDeclaredSymbol(interfaceDeclaration);
					if (!objectDiagnostic.Screen(typeSymbol)) continue;
					if (!objectDiagnostic.Diagnostics.TryGetValue(DiagnosticKey.InterfaceNaming, out DiagnosticConfig codeDiagnostic)) continue;
					// 需要的修饰符
					if (!TreeSyntaxHelper.SyntaxKindContains(interfaceDeclaration.Modifiers, codeDiagnostic.KeywordKinds)) continue;
					// 不需要检查的修饰符
					if (TreeSyntaxHelper.SyntaxKindContainsAny(interfaceDeclaration.Modifiers, codeDiagnostic.UnKeywordKinds, false)) continue;
					if (!codeDiagnostic.Check.Invoke(interfaceDeclaration.Identifier.Text) || (codeDiagnostic.NeedComment && !TreeSyntaxHelper.CheckSummaryComment(interfaceDeclaration)))
					{
						context.ReportDiagnostic(Diagnostic.Create(codeDiagnostic.Diagnostic, interfaceDeclaration.Identifier.GetLocation(), interfaceDeclaration.Identifier.Text));
					}
				}
			}
		}

	}
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(InterfaceNamingCodeFixProvider)), Shared]
	public class InterfaceNamingCodeFixProvider : NamingCodeFixProviderBase<InterfaceDeclarationSyntax>
	{
		public override SyntaxKind DeclarationKind => SyntaxKind.InterfaceDeclaration;

		protected override async Task<Document> CodeFix(DiagnosticConfig codeDiagnostic, Document document, InterfaceDeclarationSyntax decl, CancellationToken cancellationToken)
		{
			var interfaceName = decl.Identifier.Text;
			interfaceName = codeDiagnostic.FixCode?.Invoke(interfaceName);

			// 创建新的接口名并替换旧的接口名
			var newInterfaceDecl = decl.WithIdentifier(SyntaxFactory.Identifier(interfaceName));
			var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
			var newRoot = root.ReplaceNode(decl, newInterfaceDecl);

			// 返回包含修改的文档
			return document.WithSyntaxRoot(newRoot);
		}
	}
}