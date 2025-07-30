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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace WorldTree.Analyzer
{
	public abstract class NodeRuleSwitchAttributeDiagnostic<C> : NamingDiagnosticBase<C>
		where C : ProjectDiagnosticConfig, new()
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
			if (Count < 2) return;
			// 保留原有的参数索引逻辑
			AttributeArgumentSyntax methodArg = attributeSyntax.ArgumentList.Arguments[0];
			AttributeArgumentSyntax argumentArg = attributeSyntax.ArgumentList.Arguments[1];
			AttributeArgumentSyntax argumentKey = attributeSyntax.ArgumentList.Arguments[2];

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
			bool nodeRuleMatch = false;
			bool methodSwitchValueMatch = false;
			//检查方法类型与目标方法的Switch的第二个参数switchValue是否匹配
			if (methodArg.Expression is InvocationExpressionSyntax methodInvocation)
			{
				// 检查是否是 nameof 表达式
				if (methodInvocation.Expression.ToString() == "nameof")
				{
					// 获取 nameof 参数的表达式（即方法名）
					var methodNameExpression = methodInvocation.ArgumentList.Arguments.FirstOrDefault()?.Expression;
					if (methodNameExpression is IdentifierNameSyntax methodNameIdentifier)
					{
						string methodName = methodNameIdentifier.Identifier.ValueText;

						// 在当前类中查找同名方法
						var currentClass = methodDeclaration.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
						if (currentClass != null)
						{
							var targetMethod = currentClass.Members
								.OfType<MethodDeclarationSyntax>()
								.FirstOrDefault(m => m.Identifier.ValueText == methodName);

							if (targetMethod != null)
							{

								// 查找当前方法上的 NodeRule 特性
								var currentNodeRuleAttribute = methodDeclaration.AttributeLists
									.SelectMany(al => al.Attributes)
									.FirstOrDefault(attr => attr.Name.ToString().Contains("NodeRule"));

								// 查找目标方法上的 NodeRule 特性
								var targetNodeRuleAttribute = targetMethod.AttributeLists
									.SelectMany(al => al.Attributes)
									.FirstOrDefault(attr => attr.Name.ToString().Contains("NodeRule"));

								// 直接比较 NodeRule 特性的完整字符串
								if (currentNodeRuleAttribute != null && targetNodeRuleAttribute != null)
								{
									string currentNodeRuleText = currentNodeRuleAttribute.ToString();
									string targetNodeRuleText = targetNodeRuleAttribute.ToString();

									if (currentNodeRuleText == targetNodeRuleText)
									{
										nodeRuleMatch = true;
									}
								}


								// 查找目标方法上的 RuleSwitch 特性
								var targetRuleSwitchAttribute = targetMethod.AttributeLists
									.SelectMany(al => al.Attributes)
									.FirstOrDefault(attr => attr.Name.ToString().Contains("RuleSwitch"));

								if (targetRuleSwitchAttribute != null &&
									targetRuleSwitchAttribute.ArgumentList?.Arguments.Count >= 3)
								{
									// 获取目标方法 RuleSwitch 特性的第二个参数
									var targetSwitchArg = targetRuleSwitchAttribute.ArgumentList.Arguments[1];

									// 比较两个 switchValue 参数
									if (argumentArg.Expression.ToString() == targetSwitchArg.Expression.ToString())
									{
										methodSwitchValueMatch = true;
									}
								}
							}
						}
					}
				}
			}

			if (!methodSwitchValueMatch || !nodeRuleMatch)
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

	public abstract class NodeRuleSwitchAttributeCodeFixProvider<C> : NamingCodeFixProviderBase<AttributeSyntax, C>
		where C : ProjectDiagnosticConfig, new()
	{
		public override SyntaxKind DeclarationKind => SyntaxKind.Attribute;
		public override bool CheckCodeFix(DiagnosticConfig codeDiagnostic, Document document)
		{
			return codeDiagnostic.DiagnosticKey == DiagnosticKey.RuleSwitchAttributeAnalysis;
		}
		protected override Task<Document> CodeFix(DiagnosticConfig codeDiagnostic, Document document, AttributeSyntax attributeSyntax, CancellationToken cancellationToken)
		{
			int Count = attributeSyntax.ArgumentList.Arguments.Count;
			if (Count < 3) return null;

			AttributeArgumentSyntax methodArg = attributeSyntax.ArgumentList.Arguments[0];
			AttributeArgumentSyntax argumentArg = attributeSyntax.ArgumentList.Arguments[1];
			AttributeArgumentSyntax argumentKey = attributeSyntax.ArgumentList.Arguments[2];

			// 获取语义模型
			SemanticModel semanticModel = document.GetSemanticModelAsync(cancellationToken).Result;
			if (semanticModel == null) return null;

			// 获取当前方法
			var currentMethod = attributeSyntax.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();
			if (currentMethod == null) return null;

			// 获取文档根节点
			var root = document.GetSyntaxRootAsync(cancellationToken).Result;
			if (root == null) return null;

			// 首先尝试修复方法引用和switchValue参数
			if (methodArg.Expression is InvocationExpressionSyntax methodInvocation)
			{
				if (methodInvocation.Expression.ToString() == "nameof")
				{
					var methodNameExpression = methodInvocation.ArgumentList.Arguments.FirstOrDefault()?.Expression;
					if (methodNameExpression is IdentifierNameSyntax methodNameIdentifier)
					{
						string methodName = methodNameIdentifier.Identifier.ValueText;

						// 在当前类中查找目标方法
						var currentClass = currentMethod.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
						if (currentClass != null)
						{
							var targetMethod = currentClass.Members
								.OfType<MethodDeclarationSyntax>()
								.FirstOrDefault(m => m.Identifier.ValueText == methodName);

							if (targetMethod != null)
							{
								// 查找目标方法上的 NodeRule 特性
								var targetNodeRuleAttribute = targetMethod.AttributeLists
									.SelectMany(al => al.Attributes)
									.FirstOrDefault(attr => attr.Name.ToString().Contains("NodeRule"));

								// 查找目标方法上的 RuleSwitch 特性
								var targetRuleSwitchAttribute = targetMethod.AttributeLists
									.SelectMany(al => al.Attributes)
									.FirstOrDefault(attr => attr.Name.ToString().Contains("RuleSwitch"));

								if (targetRuleSwitchAttribute != null &&
									targetRuleSwitchAttribute.ArgumentList?.Arguments.Count >= 3)
								{
									// 获取目标方法 RuleSwitch 特性的参数
									var targetMethodArg = targetRuleSwitchAttribute.ArgumentList.Arguments[0];
									var targetSwitchArg = targetRuleSwitchAttribute.ArgumentList.Arguments[1];
									var targetConstantArg = targetRuleSwitchAttribute.ArgumentList.Arguments[2];

									// 创建新的 RuleSwitch 参数列表
									string newRuleSwitchArgsString = $"({targetMethodArg.Expression}, {targetSwitchArg.Expression}, {targetConstantArg.Expression})";
									AttributeArgumentListSyntax newRuleSwitchArgumentList = SyntaxFactory.ParseAttributeArgumentList(newRuleSwitchArgsString);
									AttributeSyntax newRuleSwitchAttributeSyntax = attributeSyntax.WithArgumentList(newRuleSwitchArgumentList);

									// 替换 RuleSwitch 特性
									var newRoot = root.ReplaceNode(attributeSyntax, newRuleSwitchAttributeSyntax);

									// 同时修复 NodeRule 特性（如果存在且不匹配）
									if (targetNodeRuleAttribute != null)
									{
										// 查找当前方法上的 NodeRule 特性
										var currentNodeRuleAttribute = currentMethod.AttributeLists
											.SelectMany(al => al.Attributes)
											.FirstOrDefault(attr => attr.Name.ToString().Contains("NodeRule"));

										if (currentNodeRuleAttribute != null)
										{
											// 直接复制目标方法的完整 NodeRule 特性文本
											string targetNodeRuleText = targetNodeRuleAttribute.ToString();
											string currentNodeRuleText = currentNodeRuleAttribute.ToString();

											if (currentNodeRuleText != targetNodeRuleText)
											{
												// 直接复制目标特性的名称和参数列表
												AttributeSyntax newNodeRuleAttribute = currentNodeRuleAttribute
													.WithName(targetNodeRuleAttribute.Name)
													.WithArgumentList(targetNodeRuleAttribute.ArgumentList);

												// 在新的根节点中查找对应的 NodeRule 特性并替换
												var updatedCurrentMethod = newRoot.DescendantNodes()
													.OfType<MethodDeclarationSyntax>()
													.FirstOrDefault(m => m.Identifier.ValueText == currentMethod.Identifier.ValueText);

												if (updatedCurrentMethod != null)
												{
													var updatedCurrentNodeRuleAttribute = updatedCurrentMethod.AttributeLists
														.SelectMany(al => al.Attributes)
														.FirstOrDefault(attr => attr.Name.ToString().Contains("NodeRule"));

													if (updatedCurrentNodeRuleAttribute != null)
													{
														newRoot = newRoot.ReplaceNode(updatedCurrentNodeRuleAttribute, newNodeRuleAttribute);
													}
												}
											}
										}
									}

									return Task.FromResult(document.WithSyntaxRoot(newRoot));
								}
							}
						}
					}
				}
			}

			// 如果方法引用修复失败，回退到原来的类型修复逻辑
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
				attributeArgsString = $"({methodArg.Expression}, {argumentArg.Expression}, typeof(object))";
			}
			else
			{
				attributeArgsString = $"({methodArg.Expression}, {argumentArg.Expression}, default({targetType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}))";
			}

			// 解析属性参数列表
			AttributeArgumentListSyntax newArgumentList2 = SyntaxFactory.ParseAttributeArgumentList(attributeArgsString);

			// 创建新的特性语法，保留原始特性名称但使用新的参数列表
			AttributeSyntax newAttributeSyntax2 = attributeSyntax.WithArgumentList(newArgumentList2);

			// 替换原有的特性语法节点
			var root2 = document.GetSyntaxRootAsync(cancellationToken).Result;
			if (root2 == null) return null;

			var newRoot2 = root2.ReplaceNode(attributeSyntax, newAttributeSyntax2);

			// 返回修改后的文档
			return Task.FromResult(document.WithSyntaxRoot(newRoot2));
		}

	}
}