/****************************************

* 作者：闪电黑客
* 日期：2024/6/24 14:49

* 描述：法则孤立类型分析

*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;


namespace WorldTree.Analyzer
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class NodeRuleMethodDiagnostic : NamingDiagnosticBase
	{
		public override SyntaxKind DeclarationKind => SyntaxKind.MethodDeclaration;

		private List<string> AttributeTypeNames = new();
		private List<string> AttributeTypeTNames = new();

		private List<string> MethodTypeNames = new();
		private List<string> MethodTypeTNames = new();

		protected override void DiagnosticAction(SyntaxNodeAnalysisContext context)
		{
			if (!ProjectDiagnosticSetting.ProjectDiagnostics.TryGetValue(context.Compilation.AssemblyName, out List<DiagnosticConfigGroup> objectDiagnostics))
				return;

			SemanticModel semanticModel = context.SemanticModel;
			MethodDeclarationSyntax methodDeclaration = context.Node as MethodDeclarationSyntax;
			if (methodDeclaration.AttributeLists.Count == 0) return;

			if (!TreeSyntaxHelper.TryGetAttribute(methodDeclaration, GeneratorHelper.NodeRuleAttribute, out AttributeSyntax attributeNodeRule)) return;
			AttributeArgumentSyntax argumentRule = attributeNodeRule.ArgumentList.Arguments.FirstOrDefault();
			if (argumentRule != null) return;

			if (!TryGetRuleType(semanticModel, argumentRule, out INamedTypeSymbol ruleTypeSymbol, out INamedTypeSymbol baseTypeSymbol, out RuleBaseEnum ruleBaseEnum))
				return;

			var types = baseTypeSymbol.TypeArguments;
			if (types.Length == 0) return;

			AttributeTypeNames.Clear();
			AttributeTypeTNames.Clear();
			for (int i = 0; i < types.Length; i++)
			{
				//基类第二个泛型参数是Rule类型，直接忽略
				if (i == 1) continue;
				AttributeTypeNames.Add(types[i].ToDisplayString());
				ParseTypeSymbol(types[i], AttributeTypeTNames);
			}

			// 获取方法的参数类型集合
			var parameters = methodDeclaration.ParameterList.Parameters;
			MethodTypeNames.Clear();
			MethodTypeTNames.Clear();
			foreach (var parameter in parameters)
			{
				var typeSymbol = semanticModel.GetTypeInfo(parameter.Type).Type;
				if (typeSymbol != null)
				{
					MethodTypeNames.Add(typeSymbol.ToDisplayString());
					ParseTypeSymbol(typeSymbol, MethodTypeTNames);
				}
			}
			// 获取方法的返回值类型
			var returnTypeSymbol = semanticModel.GetTypeInfo(methodDeclaration.ReturnType).Type;
			if (returnTypeSymbol != null)
			{
				MethodTypeNames.Add(returnTypeSymbol.ToDisplayString());
				ParseTypeSymbol(returnTypeSymbol, MethodTypeTNames);
			}
			foreach (DiagnosticConfigGroup objectDiagnostic in objectDiagnostics)
			{
				if (objectDiagnostic.Diagnostics.TryGetValue(DiagnosticKey.NodeRuleMethodAnalysis, out DiagnosticConfig codeDiagnostic))
				{
					bool isError = false;
					// 比较特性参数类型和方法参数类型 顺序和类型是否一致
					if (AttributeTypeNames.Count != MethodTypeNames.Count)
					{
						isError = true;
					}
					else
					{
						for (int i = 0; i < AttributeTypeNames.Count; i++)
						{
							if (AttributeTypeNames[i] == MethodTypeNames[i]) continue;
							isError = true; break;
						}
					}

					if (isError)
						context.ReportDiagnostic(Diagnostic.Create(codeDiagnostic.Diagnostic, methodDeclaration.GetLocation(), methodDeclaration.Identifier.Text));
					return;
				}
			}
		}

		private void ParseTypeSymbol(ITypeSymbol typeSymbol, List<string> typeTNames)
		{
			// 如果是命名类型（如 List<int> 或 Dictionary<string, List<T>>）
			if (typeSymbol is INamedTypeSymbol namedTypeSymbol)
			{
				if (namedTypeSymbol.TypeKind == TypeKind.Error)
				{
					typeTNames.Add(typeSymbol.ToDisplayString());
					return;
				}
				// 递归解析其泛型参数
				foreach (var typeArgument in namedTypeSymbol.TypeArguments)
				{
					ParseTypeSymbol(typeArgument, typeTNames);
				}
			}
			// 如果是泛型符号（如 T、U）
			else
			{
				typeTNames.Add(typeSymbol.ToDisplayString());
			}
		}

		/// <summary>
		/// 尝试获取法则类型
		/// </summary>
		private bool TryGetRuleType(SemanticModel semanticModel, AttributeArgumentSyntax argumentSyntax, out INamedTypeSymbol ruleTypeSymbol, out INamedTypeSymbol baseTypeSymbol, out RuleBaseEnum ruleBaseEnum)
		{
			ruleBaseEnum = default;
			ruleTypeSymbol = null;
			baseTypeSymbol = null;

			// 检查是否是 nameof 表达式
			if (argumentSyntax.Expression is not InvocationExpressionSyntax invocation) return false;
			if (invocation.Expression.ToString() != "nameof") return false;
			// 获取 nameof 参数的表达式
			var nameofArgument = invocation.ArgumentList.Arguments.FirstOrDefault()?.Expression;
			if (nameofArgument == null) return false;

			// 获取符号信息
			var symbolInfo = semanticModel.GetSymbolInfo(nameofArgument);
			if (symbolInfo.Symbol is not INamedTypeSymbol namedTypeSymbol) return false;

			ITypeSymbol typeSymbol;
			// 检查类型是否符合规则
			if (NamedSymbolHelper.CheckBase(namedTypeSymbol, GeneratorHelper.SendRule, out typeSymbol)) ruleBaseEnum = RuleBaseEnum.SendRule;
			else if (NamedSymbolHelper.CheckBase(namedTypeSymbol, GeneratorHelper.CallRule, out typeSymbol)) ruleBaseEnum = RuleBaseEnum.CallRule;
			else if (NamedSymbolHelper.CheckBase(namedTypeSymbol, GeneratorHelper.SendRuleAsync, out typeSymbol)) ruleBaseEnum = RuleBaseEnum.SendRuleAsync;
			else if (NamedSymbolHelper.CheckBase(namedTypeSymbol, GeneratorHelper.CallRuleAsync, out typeSymbol)) ruleBaseEnum = RuleBaseEnum.CallRuleAsync;
			else return false;

			baseTypeSymbol = typeSymbol as INamedTypeSymbol;
			ruleTypeSymbol = namedTypeSymbol;
			return true;
		}
	}

}