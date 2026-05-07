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
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace WorldTree.Analyzer
{

	public abstract class NodeRuleAttributeDiagnostic<C> : NamingDiagnosticBase<C>
		where C : ProjectDiagnosticConfig, new()
	{
		public override SyntaxKind DeclarationKind => SyntaxKind.Attribute;

		protected override void DiagnosticAction(SyntaxNodeAnalysisContext context)
		{
			if (!TryGetDiagnosticConfigGroup(context.Compilation.AssemblyName, out List<DiagnosticConfigGroup> DiagnosticGroups)) return;

			SemanticModel semanticModel = context.SemanticModel;

			if (context.Node is not AttributeSyntax attributeSyntax) return;
			if (!attributeSyntax.Name.ToString().Contains("NodeRule")) return;

			// 检查特性是否标记在方法上
			var targetNode = attributeSyntax.Parent?.Parent;
			if (targetNode is MethodDeclarationSyntax)
				return;

			// 如果特性没有标记在方法上，报告诊断错误
			foreach (DiagnosticConfigGroup objectDiagnostic in DiagnosticGroups)
			{
				if (objectDiagnostic.Diagnostics.TryGetValue(DiagnosticKey.NodeRuleAttributeAnalysis, out DiagnosticConfig codeDiagnostic))
				{
					context.ReportDiagnostic(Diagnostic.Create(codeDiagnostic.Diagnostic, attributeSyntax.GetLocation(), attributeSyntax.ToString()));
					return;
				}
			}
		}
	}

	public abstract class NodeRuleAttributeCodeFixProvider<C> : NamingCodeFixProviderBase<AttributeSyntax, C>
		where C : ProjectDiagnosticConfig, new()
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

			if (!NodeRuleHelper.TryGetRuleType(semanticModel, argumentRule, out INamedTypeSymbol ruleTypeSymbol, out INamedTypeSymbol baseTypeSymbol, out RuleBaseEnum ruleBaseEnum))
				return null;

			//获取包含该泛型名称的类声明
			ClassDeclarationSyntax classSyntax = attributeSyntax.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First();
			string strThis = NodeRuleHelper.GetStrThis(semanticModel, classSyntax);

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
			classCode.AppendLine(NodeRuleHelper.GetRuleMethod(ruleBaseEnum, ruleTypeName, typeTName, genericTypeParameter, outType, strThis));

			// 将 classCode 的代码插入到 attributeSyntax 的下面
			var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
			if (root == null) return document;

			var parentNode = attributeSyntax.Parent?.Parent;
			if (parentNode == null) return document;

			var newMethodNode = SyntaxFactory.ParseMemberDeclaration(classCode.ToString());
			if (newMethodNode == null) return document;

			// 插入新节点到父节点的后面
			var newRoot = root.InsertNodesAfter(parentNode, new[] { newMethodNode });
			return document.WithSyntaxRoot(newRoot);
		}
	}
}
