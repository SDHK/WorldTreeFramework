/****************************************

* 作者：闪电黑客
* 日期：2024/6/18 16:35

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
using System.Linq;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;

namespace WorldTree.Analyzer
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class FieldNamingAnalyzer : DiagnosticAnalyzer
	{
		private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
			"FieldNamingAnalyzer",
			"命名规则",
			"'{0}' 字段命名错误",
			"命名",
			DiagnosticSeverity.Warning,
			isEnabledByDefault: true);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();
			context.RegisterSyntaxNodeAction(AnalyzeFieldDeclaration, SyntaxKind.FieldDeclaration);
		}

		private void AnalyzeFieldDeclaration(SyntaxNodeAnalysisContext context)
		{
			// 从编译单元中获取项目名称
			var projectName = context.Compilation.AssemblyName;
			if (!AnalyzerSetting.ProjectAnalyzers.TryGetValue(projectName, out ProjectAnalyzer projectAnalyzer)) return;

			var fieldDeclaration = (FieldDeclarationSyntax)context.Node;

			if (projectAnalyzer.HasFlag(ProjectAnalyzer.PublicField))
			{
				if (AnalyzerSetting.AnalyzerChecks.TryGetValue(ProjectAnalyzer.PublicField, out var checks) && fieldDeclaration.Modifiers.Any(SyntaxKind.PublicKeyword))
				{
					foreach (var variable in fieldDeclaration.Declaration.Variables)
					{
						if (checks.Invoke(variable.Identifier.Text)) context.ReportDiagnostic(Diagnostic.Create(Rule, variable.GetLocation(), variable.Identifier.Text));
					}
				}
			}

			if (projectAnalyzer.HasFlag(ProjectAnalyzer.ProtectedField))
			{
				if (AnalyzerSetting.AnalyzerChecks.TryGetValue(ProjectAnalyzer.ProtectedField, out var checks) && fieldDeclaration.Modifiers.Any(SyntaxKind.ProtectedKeyword))
				{
					foreach (var variable in fieldDeclaration.Declaration.Variables)
					{
						if (checks.Invoke(variable.Identifier.Text)) context.ReportDiagnostic(Diagnostic.Create(Rule, variable.GetLocation(), variable.Identifier.Text));
					}
				}
			}

			if (projectAnalyzer.HasFlag(ProjectAnalyzer.PrivateField))
			{
				if (AnalyzerSetting.AnalyzerChecks.TryGetValue(ProjectAnalyzer.PrivateField, out var checks) && fieldDeclaration.Modifiers.Any(SyntaxKind.PrivateKeyword))
				{
					foreach (var variable in fieldDeclaration.Declaration.Variables)
					{
						if (checks.Invoke(variable.Identifier.Text)) context.ReportDiagnostic(Diagnostic.Create(Rule, variable.GetLocation(), variable.Identifier.Text));
					}
				}
			}
		}
	}

	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FieldNamingCodeFixProvider)), Shared]
	public class FieldNamingCodeFixProvider : CodeFixProvider
	{
		public override sealed ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("FieldNamingAnalyzer");

		public override sealed FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

		public override sealed async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
			var diagnostic = context.Diagnostics[0];
			var diagnosticSpan = diagnostic.Location.SourceSpan;

			// 找到需要修复的字段声明
			var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<VariableDeclaratorSyntax>().First();

			// 注册一个代码修复动作
			context.RegisterCodeFix(
				CodeAction.Create(
					title: "修正命名",
					createChangedDocument: c => MakeCamelCaseAsync(context.Document, declaration, c),
					equivalenceKey: "修正命名"),//camelCase
				diagnostic);
		}

		private async Task<Microsoft.CodeAnalysis.Document> MakeCamelCaseAsync(Microsoft.CodeAnalysis.Document document, VariableDeclaratorSyntax fieldDecl, CancellationToken cancellationToken)
		{
			// 实现将字段名修改为camelCase的逻辑
			var fieldName = fieldDecl.Identifier.Text;

			//var camelCaseName = Char.ToLowerInvariant(fieldName[0]) + fieldName.Substring(1);
			var camelCaseName = Char.ToUpperInvariant(fieldName[0]) + fieldName.Substring(1);

			// 创建新的字段名并替换旧的字段名
			var newFieldDecl = fieldDecl.WithIdentifier(SyntaxFactory.Identifier(camelCaseName));
			var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
			var newRoot = root.ReplaceNode(fieldDecl, newFieldDecl);

			// 返回包含修改的文档
			return document.WithSyntaxRoot(newRoot);
		}
	}
}