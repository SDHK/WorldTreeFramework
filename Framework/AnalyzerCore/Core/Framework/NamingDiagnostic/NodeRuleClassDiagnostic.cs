/****************************************

* 作者：闪电黑客
* 日期： 2024/6/24 14:49

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

		/// <summary>
		/// 同时注册标识符和泛型名：接口 Open 是 IdentifierName，规则类 OpenRule&lt;T&gt; 是 GenericName
		/// </summary>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();
			context.RegisterCompilationStartAction(analysisContext =>
			{
				if (!TryGetDiagnosticConfigGroup(analysisContext.Compilation.AssemblyName, out _)) return;
				analysisContext.RegisterSyntaxNodeAction(
					ctx => { if (Configs.IsAuthorMatch(ctx.Node)) DiagnosticAction(ctx); },
					SyntaxKind.IdentifierName,
					SyntaxKind.GenericName);
			});
		}

		protected override void DiagnosticAction(SyntaxNodeAnalysisContext context)
		{
			if (!TryGetDiagnosticConfigGroup(context.Compilation.AssemblyName, out List<DiagnosticConfigGroup> DiagnosticGroups)) return;

			// 这可能是一个孤立的类型标识符（接口 Open 或规则类 OpenRule<T>）
			if (context.Node is not SimpleNameSyntax standaloneName) return;
			if (standaloneName.Parent is not MemberDeclarationSyntax) return;

			var symbolInfo = context.SemanticModel.GetSymbolInfo(standaloneName);
			if (symbolInfo.Symbol is not INamedTypeSymbol namedTypeSymbol) return;

			// 只认法则接口
			if (namedTypeSymbol.TypeKind != TypeKind.Interface) return;
			if (!(NamedSymbolHelper.TryGetInterfaceName(namedTypeSymbol, GeneratorHelper.ISendRule, out _) ||
				NamedSymbolHelper.TryGetInterfaceName(namedTypeSymbol, GeneratorHelper.ISendRuleAsync, out _) ||
				NamedSymbolHelper.TryGetInterfaceName(namedTypeSymbol, GeneratorHelper.ICallRule, out _) ||
				NamedSymbolHelper.TryGetInterfaceName(namedTypeSymbol, GeneratorHelper.ICallRuleAsync, out _))) return;

			foreach (var objectDiagnostic in DiagnosticGroups)
			{
				if (objectDiagnostic.Diagnostics.TryGetValue(DiagnosticKey.NodeRuleClassAnalysis, out DiagnosticConfig codeDiagnostic))
				{
					context.ReportDiagnostic(Diagnostic.Create(codeDiagnostic.Diagnostic, standaloneName.GetLocation(), standaloneName.ToString()));
					return;
				}
			}
		}
	}


	public abstract class NodeRuleClassCodeFixProvider<C> : NamingCodeFixProviderBase<SimpleNameSyntax, C>
		where C : ProjectDiagnosticConfig, new()
	{
		private List<string> typeNames = new();
		private List<string> typeTNames = new();

		public override SyntaxKind DeclarationKind => SyntaxKind.GenericName;

		protected override async Task<Document> CodeFix(DiagnosticConfig codeDiagnostic, Document document, SimpleNameSyntax standaloneName, CancellationToken cancellationToken)
		{
			SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken);

			//获取包含该类型名称的类声明
			ClassDeclarationSyntax classSyntax = standaloneName.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First();

			if (!TryGetRuleType(semanticModel, classSyntax, standaloneName, out INamedTypeSymbol ruleTypeSymbol, out INamedTypeSymbol baseTypeSymbol, out RuleBaseEnum ruleBaseEnum))
				return null;

			string strThis = NodeRuleHelper.GetStrThis(semanticModel, classSyntax);

			//拿到类型名称的代码文字
			var typeText = standaloneName.ToFullString().TrimStart().TrimEnd();
			// 删除 typeText 末尾的任何标点符号
			typeText = typeText.TrimEnd(';', '}', '.', ',', '!', '?', ':', ')', ']', ' ', '\'', '\"');

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

			// nameof(LayerChange) -> OnLayerChangeRule
			string simpleName = standaloneName.Identifier.Text;
			string ruleTypeName = "On" + (simpleName.EndsWith("Rule") ? simpleName : simpleName + "Rule");

			StringBuilder classCode = new();
			classCode.AppendLine($"		[NodeRule(nameof({typeText}))]");
			classCode.AppendLine(NodeRuleHelper.GetRuleMethod(ruleBaseEnum, ruleTypeName, typeTName, genericTypeParameter, outType, strThis));
			// 生成代码，并替换原有的类型节点
			var root = await document.GetSyntaxRootAsync(cancellationToken);
			var newMethod = SyntaxFactory.ParseMemberDeclaration(classCode.ToString())
				.WithLeadingTrivia(standaloneName.GetLeadingTrivia())
				.WithTrailingTrivia(standaloneName.GetTrailingTrivia());

			// 替换原有的节点
			var newRoot = root.ReplaceNode(standaloneName.Parent, newMethod);
			return document.WithSyntaxRoot(newRoot);
		}

		/// <summary>
		/// 尝试获取法则类型：接口形式走接口，类形式走生成的规则类
		/// </summary>
		private bool TryGetRuleType(SemanticModel semanticModel, ClassDeclarationSyntax classSyntax, SimpleNameSyntax standaloneName, out INamedTypeSymbol ruleTypeSymbol, out INamedTypeSymbol baseTypeSymbol, out RuleBaseEnum ruleBaseEnum)
		{
			ruleBaseEnum = default;
			ruleTypeSymbol = null;
			baseTypeSymbol = null;

			if (semanticModel.GetSymbolInfo(standaloneName).Symbol is not INamedTypeSymbol namedTypeSymbol) return false;
			if (namedTypeSymbol.TypeKind != TypeKind.Interface) return false;

			if (!NodeRuleHelper.TryGetInterfaceRule(semanticModel, standaloneName, out INamedTypeSymbol interfaceSymbol, out INamedTypeSymbol baseInterface, out ruleBaseEnum)) return false;
			return NodeRuleHelper.TryGetRuleTypeFromInterfaceSymbol(semanticModel, classSyntax, interfaceSymbol, baseInterface, ruleBaseEnum, out ruleTypeSymbol, out baseTypeSymbol);
		}
	}

}
