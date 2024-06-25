/****************************************

* 作者：闪电黑客
* 日期：2024/6/24 15:17

* 描述：

*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WorldTree.Analyzer
{
	/// <summary>
	/// 命名规范的诊断基类
	/// </summary>
	public abstract class NamingDiagnosticBase : DiagnosticAnalyzer
	{
		/// <summary>
		/// 命名规范的诊断描述
		/// </summary>
		public abstract SyntaxKind DeclarationKind { get; }

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
			=> ImmutableArray.Create(ProjectDiagnosticSetting.GetDiagnosticDescriptors(DeclarationKind));

		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();
			//context.RegisterSyntaxNodeAction(DiagnosticAction, DeclarationKind);
			context.RegisterCompilationStartAction(analysisContext =>
					{
						if (!ProjectDiagnosticSetting.ProjectDiagnostics.TryGetValue(analysisContext.Compilation.AssemblyName, out List<DiagnosticConfigGroup> objectDiagnostics)) return;
						analysisContext.RegisterSyntaxNodeAction(DiagnosticAction, DeclarationKind);
					}
				);

		}
		protected abstract void DiagnosticAction(SyntaxNodeAnalysisContext context);
	}

	/// <summary>
	/// 命名规范代码修复基类
	/// </summary>
	public abstract class NamingCodeFixProviderBase<T> : CodeFixProvider
		where T : SyntaxNode
	{
		public abstract SyntaxKind DeclarationKind { get; }
		public override sealed ImmutableArray<string> FixableDiagnosticIds
			=> ImmutableArray.Create(ProjectDiagnosticSetting.GetDiagnosticDescriptorsId(DeclarationKind));

		public override sealed FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

		public override sealed async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
			Diagnostic diagnostic = context.Diagnostics[0];
			var diagnosticSpan = diagnostic.Location.SourceSpan;

			var projectName = context.Document.Project.AssemblyName;
			if (!ProjectDiagnosticSetting.ProjectDiagnostics.TryGetValue(projectName, out _)) return;

			// 找到需要修复的委托声明
			T declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<T>().First();

			// 根据不同的诊断类型注册不同的代码修复
			if (ProjectDiagnosticSetting.TryFindDiagnosticDescriptor(diagnostic.Id, out DiagnosticConfig codeDiagnostic))
			{
				context.RegisterCodeFix(
				CodeAction.Create(title: codeDiagnostic.CodeFixTitle,
				createChangedDocument: c => CodeFix(codeDiagnostic, context.Document, declaration, c),
				equivalenceKey: codeDiagnostic.Diagnostic.Id),
				diagnostic);
			}
		}
		protected abstract Task<Document> CodeFix(DiagnosticConfig codeDiagnostic, Document document, T decl, CancellationToken cancellationToken);
	}


}