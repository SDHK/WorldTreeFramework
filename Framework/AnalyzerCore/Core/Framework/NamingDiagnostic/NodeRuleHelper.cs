/****************************************

* 作者：闪电黑客
* 日期：2024/6/24 14:49

* 描述：法则诊断共用辅助方法

*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace WorldTree.Analyzer
{
	internal static class NodeRuleHelper
	{
		/// <summary>
		/// 判断类是否实现 INode 接口，返回对应的 this 修饰符字符串
		/// </summary>
		public static string GetStrThis(SemanticModel semanticModel, ClassDeclarationSyntax classSyntax)
		{
			INamedTypeSymbol classSymbol = semanticModel.GetDeclaredSymbol(classSyntax) as INamedTypeSymbol;
			INamedTypeSymbol iNodeSymbol = NamedSymbolHelper.ToINamedTypeSymbol(semanticModel.Compilation, GeneratorHelper.INode);
			bool isNode = classSymbol != null
					   && iNodeSymbol != null
					   && NamedSymbolHelper.CheckInterface(classSymbol, iNodeSymbol);
			return isNode ? "" : "this ";
		}

		/// <summary>
		/// 收集外层类已声明的泛型参数名集合
		/// </summary>
		public static HashSet<string> GetOuterTypeParams(ClassDeclarationSyntax classSyntax)
		{
			var outerTypeParams = new HashSet<string>();
			if (classSyntax?.TypeParameterList != null)
				foreach (var tp in classSyntax.TypeParameterList.Parameters)
					outerTypeParams.Add(tp.Identifier.Text);
			return outerTypeParams;
		}

		/// <summary>
		/// 过滤掉外层类已声明的泛型参数，生成方法泛型声明字符串，如 &lt;T1, T2&gt;
		/// </summary>
		public static string GetTypeTName(List<string> typeTNames, ClassDeclarationSyntax classSyntax)
		{
			var outerTypeParams = GetOuterTypeParams(classSyntax);
			var filtered = typeTNames.Where(t => !outerTypeParams.Contains(t)).ToList();
			return filtered.Count == 0 ? "" : $"<{string.Join(", ", filtered)}>";
		}

		/// <summary>
		/// 获取法则方法代码文本
		/// </summary>
		public static string GetRuleMethod(RuleBaseEnum ruleBaseEnum, string ruleTypeName, string typeTName, string genericTypeParameter, string outType, string strThis)
		=> ruleBaseEnum switch
		{
			RuleBaseEnum.SendRule or RuleBaseEnum.ListenerRule =>
			$$"""
					private static void {{ruleTypeName}}{{typeTName}}({{strThis}}{{genericTypeParameter}})
					{ 
					}
			""",
			RuleBaseEnum.SendRuleAsync =>
			$$"""
					private static async TreeTask {{ruleTypeName}}{{typeTName}}({{strThis}}{{genericTypeParameter}})
					{ 
						await self.TreeTaskCompleted();
					}
			""",
			RuleBaseEnum.CallRule =>
			$$"""
					private static {{outType}} {{ruleTypeName}}{{typeTName}}({{strThis}}{{genericTypeParameter}})
					{ 
						return default;
					}
			""",
			RuleBaseEnum.CallRuleAsync =>
			$$"""
					private static async TreeTask<{{outType}}> {{ruleTypeName}}{{typeTName}}({{strThis}}{{genericTypeParameter}})
					{ 
						await self.TreeTaskCompleted();
						return default;
					}
			""",
			_ => null
		};

		/// <summary>
		/// 转换泛型参数：T0, T1, T2, T3 => T0 self, T1 arg1, T2 arg2, T3 arg3
		/// </summary>
		public static string GetRuleTypeParameter(List<string> Types, bool isCall, out string outType, List<string> ArgNames = null)
		{
			bool isArgName = ArgNames != null && ArgNames.Count > 1;
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
						result.Append($", {Types[i]} {ArgNames[i]}");
					else
						result.Append($", {Types[i]} arg{i}");
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

		/// <summary>
		/// 尝试从 nameof 特性参数获取法则类型
		/// </summary>
		public static bool TryGetRuleType(SemanticModel semanticModel, AttributeArgumentSyntax argumentSyntax, out INamedTypeSymbol ruleTypeSymbol, out INamedTypeSymbol baseTypeSymbol, out RuleBaseEnum ruleBaseEnum)
		{
			ruleBaseEnum = default;
			ruleTypeSymbol = null;
			baseTypeSymbol = null;

			if (argumentSyntax.Expression is not InvocationExpressionSyntax invocation) return false;
			if (invocation.Expression.ToString() != "nameof") return false;
			var nameofArgument = invocation.ArgumentList.Arguments.FirstOrDefault()?.Expression;
			if (nameofArgument == null) return false;

			var symbolInfo = semanticModel.GetSymbolInfo(nameofArgument);
			if (symbolInfo.Symbol is not INamedTypeSymbol namedTypeSymbol) return false;

			ITypeSymbol typeSymbol;
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
		/// 递归解析类型符号中的泛型参数名称
		/// </summary>
		public static void ParseTypeSymbol(ITypeSymbol typeSymbol, List<string> typeTNames)
		{
			if (typeSymbol is INamedTypeSymbol namedTypeSymbol)
			{
				if (namedTypeSymbol.TypeKind == TypeKind.Error)
				{
					typeTNames.Add(typeSymbol.ToDisplayString());
					return;
				}
				foreach (var typeArgument in namedTypeSymbol.TypeArguments)
					ParseTypeSymbol(typeArgument, typeTNames);
			}
			else
			{
				typeTNames.Add(typeSymbol.ToDisplayString());
			}
		}
	}
}
