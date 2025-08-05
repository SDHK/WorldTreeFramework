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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace WorldTree.Analyzer
{
	public abstract class NodeRuleMethodDiagnostic<C> : NamingDiagnosticBase<C>
		where C : ProjectDiagnosticConfig, new()
	{
		public override SyntaxKind DeclarationKind => SyntaxKind.MethodDeclaration;

		protected override void DiagnosticAction(SyntaxNodeAnalysisContext context)
		{
			if (!TryGetDiagnosticConfigGroup(context.Compilation.AssemblyName, out List<DiagnosticConfigGroup> DiagnosticGroups)) return;

			SemanticModel semanticModel = context.SemanticModel;
			MethodDeclarationSyntax methodDeclaration = context.Node as MethodDeclarationSyntax;
			if (methodDeclaration.AttributeLists.Count == 0) return;

			if (!TreeSyntaxHelper.TryGetAttribute(methodDeclaration, GeneratorHelper.NodeRuleAttribute, out AttributeSyntax attributeNodeRule)) return;


			AttributeArgumentSyntax argumentRule = attributeNodeRule.ArgumentList.Arguments.FirstOrDefault();
			if (argumentRule == null) return;

			if (!TryGetRuleType(semanticModel, argumentRule, out INamedTypeSymbol ruleTypeSymbol, out INamedTypeSymbol baseTypeSymbol, out RuleBaseEnum ruleBaseEnum))
				return;

			List<ITypeSymbol> types = baseTypeSymbol.TypeArguments.ToList();
			//监听法则最后一个泛型参数是Rule类型，直接忽略
			if (ruleBaseEnum == RuleBaseEnum.ListenerRule) types.RemoveAt(types.Count - 1);
			if (types.Count == 0) return;

			List<ITypeSymbol> AttributeTypeNames = new();
			List<string> AttributeTypeTNames = new();
			List<ITypeSymbol> MethodTypeNames = new();
			List<string> MethodTypeTNames = new();
			for (int i = 0; i < types.Count; i++)
			{
				//基类第二个泛型参数是Rule类型，直接忽略
				if (i == 1) continue;
				AttributeTypeNames.Add(types[i]);
				ParseTypeSymbol(types[i], AttributeTypeTNames);
			}

			// 获取方法的参数类型集合
			var parameters = methodDeclaration.ParameterList.Parameters;
			foreach (var parameter in parameters)
			{
				ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(parameter.Type).Type;
				if (typeSymbol != null)
				{
					// 获取完全限定的类型名称
					MethodTypeNames.Add(typeSymbol);
				}
			}

			// 获取方法的返回值类型
			if (!(ruleBaseEnum == RuleBaseEnum.SendRule || ruleBaseEnum == RuleBaseEnum.ListenerRule))
			{
				var returnTypeSymbol = semanticModel.GetTypeInfo(methodDeclaration.ReturnType).Type;
				if (returnTypeSymbol != null)
				{
					MethodTypeNames.Add(returnTypeSymbol);

				}
			}
			// 获取方法的泛型参数类型
			if (methodDeclaration.TypeParameterList != null)
			{
				foreach (var typeParameter in methodDeclaration.TypeParameterList.Parameters)
				{
					ITypeSymbol typeSymbol = semanticModel.GetDeclaredSymbol(typeParameter);
					if (typeSymbol != null)
					{
						MethodTypeTNames.Add(typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
					}
				}
			}

			bool isError = false;
			//拿到方法名称 ，判断是否是On开头
			if (!methodDeclaration.Identifier.Text.StartsWith("On")) isError = true;
			//判断是否是Rule结尾
			if (!methodDeclaration.Identifier.Text.EndsWith("Rule")) isError = true;
			// 比较特性参数类型和方法参数类型 顺序和类型是否一致
			if (AttributeTypeNames.Count != MethodTypeNames.Count || AttributeTypeTNames.Count != MethodTypeTNames.Count)
			{
				isError = true;
			}
			else
			{
				for (int i = 0; i < AttributeTypeNames.Count; i++)
				{
					if (AttributeTypeNames[i].ToDisplayString() != MethodTypeNames[i].ToDisplayString())
					{
						isError = true; break;
					}
					//else if (AttributeTypeTNames[i] != MethodTypeTNames[i])
					//{
					//	isError = true; break;
					//}
				}
			}


			foreach (DiagnosticConfigGroup objectDiagnostic in DiagnosticGroups)
			{
				if (objectDiagnostic.Diagnostics.TryGetValue(DiagnosticKey.NodeRuleMethodAnalysis, out DiagnosticConfig codeDiagnostic))
				{
					if (isError)
						context.ReportDiagnostic(Diagnostic.Create(codeDiagnostic.Diagnostic, methodDeclaration.GetLocation(), methodDeclaration.Identifier.Text));
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
			else if (NamedSymbolHelper.CheckBase(namedTypeSymbol, GeneratorHelper.ListenerRule, out typeSymbol)) ruleBaseEnum = RuleBaseEnum.ListenerRule;
			else return false;

			baseTypeSymbol = typeSymbol as INamedTypeSymbol;
			ruleTypeSymbol = namedTypeSymbol;
			return true;
		}
	}

	public abstract class NodeRuleMethodCodeFixProvider<C> : NamingCodeFixProviderBase<MethodDeclarationSyntax, C>
		where C : ProjectDiagnosticConfig, new()
	{
		private List<string> typeNames = new();
		private List<string> typeTNames = new();
		private List<string> typeArgNames = new();


		public override SyntaxKind DeclarationKind => SyntaxKind.MethodDeclaration;

		public override bool CheckCodeFix(DiagnosticConfig codeDiagnostic, Document document)
		{
			return codeDiagnostic.DiagnosticKey == DiagnosticKey.NodeRuleMethodAnalysis;
		}

		protected override async Task<Document> CodeFix(DiagnosticConfig codeDiagnostic, Document document, MethodDeclarationSyntax methodDeclaration, CancellationToken cancellationToken)
		{

			//获取方法的特性
			if (!TreeSyntaxHelper.TryGetAttribute(methodDeclaration, GeneratorHelper.NodeRuleAttribute, out AttributeSyntax attributeNodeRule)) return null;
			if (attributeNodeRule.ArgumentList.Arguments.Count == 0) return null;

			AttributeArgumentSyntax genericNameSyntax = attributeNodeRule.ArgumentList.Arguments[0];
			SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken);
			if (!TryGetRuleType(semanticModel, genericNameSyntax, out INamedTypeSymbol ruleTypeSymbol, out INamedTypeSymbol baseTypeSymbol, out RuleBaseEnum ruleBaseEnum))
				return null;

			typeNames.Clear();
			typeTNames.Clear();
			typeArgNames.Clear();
			List<ITypeSymbol> types = baseTypeSymbol.TypeArguments.ToList();
			//监听法则最后一个泛型参数是Rule类型，直接忽略
			if (ruleBaseEnum == RuleBaseEnum.ListenerRule) types.RemoveAt(types.Count - 1);

			for (int i = 0; i < types.Count; i++)
			{
				//基类第二个泛型参数是Rule类型，直接忽略
				if (i == 1) continue;

				typeNames.Add(types[i].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
				ParseTypeSymbol(types[i], typeTNames);

			}
			// 收集参数变量名称
			for (int i = 0; i < methodDeclaration.ParameterList.Parameters.Count; i++)
			{
				//如果是监听法则，且是最后一个泛型参数，则直接跳过，不添加
				if (ruleBaseEnum == RuleBaseEnum.ListenerRule && i == types.Count - 1) continue;
				var parameter = methodDeclaration.ParameterList.Parameters[i];
				typeArgNames.Add(parameter.Identifier.Text); // 获取参数变量名称
			}

			bool isCall = ruleBaseEnum is RuleBaseEnum.CallRule or RuleBaseEnum.CallRuleAsync;
			string genericTypeParameter = GetRuleTypeParameter(typeNames, isCall, out string outType, typeArgNames);
			string typeTName = typeTNames.Count == 0 ? "" : $"<{string.Join(", ", typeTNames)}>";

			//拿到genericNameSyntax 的代码文字
			var genericNameText = genericNameSyntax.ToFullString().TrimStart().TrimEnd();


			// 获取 ruleTypeSymbol 的类型名称（不带命名空间和泛型）
			var ruleTypeName = "On" + ruleTypeSymbol.Name;

			StringBuilder classCode = new();
			classCode.AppendLine(GetRuleMethod(ruleBaseEnum, ruleTypeName, typeTName, genericTypeParameter, outType));
			// 生成代码，并替换方法名称和参数
			var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

			// 解析新的方法声明，仅替换方法名和参数
			var newMethodDeclaration = SyntaxFactory.ParseMemberDeclaration(classCode.ToString())
				.WithLeadingTrivia(methodDeclaration.GetLeadingTrivia())
				.WithTrailingTrivia(methodDeclaration.GetTrailingTrivia()) as MethodDeclarationSyntax;

			// 保留原方法的特性和方法体
			var updatedMethod = methodDeclaration
				.WithIdentifier(newMethodDeclaration.Identifier) // 替换方法名
				.WithParameterList(newMethodDeclaration.ParameterList) // 替换参数列表
				.WithTypeParameterList(newMethodDeclaration.TypeParameterList); // 替换泛型参数列表


			// 替换原有的方法节点
			var newRoot = root.ReplaceNode(methodDeclaration, updatedMethod);
			return document.WithSyntaxRoot(newRoot);

		}

		/// <summary>
		/// 获取法则方法
		/// </summary>
		public string GetRuleMethod(RuleBaseEnum ruleBaseEnum, string ruleTypeName, string typeTName, string genericTypeParameter, string outType)
		=> ruleBaseEnum switch
		{
			RuleBaseEnum.SendRule or RuleBaseEnum.ListenerRule =>
			$$"""
					private static void {{ruleTypeName}}{{typeTName}}(this {{genericTypeParameter}})
					{ 
					}
			""",
			RuleBaseEnum.SendRuleAsync =>
			$$"""
					private static async TreeTask {{ruleTypeName}}{{typeTName}}(this {{genericTypeParameter}})
					{ 
						await self.TreeTaskCompleted();
					}
			""",
			RuleBaseEnum.CallRule =>
			$$"""
					private static {{outType}} {{ruleTypeName}}{{typeTName}}(this {{genericTypeParameter}})
					{ 
						return default;
					}
			""",
			RuleBaseEnum.CallRuleAsync =>
			$$"""
					private static async TreeTask<{{outType}}> {{ruleTypeName}}{{typeTName}}(this {{genericTypeParameter}})
					{ 
						await self.TreeTaskCompleted();
						return default;
					}
			""",
			_ => null
		};

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
			else if (NamedSymbolHelper.CheckBase(namedTypeSymbol, GeneratorHelper.ListenerRule, out typeSymbol)) ruleBaseEnum = RuleBaseEnum.ListenerRule;
			else return false;

			baseTypeSymbol = typeSymbol as INamedTypeSymbol;
			ruleTypeSymbol = namedTypeSymbol;
			return true;
		}

		/// <summary>
		/// 转换 泛型参数 ：T0, T1, T2, T3 =&gt; T0 self, T1 arg1, T2 arg2, T3 arg3
		/// </summary>
		private string GetRuleTypeParameter(List<string> Types, bool isCall, out string outType, List<string> ArgNames = null)
		{
			bool isArgName = false;
			if (ArgNames != null && ArgNames.Count > 1) isArgName = true;

			var result = new StringBuilder();
			for (int i = 0; i < Types.Count; i++)
			{
				if (i == 0)
				{
					result.Append($"{Types[i]} self");
				}
				else if (!(isCall && i == Types.Count - 1))
				{
					if (isArgName && i < ArgNames.Count)
					{
						result.Append($", {Types[i]} {ArgNames[i]}");
					}
					else
					{
						result.Append($", {Types[i]} arg{i}");
					}
				}
			}
			if (isCall)
			{
				outType = Types[Types.Count - 1];
				return result.ToString();
			}
			else
			{
				outType = "void";
				return result.ToString();
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

	}
}