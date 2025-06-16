/****************************************

* 作者：闪电黑客
* 日期：2024/6/25 11:59

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
	/// 泛型类型参数命名规范诊断器
	/// </summary>
	public abstract class TypeParameterNamingDiagnostic<C> : NamingDiagnosticBase<C>
		where C : ProjectDiagnosticConfig, new()
	{
		public override SyntaxKind DeclarationKind => SyntaxKind.TypeParameter;
		protected override void DiagnosticAction(SyntaxNodeAnalysisContext context)
		{
			if (!TryGetDiagnosticConfigGroup(context.Compilation.AssemblyName, out List<DiagnosticConfigGroup> DiagnosticGroups)) return;
			// 获取语义模型
			SemanticModel semanticModel = context.SemanticModel;
			TypeParameterSyntax typeParameter = (TypeParameterSyntax)context.Node;
			foreach (DiagnosticConfigGroup objectDiagnostic in DiagnosticGroups)
			{
				if (objectDiagnostic.Diagnostics.TryGetValue(DiagnosticKey.TypeParameterNaming, out DiagnosticConfig codeDiagnostic))
				{
					if (!codeDiagnostic.Check.Invoke(semanticModel, typeParameter.Identifier))
					{
						context.ReportDiagnostic(Diagnostic.Create(codeDiagnostic.Diagnostic, typeParameter.GetLocation(), typeParameter.Identifier.Text));
					}
				}
			}
		}
	}
	public abstract class TypeParameterNamingProvider<C> : NamingCodeFixProviderBase<TypeParameterSyntax, C>
		where C : ProjectDiagnosticConfig, new()
	{
		public override SyntaxKind DeclarationKind => SyntaxKind.TypeParameter;
		protected override async Task<Document> CodeFix(DiagnosticConfig codeDiagnostic, Document document, TypeParameterSyntax decl, CancellationToken cancellationToken)
		{
			var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
			var newRoot = root;
			var fieldName = decl.Identifier.Text;
			fieldName = codeDiagnostic.FixCode?.Invoke(fieldName);

			// 创建新的字段名并替换旧的字段名
			var newFieldDecl = decl.WithIdentifier(SyntaxFactory.Identifier(fieldName));
			newRoot = newRoot.ReplaceNode(decl, newFieldDecl);

			// 返回包含修改的文档
			return document.WithSyntaxRoot(newRoot);
		}
	}
}