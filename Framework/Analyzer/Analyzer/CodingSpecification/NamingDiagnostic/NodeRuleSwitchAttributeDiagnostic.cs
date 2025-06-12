/****************************************

* 作者：闪电黑客
* 日期：2024/6/24 14:49

* 描述：法则类型特性标记分析

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
	public abstract class NodeRuleSwitchAttributeDiagnostic : NamingDiagnosticBase
	{
		public override SyntaxKind DeclarationKind => SyntaxKind.Attribute;
		protected override void DiagnosticAction(SyntaxNodeAnalysisContext context)
		{
			if (!TryGetDiagnosticConfigGroup(context.Compilation.AssemblyName, out List<DiagnosticConfigGroup> DiagnosticGroups)) return;

			// 获取语义模型
			SemanticModel semanticModel = context.SemanticModel;

			// 确保当前节点是 AttributeSyntax
			if (context.Node is not AttributeSyntax attributeSyntax) return;
			if (!attributeSyntax.Name.ToString().Contains("RuleSwitch")) return;
			// 检查特性是否标记在方法上
			var targetNode = attributeSyntax.Parent?.Parent;
			if (targetNode is not MethodDeclarationSyntax methodDeclaration) return;
			int Count = attributeSyntax.ArgumentList.Arguments.Count;
			if (Count < 1) return;
			// 保留原有的参数索引逻辑
			AttributeArgumentSyntax argumentArg = attributeSyntax.ArgumentList.Arguments[Count <= 2 ? 0 : 1];
			AttributeArgumentSyntax argumentKey = attributeSyntax.ArgumentList.Arguments[Count <= 2 ? 1 : 2];

			// 检查是否是 nameof 表达式
			if (argumentArg.Expression is not InvocationExpressionSyntax invocation) return;
			if (invocation.Expression.ToString() != "nameof") return;

			// 获取 nameof 参数的表达式
			var nameofArgument = invocation.ArgumentList.Arguments.FirstOrDefault()?.Expression;
			if (nameofArgument == null) return;

			// 处理两种情况：参数本身 或 参数的成员
			ITypeSymbol targetType = null;

			if (nameofArgument is MemberAccessExpressionSyntax memberAccess)
			{
				// 情况1：参数的成员 (e.g., self.ConfigId)
				var selfSymbolInfo = semanticModel.GetSymbolInfo(memberAccess.Expression);
				if (selfSymbolInfo.Symbol is not IParameterSymbol parameterSymbol) return;

				// 确保是方法的参数
				if (parameterSymbol.ContainingSymbol is not IMethodSymbol) return;

				// 获取成员的类型（属性或字段）
				var memberSymbolInfo = semanticModel.GetSymbolInfo(memberAccess);
				if (memberSymbolInfo.Symbol is IPropertySymbol propertySymbol)
				{
					targetType = propertySymbol.Type;
				}
				else if (memberSymbolInfo.Symbol is IFieldSymbol fieldSymbol)
				{
					targetType = fieldSymbol.Type;
				}
				else
				{
					return; // 不是有效的成员
				}
			}
			else if (nameofArgument is IdentifierNameSyntax identifier)
			{
				// 情况2：参数本身
				var symbolInfo = semanticModel.GetSymbolInfo(identifier);
				if (symbolInfo.Symbol is IParameterSymbol paramSymbol)
				{
					targetType = paramSymbol.Type;
				}
				else
				{
					return;// 不是有效的参数
				}
			}
			else
			{
				return;// 不是成员访问或参数标识符
			}

			bool typeMatch = false;
			// 检查常量类型与目标类型是否匹配
			var constantType = semanticModel.GetTypeInfo(argumentKey.Expression).Type;
			if (targetType != null && constantType != null)
			{
				//检查类型是否匹配（考虑类型转换）
				typeMatch = constantType.Equals(targetType, SymbolEqualityComparer.Default);
				if (!typeMatch)
				{
					// 检查是否可以隐式转换
					var conversion = semanticModel.ClassifyConversion(argumentKey.Expression, targetType);
					typeMatch = conversion.Exists && conversion.IsImplicit;
					// 特殊处理 Type 类型参数和 typeof 表达式
					if (!typeMatch && targetType.Equals(context.Compilation.GetTypeByMetadataName("System.Type"), SymbolEqualityComparer.Default))
					{
						// 如果目标类型是 typeof(类型)
						if (argumentKey.Expression is TypeOfExpressionSyntax)
						{
							typeMatch = true;
						}
					}
				}
			}

			if (!typeMatch)
			{
				// 报告诊断错误
				foreach (DiagnosticConfigGroup objectDiagnostic in DiagnosticGroups)
				{
					if (objectDiagnostic.Diagnostics.TryGetValue(DiagnosticKey.RuleSwitchAttributeAnalysis, out DiagnosticConfig codeDiagnostic))
					{
						context.ReportDiagnostic(Diagnostic.Create(codeDiagnostic.Diagnostic, attributeSyntax.GetLocation(), attributeSyntax.ToString()));
						return;
					}
				}
			}
		}
	}

	public abstract class NodeRuleSwitchAttributeCodeFixProvider : NamingCodeFixProviderBase<AttributeSyntax>
	{
		public override SyntaxKind DeclarationKind => SyntaxKind.Attribute;
		public override bool CheckCodeFix(DiagnosticConfig codeDiagnostic, Document document)
		{
			return codeDiagnostic.DiagnosticKey == DiagnosticKey.RuleSwitchAttributeAnalysis;
		}
		protected override Task<Document> CodeFix(DiagnosticConfig codeDiagnostic, Document document, AttributeSyntax attributeSyntax, CancellationToken cancellationToken)
		{
			int Count = attributeSyntax.ArgumentList.Arguments.Count;
			if (Count == 0) return null;
			AttributeArgumentSyntax argumentArg = attributeSyntax.ArgumentList.Arguments[Count <= 2 ? 0 : 1];
			if (argumentArg == null) return null;
			// 获取语义模型
			SemanticModel semanticModel = document.GetSemanticModelAsync(cancellationToken).Result;
			if (semanticModel == null) return null;

			// 检查是否是 nameof 表达式
			if (argumentArg.Expression is not InvocationExpressionSyntax invocation) return null;
			if (invocation.Expression.ToString() != "nameof") return null;

			// 获取 nameof 参数的表达式
			var nameofArgument = invocation.ArgumentList.Arguments.FirstOrDefault()?.Expression;
			if (nameofArgument == null) return null;

			// 处理两种情况：参数本身 或 参数的成员
			ITypeSymbol targetType = null;

			if (nameofArgument is MemberAccessExpressionSyntax memberAccess)
			{
				// 情况1：参数的成员 (e.g., self.ConfigId)
				var selfSymbolInfo = semanticModel.GetSymbolInfo(memberAccess.Expression);
				if (selfSymbolInfo.Symbol is not IParameterSymbol parameterSymbol) return null;

				// 确保是方法的参数
				if (parameterSymbol.ContainingSymbol is not IMethodSymbol) return null;

				// 获取成员的类型（属性或字段）
				var memberSymbolInfo = semanticModel.GetSymbolInfo(memberAccess);
				if (memberSymbolInfo.Symbol is IPropertySymbol propertySymbol)
				{
					targetType = propertySymbol.Type;
				}
				else if (memberSymbolInfo.Symbol is IFieldSymbol fieldSymbol)
				{
					targetType = fieldSymbol.Type;
				}
				else
				{
					return null; // 不是有效的成员
				}
			}
			else if (nameofArgument is IdentifierNameSyntax identifier)
			{
				// 情况2：参数本身
				var symbolInfo = semanticModel.GetSymbolInfo(identifier);
				if (symbolInfo.Symbol is IParameterSymbol paramSymbol)
				{
					targetType = paramSymbol.Type;
				}
				else
				{
					return null;// 不是有效的参数
				}
			}
			else
			{
				return null;// 不是成员访问或参数标识符
			}

			// 生成一个特性字符串，但不包括特性名称，只包括参数部分
			string attributeArgsString;
			// 判断目标类型是否是Type
			if (targetType != null && targetType.Equals(semanticModel.Compilation.GetTypeByMetadataName("System.Type"), SymbolEqualityComparer.Default))
			{
				// 如果目标类型是 System.Type，则无需进一步处理
				attributeArgsString = $"({argumentArg.Expression}, typeof(object))";
			}
			else
			{
				attributeArgsString = $"({argumentArg.Expression}, default({targetType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}))";

			}

			// 解析属性参数列表
			AttributeArgumentListSyntax newArgumentList = SyntaxFactory.ParseAttributeArgumentList(attributeArgsString);

			// 创建新的特性语法，保留原始特性名称但使用新的参数列表
			AttributeSyntax newAttributeSyntax = attributeSyntax.WithArgumentList(newArgumentList);

			// 替换原有的特性语法节点
			var root = document.GetSyntaxRootAsync(cancellationToken).Result;
			if (root == null) return null;

			var newRoot = root.ReplaceNode(attributeSyntax, newAttributeSyntax);

			// 返回修改后的文档
			return Task.FromResult(document.WithSyntaxRoot(newRoot));
		}

	}
}