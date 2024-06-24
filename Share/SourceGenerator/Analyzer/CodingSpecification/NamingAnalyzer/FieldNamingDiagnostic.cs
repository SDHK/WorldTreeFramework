/****************************************

* 作者：闪电黑客
* 日期：2024/6/20 16:35

* 描述：字段命名规范诊断器

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
	/// 字段命名规范诊断器
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class FieldNamingDiagnostic : NamingDiagnosticBase
	{
		public override SyntaxKind DeclarationKind => SyntaxKind.FieldDeclaration;

		protected override void DiagnosticAction(SyntaxNodeAnalysisContext context)
		{
			// 获取语义模型
			SemanticModel semanticModel = context.SemanticModel;

			FieldDeclarationSyntax fieldDeclaration = (FieldDeclarationSyntax)context.Node;

			if (AnalyzerSetting.ProjectDiagnostics.TryGetValue(context.Compilation.AssemblyName, out List<ObjectDiagnostic> objectDiagnostics))
			{
				foreach (ObjectDiagnostic objectDiagnostic in objectDiagnostics)
				{
					//获取当前字段的类型
					ITypeSymbol fieldTypeSymbol = semanticModel.GetTypeInfo(fieldDeclaration.Declaration.Type).Type;
					if (objectDiagnostic.Screen(fieldTypeSymbol))
					{
						if (objectDiagnostic.CodeDiagnostics.TryGetValue(DiagnosticKey.ClassFieldNaming, out CodeDiagnosticConfig codeDiagnostic))
						{
							foreach (var variable in fieldDeclaration.Declaration.Variables)
							{
								if (!codeDiagnostic.Check.Invoke(variable.Identifier.Text))
								{
									context.ReportDiagnostic(Diagnostic.Create(codeDiagnostic.Diagnostic, variable.GetLocation(), variable.Identifier.Text));
								}
							}
						}
					}

					//获取当前字段所在的类型名称
					BaseTypeDeclarationSyntax parentType = TreeSyntaxHelper.GetParentType(fieldDeclaration);
					INamedTypeSymbol? typeSymbol = semanticModel.GetDeclaredSymbol(parentType);
					if (objectDiagnostic.Screen(typeSymbol))
					{
						foreach (CodeDiagnosticConfig codeDiagnostic in objectDiagnostic.CodeDiagnostics.Values)
						{
							// 字段声明
							if (codeDiagnostic.DeclarationKind != SyntaxKind.FieldDeclaration) continue;
							// 需要的修饰符
							if (!TreeSyntaxHelper.SyntaxKindContains(fieldDeclaration.Modifiers, codeDiagnostic.KeywordKinds)) continue;
							// 不需要检查的修饰符
							if (TreeSyntaxHelper.SyntaxKindContainsAny(fieldDeclaration.Modifiers, codeDiagnostic.UnKeywordKinds, false)) continue;

							// 检查属性名是否符合规范
							foreach (var variable in fieldDeclaration.Declaration.Variables)
							{
								if (!codeDiagnostic.Check.Invoke(variable.Identifier.Text) || (codeDiagnostic.NeedComment && !TreeSyntaxHelper.CheckSummaryComment(fieldDeclaration)))
								{
									context.ReportDiagnostic(Diagnostic.Create(codeDiagnostic.Diagnostic, variable.GetLocation(), variable.Identifier.Text));
								}
							}
						}
						return;
					}
				}
			}
		}


	}

	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FieldNamingCodeFixProvider)), Shared]
	public class FieldNamingCodeFixProvider : NamingCodeFixProviderBase<VariableDeclaratorSyntax>
	{
		public override SyntaxKind DeclarationKind => SyntaxKind.FieldDeclaration;

		protected override async Task<Document> CodeFix(CodeDiagnosticConfig codeDiagnostic, Document document, VariableDeclaratorSyntax decl, CancellationToken cancellationToken)
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