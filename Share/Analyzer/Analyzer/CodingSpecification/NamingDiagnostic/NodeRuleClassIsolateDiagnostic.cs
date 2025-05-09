/****************************************

* 作者：闪电黑客
* 日期：2024/6/24 14:49

* 描述：法则类型孤立分析

*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;


namespace WorldTree.Analyzer
{
	//[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class NodeRuleClassIsolateDiagnostic : NamingDiagnosticBase
	{
		public override SyntaxKind DeclarationKind => SyntaxKind.ExpressionStatement;
		protected override void DiagnosticAction(SyntaxNodeAnalysisContext context)
		{
			// 检查是否为表达式语句
			var expressionStatement = (ExpressionStatementSyntax)context.Node;

			// 检查表达式是否为类型名称
			if (expressionStatement.Expression is IdentifierNameSyntax ||
				expressionStatement.Expression is GenericNameSyntax)
			{
				var semanticModel = context.SemanticModel;
				var symbol = semanticModel.GetSymbolInfo(expressionStatement.Expression).Symbol;

				// 如果表达式引用的是一个类型而不是其他种类的符号（如方法、属性等）
				if (symbol is ITypeSymbol)
				{
					//var diagnostic = Diagnostic.Create(Rule, expressionStatement.GetLocation(),
					//	expressionStatement.Expression.ToString());
					//context.ReportDiagnostic(diagnostic);
				}
				// 或者表达式解析为空，但表达式看起来像一个类型
				else if (symbol == null &&
						 (expressionStatement.Expression is GenericNameSyntax ||
						  (expressionStatement.Expression is IdentifierNameSyntax id &&
						   char.IsUpper(id.Identifier.ValueText[0]))))
				{
					//var diagnostic = Diagnostic.Create(Rule, expressionStatement.GetLocation(),
					//	expressionStatement.Expression.ToString());
					//context.ReportDiagnostic(diagnostic);
				}
			}
		}

	}




}