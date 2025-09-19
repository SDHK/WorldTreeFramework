/****************************************

* 作者：闪电黑客
* 日期：2024/6/19 19:49

* 描述：属性命名规范诊断器

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
	/// 属性命名规范诊断器
	/// </summary>
	public abstract class PropertyNamingDiagnostic<C> : NamingDiagnosticBase<C>
		where C : ProjectDiagnosticConfig, new()
	{
		public override SyntaxKind DeclarationKind => SyntaxKind.PropertyDeclaration;

		protected override void DiagnosticAction(SyntaxNodeAnalysisContext context)
		{
			if (!TryGetDiagnosticConfigGroup(context.Compilation.AssemblyName, out List<DiagnosticConfigGroup> DiagnosticGroups)) return;
			DiagnosticProperty(context, DiagnosticKey.PublicPropertyNaming, DiagnosticGroups);
			DiagnosticProperty(context, DiagnosticKey.PrivatePropertyNaming, DiagnosticGroups);
			DiagnosticProperty(context, DiagnosticKey.ProtectedPropertyNaming, DiagnosticGroups);
			DiagnosticClassName(context, DiagnosticGroups);
		}

		private void DiagnosticClassName(SyntaxNodeAnalysisContext context, List<DiagnosticConfigGroup> objectDiagnostics)
		{
			SemanticModel semanticModel = context.SemanticModel;
			PropertyDeclarationSyntax propertyDeclaration = (PropertyDeclarationSyntax)context.Node;
			ITypeSymbol propertyTypeSymbol = semanticModel.GetTypeInfo(propertyDeclaration.Type).Type;
			foreach (DiagnosticConfigGroup objectDiagnostic in objectDiagnostics)
			{
				if (objectDiagnostic.Screen(context.Compilation, propertyTypeSymbol))
				{
					if (objectDiagnostic.Diagnostics.TryGetValue(DiagnosticKey.ClassPropertyNaming, out DiagnosticConfig diagnosticConfig))
					{
						if (!diagnosticConfig.Check.Invoke(semanticModel, propertyDeclaration.Identifier))
						{
							context.ReportDiagnostic(Diagnostic.Create(diagnosticConfig.Diagnostic, propertyDeclaration.GetLocation(), propertyDeclaration.Identifier.Text));
						}
						return;
					}
				}
			}
		}

		private void DiagnosticProperty(SyntaxNodeAnalysisContext context, DiagnosticKey diagnosticKey, List<DiagnosticConfigGroup> objectDiagnostics)
		{
			// 获取语义模型
			SemanticModel semanticModel = context.SemanticModel;
			PropertyDeclarationSyntax propertyDeclaration = (PropertyDeclarationSyntax)context.Node;

			//获取当前属性所在的类型名称
			BaseTypeDeclarationSyntax parentType = TreeSyntaxHelper.GetParentType(propertyDeclaration);
			IPropertySymbol? propertySymbol = semanticModel.GetDeclaredSymbol(propertyDeclaration);
			INamedTypeSymbol? propertyParentSymbol = semanticModel.GetDeclaredSymbol(parentType);

			//获取当前属性的类型
			ITypeSymbol propertyTypeSymbol = semanticModel.GetTypeInfo(propertyDeclaration.Type).Type;
			foreach (DiagnosticConfigGroup objectDiagnostic in objectDiagnostics)
			{
				if (!objectDiagnostic.Screen(context.Compilation, propertyParentSymbol)) continue;
				if (objectDiagnostic.Diagnostics.TryGetValue(diagnosticKey, out DiagnosticConfig codeDiagnostic))
				{
					// 需要的修饰符
					if (TreeSyntaxHelper.SyntaxKindContains(propertyDeclaration.Modifiers, codeDiagnostic.KeywordKinds))
					{
						// 不需要检查的修饰符
						if (!TreeSyntaxHelper.SyntaxKindContainsAny(propertyDeclaration.Modifiers, codeDiagnostic.UnKeywordKinds, false))
						{
							// 检查属性名是否符合规范
							if (!codeDiagnostic.Check.Invoke(semanticModel, propertyDeclaration.Identifier))
							{
								context.ReportDiagnostic(Diagnostic.Create(codeDiagnostic.Diagnostic, propertyDeclaration.GetLocation(), propertyDeclaration.Identifier.Text));
							}
							//是否需要注释
							if (codeDiagnostic.NeedComment.Invoke(semanticModel, propertyDeclaration.Identifier))
							{
								// 检查属性是否为重写的
								bool isOverride = propertySymbol.IsOverride;
								if (!isOverride)
								{
									// 检查属性是否直接声明在当前类中
									isOverride = !propertySymbol.ContainingType.Equals(propertyParentSymbol, SymbolEqualityComparer.Default);
									if (!isOverride)
									{
										// 检查属性是否实现了任何接口
										isOverride = NamedSymbolHelper.CheckInterfaceImplements(propertySymbol);
									}
								}
								if (!isOverride)
								{
									if (!TreeSyntaxHelper.CheckSummaryComment(propertyDeclaration))
									{
										context.ReportDiagnostic(Diagnostic.Create(codeDiagnostic.Diagnostic, propertyDeclaration.GetLocation(), propertyDeclaration.Identifier.Text));
									}
								}
							}
						}

					}
					return;
				}
			}
		}
	}

	public abstract class PropertyNamingProvider<C> : NamingCodeFixProviderBase<PropertyDeclarationSyntax, C>
		where C : ProjectDiagnosticConfig, new()
	{
		public override SyntaxKind DeclarationKind => SyntaxKind.PropertyDeclaration;

		protected override async Task<Document> CodeFix(DiagnosticConfig codeDiagnostic, Document document, PropertyDeclarationSyntax decl, CancellationToken cancellationToken)
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