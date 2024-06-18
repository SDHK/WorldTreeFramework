/****************************************

* 作者：闪电黑客
* 日期：2024/6/18 16:35

* 描述：

*/
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
	public class FieldNamingAnalyzer : DiagnosticAnalyzer
	{
		private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
			"FieldNaming001",
			"字段命名规则",
			"字段 '{0}' 应该使用PascalCase 大驼峰命名",
			"命名",
			DiagnosticSeverity.Warning,
			isEnabledByDefault: true);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();
			context.RegisterSyntaxNodeAction(AnalyzeFieldDeclaration, SyntaxKind.FieldDeclaration);
		}

		private void AnalyzeFieldDeclaration(SyntaxNodeAnalysisContext context)
		{
			var fieldDeclaration = (FieldDeclarationSyntax)context.Node;

			// 跳过私有字段
			if (fieldDeclaration.Modifiers.Any(SyntaxKind.PrivateKeyword))
			{
				return;
			}

			foreach (var variable in fieldDeclaration.Declaration.Variables)
			{
				var fieldName = variable.Identifier.Text;
				if (!Regex.IsMatch(fieldName, "^[A-Z][a-zA-Z]*$"))
				{
					var diagnostic = Diagnostic.Create(Rule, variable.GetLocation(), fieldName);
					context.ReportDiagnostic(diagnostic);
				}
			}
		}
	}

}