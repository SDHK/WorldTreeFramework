/****************************************

* 作者：闪电黑客
* 日期：2024/6/21 18:14

* 描述：

*/
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WorldTree.SourceGenerator;
using Microsoft.CodeAnalysis.CodeFixes;
using System.Composition;
using Microsoft.CodeAnalysis.CodeActions;

namespace WorldTree.Analyzer
{
	/// <summary>
	/// 类型命名规范诊断器
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class ClassNamingDiagnostic : DiagnosticAnalyzer
	{
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
			=> ImmutableArray.Create(AnalyzerSetting.GetDiagnosticDescriptors(SyntaxKind.ClassDeclaration));

		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();
			context.RegisterSyntaxNodeAction(AnalyzeClassDeclaration, SyntaxKind.ClassDeclaration);
		}

		private void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
		{
			// 获取语义模型
			SemanticModel semanticModel = context.SemanticModel;

			ClassDeclarationSyntax classDeclaration = (ClassDeclarationSyntax)context.Node;

			if (AnalyzerSetting.ProjectDiagnostics.TryGetValue(context.Compilation.AssemblyName, out List<ObjectDiagnostic> objectDiagnostics))
			{
				foreach (ObjectDiagnostic objectDiagnostic in objectDiagnostics)
				{
					//获取当前类的类型
					INamedTypeSymbol? typeSymbol = semanticModel.GetDeclaredSymbol(classDeclaration);
					if (!objectDiagnostic.Screen(typeSymbol)) continue;
					if (!objectDiagnostic.CodeDiagnostics.TryGetValue(DiagnosticKey.ClassNaming, out CodeDiagnosticConfig codeDiagnostic)) continue;
					if (!codeDiagnostic.Check.Invoke(classDeclaration.Identifier.Text))
					{
						context.ReportDiagnostic(Diagnostic.Create(codeDiagnostic.Diagnostic, classDeclaration.GetLocation(), classDeclaration.Identifier.Text));
					}
					return;
				}
			}
		}
	}

	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ClassNamingCodeFixProvider)), Shared]
	public class ClassNamingCodeFixProvider : CodeFixProvider
	{
		public override ImmutableArray<string> FixableDiagnosticIds
			 => ImmutableArray.Create(AnalyzerSetting.GetDiagnosticDescriptorsId(SyntaxKind.ClassDeclaration));


		public override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
			Diagnostic diagnostic = context.Diagnostics[0];
			var diagnosticSpan = diagnostic.Location.SourceSpan;
			var projectName = context.Document.Project.AssemblyName;
			if (!AnalyzerSetting.ProjectDiagnostics.TryGetValue(projectName, out _)) return;
			ClassDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().FirstOrDefault();

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

		private async Task<Document> CodeFix(CodeDiagnosticConfig codeDiagnostic, Document document, ClassDeclarationSyntax methodDecl, CancellationToken cancellationToken)
		{
			// 实现将字段名修改为camelCase的逻辑
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