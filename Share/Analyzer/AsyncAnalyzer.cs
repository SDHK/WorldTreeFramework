using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

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


			IMethodSymbol? methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclarationSyntax) as IMethodSymbol;
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

		public static readonly DiagnosticDescriptor Role =new DiagnosticDescriptor(
					"Test001",
					Title,
					MessageFormat,
					"Hotfix",
					DiagnosticSeverity.Error,
					true,
					Description);
	}


}
