/****************************************

* 作者：闪电黑客
* 日期：2024/6/25 11:27

* 描述：

*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WorldTree.Analyzer
{
	/// <summary>
	/// 局部变量命名规范诊断器
	/// </summary>
	public abstract class LocalVariableNamingDiagnostic<C> : NamingDiagnosticBase<C>
		where C : ProjectDiagnosticConfig, new()
	{
		public override SyntaxKind DeclarationKind => SyntaxKind.LocalDeclarationStatement;

		protected override void DiagnosticAction(SyntaxNodeAnalysisContext context)
		{
			if (!TryGetDiagnosticConfigGroup(context.Compilation.AssemblyName, out List<DiagnosticConfigGroup> DiagnosticGroups)) return;
			DiagnosticLocalVariable(context, DiagnosticGroups);
			DiagnosticClassName(context, DiagnosticGroups);
		}

		private void DiagnosticClassName(SyntaxNodeAnalysisContext context, List<DiagnosticConfigGroup> DiagnosticGroups)
		{
			SemanticModel semanticModel = context.SemanticModel;
			LocalDeclarationStatementSyntax localVariableDeclaration = (LocalDeclarationStatementSyntax)context.Node;
			ITypeSymbol localVariableSymbol = semanticModel.GetTypeInfo(localVariableDeclaration.Declaration.Type).Type;
			foreach (DiagnosticConfigGroup objectDiagnostic in DiagnosticGroups)
			{
				if (objectDiagnostic.Screen(context.Compilation, localVariableSymbol))
				{
					if (objectDiagnostic.Diagnostics.TryGetValue(DiagnosticKey.ClassLocalVariableNaming, out DiagnosticConfig DiagnosticConfig))
					{
						foreach (var variable in localVariableDeclaration.Declaration.Variables)
						{
							if (!DiagnosticConfig.Check.Invoke(semanticModel, variable.Identifier))
							{
								context.ReportDiagnostic(Diagnostic.Create(DiagnosticConfig.Diagnostic, variable.GetLocation(), variable.Identifier.Text));
							}
						}
					}
					return;
				}
			}
		}

		private void DiagnosticLocalVariable(SyntaxNodeAnalysisContext context, List<DiagnosticConfigGroup> DiagnosticGroups)
		{
			// 获取语义模型
			SemanticModel semanticModel = context.SemanticModel;
			LocalDeclarationStatementSyntax? localDeclaration = context.Node as LocalDeclarationStatementSyntax;
			foreach (DiagnosticConfigGroup objectDiagnostic in DiagnosticGroups)
			{
				if (objectDiagnostic.Diagnostics.TryGetValue(DiagnosticKey.LocalVariableNaming, out DiagnosticConfig codeDiagnostic))
				{
					// 需要的修饰符
					if (TreeSyntaxHelper.SyntaxKindContains(localDeclaration.Modifiers, codeDiagnostic.KeywordKinds))
					{
						// 不需要检查的修饰符
						if (!TreeSyntaxHelper.SyntaxKindContainsAny(localDeclaration.Modifiers, codeDiagnostic.UnKeywordKinds, false))
						{
							//检查变量名
							foreach (VariableDeclaratorSyntax variable in localDeclaration.Declaration.Variables)
							{
								if (!codeDiagnostic.Check.Invoke(semanticModel, variable.Identifier))
								{
									context.ReportDiagnostic(Diagnostic.Create(codeDiagnostic.Diagnostic, variable.GetLocation(), variable.Identifier.Text));
								}
							}
							if (codeDiagnostic.NeedComment.Invoke(semanticModel, default) && !TreeSyntaxHelper.CheckComment(localDeclaration))
							{
								context.ReportDiagnostic(Diagnostic.Create(codeDiagnostic.Diagnostic, localDeclaration.GetLocation()));
							}
						}
					}
					return;
				}
			}
		}
	}

	public abstract class LocalVariableNamingProvider<C> : NamingCodeFixProviderBase<LocalDeclarationStatementSyntax, C>
		where C : ProjectDiagnosticConfig, new()
	{
		public override SyntaxKind DeclarationKind => SyntaxKind.LocalDeclarationStatement;
		protected override async Task<Document> CodeFix(DiagnosticConfig codeDiagnostic, Document document, LocalDeclarationStatementSyntax decl, CancellationToken cancellationToken)
		{
			var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
			var newRoot = root;
			foreach (VariableDeclaratorSyntax variable in decl.Declaration.Variables)
			{
				var fieldName = variable.Identifier.Text;
				fieldName = codeDiagnostic.FixCode?.Invoke(fieldName);

				// 创建新的字段名并替换旧的字段名
				var newFieldDecl = variable.WithIdentifier(SyntaxFactory.Identifier(fieldName));
				newRoot = newRoot.ReplaceNode(variable, newFieldDecl);
			}

			// 返回包含修改的文档
			return document.WithSyntaxRoot(newRoot);
		}
	}
}