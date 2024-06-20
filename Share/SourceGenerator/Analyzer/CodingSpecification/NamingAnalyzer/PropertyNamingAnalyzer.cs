/****************************************

* 作者：闪电黑客
* 日期：2024/6/19 19:49

* 描述：属性命名规范诊断器

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
	///// <summary>
	///// 属性命名规范诊断器
	///// </summary>
	//[DiagnosticAnalyzer(LanguageNames.CSharp)]
	//public class PropertyNamingAnalyzer : DiagnosticAnalyzer
	//{
	//	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(new DiagnosticDescriptor[]
	//	 {
	//			 PublicPropertyNamingDiagnosticRule.Rule,
	//			 PrivatePropertyNamingDiagnosticRule.Rule,
	//			 ProtectedPropertyNamingDiagnosticRule.Rule
	//	 });

	//	public override void Initialize(AnalysisContext context)
	//	{
	//		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
	//		context.EnableConcurrentExecution();
	//		context.RegisterSyntaxNodeAction(AnalyzePropertyDeclaration, SyntaxKind.PropertyDeclaration);
	//	}

	//	private void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
	//	{
	//		PropertyDeclarationSyntax propertyDeclaration = (PropertyDeclarationSyntax)context.Node;

	//		if (AnalyzerSetting.ProjectAnalyzers.TryGetValue(context.Compilation.AssemblyName, out DiagnosticKey projectAnalyzer))
	//		{
	//			if (propertyDeclaration.Modifiers.Any(SyntaxKind.PublicKeyword))
	//			{
	//				CheckProperty(context, projectAnalyzer, DiagnosticKey.PublicPropertyNaming, PublicPropertyNamingDiagnosticRule.Rule);
	//			}
	//			else if (TreeSyntaxHelper.ModifiersCheckPrivateKeyword(propertyDeclaration.Modifiers))
	//			{
	//				CheckProperty(context, projectAnalyzer, DiagnosticKey.PrivatePropertyNaming, PrivatePropertyNamingDiagnosticRule.Rule);
	//			}
	//			else if (propertyDeclaration.Modifiers.Any(SyntaxKind.ProtectedKeyword))
	//			{
	//				CheckProperty(context, projectAnalyzer, DiagnosticKey.ProtectedPropertyNaming, ProtectedPropertyNamingDiagnosticRule.Rule);
	//			}
	//		}
	//	}

	//	private void CheckProperty(SyntaxNodeAnalysisContext context, DiagnosticKey projectAnalyzer, DiagnosticKey diagnosticKey, DiagnosticDescriptor rule)
	//	{
	//		if (projectAnalyzer.HasFlag(diagnosticKey))
	//		{
	//			PropertyDeclarationSyntax propertyDeclaration = (PropertyDeclarationSyntax)context.Node;

	//			if (AnalyzerSetting.AnalyzerChecks.TryGetValue(diagnosticKey, out var checks))
	//			{
	//				if (!checks.Invoke(propertyDeclaration.Identifier.Text)) context.ReportDiagnostic(Diagnostic.Create(rule, propertyDeclaration.GetLocation(), propertyDeclaration.Identifier.Text));
	//			}
	//		}
	//	}
	//}

	//[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PropertyNamingCodeFixProvider)), Shared]
	//public class PropertyNamingCodeFixProvider : CodeFixProvider
	//{
	//	public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
	//					DiagnosticKey.PublicPropertyNaming.ToString(),
	//					DiagnosticKey.PrivatePropertyNaming.ToString(),
	//					DiagnosticKey.ProtectedPropertyNaming.ToString()
	//	);

	//	public override async Task RegisterCodeFixesAsync(CodeFixContext context)
	//	{
	//		var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
	//		Diagnostic diagnostic = context.Diagnostics[0];
	//		var diagnosticSpan = diagnostic.Location.SourceSpan;
	//		var projectName = context.Document.Project.AssemblyName;
	//		if (!AnalyzerSetting.ProjectAnalyzers.TryGetValue(projectName, out DiagnosticKey projectAnalyzer)) return;

	//		PropertyDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf().OfType<PropertyDeclarationSyntax>().FirstOrDefault();
	//		if (diagnostic.Id == DiagnosticKey.PublicPropertyNaming.ToString())
	//		{
	//			RegisterCodeFix(context, diagnostic, declaration, DiagnosticKey.PublicPropertyNaming, EquivalenceKey.PublicPropertyNamingFix, PublicPropertyNamingDiagnosticRule.CodeFixTitle);
	//		}
	//		else if (diagnostic.Id == DiagnosticKey.PrivatePropertyNaming.ToString())
	//		{
	//			RegisterCodeFix(context, diagnostic, declaration, DiagnosticKey.PrivatePropertyNaming, EquivalenceKey.PrivatePropertyNamingFix, PrivatePropertyNamingDiagnosticRule.CodeFixTitle);
	//		}
	//		else if (diagnostic.Id == DiagnosticKey.ProtectedPropertyNaming.ToString())
	//		{
	//			RegisterCodeFix(context, diagnostic, declaration, DiagnosticKey.ProtectedPropertyNaming, EquivalenceKey.ProtectedPropertyNamingFix, ProtectedPropertyNamingDiagnosticRule.CodeFixTitle);
	//		}
	//	}

	//	/// <summary>
	//	/// 注册代码修复
	//	/// </summary>
	//	private void RegisterCodeFix(CodeFixContext context, Diagnostic diagnostic, PropertyDeclarationSyntax propertyDecl, DiagnosticKey DiagnosticKey, EquivalenceKey equivalence, string CodeFixTitle)
	//	{
	//		context.RegisterCodeFix(
	//		CodeAction.Create(title: CodeFixTitle,
	//			createChangedDocument: c => CodeFix(DiagnosticKey, context.Document, propertyDecl, c),
	//			equivalenceKey: equivalence.ToString()),
	//		diagnostic);
	//	}

	//	private async Task<Document> CodeFix(DiagnosticKey projectAnalyze, Document document, PropertyDeclarationSyntax propertyDecl, CancellationToken cancellationToken)
	//	{
	//		var fieldName = propertyDecl.Identifier.Text;

	//		if (AnalyzerSetting.AnalyzerCodeFix.TryGetValue(projectAnalyze, out var codeFix)) fieldName = codeFix?.Invoke(fieldName);

	//		// 创建新的字段名并替换旧的字段名
	//		var newFieldDecl = propertyDecl.WithIdentifier(SyntaxFactory.Identifier(fieldName));
	//		var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
	//		var newRoot = root.ReplaceNode(propertyDecl, newFieldDecl);

	//		// 返回包含修改的文档
	//		return document.WithSyntaxRoot(newRoot);
	//	}
	//}
}