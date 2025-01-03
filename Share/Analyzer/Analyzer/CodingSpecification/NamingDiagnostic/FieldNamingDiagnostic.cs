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
using System.Collections.Generic;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;

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
			if (!ProjectDiagnosticSetting.ProjectDiagnostics.TryGetValue(context.Compilation.AssemblyName, out List<DiagnosticConfigGroup> objectDiagnostics)) return;
			DiagnosticField(context, DiagnosticKey.PublicFieldNaming, objectDiagnostics);
			DiagnosticField(context, DiagnosticKey.PrivateFieldNaming, objectDiagnostics);
			DiagnosticField(context, DiagnosticKey.ProtectedFieldNaming, objectDiagnostics);
			DiagnosticField(context, DiagnosticKey.ConstNaming, objectDiagnostics);
			DiagnosticClassName(context, objectDiagnostics);
		}

		private void DiagnosticClassName(SyntaxNodeAnalysisContext context, List<DiagnosticConfigGroup> objectDiagnostics)
		{
			SemanticModel semanticModel = context.SemanticModel;
			FieldDeclarationSyntax fieldDeclaration = (FieldDeclarationSyntax)context.Node;
			ITypeSymbol fieldTypeSymbol = semanticModel.GetTypeInfo(fieldDeclaration.Declaration.Type).Type;
			foreach (DiagnosticConfigGroup objectDiagnostic in objectDiagnostics)
			{
				if (objectDiagnostic.Screen(fieldTypeSymbol))
				{
					if (objectDiagnostic.Diagnostics.TryGetValue(DiagnosticKey.ClassFieldNaming, out DiagnosticConfig DiagnosticConfig))
					{
						foreach (var variable in fieldDeclaration.Declaration.Variables)
						{
							if (!DiagnosticConfig.Check.Invoke(semanticModel, variable.Identifier))
							{
								context.ReportDiagnostic(Diagnostic.Create(DiagnosticConfig.Diagnostic, variable.GetLocation(), variable.Identifier.Text));
							}
						}
					}
				}
			}
		}

		private void DiagnosticField(SyntaxNodeAnalysisContext context, DiagnosticKey diagnosticKey, List<DiagnosticConfigGroup> objectDiagnostics)
		{
			// 获取语义模型
			SemanticModel semanticModel = context.SemanticModel;

			FieldDeclarationSyntax fieldDeclaration = (FieldDeclarationSyntax)context.Node;

			//获取当前字段的类型
			ITypeSymbol fieldTypeSymbol = semanticModel.GetTypeInfo(fieldDeclaration.Declaration.Type).Type;

			//获取当前字段所在的类型名称
			BaseTypeDeclarationSyntax parentType = TreeSyntaxHelper.GetParentType(fieldDeclaration);
			INamedTypeSymbol? typeSymbol = semanticModel.GetDeclaredSymbol(parentType);
			foreach (DiagnosticConfigGroup objectDiagnostic in objectDiagnostics)
			{
				if (!objectDiagnostic.Screen(typeSymbol)) continue;
				if (objectDiagnostic.Diagnostics.TryGetValue(diagnosticKey, out DiagnosticConfig codeDiagnostic))
				{
					// 需要的修饰符
					if (TreeSyntaxHelper.SyntaxKindContains(fieldDeclaration.Modifiers, codeDiagnostic.KeywordKinds))
					{
						// 不需要检查的修饰符
						if (!TreeSyntaxHelper.SyntaxKindContainsAny(fieldDeclaration.Modifiers, codeDiagnostic.UnKeywordKinds, false))
						{
							// 检查属性名是否符合规范
							foreach (VariableDeclaratorSyntax variable in fieldDeclaration.Declaration.Variables)
							{
								//获取字段类型名称


								if (!codeDiagnostic.Check.Invoke(semanticModel, variable.Identifier))
								{
									context.ReportDiagnostic(Diagnostic.Create(codeDiagnostic.Diagnostic, variable.GetLocation(), variable.Identifier.Text));
								}
							}
							if (codeDiagnostic.NeedComment && !TreeSyntaxHelper.CheckSummaryComment(fieldDeclaration))
							{
								context.ReportDiagnostic(Diagnostic.Create(codeDiagnostic.Diagnostic, fieldDeclaration.GetLocation()));
							}
						}
					}
					return;
				}
			}
		}
	}

	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FieldNamingCodeFixProvider)), Shared]
	public class FieldNamingCodeFixProvider : NamingCodeFixProviderBase<VariableDeclaratorSyntax>
	{
		public override SyntaxKind DeclarationKind => SyntaxKind.FieldDeclaration;

		protected override async Task<Document> CodeFix(DiagnosticConfig codeDiagnostic, Document document, VariableDeclaratorSyntax decl, CancellationToken cancellationToken)
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