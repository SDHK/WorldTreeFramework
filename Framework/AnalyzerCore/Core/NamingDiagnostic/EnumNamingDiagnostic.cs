/****************************************

* 作者：闪电黑客
* 日期：2024/6/24 15:41

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
	/// 枚举命名规范诊断器
	/// </summary>
	public abstract class EnumNamingDiagnostic : NamingDiagnosticBase
	{
		public override SyntaxKind DeclarationKind => SyntaxKind.EnumDeclaration;

		protected override void DiagnosticAction(SyntaxNodeAnalysisContext context)
		{

			if (!TryGetDiagnosticConfigGroup(context.Compilation.AssemblyName, out List<DiagnosticConfigGroup> DiagnosticGroups)) return;

			// 获取语义模型
			SemanticModel semanticModel = context.SemanticModel;

			EnumDeclarationSyntax enumDeclaration = (EnumDeclarationSyntax)context.Node;
			//获取当前枚举的类型
			INamedTypeSymbol typeSymbol = semanticModel.GetDeclaredSymbol(enumDeclaration);
			foreach (DiagnosticConfigGroup objectDiagnostic in DiagnosticGroups)
			{
				if (!objectDiagnostic.Screen(context.Compilation, typeSymbol)) continue;
				if (objectDiagnostic.Diagnostics.TryGetValue(DiagnosticKey.EnumNaming, out DiagnosticConfig codeDiagnostic))
				{
					// 需要的修饰符
					if (TreeSyntaxHelper.SyntaxKindContains(enumDeclaration.Modifiers, codeDiagnostic.KeywordKinds))
					{
						// 不需要检查的修饰符
						if (!TreeSyntaxHelper.SyntaxKindContainsAny(enumDeclaration.Modifiers, codeDiagnostic.UnKeywordKinds, false))
						{
							if (!codeDiagnostic.Check.Invoke(semanticModel, enumDeclaration.Identifier) || (codeDiagnostic.NeedComment.Invoke(semanticModel, enumDeclaration.Identifier) && !TreeSyntaxHelper.CheckSummaryComment(enumDeclaration)))
							{
								context.ReportDiagnostic(Diagnostic.Create(codeDiagnostic.Diagnostic, enumDeclaration.Identifier.GetLocation(), enumDeclaration.Identifier.Text));
							}

						}
					}
					return;
				}
			}
		}
	}

	public abstract class EnumNamingProvider : NamingCodeFixProviderBase<EnumDeclarationSyntax>
	{
		public override SyntaxKind DeclarationKind => SyntaxKind.EnumDeclaration;
		protected override async Task<Document> CodeFix(DiagnosticConfig codeDiagnostic, Document document, EnumDeclarationSyntax decl, CancellationToken cancellationToken)
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