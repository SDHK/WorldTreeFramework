﻿/****************************************

* 作者：闪电黑客
* 日期：2024/6/20 16:35

* 描述：字段命名规范诊断器

*/
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WorldTree.SourceGenerator;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using System.Composition;

namespace WorldTree.Analyzer
{

	/// <summary>
	/// 字段命名规范诊断器
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class FieldNamingDiagnostic : DiagnosticAnalyzer
	{
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(AnalyzerSetting.GetDiagnosticDescriptors(SyntaxKind.FieldDeclaration));

		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();
			context.RegisterSyntaxNodeAction(AnalyzeFieldDeclaration, SyntaxKind.FieldDeclaration);
		}
		private void AnalyzeFieldDeclaration(SyntaxNodeAnalysisContext context)
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
							if (TreeSyntaxHelper.SyntaxKindContains(fieldDeclaration.Modifiers, codeDiagnostic.UnKeywordKinds, false)) continue;

							// 检查属性名是否符合规范
							foreach (var variable in fieldDeclaration.Declaration.Variables)
							{
								if (!codeDiagnostic.Check.Invoke(variable.Identifier.Text))
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
	public class FieldNamingCodeFixProvider : CodeFixProvider
	{
		public override sealed ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(AnalyzerSetting.GetDiagnosticDescriptorsId(SyntaxKind.FieldDeclaration));

		public override sealed FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

		public override sealed async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
			Diagnostic diagnostic = context.Diagnostics[0];
			var diagnosticSpan = diagnostic.Location.SourceSpan;

			var projectName = context.Document.Project.AssemblyName;
			if (!AnalyzerSetting.ProjectDiagnostics.TryGetValue(projectName, out _)) return;

			// 找到需要修复的字段声明
			VariableDeclaratorSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<VariableDeclaratorSyntax>().First();

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

		private async Task<Document> CodeFix(CodeDiagnosticConfig codeDiagnostic, Document document, VariableDeclaratorSyntax fieldDecl, CancellationToken cancellationToken)
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