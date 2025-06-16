/****************************************

* 作者：闪电黑客
* 日期：2024/6/24 10:53

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
	/// 结构体命名规范诊断器
	/// </summary>
	public abstract class StructNamingDiagnostic<C> : NamingDiagnosticBase<C>
		where C : ProjectDiagnosticConfig, new()
	{
		public override SyntaxKind DeclarationKind => SyntaxKind.StructDeclaration;

		protected override void DiagnosticAction(SyntaxNodeAnalysisContext context)
		{
			if (!TryGetDiagnosticConfigGroup(context.Compilation.AssemblyName, out List<DiagnosticConfigGroup> DiagnosticGroups)) return;

			// 获取语义模型
			SemanticModel semanticModel = context.SemanticModel;
			StructDeclarationSyntax structDeclaration = (StructDeclarationSyntax)context.Node;
			foreach (DiagnosticConfigGroup objectDiagnostic in DiagnosticGroups)
			{
				//获取当前类的类型
				INamedTypeSymbol typeSymbol = semanticModel.GetDeclaredSymbol(structDeclaration);
				if (!objectDiagnostic.Screen(context.Compilation, typeSymbol)) continue;
				if (objectDiagnostic.Diagnostics.TryGetValue(DiagnosticKey.StructNaming, out DiagnosticConfig codeDiagnostic))
				{
					// 需要的修饰符
					if (TreeSyntaxHelper.SyntaxKindContains(structDeclaration.Modifiers, codeDiagnostic.KeywordKinds))
					{
						// 不需要检查的修饰符
						if (!TreeSyntaxHelper.SyntaxKindContainsAny(structDeclaration.Modifiers, codeDiagnostic.UnKeywordKinds, false))
						{

							if (!codeDiagnostic.Check.Invoke(semanticModel, structDeclaration.Identifier))
							{
								context.ReportDiagnostic(Diagnostic.Create(codeDiagnostic.Diagnostic, structDeclaration.GetLocation(), structDeclaration.Identifier.Text));
							}
							//检查是否需要添加注释
							else if (codeDiagnostic.NeedComment.Invoke(semanticModel, structDeclaration.Identifier))
							{
								//判断是否是部分结构体
								if (structDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword))
								{
									// 获取所有部分结构体的语法声明
									IEnumerable<StructDeclarationSyntax> partialDeclarations = typeSymbol.DeclaringSyntaxReferences.Select(syntaxRef => syntaxRef.GetSyntax()).OfType<StructDeclarationSyntax>();
									bool isComment = false;
									// 遍历所有部分结构体
									foreach (StructDeclarationSyntax partialDecl in partialDeclarations)
									{
										if (TreeSyntaxHelper.CheckSummaryComment(partialDecl))
										{
											isComment = true;
											break;
										}
									}
									if (!isComment)
									{
										context.ReportDiagnostic(Diagnostic.Create(codeDiagnostic.Diagnostic, structDeclaration.GetLocation(), structDeclaration.Identifier.Text));
									}
								}
								else if (!TreeSyntaxHelper.CheckSummaryComment(structDeclaration))
								{
									context.ReportDiagnostic(Diagnostic.Create(codeDiagnostic.Diagnostic, structDeclaration.GetLocation(), structDeclaration.Identifier.Text));
								}
							}
						}
					}
					return;
				}
			}
		}
	}

	public abstract class StructNamingProvider<C> : NamingCodeFixProviderBase<StructDeclarationSyntax, C>
		where C : ProjectDiagnosticConfig, new()
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