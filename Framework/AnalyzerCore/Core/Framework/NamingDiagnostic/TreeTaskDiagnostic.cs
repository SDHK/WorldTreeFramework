/****************************************

* 作者：闪电黑客
* 日期：2026/10/31

* 描述：async TreeTask 必须有 await；第一个 await 之前不能 return/throw。
*       抽象方法没有方法体，不参与这两条检查。诊断配置走 ProjectAnalyzerSetting。

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WorldTree.Analyzer
{
	/// <summary>
	/// TreeTask 语法检查
	/// </summary>
	public static class TreeTaskDiagnosticHelper
	{
		/// <summary>
		/// 是否是 WorldTree.TreeTask / TreeTask<T>
		/// </summary>
		public static bool IsTreeTask(ITypeSymbol type)
		{
			if (type is not INamedTypeSymbol named) return false;
			if (named.ContainingNamespace?.ToDisplayString() != "WorldTree") return false;
			return named.Name == "TreeTask" && named.Arity <= 1;
		}

		/// <summary>
		/// 方法体里完全没有 await
		/// </summary>
		public static bool HasNoAwait(SemanticModel model, SyntaxToken identifier)
		{
			MethodDeclarationSyntax method = identifier.Parent as MethodDeclarationSyntax;
			if (!IsTarget(model, method)) return false;
			return !Visit(method).HasAwait;
		}

		/// <summary>
		/// 第一个 await 之前有 return/throw
		/// </summary>
		public static bool HasEarlyExit(SemanticModel model, SyntaxToken identifier)
		{
			MethodDeclarationSyntax method = identifier.Parent as MethodDeclarationSyntax;
			if (!IsTarget(model, method)) return false;

			MethodBodyVisitor visitor = Visit(method);
			return visitor.HasAwait
				&& visitor.FirstExitNode != null
				&& visitor.FirstExitStart < visitor.FirstAwaitStart;
		}

		private static bool IsTarget(SemanticModel model, MethodDeclarationSyntax method)
		{
			if (method == null) return false;
			// 抽象方法、接口声明没有方法体，无从检查 await
			if (method.Body == null && method.ExpressionBody == null) return false;

			IMethodSymbol symbol = model.GetDeclaredSymbol(method) as IMethodSymbol;
			if (symbol == null || symbol.IsAbstract) return false;
			return symbol.IsAsync && IsTreeTask(symbol.ReturnType);
		}

		private static MethodBodyVisitor Visit(MethodDeclarationSyntax method)
		{
			MethodBodyVisitor visitor = new MethodBodyVisitor();
			if (method.Body != null) visitor.Visit(method.Body);
			else if (method.ExpressionBody != null) visitor.Visit(method.ExpressionBody.Expression);
			return visitor;
		}

		/// <summary>
		/// 只收集当前方法自己的 await / return / throw，跳过 lambda 和局部函数。
		/// </summary>
		private sealed class MethodBodyVisitor : CSharpSyntaxWalker
		{
			public bool HasAwait;
			public int FirstAwaitStart = int.MaxValue;
			public int FirstExitStart = int.MaxValue;
			public SyntaxNode FirstExitNode;

			public override void VisitAwaitExpression(AwaitExpressionSyntax node)
			{
				HasAwait = true;
				if (node.SpanStart < FirstAwaitStart) FirstAwaitStart = node.SpanStart;
				base.VisitAwaitExpression(node);
			}

			public override void VisitReturnStatement(ReturnStatementSyntax node)
			{
				// return await X：await 先执行，返回本身不算“await 前退出”
				if (node.Expression == null || !node.Expression.DescendantNodesAndSelf().OfType<AwaitExpressionSyntax>().Any())
					RecordExit(node);
				base.VisitReturnStatement(node);
			}

			public override void VisitThrowStatement(ThrowStatementSyntax node)
			{
				RecordExit(node);
				base.VisitThrowStatement(node);
			}

			public override void VisitThrowExpression(ThrowExpressionSyntax node)
			{
				RecordExit(node);
				base.VisitThrowExpression(node);
			}

			public override void VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node) { }
			public override void VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node) { }
			public override void VisitAnonymousMethodExpression(AnonymousMethodExpressionSyntax node) { }
			public override void VisitLocalFunctionStatement(LocalFunctionStatementSyntax node) { }

			private void RecordExit(SyntaxNode node)
			{
				if (node.SpanStart < FirstExitStart)
				{
					FirstExitStart = node.SpanStart;
					FirstExitNode = node;
				}
			}
		}
	}

	/// <summary>
	/// async TreeTask 分析器
	/// </summary>
	public abstract class TreeTaskDiagnostic<C> : NamingDiagnosticBase<C>
		where C : ProjectDiagnosticConfig, new()
	{
		public override SyntaxKind DeclarationKind => SyntaxKind.MethodDeclaration;

		/// <summary>
		/// 正确性规则，不按作者过滤；程序集过滤仍走 Configs。
		/// </summary>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();
			context.RegisterCompilationStartAction(analysisContext =>
			{
				if (!TryGetDiagnosticConfigGroup(analysisContext.Compilation.AssemblyName, out _)) return;
				analysisContext.RegisterSyntaxNodeAction(DiagnosticAction, DeclarationKind);
			});
		}

		protected override void DiagnosticAction(SyntaxNodeAnalysisContext context)
		{
			if (!TryGetDiagnosticConfigGroup(context.Compilation.AssemblyName, out List<DiagnosticConfigGroup> groups)) return;

			MethodDeclarationSyntax method = (MethodDeclarationSyntax)context.Node;
			IMethodSymbol symbol = context.SemanticModel.GetDeclaredSymbol(method, context.CancellationToken) as IMethodSymbol;
			if (symbol == null) return;

			foreach (DiagnosticConfigGroup group in groups)
			{
				if (group.Screen != null && !group.Screen(context.Compilation, symbol)) continue;

				if (group.Diagnostics.TryGetValue(DiagnosticKey.TreeTaskAwaitMissing, out DiagnosticConfig awaitConfig)
					&& awaitConfig.Check(context.SemanticModel, method.Identifier))
				{
					Report(context, awaitConfig, method, symbol);
				}

				if (group.Diagnostics.TryGetValue(DiagnosticKey.TreeTaskEarlyExit, out DiagnosticConfig exitConfig)
					&& exitConfig.Check(context.SemanticModel, method.Identifier))
				{
					Report(context, exitConfig, method, symbol);
				}
			}
		}

		private static void Report(
			SyntaxNodeAnalysisContext context,
			DiagnosticConfig config,
			MethodDeclarationSyntax method,
			IMethodSymbol symbol)
		{
			context.ReportDiagnostic(Diagnostic.Create(
				config.Diagnostic,
				method.Identifier.GetLocation(),
				symbol.Name));
		}
	}

	/// <summary>
	/// 在方法体开头插入 await TreeTaskCompleted()
	/// </summary>
	public abstract class TreeTaskDiagnosticCodeFixProvider<C> : NamingCodeFixProviderBase<MethodDeclarationSyntax, C>
		where C : ProjectDiagnosticConfig, new()
	{
		public override SyntaxKind DeclarationKind => SyntaxKind.MethodDeclaration;

		protected override async Task<Document> CodeFix(
			DiagnosticConfig codeDiagnostic,
			Document document,
			MethodDeclarationSyntax decl,
			CancellationToken cancellationToken)
		{
			if (decl.Body == null) return document;

			SemanticModel model = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
			IMethodSymbol symbol = model?.GetDeclaredSymbol(decl, cancellationToken) as IMethodSymbol;
			if (symbol == null) return document;

			bool isStatic = decl.Modifiers.Any(SyntaxKind.StaticKeyword);
			ITypeSymbol receiverType;
			ExpressionSyntax receiver;

			if (isStatic)
			{
				IParameterSymbol parameter = symbol.Parameters.FirstOrDefault(x => x.Name == "self")
					?? symbol.Parameters.FirstOrDefault();
				if (parameter == null) return document;
				receiverType = parameter.Type;
				receiver = SyntaxFactory.IdentifierName(parameter.Name);
			}
			else
			{
				receiverType = symbol.ContainingType;
				receiver = SyntaxFactory.ThisExpression();
			}

			// 接收者不是 IUnit 时不修改，避免生成 await this.TreeTaskCompleted()
			if (!IsUnit(receiverType)) return document;

			ExpressionStatementSyntax statement = SyntaxFactory.ExpressionStatement(
				SyntaxFactory.AwaitExpression(
					SyntaxFactory.InvocationExpression(
						SyntaxFactory.MemberAccessExpression(
							SyntaxKind.SimpleMemberAccessExpression,
							receiver,
							SyntaxFactory.IdentifierName("TreeTaskCompleted")))));

			BlockSyntax newBody = decl.Body.WithStatements(decl.Body.Statements.Insert(0, statement));
			SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
			if (root == null) return document;

			return document.WithSyntaxRoot(root.ReplaceNode(decl, decl.WithBody(newBody)));
		}

		private static bool IsUnit(ITypeSymbol type)
		{
			if (type == null) return false;
			if (type.ToDisplayString() == "WorldTree.IUnit") return true;
			return type.AllInterfaces.Any(x => x.ToDisplayString() == "WorldTree.IUnit");
		}
	}
}
