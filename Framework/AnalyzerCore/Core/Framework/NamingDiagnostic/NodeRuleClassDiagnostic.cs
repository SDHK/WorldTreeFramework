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

	public abstract class NodeRuleClassDiagnostic<C> : NamingDiagnosticBase<C>
		where C : ProjectDiagnosticConfig, new()
	{
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


	public abstract class NodeRuleClassCodeFixProvider<C> : NamingCodeFixProviderBase<GenericNameSyntax, C>
		where C : ProjectDiagnosticConfig, new()
	{
		private List<string> typeNames = new();
		private List<string> typeTNames = new();

		public override SyntaxKind DeclarationKind => SyntaxKind.GenericName;

		protected override async Task<Document> CodeFix(DiagnosticConfig codeDiagnostic, Document document, GenericNameSyntax genericNameSyntax, CancellationToken cancellationToken)
		{
			SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken);

			if (!TryGetRuleType(semanticModel, genericNameSyntax, out INamedTypeSymbol ruleTypeSymbol, out INamedTypeSymbol baseTypeSymbol, out RuleBaseEnum ruleBaseEnum))
				return null;

			//获取包含该泛型名称的类声明
			ClassDeclarationSyntax classSyntax = genericNameSyntax.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First();
			string strThis = NodeRuleHelper.GetStrThis(semanticModel, classSyntax);

			//拿到genericNameSyntax 的代码文字
			var genericNameText = genericNameSyntax.ToFullString().TrimStart().TrimEnd();
			// 删除 genericNameText 末尾的任何标点符号
			genericNameText = genericNameText.TrimEnd(';', '}', '.', ',', '!', '?', ':', ')', ']', ' ', '\'', '\"');

			typeNames.Clear();
			typeTNames.Clear();
			List<ITypeSymbol> types = baseTypeSymbol.TypeArguments.ToList();
			//监听法则最后一个泛型参数是Rule类型，直接忽略
			if (ruleBaseEnum == RuleBaseEnum.ListenerRule) types.RemoveAt(types.Count - 1);
			for (int i = 0; i < types.Count; i++)
			{
				//基类第二个泛型参数是Rule类型，直接忽略
				if (i == 1) continue;
				typeNames.Add(types[i].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
				NodeRuleHelper.ParseTypeSymbol(types[i], typeTNames);
			}
			bool isCall = ruleBaseEnum is RuleBaseEnum.CallRule or RuleBaseEnum.CallRuleAsync;
			string genericTypeParameter = NodeRuleHelper.GetRuleTypeParameter(typeNames, isCall, out string outType);
			string typeTName = NodeRuleHelper.GetTypeTName(typeTNames, classSyntax);

			// 获取 ruleTypeSymbol 的类型名称（不带命名空间和泛型）
			var ruleTypeName = "On" + ruleTypeSymbol.Name;

			StringBuilder classCode = new();
			classCode.AppendLine($"		[NodeRule(nameof({genericNameText}))]");
			classCode.AppendLine(NodeRuleHelper.GetRuleMethod(ruleBaseEnum, ruleTypeName, typeTName, genericTypeParameter, outType, strThis));
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
		/// 尝试获取法则类型
		/// </summary>
		private bool TryGetRuleType(SemanticModel semanticModel, GenericNameSyntax standaloneIdentifier, out INamedTypeSymbol ruleTypeSymbol, out INamedTypeSymbol baseTypeSymbol, out RuleBaseEnum ruleBaseEnum)
		{
			ruleBaseEnum = default;
			ruleTypeSymbol = null;
			baseTypeSymbol = null;

			var symbolInfo = semanticModel.GetSymbolInfo(standaloneIdentifier);
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
	}

}
