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
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace WorldTree.Analyzer
{

	public abstract class NodeRuleClassDiagnostic : NamingDiagnosticBase
	{
		// 更改为处理字段声明
		public override SyntaxKind DeclarationKind => SyntaxKind.GenericName;

		protected override void DiagnosticAction(SyntaxNodeAnalysisContext context)
		{
			if (!TryGetDiagnosticConfigGroup(context.Compilation.AssemblyName, out List<DiagnosticConfigGroup> DiagnosticGroups)) return;

			// 这可能是一个孤立的泛型类型标识符
			if (context.Node is not GenericNameSyntax standaloneGeneric) return;
			if (standaloneGeneric.Parent is not MemberDeclarationSyntax) return;

			var symbolInfo = context.SemanticModel.GetSymbolInfo(standaloneGeneric);
			if (symbolInfo.Symbol is not INamedTypeSymbol namedTypeSymbol) return;

			if (!NamedSymbolHelper.IsDerivedFrom(namedTypeSymbol, NamedSymbolHelper.ToINamedTypeSymbol(context.Compilation, GeneratorHelper.IRule), out _)) return;

			foreach (var objectDiagnostic in DiagnosticGroups)
			{
				if (objectDiagnostic.Diagnostics.TryGetValue(DiagnosticKey.NodeRuleClassAnalysis, out DiagnosticConfig codeDiagnostic))
				{
					context.ReportDiagnostic(Diagnostic.Create(codeDiagnostic.Diagnostic, standaloneGeneric.GetLocation(), standaloneGeneric.ToString()));
					return;
				}
			}
		}
	}


	public abstract class NodeRuleClassCodeFixProvider : NamingCodeFixProviderBase<GenericNameSyntax>
	{
		private List<string> typeNames = new();
		private List<string> typeTNames = new();

		public override SyntaxKind DeclarationKind => SyntaxKind.GenericName;

		protected override async Task<Document> CodeFix(DiagnosticConfig codeDiagnostic, Document document, GenericNameSyntax genericNameSyntax, CancellationToken cancellationToken)
		{
			SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken);

			if (!TryGetRuleType(semanticModel, genericNameSyntax, out INamedTypeSymbol ruleTypeSymbol, out INamedTypeSymbol baseTypeSymbol, out RuleBaseEnum ruleBaseEnum))
				return null;

			//拿到genericNameSyntax 的代码文字
			var genericNameText = genericNameSyntax.ToFullString().TrimStart().TrimEnd();
			// 删除 genericNameText 末尾的任何标点符号
			genericNameText = genericNameText.TrimEnd(';', '}', '.', ',', '!', '?', ':', ')', ']', ' ', '\'', '\"');

			typeNames.Clear();
			typeTNames.Clear();
			var types = baseTypeSymbol.TypeArguments;
			for (int i = 0; i < types.Length; i++)
			{
				//基类第二个泛型参数是Rule类型，直接忽略
				if (i == 1) continue;
				typeNames.Add(types[i].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
				ParseTypeSymbol(types[i], typeTNames);
			}
			bool isCall = ruleBaseEnum is RuleBaseEnum.CallRule or RuleBaseEnum.CallRuleAsync;
			string genericTypeParameter = GetRuleTypeParameter(typeNames, isCall, out string outType);
			string typeTName = typeTNames.Count == 0 ? "" : $"<{string.Join(", ", typeTNames)}>";


			// 获取 ruleTypeSymbol 的类型名称（不带命名空间和泛型）
			var ruleTypeName = "On" + ruleTypeSymbol.Name.Replace("Rule", "");

			StringBuilder classCode = new();
			classCode.AppendLine($"		[NodeRule(nameof({genericNameText}))]");
			classCode.AppendLine(GetRuleMethod(ruleBaseEnum, ruleTypeName, typeTName, genericTypeParameter, outType));
			// 生成代码，并替换原有的泛型代码
			var root = await document.GetSyntaxRootAsync(cancellationToken);
			var newMethod = SyntaxFactory.ParseMemberDeclaration(classCode.ToString())
				.WithLeadingTrivia(genericNameSyntax.GetLeadingTrivia())
				.WithTrailingTrivia(genericNameSyntax.GetTrailingTrivia());

			// 替换原有的泛型节点
			var newRoot = root.ReplaceNode(genericNameSyntax.Parent, newMethod);
			return document.WithSyntaxRoot(newRoot);
		}

		/// <summary>
		/// 获取法则方法
		/// </summary>
		public string GetRuleMethod(RuleBaseEnum ruleBaseEnum, string ruleTypeName, string typeTName, string genericTypeParameter, string outType)
		=> ruleBaseEnum switch
		{
			RuleBaseEnum.SendRule =>
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
		private bool TryGetRuleType(SemanticModel semanticModel, GenericNameSyntax standaloneIdentifier, out INamedTypeSymbol ruleTypeSymbol, out INamedTypeSymbol baseTypeSymbol, out RuleBaseEnum ruleBaseEnum)
		{
			ruleBaseEnum = default;
			ruleTypeSymbol = null;
			baseTypeSymbol = null;

			// 获取符号信息
			var symbolInfo = semanticModel.GetSymbolInfo(standaloneIdentifier);
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

		/// <summary>
		/// 转换 泛型参数 ：T0, T1, T2, T3 =&gt; T0 self, T1 arg1, T2 arg2, T3 arg3
		/// </summary>
		private string GetRuleTypeParameter(List<string> Types, bool isCall, out string outType)
		{
			var result = new StringBuilder();
			for (int i = 0; i < Types.Count; i++)
			{
				if (i == 0)
				{
					result.Append($"{Types[i]} self");
				}
				else if (!(isCall && i == Types.Count - 1))
				{
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