/****************************************

* 作者：闪电黑客
* 日期：2024/6/18 16:35

* 描述：字段命名规范诊断器

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
	/// 字段命名规范诊断器
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class FieldNamingAnalyzer : DiagnosticAnalyzer
	{
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(new DiagnosticDescriptor[]
			 {
				 PublicFieldNamingDiagnosticRule.Rule,
				 PrivateFieldNamingDiagnosticRule.Rule,
				 ProtectedFieldNamingDiagnosticRule.Rule
			 });

		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();
			context.RegisterSyntaxNodeAction(AnalyzeFieldDeclaration, SyntaxKind.FieldDeclaration);
		}

		private void AnalyzeFieldDeclaration(SyntaxNodeAnalysisContext context)
		{
			FieldDeclarationSyntax fieldDeclaration = (FieldDeclarationSyntax)context.Node;

			if (AnalyzerSetting.ProjectAnalyzers.TryGetValue(context.Compilation.AssemblyName, out DiagnosticKey projectAnalyzer))
			{
				if (fieldDeclaration.Modifiers.Any(SyntaxKind.PublicKeyword))
				{
					CheckField(context, projectAnalyzer, DiagnosticKey.PublicFieldNaming, PublicFieldNamingDiagnosticRule.Rule);
				}
				else if (TreeSyntaxHelper.ModifiersCheckPrivateKeyword(fieldDeclaration.Modifiers))
				{
					CheckField(context, projectAnalyzer, DiagnosticKey.PrivateFieldNaming, PrivateFieldNamingDiagnosticRule.Rule);
				}
				else if (fieldDeclaration.Modifiers.Any(SyntaxKind.ProtectedKeyword))
				{
					CheckField(context, projectAnalyzer, DiagnosticKey.ProtectedFieldNaming, ProtectedFieldNamingDiagnosticRule.Rule);
				}
			}
		}

		/// <summary>
		/// 检查字段
		/// </summary>
		private void CheckField(SyntaxNodeAnalysisContext context, DiagnosticKey projectAnalyzer, DiagnosticKey diagnosticKey, DiagnosticDescriptor Rule)
		{
			if (projectAnalyzer.HasFlag(diagnosticKey))
			{
				if (AnalyzerSetting.AnalyzerChecks.TryGetValue(diagnosticKey, out var checks))
				{
					FieldDeclarationSyntax fieldDeclaration = (FieldDeclarationSyntax)context.Node;

					foreach (var variable in fieldDeclaration.Declaration.Variables)
					{
						if (!checks.Invoke(variable.Identifier.Text)) context.ReportDiagnostic(Diagnostic.Create(Rule, variable.GetLocation(), variable.Identifier.Text));
					}
				}
			}
		}
	}

	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FieldNamingCodeFixProvider)), Shared]
	public class FieldNamingCodeFixProvider : CodeFixProvider
	{
		public override sealed ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(new string[]
				{
					DiagnosticKey.PublicFieldNaming.ToString(),
					DiagnosticKey.PrivateFieldNaming.ToString(),
					DiagnosticKey.ProtectedFieldNaming.ToString()
				});

		public override sealed FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

		public override sealed async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
			Diagnostic diagnostic = context.Diagnostics[0];
			var diagnosticSpan = diagnostic.Location.SourceSpan;

			var projectName = context.Document.Project.AssemblyName;
			if (!AnalyzerSetting.ProjectAnalyzers.TryGetValue(projectName, out DiagnosticKey projectAnalyzer)) return;

			// 找到需要修复的字段声明
			VariableDeclaratorSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<VariableDeclaratorSyntax>().First();

			if (diagnostic.Id == DiagnosticKey.PublicFieldNaming.ToString())
			{
				RegisterCodeFix(context, diagnostic, declaration, DiagnosticKey.PublicFieldNaming, EquivalenceKey.PublicFieldNamingFix, PublicFieldNamingDiagnosticRule.CodeFixTitle);
			}
			else if (diagnostic.Id == DiagnosticKey.PrivateFieldNaming.ToString())
			{
				RegisterCodeFix(context, diagnostic, declaration, DiagnosticKey.PrivateFieldNaming, EquivalenceKey.PrivateFieldNamingFix, PrivateFieldNamingDiagnosticRule.CodeFixTitle);
			}
			else if (diagnostic.Id == DiagnosticKey.ProtectedFieldNaming.ToString())
			{
				RegisterCodeFix(context, diagnostic, declaration, DiagnosticKey.ProtectedFieldNaming, EquivalenceKey.ProtectedFieldNamingFix, ProtectedFieldNamingDiagnosticRule.CodeFixTitle);
			}
		}

		/// <summary>
		/// 注册代码修复
		/// </summary>
		private void RegisterCodeFix(CodeFixContext context, Diagnostic diagnostic, VariableDeclaratorSyntax declaration, DiagnosticKey DiagnosticKey, EquivalenceKey equivalence, string CodeFixTitle)
		{
			context.RegisterCodeFix(
			CodeAction.Create(title: CodeFixTitle,
				createChangedDocument: c => CodeFix(DiagnosticKey, context.Document, declaration, c),
				equivalenceKey: equivalence.ToString()),
			diagnostic);
		}

		private async Task<Document> CodeFix(DiagnosticKey projectAnalyze, Document document, VariableDeclaratorSyntax fieldDecl, CancellationToken cancellationToken)
		{
			// 实现将字段名修改为camelCase的逻辑
			var fieldName = fieldDecl.Identifier.Text;

			if (AnalyzerSetting.AnalyzerCodeFix.TryGetValue(projectAnalyze, out var codeFix)) fieldName = codeFix?.Invoke(fieldName);

			// 创建新的字段名并替换旧的字段名
			var newFieldDecl = fieldDecl.WithIdentifier(SyntaxFactory.Identifier(fieldName));
			var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
			var newRoot = root.ReplaceNode(fieldDecl, newFieldDecl);

			// 返回包含修改的文档
			return document.WithSyntaxRoot(newRoot);
		}
	}
}