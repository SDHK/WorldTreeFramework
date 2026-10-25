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
		/// 法则方法默认名称：接口形式 nameof(LayerChange) → OnLayerChangeRule；类形式 nameof(OpenRule&lt;N&gt;) → OnOpenRule
		/// </summary>
		public static string GetRuleMethodName(SemanticModel semanticModel, AttributeArgumentSyntax argumentSyntax, INamedTypeSymbol ruleTypeSymbol)
		{
			if (argumentSyntax.Expression is InvocationExpressionSyntax invocation && invocation.Expression.ToString() == "nameof")
			{
				ExpressionSyntax inner = invocation.ArgumentList.Arguments.FirstOrDefault()?.Expression;
				string simple = null;
				if (inner is IdentifierNameSyntax identifier) simple = identifier.Identifier.Text;
				else if (inner is GenericNameSyntax generic) simple = generic.Identifier.Text;
				else if (inner is MemberAccessExpressionSyntax member) simple = member.Name.Identifier.Text;

				if (simple != null && semanticModel.GetSymbolInfo(inner).Symbol is INamedTypeSymbol named)
				{
					if (named.TypeKind == TypeKind.Interface)
						return "On" + (simple.EndsWith("Rule") ? simple : simple + "Rule");
					return "On" + simple;
				}
			}
			return "On" + ruleTypeSymbol.Name;
		}

		/// <summary>
		/// 精确判断是否是 NodeRule 特性（排除 NodeRuleTarget 等衍生名）
		/// </summary>
		public static bool IsNodeRuleAttribute(string name)
		{
			int dot = name.LastIndexOf('.');
			if (dot >= 0) name = name.Substring(dot + 1);
			return name == "NodeRule" || name == "NodeRuleAttribute";
		}


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
		/// <summary>
		/// 解析法则类型：接口形式（nameof(Open)，N 取方法第一个参数）优先，失败回退类形式。
		/// </summary>
		public static bool TryGetRuleType(SemanticModel semanticModel, MethodDeclarationSyntax method, AttributeArgumentSyntax argumentSyntax, out INamedTypeSymbol ruleTypeSymbol, out INamedTypeSymbol baseTypeSymbol, out RuleBaseEnum ruleBaseEnum)
			=> TryGetRuleTypeByInterface(semanticModel, method, argumentSyntax, out ruleTypeSymbol, out baseTypeSymbol, out ruleBaseEnum);

		/// <summary>
		/// 静态规则类兜底：方法不存在时，N 取类上的 [NodeRuleTarget]，或包含类本身（实现 INode）。
		/// </summary>
		public static bool TryGetRuleTypeFromClass(SemanticModel semanticModel, ClassDeclarationSyntax classSyntax, AttributeArgumentSyntax argumentSyntax, out INamedTypeSymbol ruleTypeSymbol, out INamedTypeSymbol baseTypeSymbol, out RuleBaseEnum ruleBaseEnum)
		{
			ruleTypeSymbol = null;
			baseTypeSymbol = null;
			ruleBaseEnum = default;

			if (!TryGetInterfaceRule(semanticModel, argumentSyntax, out INamedTypeSymbol interfaceSymbol, out INamedTypeSymbol baseInterface, out ruleBaseEnum)) return false;

			return TryGetRuleTypeFromInterfaceSymbol(semanticModel, classSyntax, interfaceSymbol, baseInterface, ruleBaseEnum, out ruleTypeSymbol, out baseTypeSymbol);
		}

		private static bool TryGetRuleTypeByInterface(SemanticModel semanticModel, MethodDeclarationSyntax method, AttributeArgumentSyntax argumentSyntax, out INamedTypeSymbol ruleTypeSymbol, out INamedTypeSymbol baseTypeSymbol, out RuleBaseEnum ruleBaseEnum)
		{
			ruleTypeSymbol = null;
			baseTypeSymbol = null;
			ruleBaseEnum = default;

			if (!TryGetInterfaceRule(semanticModel, argumentSyntax, out INamedTypeSymbol interfaceSymbol, out INamedTypeSymbol baseInterface, out ruleBaseEnum)) return false;

			if (semanticModel.GetDeclaredSymbol(method) is not IMethodSymbol methodSymbol) return false;
			if (methodSymbol.Parameters.Length == 0) return false;

			ITypeSymbol nodeType = methodSymbol.Parameters[0].Type;
			if (!ValidateNode(nodeType, interfaceSymbol)) return false;

			return TryCreateRuleBase(semanticModel.Compilation, ruleBaseEnum, nodeType, interfaceSymbol, baseInterface, out ruleTypeSymbol, out baseTypeSymbol);
		}

		/// <summary>
		/// 解析一个类型名（接口形式），并确定其法则基类种类
		/// </summary>
		public static bool TryGetInterfaceRule(SemanticModel semanticModel, SimpleNameSyntax name, out INamedTypeSymbol interfaceSymbol, out INamedTypeSymbol baseInterface, out RuleBaseEnum ruleBaseEnum)
		{
			interfaceSymbol = null;
			baseInterface = null;
			ruleBaseEnum = default;

			if (semanticModel.GetSymbolInfo(name).Symbol is not INamedTypeSymbol named) return false;
			if (named.TypeKind != TypeKind.Interface) return false;

			if (NamedSymbolHelper.TryGetInterfaceName(named, GeneratorHelper.ISendRule, out baseInterface)) ruleBaseEnum = RuleBaseEnum.SendRule;
			else if (NamedSymbolHelper.TryGetInterfaceName(named, GeneratorHelper.ISendRuleAsync, out baseInterface)) ruleBaseEnum = RuleBaseEnum.SendRuleAsync;
			else if (NamedSymbolHelper.TryGetInterfaceName(named, GeneratorHelper.ICallRule, out baseInterface)) ruleBaseEnum = RuleBaseEnum.CallRule;
			else if (NamedSymbolHelper.TryGetInterfaceName(named, GeneratorHelper.ICallRuleAsync, out baseInterface)) ruleBaseEnum = RuleBaseEnum.CallRuleAsync;
			else return false;

			interfaceSymbol = named;
			return true;
		}

		/// <summary>
		/// 用接口符号 + 包含类（[NodeRuleTarget] 或 INode）合成法则基类
		/// </summary>
		public static bool TryGetRuleTypeFromInterfaceSymbol(SemanticModel semanticModel, ClassDeclarationSyntax classSyntax, INamedTypeSymbol interfaceSymbol, INamedTypeSymbol baseInterface, RuleBaseEnum ruleBaseEnum, out INamedTypeSymbol ruleTypeSymbol, out INamedTypeSymbol baseTypeSymbol)
		{
			ruleTypeSymbol = null;
			baseTypeSymbol = null;

			List<ITypeSymbol> targets = GetTargetNodes(semanticModel, classSyntax);
			if (targets.Count == 0 && classSyntax != null)
			{
				if (semanticModel.GetDeclaredSymbol(classSyntax) is INamedTypeSymbol classSymbol
					&& NamedSymbolHelper.TryGetInterfaceName(classSymbol, "INode", out _))
					targets.Add(classSymbol);
			}

			foreach (ITypeSymbol target in targets)
			{
				if (!ValidateNode(target, interfaceSymbol)) continue;
				if (TryCreateRuleBase(semanticModel.Compilation, ruleBaseEnum, target, interfaceSymbol, baseInterface, out ruleTypeSymbol, out baseTypeSymbol)) return true;
			}

			// 没有目标（或都不匹配）：默认 INode 占位，生成后由用户改成真实节点类型
			ITypeSymbol iNode = semanticModel.Compilation.ToINamedTypeSymbol(GeneratorHelper.INode);
			if (iNode == null) return false;
			return TryCreateRuleBase(semanticModel.Compilation, ruleBaseEnum, iNode, interfaceSymbol, baseInterface, out ruleTypeSymbol, out baseTypeSymbol);
		}

		/// <summary>
		/// 解析 nameof(接口)，并确定其法则基类种类
		/// </summary>
		private static bool TryGetInterfaceRule(SemanticModel semanticModel, AttributeArgumentSyntax argumentSyntax, out INamedTypeSymbol interfaceSymbol, out INamedTypeSymbol baseInterface, out RuleBaseEnum ruleBaseEnum)
		{
			interfaceSymbol = null;
			baseInterface = null;
			ruleBaseEnum = default;

			if (argumentSyntax.Expression is not InvocationExpressionSyntax invocation) return false;
			if (invocation.Expression.ToString() != "nameof") return false;
			var nameofArgument = invocation.ArgumentList.Arguments.FirstOrDefault()?.Expression;
			if (nameofArgument == null) return false;
			if (nameofArgument is not SimpleNameSyntax simpleName) return false;
			return TryGetInterfaceRule(semanticModel, simpleName, out interfaceSymbol, out baseInterface, out ruleBaseEnum);
		}

		/// <summary>
		/// 收集类上的 [NodeRuleTarget(nameof(...))] 目标节点
		/// </summary>
		public static List<ITypeSymbol> GetTargetNodes(SemanticModel semanticModel, ClassDeclarationSyntax classSyntax)
		{
			var result = new List<ITypeSymbol>();
			if (classSyntax == null) return result;

			foreach (AttributeListSyntax list in classSyntax.AttributeLists)
			{
				foreach (AttributeSyntax attribute in list.Attributes)
				{
					if (!attribute.Name.ToString().Contains("NodeRuleTarget")) continue;
					if (attribute.ArgumentList == null || attribute.ArgumentList.Arguments.Count == 0) continue;
					if (attribute.ArgumentList.Arguments[0].Expression is not InvocationExpressionSyntax invocation) continue;
					if (invocation.Expression.ToString() != "nameof") continue;
					var nameofArgument = invocation.ArgumentList.Arguments.FirstOrDefault()?.Expression;
					if (nameofArgument == null) continue;
					if (semanticModel.GetSymbolInfo(nameofArgument).Symbol is INamedTypeSymbol target) result.Add(target);
				}
			}
			return result;
		}

		private static bool ValidateNode(ITypeSymbol node, INamedTypeSymbol interfaceSymbol)
		{
			if (!NamedSymbolHelper.TryGetInterfaceName(node, "INode", out _)) return false;
			return node.AllInterfaces.Any(i =>
				i.Name == "AsRule" &&
				i.TypeArguments.Length == 1 &&
				SymbolEqualityComparer.Default.Equals(i.TypeArguments[0], interfaceSymbol));
		}

		private static bool TryCreateRuleBase(Compilation compilation, RuleBaseEnum ruleBaseEnum, ITypeSymbol node, INamedTypeSymbol interfaceSymbol, INamedTypeSymbol baseInterface, out INamedTypeSymbol ruleTypeSymbol, out INamedTypeSymbol baseTypeSymbol)
		{
			ruleTypeSymbol = null;
			baseTypeSymbol = null;

			string baseName;
			switch (ruleBaseEnum)
			{
				case RuleBaseEnum.SendRule: baseName = GeneratorHelper.SendRule; break;
				case RuleBaseEnum.SendRuleAsync: baseName = GeneratorHelper.SendRuleAsync; break;
				case RuleBaseEnum.CallRule: baseName = GeneratorHelper.CallRule; break;
				case RuleBaseEnum.CallRuleAsync: baseName = GeneratorHelper.CallRuleAsync; break;
				default: return false;
			}

			int arity = 2 + baseInterface.TypeArguments.Length;
			INamedTypeSymbol baseDefinition = compilation.GetTypeByMetadataName($"WorldTree.{baseName}`{arity}");
			if (baseDefinition == null) return false;

			ITypeSymbol[] arguments = new ITypeSymbol[arity];
			arguments[0] = node;
			arguments[1] = interfaceSymbol;
			for (int i = 0; i < baseInterface.TypeArguments.Length; i++) arguments[i + 2] = baseInterface.TypeArguments[i];

			INamedTypeSymbol constructed = baseDefinition.Construct(arguments);
			ruleTypeSymbol = constructed;
			baseTypeSymbol = constructed;
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
