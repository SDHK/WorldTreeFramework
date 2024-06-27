﻿/****************************************

* 作者：闪电黑客
* 日期：2024/6/21 18:14

* 描述：

*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WorldTree.Analyzer
{
	/// <summary>
	/// 类型命名规范诊断器
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class ClassNamingDiagnostic : NamingDiagnosticBase
	{
		public override SyntaxKind DeclarationKind => SyntaxKind.ClassDeclaration;

		protected override void DiagnosticAction(SyntaxNodeAnalysisContext context)
		{
			if (!ProjectDiagnosticSetting.ProjectDiagnostics.TryGetValue(context.Compilation.AssemblyName, out List<DiagnosticConfigGroup> DiagnosticGroups)) return;

			// 获取语义模型
			SemanticModel semanticModel = context.SemanticModel;

			ClassDeclarationSyntax classDeclaration = (ClassDeclarationSyntax)context.Node;

			foreach (DiagnosticConfigGroup DiagnosticGroup in DiagnosticGroups)
			{
				//获取当前类的类型
				INamedTypeSymbol? typeSymbol = semanticModel.GetDeclaredSymbol(classDeclaration);
				if (!DiagnosticGroup.Screen(typeSymbol)) continue;
				if (DiagnosticGroup.Diagnostics.TryGetValue(DiagnosticKey.ClassNaming, out DiagnosticConfig codeDiagnostic))
				{
					// 需要的修饰符
					if (TreeSyntaxHelper.SyntaxKindContains(classDeclaration.Modifiers, codeDiagnostic.KeywordKinds))
					{
						// 不需要检查的修饰符
						if (!TreeSyntaxHelper.SyntaxKindContainsAny(classDeclaration.Modifiers, codeDiagnostic.UnKeywordKinds, false))
						{
							if (!codeDiagnostic.Check.Invoke(classDeclaration.Identifier.Text))
							{
								context.ReportDiagnostic(Diagnostic.Create(codeDiagnostic.Diagnostic, classDeclaration.GetLocation(), classDeclaration.Identifier.Text));
							}
							//检查是否需要添加注释
							else if (codeDiagnostic.NeedComment)
							{
								//判断是否是部分类
								if (classDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword))
								{
									// 获取所有部分类的语法声明
									IEnumerable<ClassDeclarationSyntax> partialDeclarations = typeSymbol.DeclaringSyntaxReferences.Select(syntaxRef => syntaxRef.GetSyntax()).OfType<ClassDeclarationSyntax>();
									bool isComment = false;

									// 遍历所有部分类
									foreach (ClassDeclarationSyntax partialDecl in partialDeclarations)
									{
										if (TreeSyntaxHelper.CheckSummaryComment(partialDecl))
										{
											isComment = true;
											break;
										}
									}
									if (!isComment)
									{
										context.ReportDiagnostic(Diagnostic.Create(codeDiagnostic.Diagnostic, classDeclaration.GetLocation(), classDeclaration.Identifier.Text));
									}
								}
								else if (!TreeSyntaxHelper.CheckSummaryComment(classDeclaration))
								{
									context.ReportDiagnostic(Diagnostic.Create(codeDiagnostic.Diagnostic, classDeclaration.GetLocation(), classDeclaration.Identifier.Text));
								}
							}
						}
					}
					return;
				}
			}
		}
	}

	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ClassNamingCodeFixProvider)), Shared]
	public class ClassNamingCodeFixProvider : NamingCodeFixProviderBase<ClassDeclarationSyntax>
	{
		public override SyntaxKind DeclarationKind => SyntaxKind.ClassDeclaration;

		protected override async Task<Document> CodeFix(DiagnosticConfig codeDiagnostic, Document document, ClassDeclarationSyntax decl, CancellationToken cancellationToken)
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