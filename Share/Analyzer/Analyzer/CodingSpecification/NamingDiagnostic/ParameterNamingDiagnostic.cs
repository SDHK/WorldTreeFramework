/****************************************

* 作者：闪电黑客
* 日期：2024/6/24 16:16

* 描述：

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
	/// 方法参数命名规范诊断器
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class ParameterNamingDiagnostic : NamingDiagnosticBase
	{
		public override SyntaxKind DeclarationKind => SyntaxKind.Parameter;

		protected override void DiagnosticAction(SyntaxNodeAnalysisContext context)
		{
			if (!ProjectDiagnosticSetting.ProjectDiagnostics.TryGetValue(context.Compilation.AssemblyName, out List<DiagnosticConfigGroup> objectDiagnostics)) return;
			// 获取语义模型
			SemanticModel semanticModel = context.SemanticModel;
			ParameterSyntax parameter = (ParameterSyntax)context.Node;
			foreach (DiagnosticConfigGroup objectDiagnostic in objectDiagnostics)
			{
				//获取当前参数的类型
				ISymbol? typeSymbol = semanticModel.GetDeclaredSymbol(parameter);
				if (!objectDiagnostic.Screen(typeSymbol)) continue;
				if (objectDiagnostic.Diagnostics.TryGetValue(DiagnosticKey.ParameterNaming, out DiagnosticConfig codeDiagnostic))
				{
					// 需要的修饰符
					if (TreeSyntaxHelper.SyntaxKindContains(parameter.Modifiers, codeDiagnostic.KeywordKinds))
					{
						// 不需要检查的修饰符
						if (!TreeSyntaxHelper.SyntaxKindContainsAny(parameter.Modifiers, codeDiagnostic.UnKeywordKinds, false))
						{
							if (!codeDiagnostic.Check.Invoke(semanticModel, parameter.Identifier))
							{
								context.ReportDiagnostic(Diagnostic.Create(codeDiagnostic.Diagnostic, parameter.Identifier.GetLocation(), parameter.Identifier.Text));
							}
						}
					}
				}
			}
		}
	}
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ParameterNamingCodeFixProvider)), Shared]
	public class ParameterNamingCodeFixProvider : NamingCodeFixProviderBase<ParameterSyntax>
	{
		public override SyntaxKind DeclarationKind => SyntaxKind.Parameter;
		protected override async Task<Document> CodeFix(DiagnosticConfig codeDiagnostic, Document document, ParameterSyntax decl, CancellationToken cancellationToken)
		{
			var parameterName = decl.Identifier.Text;
			parameterName = codeDiagnostic.FixCode?.Invoke(parameterName);

			// 创建新的参数名并替换旧的参数名
			var newParameterDecl = decl.WithIdentifier(SyntaxFactory.Identifier(parameterName));
			var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
			var newRoot = root.ReplaceNode(decl, newParameterDecl);

			// 返回包含修改的文档
			return document.WithSyntaxRoot(newRoot);
		}
	}
}