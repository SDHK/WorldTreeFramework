/****************************************

* 作者：闪电黑客
* 日期：2024/6/24 10:53

* 描述：

*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Composition;
using WorldTree.SourceGenerator;

namespace WorldTree.Analyzer
{
	/// <summary>
	/// 结构体命名规范诊断器
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class StructNamingDiagnostic : DiagnosticAnalyzer
	{
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
			=> ImmutableArray.Create(AnalyzerSetting.GetDiagnosticDescriptors(SyntaxKind.StructDeclaration));

		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();
			context.RegisterSyntaxNodeAction(AnalyzeStructDeclaration, SyntaxKind.StructDeclaration);
		}

		private void AnalyzeStructDeclaration(SyntaxNodeAnalysisContext context)
		{
			// 获取语义模型
			SemanticModel semanticModel = context.SemanticModel;

			StructDeclarationSyntax structDeclaration = (StructDeclarationSyntax)context.Node;

			if (AnalyzerSetting.ProjectDiagnostics.TryGetValue(context.Compilation.AssemblyName, out List<ObjectDiagnostic> objectDiagnostics))
			{
				foreach (ObjectDiagnostic objectDiagnostic in objectDiagnostics)
				{
					//获取当前类的类型
					INamedTypeSymbol? typeSymbol = semanticModel.GetDeclaredSymbol(structDeclaration);
					if (!objectDiagnostic.Screen(typeSymbol)) continue;
					if (!objectDiagnostic.CodeDiagnostics.TryGetValue(DiagnosticKey.StructNaming, out CodeDiagnosticConfig codeDiagnostic)) continue;
					// 需要的修饰符
					if (!TreeSyntaxHelper.SyntaxKindContains(structDeclaration.Modifiers, codeDiagnostic.KeywordKinds)) continue;
					// 不需要检查的修饰符
					if (TreeSyntaxHelper.SyntaxKindContains(structDeclaration.Modifiers, codeDiagnostic.UnKeywordKinds, false)) continue;
					if (!codeDiagnostic.Check.Invoke(structDeclaration.Identifier.Text))
					{
						context.ReportDiagnostic(Diagnostic.Create(codeDiagnostic.Diagnostic, structDeclaration.GetLocation(), structDeclaration.Identifier.Text));
					}
					return;
				}
			}
		}
	}

	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StructNamingCodeFixProvider)), Shared]
	public class StructNamingCodeFixProvider : CodeFixProvider
	{
		public override ImmutableArray<string> FixableDiagnosticIds
			 => ImmutableArray.Create(AnalyzerSetting.GetDiagnosticDescriptorsId(SyntaxKind.StructDeclaration));

		public override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
			Diagnostic diagnostic = context.Diagnostics[0];
			var diagnosticSpan = diagnostic.Location.SourceSpan;
			var projectName = context.Document.Project.AssemblyName;
			if (!AnalyzerSetting.ProjectDiagnostics.TryGetValue(projectName, out _)) return;
			StructDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf().OfType<StructDeclarationSyntax>().FirstOrDefault();

			// 根据不同的诊断类型注册不同的代码修复
			if (AnalyzerSetting.TryFindDiagnosticDescriptor(diagnostic.Id, out CodeDiagnosticConfig codeDiagnostic))
			{
				context.RegisterCodeFix(
					CodeAction.Create(title: codeDiagnostic.CodeFixTitle,
					createChangedDocument: c => CodeFix(codeDiagnostic, context.Document, declaration, c),
					equivalenceKey: codeDiagnostic.Diagnostic.Id),
					diagnostic);
			}
		}

		private async Task<Document> CodeFix(CodeDiagnosticConfig codeDiagnostic, Document document, StructDeclarationSyntax methodDecl, CancellationToken cancellationToken)
		{
			var fieldName = methodDecl.Identifier.Text;

			fieldName = codeDiagnostic.FixCode?.Invoke(fieldName);

			// 创建新的字段名并替换旧的字段名
			var newFieldDecl = methodDecl.WithIdentifier(SyntaxFactory.Identifier(fieldName));
			var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
			var newRoot = root.ReplaceNode(methodDecl, newFieldDecl);

			// 返回包含修改的文档
			return document.WithSyntaxRoot(newRoot);
		}
	}
}