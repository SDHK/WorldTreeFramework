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
using System.Collections.Generic;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;

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
			if (!ProjectDiagnosticSetting.ProjectDiagnostics.TryGetValue(context.Compilation.AssemblyName, out List<DiagnosticConfigGroup> diagnosticConfigGroups)) return;
			// 获取语义模型
			SemanticModel semanticModel = context.SemanticModel;
			MethodDeclarationSyntax? methodDeclaration = context.Node as MethodDeclarationSyntax;
			foreach (DiagnosticConfigGroup diagnosticConfigGroup in diagnosticConfigGroups)
			{
				BaseTypeDeclarationSyntax parentType = TreeSyntaxHelper.GetParentType(methodDeclaration);
				IMethodSymbol? methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration);
				INamedTypeSymbol? typeSymbol = semanticModel.GetDeclaredSymbol(parentType);
				if (!diagnosticConfigGroup.Screen(typeSymbol)) continue;
				if (diagnosticConfigGroup.Diagnostics.TryGetValue(DiagnosticKey.MethodNaming, out DiagnosticConfig codeDiagnostic))
				{
					// 需要的修饰符
					if (TreeSyntaxHelper.SyntaxKindContains(methodDeclaration.Modifiers, codeDiagnostic.KeywordKinds))
					{
						// 不需要检查的修饰符
						if (!TreeSyntaxHelper.SyntaxKindContainsAny(methodDeclaration.Modifiers, codeDiagnostic.UnKeywordKinds, false))
						{
							// 检查方法是否为重写的方法
							bool isOverride = methodSymbol.IsOverride;
							if (!isOverride)
							{
								// 检查方法是否直接声明在当前类中
								INamedTypeSymbol? parentTypeSymbol = semanticModel.GetDeclaredSymbol(parentType);
								isOverride = !methodSymbol.ContainingType.Equals(parentTypeSymbol, SymbolEqualityComparer.Default);
								if (!isOverride)
								{
									// 检查方法是否实现了任何接口
									isOverride = NamedSymbolHelper.CheckInterfaceImplements(methodSymbol);
								}
							}

							//是否为原声明的方法，而不是重写的方法，或者是实现的接口方法
							if (!isOverride)
							{
								if (codeDiagnostic.NeedComment && !TreeSyntaxHelper.CheckSummaryComment(methodDeclaration))
								{
									context.ReportDiagnostic(Diagnostic.Create(codeDiagnostic.Diagnostic, methodDeclaration.GetLocation(), methodDeclaration.Identifier.Text));
								}

								if (!codeDiagnostic.Check.Invoke(methodDeclaration.Identifier.Text))
								{
									context.ReportDiagnostic(Diagnostic.Create(codeDiagnostic.Diagnostic, methodDeclaration.GetLocation(), methodDeclaration.Identifier.Text));
								}
							}
						}
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