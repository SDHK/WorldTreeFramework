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

			if (!NodeRuleHelper.TryGetRuleType(semanticModel, argumentRule, out INamedTypeSymbol ruleTypeSymbol, out INamedTypeSymbol baseTypeSymbol, out RuleBaseEnum ruleBaseEnum))
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
				NodeRuleHelper.ParseTypeSymbol(types[i], AttributeTypeTNames);
			}

			// 获取方法的参数类型集合
			var parameters = methodDeclaration.ParameterList.Parameters;
			foreach (var parameter in parameters)
			{
				ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(parameter.Type).Type;
				if (typeSymbol != null)
					MethodTypeNames.Add(typeSymbol);
			}

			// 获取方法的返回值类型
			if (!(ruleBaseEnum == RuleBaseEnum.SendRule || ruleBaseEnum == RuleBaseEnum.ListenerRule))
			{
				var returnTypeSymbol = semanticModel.GetTypeInfo(methodDeclaration.ReturnType).Type;
				if (returnTypeSymbol != null)
					MethodTypeNames.Add(returnTypeSymbol);
			}
			// 获取方法的泛型参数类型
			if (methodDeclaration.TypeParameterList != null)
			{
				foreach (var typeParameter in methodDeclaration.TypeParameterList.Parameters)
				{
					ITypeSymbol typeSymbol = semanticModel.GetDeclaredSymbol(typeParameter);
					if (typeSymbol != null)
						MethodTypeTNames.Add(typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
				}
			}

			// 获取外层类的泛型参数，过滤掉已在外层类声明的类型参数
			var outerClassSyntax = methodDeclaration.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
			var outerTypeParams = NodeRuleHelper.GetOuterTypeParams(outerClassSyntax);
			List<string> filteredAttributeTypeTNames = AttributeTypeTNames.Where(t => !outerTypeParams.Contains(t)).ToList();

			bool isError = false;
			//拿到方法名称 ，判断是否是On开头
			if (!methodDeclaration.Identifier.Text.StartsWith("On")) isError = true;
			//判断是否是Rule结尾
			if (!methodDeclaration.Identifier.Text.EndsWith("Rule")) isError = true;
			// 比较特性参数类型和方法参数类型 顺序和类型是否一致
			if (AttributeTypeNames.Count != MethodTypeNames.Count || filteredAttributeTypeTNames.Count != MethodTypeTNames.Count)
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
			if (!NodeRuleHelper.TryGetRuleType(semanticModel, genericNameSyntax, out INamedTypeSymbol ruleTypeSymbol, out INamedTypeSymbol baseTypeSymbol, out RuleBaseEnum ruleBaseEnum))
				return null;

			//获取包含该泛型名称的类声明
			ClassDeclarationSyntax classSyntax = methodDeclaration.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First();
			string strThis = NodeRuleHelper.GetStrThis(semanticModel, classSyntax);

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
				NodeRuleHelper.ParseTypeSymbol(types[i], typeTNames);
			}
			// 收集参数变量名称
			for (int i = 0; i < methodDeclaration.ParameterList.Parameters.Count; i++)
			{
				//如果是监听法则，且是最后一个泛型参数，则直接跳过，不添加
				if (ruleBaseEnum == RuleBaseEnum.ListenerRule && i == types.Count - 1) continue;
				var parameter = methodDeclaration.ParameterList.Parameters[i];
				typeArgNames.Add(parameter.Identifier.Text);
			}

			bool isCall = ruleBaseEnum is RuleBaseEnum.CallRule or RuleBaseEnum.CallRuleAsync;
			string genericTypeParameter = NodeRuleHelper.GetRuleTypeParameter(typeNames, isCall, out string outType, typeArgNames);
			string typeTName = NodeRuleHelper.GetTypeTName(typeTNames, classSyntax);

			//拿到genericNameSyntax 的代码文字
			var genericNameText = genericNameSyntax.ToFullString().TrimStart().TrimEnd();

			// 获取 ruleTypeSymbol 的类型名称（不带命名空间和泛型）
			var ruleTypeName = "On" + ruleTypeSymbol.Name;

			StringBuilder classCode = new();
			classCode.AppendLine(NodeRuleHelper.GetRuleMethod(ruleBaseEnum, ruleTypeName, typeTName, genericTypeParameter, outType, strThis));
			// 生成代码，并替换方法名称和参数
			var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

			var newMethodDeclaration = SyntaxFactory.ParseMemberDeclaration(classCode.ToString())
				.WithLeadingTrivia(methodDeclaration.GetLeadingTrivia())
				.WithTrailingTrivia(methodDeclaration.GetTrailingTrivia()) as MethodDeclarationSyntax;

			// 保留原方法的特性和方法体
			var updatedMethod = methodDeclaration
				.WithIdentifier(newMethodDeclaration.Identifier)
				.WithParameterList(newMethodDeclaration.ParameterList)
				.WithTypeParameterList(newMethodDeclaration.TypeParameterList);

			// 替换原有的方法节点
			var newRoot = root.ReplaceNode(methodDeclaration, updatedMethod);
			return document.WithSyntaxRoot(newRoot);
		}
	}
}
