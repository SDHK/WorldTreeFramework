using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;

namespace WorldTree.Analyzer
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class AsyncAnalyzer : DiagnosticAnalyzer
	{
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(AsyncMethodAnalyzerRole.Role);

		public override void Initialize(AnalysisContext context)
		{
			//不对生成的代码进行分析
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

			//启动并发执行
			context.EnableConcurrentExecution();

			//注册语法树分析回调
			context.RegisterSyntaxNodeAction(this.AnalyzeMemberAccessExpression, SyntaxKind.MethodDeclaration);
		}

		private void AnalyzeMemberAccessExpression(SyntaxNodeAnalysisContext context)
		{
			//节点不是方法声明
			if (context.Node is not MethodDeclarationSyntax methodDeclarationSyntax) return;

			IMethodSymbol methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclarationSyntax) as IMethodSymbol;
			if (methodSymbol == null) return;

			if (methodSymbol.IsAsync && methodSymbol.ReturnsVoid)
			{
				var diagnostic = Diagnostic.Create(AsyncMethodAnalyzerRole.Role, methodDeclarationSyntax.Identifier.GetLocation(), methodSymbol.Name);
				context.ReportDiagnostic(diagnostic);
			}

			////节点不是成员访问表达式
			//if (context.Node is not MemberAccessExpressionSyntax memberAccessExpressionSyntax) return;

			//if (memberAccessExpressionSyntax?.Parent is not InvocationExpressionSyntax invocationExpressionSyntax
			//	|| context.SemanticModel.GetSymbolInfo(invocationExpressionSyntax).Symbol is not IMethodSymbol methodSymbol) return;

			//if (methodSymbol.IsAsync && methodSymbol.ReturnsVoid)
			//{
			//	Diagnostic diagnostic = Diagnostic.Create(AsyncMethodAnalyzerRole.Role, memberAccessExpressionSyntax?.Name.Identifier.GetLocation(), memberAccessExpressionSyntax?.Name);
			//	context.ReportDiagnostic(diagnostic);
			//}
		}
	}

	public static class AsyncMethodAnalyzerRole
	{
		private const string Title = "async方法使用void类型返回值";

		private const string MessageFormat = "方法: {0} 需要有非void类型的返回值???";

		private const string Description = "async方法使用void类型返回值.???";

		public static readonly DiagnosticDescriptor Role = new DiagnosticDescriptor(
					"Test001",
					Title,
					MessageFormat,
					"Hotfix",
					DiagnosticSeverity.Error,
					true,
					Description);
	}

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class ExtensionMethodConstraintAnalyzer : DiagnosticAnalyzer
	{
		private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
			id: "ExtensionMethodConstraint",
			title: "Extension method constraint violation",

			//messageFormat: "The extension method call does not satisfy the defined constraints.",
			messageFormat: "扩展方法调用不满足定义的约束。",
			category: "Usage",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			//context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.InvocationExpression);
		}

		private void AnalyzeNode(SyntaxNodeAnalysisContext context)
		{
			var invocationExpression = (InvocationExpressionSyntax)context.Node;
			var semanticModel = context.SemanticModel;

			// 检查是否为扩展方法调用
			var symbolInfo = semanticModel.GetSymbolInfo(invocationExpression);
			var methodSymbol = symbolInfo.Symbol as IMethodSymbol; if (methodSymbol == null ||
						!methodSymbol.IsStatic || methodSymbol.ReceiverType == null) return;

			// 获取扩展方法的this参数类型
			var thisParameterType = methodSymbol.ReceiverType;

			// 获取扩展方法调用的目标类型
			var invocationTargetType = semanticModel.GetTypeInfo(invocationExpression.Expression).Type;

			// 检查目标类型是否为值类型或者继承自this参数类型
			if (invocationTargetType.IsValueType || invocationTargetType.BaseType == null || !invocationTargetType.BaseType.Equals(thisParameterType, SymbolEqualityComparer.Default))
			{
				var diagnostic = Diagnostic.Create(Rule, invocationExpression.GetLocation());
				context.ReportDiagnostic(diagnostic);
			}
			else
			{
				// 检查传入参数是否为值类型或者实现了约束接口
				foreach (var parameterType in methodSymbol.TypeParameters)
				{
					var argumentType = semanticModel.GetTypeInfo(invocationExpression.ArgumentList.Arguments[parameterType.Ordinal]).Type;
					if (argumentType.IsValueType || !parameterType.ConstraintTypes.Any(constraintType => argumentType.AllInterfaces.Contains(constraintType, SymbolEqualityComparer.Default)))
					{
						var diagnostic = Diagnostic.Create(Rule, invocationExpression.GetLocation());
						context.ReportDiagnostic(diagnostic);
						break;
					}
				}
			}
		}

		private void AnalyzeNode1(SyntaxNodeAnalysisContext context)
		{
			var invocationExpression = (InvocationExpressionSyntax)context.Node;
			var semanticModel = context.SemanticModel;

			// 检查是否为扩展方法调用
			var symbolInfo = semanticModel.GetSymbolInfo(invocationExpression); var
			methodSymbol = symbolInfo.Symbol as IMethodSymbol; if (methodSymbol == null ||
			!methodSymbol.IsStatic || methodSymbol.ReceiverType == null) return;

			// 对于所有扩展方法调用,都报告诊断结果
			var diagnostic = Diagnostic.Create(Rule, invocationExpression.GetLocation());
			context.ReportDiagnostic(diagnostic);
		}
	}
}