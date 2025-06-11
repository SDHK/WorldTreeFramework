/****************************************

* 作者：闪电黑客
* 日期：2024/6/24 14:38

* 描述：

*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WorldTree.Analyzer
{
	/// <summary>
	/// 接口命名规范诊断器
	/// </summary>
	public abstract class InterfaceNamingDiagnostic : NamingDiagnosticBase
	{
		public override SyntaxKind DeclarationKind => SyntaxKind.InterfaceDeclaration;

		protected override void DiagnosticAction(SyntaxNodeAnalysisContext context)
		{
			if (!TryGetDiagnosticConfigGroup(context.Compilation.AssemblyName, out List<DiagnosticConfigGroup> DiagnosticGroups)) return;

			// 获取语义模型
			SemanticModel semanticModel = context.SemanticModel;
			InterfaceDeclarationSyntax interfaceDeclaration = (InterfaceDeclarationSyntax)context.Node;
			INamedTypeSymbol? typeSymbol = semanticModel.GetDeclaredSymbol(interfaceDeclaration);
			foreach (DiagnosticConfigGroup objectDiagnostic in DiagnosticGroups)
			{
				//获取当前接口的类型
				if (!objectDiagnostic.Screen(context.Compilation, typeSymbol)) continue;
				if (objectDiagnostic.Diagnostics.TryGetValue(DiagnosticKey.InterfaceNaming, out DiagnosticConfig codeDiagnostic))
				{
					// 需要的修饰符
					if (TreeSyntaxHelper.SyntaxKindContains(interfaceDeclaration.Modifiers, codeDiagnostic.KeywordKinds))
					{
						// 不需要检查的修饰符
						if (!TreeSyntaxHelper.SyntaxKindContainsAny(interfaceDeclaration.Modifiers, codeDiagnostic.UnKeywordKinds, false))
						{
							if (!codeDiagnostic.Check.Invoke(semanticModel, interfaceDeclaration.Identifier))
							{
								context.ReportDiagnostic(Diagnostic.Create(codeDiagnostic.Diagnostic, interfaceDeclaration.Identifier.GetLocation(), interfaceDeclaration.Identifier.Text));
							}
							else if (codeDiagnostic.NeedComment.Invoke(semanticModel, interfaceDeclaration.Identifier))
							{
								//判断是否是部分接口
								if (interfaceDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword))
								{
									// 获取所有部分接口的语法声明
									IEnumerable<InterfaceDeclarationSyntax> partialDeclarations = typeSymbol.DeclaringSyntaxReferences.Select(syntaxRef => syntaxRef.GetSyntax()).OfType<InterfaceDeclarationSyntax>();
									bool isComment = false;

									foreach (InterfaceDeclarationSyntax partialDecl in partialDeclarations)
									{
										if (TreeSyntaxHelper.CheckSummaryComment(partialDecl))
										{
											isComment = true;
											break;
										}
									}
									if (!isComment)
									{
										context.ReportDiagnostic(Diagnostic.Create(codeDiagnostic.Diagnostic, interfaceDeclaration.GetLocation(), interfaceDeclaration.Identifier.Text));
									}
								}
								else if (!TreeSyntaxHelper.CheckSummaryComment(interfaceDeclaration))
								{
									context.ReportDiagnostic(Diagnostic.Create(codeDiagnostic.Diagnostic, interfaceDeclaration.GetLocation(), interfaceDeclaration.Identifier.Text));
								}
							}
						}
					}
					return;
				}
			}
		}

	}
	public abstract class InterfaceNamingProvider : NamingCodeFixProviderBase<InterfaceDeclarationSyntax>
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