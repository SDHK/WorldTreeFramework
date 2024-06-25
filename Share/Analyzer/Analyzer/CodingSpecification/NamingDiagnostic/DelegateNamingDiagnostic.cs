/****************************************

* 作者：闪电黑客
* 日期：2024/6/24 14:49

* 描述：

*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;

namespace WorldTree.Analyzer
{
	/// <summary>
	/// 委托命名规范诊断器
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class DelegateNamingDiagnostic : NamingDiagnosticBase
	{
		public override SyntaxKind DeclarationKind => SyntaxKind.DelegateDeclaration;
		protected override void DiagnosticAction(SyntaxNodeAnalysisContext context)
		{
			if (!ProjectDiagnosticSetting.ProjectDiagnostics.TryGetValue(context.Compilation.AssemblyName, out List<DiagnosticConfigGroup> objectDiagnostics)) return;

			// 获取语义模型
			SemanticModel semanticModel = context.SemanticModel;
			DelegateDeclarationSyntax delegateDeclaration = (DelegateDeclarationSyntax)context.Node;

			foreach (DiagnosticConfigGroup objectDiagnostic in objectDiagnostics)
			{
				//获取当前委托的类型
				INamedTypeSymbol? typeSymbol = semanticModel.GetDeclaredSymbol(delegateDeclaration);
				if (!objectDiagnostic.Screen(typeSymbol)) continue;
				if (!objectDiagnostic.Diagnostics.TryGetValue(DiagnosticKey.DelegateNaming, out DiagnosticConfig codeDiagnostic)) continue;
				// 需要的修饰符
				if (!TreeSyntaxHelper.SyntaxKindContains(delegateDeclaration.Modifiers, codeDiagnostic.KeywordKinds)) continue;
				// 不需要检查的修饰符
				if (TreeSyntaxHelper.SyntaxKindContainsAny(delegateDeclaration.Modifiers, codeDiagnostic.UnKeywordKinds, false)) continue;
				if (!codeDiagnostic.Check.Invoke(delegateDeclaration.Identifier.Text) || (codeDiagnostic.NeedComment && !TreeSyntaxHelper.CheckSummaryComment(delegateDeclaration)))
				{
					context.ReportDiagnostic(Diagnostic.Create(codeDiagnostic.Diagnostic, delegateDeclaration.Identifier.GetLocation(), delegateDeclaration.Identifier.Text));
				}
			}
		}
	}

	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DelegateNamingCodeFixProvider)), Shared]
	public class DelegateNamingCodeFixProvider : NamingCodeFixProviderBase<DelegateDeclarationSyntax>
	{
		public override SyntaxKind DeclarationKind => SyntaxKind.DelegateDeclaration;
		protected override async Task<Document> CodeFix(DiagnosticConfig codeDiagnostic, Document document, DelegateDeclarationSyntax delegateDecl, CancellationToken cancellationToken)
		{
			var delegateName = delegateDecl.Identifier.Text;
			delegateName = codeDiagnostic.FixCode?.Invoke(delegateName);

			// 创建新的委托名并替换旧的委托名
			var newDelegateDecl = delegateDecl.WithIdentifier(SyntaxFactory.Identifier(delegateName));
			var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
			var newRoot = root.ReplaceNode(delegateDecl, newDelegateDecl);

			// 返回包含修改的文档
			return document.WithSyntaxRoot(newRoot);
		}
	}
}