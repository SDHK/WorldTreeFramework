/****************************************

* 作者：闪电黑客
* 日期：2024/6/24 14:49

* 描述：

*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WorldTree.Analyzer
{

	/// <summary>
	/// 委托命名规范诊断器
	/// </summary>
	public abstract class DelegateNamingDiagnostic<C> : NamingDiagnosticBase<C>
		where C : ProjectDiagnosticConfig, new()
	{
		public override SyntaxKind DeclarationKind => SyntaxKind.DelegateDeclaration;
		protected override void DiagnosticAction(SyntaxNodeAnalysisContext context)
		{
			if (!TryGetDiagnosticConfigGroup(context.Compilation.AssemblyName, out List<DiagnosticConfigGroup> DiagnosticGroups)) return;

			// 获取语义模型
			SemanticModel semanticModel = context.SemanticModel;
			DelegateDeclarationSyntax delegateDeclaration = (DelegateDeclarationSyntax)context.Node;
			//获取当前委托的类型
			INamedTypeSymbol typeSymbol = semanticModel.GetDeclaredSymbol(delegateDeclaration);
			foreach (DiagnosticConfigGroup objectDiagnostic in DiagnosticGroups)
			{
				if (!objectDiagnostic.Screen(context.Compilation, typeSymbol)) continue;
				if (objectDiagnostic.Diagnostics.TryGetValue(DiagnosticKey.DelegateNaming, out DiagnosticConfig codeDiagnostic))
				{
					// 需要的修饰符
					if (TreeSyntaxHelper.SyntaxKindContains(delegateDeclaration.Modifiers, codeDiagnostic.KeywordKinds))
					{
						// 不需要检查的修饰符
						if (!TreeSyntaxHelper.SyntaxKindContainsAny(delegateDeclaration.Modifiers, codeDiagnostic.UnKeywordKinds, false))
						{
							if (!codeDiagnostic.Check.Invoke(semanticModel, delegateDeclaration.Identifier) || (codeDiagnostic.NeedComment.Invoke(semanticModel, delegateDeclaration.Identifier) && !TreeSyntaxHelper.CheckSummaryComment(delegateDeclaration)))
							{
								context.ReportDiagnostic(Diagnostic.Create(codeDiagnostic.Diagnostic, delegateDeclaration.Identifier.GetLocation(), delegateDeclaration.Identifier.Text));
							}
						}
					}
					return;
				}
			}
		}
	}

	public abstract class DelegateNamingProvider<C> : NamingCodeFixProviderBase<DelegateDeclarationSyntax, C>
		where C : ProjectDiagnosticConfig, new()
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