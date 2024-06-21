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
	/// <summary>
	/// 属性命名规范诊断器
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class PropertyNamingDiagnostic : DiagnosticAnalyzer
	{
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(AnalyzerSetting.GetDiagnosticDescriptors(SyntaxKind.PropertyDeclaration));
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();
			context.RegisterSyntaxNodeAction(AnalyzePropertyDeclaration, SyntaxKind.PropertyDeclaration);
		}

		private void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
		{
			// 获取语义模型
			SemanticModel semanticModel = context.SemanticModel;

			PropertyDeclarationSyntax propertyDeclaration = (PropertyDeclarationSyntax)context.Node;

			if (AnalyzerSetting.ProjectDiagnostics.TryGetValue(context.Compilation.AssemblyName, out List<ObjectDiagnostic> objectDiagnostics))
			{
				foreach (ObjectDiagnostic objectDiagnostic in objectDiagnostics)
				{

					//获取当前属性的类型
					ITypeSymbol fieldTypeSymbol = semanticModel.GetTypeInfo(propertyDeclaration.Type).Type;
					if (objectDiagnostic.Screen(fieldTypeSymbol))
					{
						if (objectDiagnostic.CodeDiagnostics.TryGetValue(DiagnosticKey.ClassPropertyNaming, out CodeDiagnosticConfig codeDiagnostic))
						{
							if (!codeDiagnostic.Check.Invoke(propertyDeclaration.Identifier.Text))
							{
								context.ReportDiagnostic(Diagnostic.Create(codeDiagnostic.Diagnostic, propertyDeclaration.GetLocation(), propertyDeclaration.Identifier.Text));
							}
						}
					}

					//获取当前属性所在的类型名称
					BaseTypeDeclarationSyntax parentType = TreeSyntaxHelper.GetParentType(propertyDeclaration);
					INamedTypeSymbol? typeSymbol = semanticModel.GetDeclaredSymbol(parentType);
					if (objectDiagnostic.Screen(typeSymbol))
					{
						foreach (CodeDiagnosticConfig codeDiagnostic in objectDiagnostic.CodeDiagnostics.Values)
						{
							if (codeDiagnostic.DeclarationKind != SyntaxKind.PropertyDeclaration) continue;
							if (TreeSyntaxHelper.SyntaxKindContains(propertyDeclaration.Modifiers, codeDiagnostic.KeywordSyntaxKinds))
							{
								if (!codeDiagnostic.Check.Invoke(propertyDeclaration.Identifier.Text))
								{
									context.ReportDiagnostic(Diagnostic.Create(codeDiagnostic.Diagnostic, propertyDeclaration.GetLocation(), propertyDeclaration.Identifier.Text));
								}
							}
						}
						return;
					}
				}
			}
		}
	}

	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PropertyNamingCodeFixProvider)), Shared]
	public class PropertyNamingCodeFixProvider : CodeFixProvider
	{
		public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(AnalyzerSetting.GetDiagnosticDescriptorsId(SyntaxKind.PropertyDeclaration));

		public override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
			Diagnostic diagnostic = context.Diagnostics[0];
			var diagnosticSpan = diagnostic.Location.SourceSpan;
			var projectName = context.Document.Project.AssemblyName;
			if (!AnalyzerSetting.ProjectDiagnostics.TryGetValue(projectName, out _)) return;

			// 找到需要修复的字段声明
			PropertyDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf().OfType<PropertyDeclarationSyntax>().FirstOrDefault();

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

		private async Task<Document> CodeFix(CodeDiagnosticConfig codeDiagnostic, Document document, PropertyDeclarationSyntax fieldDecl, CancellationToken cancellationToken)
		{
			// 实现将字段名修改为camelCase的逻辑
			var fieldName = fieldDecl.Identifier.Text;

			fieldName = codeDiagnostic.FixCode?.Invoke(fieldName);

			// 创建新的字段名并替换旧的字段名
			var newFieldDecl = fieldDecl.WithIdentifier(SyntaxFactory.Identifier(fieldName));
			var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
			var newRoot = root.ReplaceNode(fieldDecl, newFieldDecl);

			// 返回包含修改的文档
			return document.WithSyntaxRoot(newRoot);
		}
	}
}