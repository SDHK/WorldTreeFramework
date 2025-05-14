/****************************************

* 作者：闪电黑客
* 日期：2024/6/24 14:49

* 描述：法则类型特性标记分析

*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Composition;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace WorldTree.Analyzer
{


	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class NodeRuleAttributeDiagnostic : NamingDiagnosticBase
	{
		public override SyntaxKind DeclarationKind => SyntaxKind.Attribute;

		protected override void DiagnosticAction(SyntaxNodeAnalysisContext context)
		{
			if (!ProjectDiagnosticSetting.ProjectDiagnostics.TryGetValue(context.Compilation.AssemblyName, out List<DiagnosticConfigGroup> objectDiagnostics)) return;

			// 获取语义模型
			SemanticModel semanticModel = context.SemanticModel;

			// 确保当前节点是 AttributeSyntax
			if (context.Node is not AttributeSyntax attributeSyntax) return;
			if (!attributeSyntax.Name.ToString().Contains("NodeRule")) return;


			// 检查特性是否标记在方法上
			var targetNode = attributeSyntax.Parent?.Parent;
			if (targetNode is MethodDeclarationSyntax methodDeclaration)
			{
				return;
			}

			// 如果特性没有标记在方法上，或者有空行，报告诊断错误
			foreach (DiagnosticConfigGroup objectDiagnostic in objectDiagnostics)
			{
				if (objectDiagnostic.Diagnostics.TryGetValue(DiagnosticKey.NodeRuleAttributeAnalysis, out DiagnosticConfig codeDiagnostic))
				{
					context.ReportDiagnostic(Diagnostic.Create(codeDiagnostic.Diagnostic, attributeSyntax.GetLocation(), attributeSyntax.ToString()));
					return;
				}
			}
		}
	}

	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NodeRuleAttributeNamingCodeFixProvider)), Shared]
	public class NodeRuleAttributeNamingCodeFixProvider : NamingCodeFixProviderBase<AttributeSyntax>
	{
		private List<string> typeNames = new();
		private List<string> typeTNames = new();

		public override SyntaxKind DeclarationKind => SyntaxKind.Attribute;

		public override bool CheckCodeFix(DiagnosticConfig codeDiagnostic, Document document)
		{
			return codeDiagnostic.DiagnosticKey == DiagnosticKey.NodeRuleAttributeAnalysis;
		}

		protected override async Task<Document> CodeFix(DiagnosticConfig codeDiagnostic, Document document, AttributeSyntax attributeSyntax, CancellationToken cancellationToken)
		{
			if (attributeSyntax.ArgumentList.Arguments.Count == 0) return null;

			AttributeArgumentSyntax argumentRule = attributeSyntax.ArgumentList.Arguments[0];

			SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken);

			if (!TryGetRuleType(semanticModel, argumentRule, out INamedTypeSymbol ruleTypeSymbol, out INamedTypeSymbol baseTypeSymbol, out RuleBaseEnum ruleBaseEnum))
				return null;



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
			classCode.AppendLine(GetRuleMethod(ruleBaseEnum, ruleTypeName, typeTName, genericTypeParameter, outType));

			// 将 classCode 的代码插入到 attributeSyntax 的下面
			var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
			if (root == null) return document;

			// 获取 attributeSyntax 的父节点
			var parentNode = attributeSyntax.Parent?.Parent;
			if (parentNode == null) return document;

			// 创建新的语法节点
			var newMethodNode = SyntaxFactory.ParseMemberDeclaration(classCode.ToString());
			if (newMethodNode == null) return document;

			// 插入新节点到父节点的后面
			var newRoot = root.InsertNodesAfter(parentNode, new[] { newMethodNode });

			// 返回更新后的文档
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