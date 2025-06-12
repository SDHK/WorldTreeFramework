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
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WorldTree.Analyzer
{

	/// <summary>
	/// 项目诊断配置集合
	/// </summary>
	public class ProjectDiagnostics : Dictionary<string, List<DiagnosticConfigGroup>> { }


	/// <summary>
	/// 命名规范的诊断基类
	/// </summary>
	public abstract class NamingDiagnosticBase : DiagnosticAnalyzer
	{
		/// <summary>
		/// 命名规范的诊断描述
		/// </summary>
		public abstract SyntaxKind DeclarationKind { get; }

		public virtual ProjectDiagnostics Configs { get; }

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
			=> ImmutableArray.Create(GetDiagnosticDescriptors(DeclarationKind));


		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();
			context.RegisterCompilationStartAction(analysisContext =>
					{
						if (!TryGetDiagnosticConfigGroup(analysisContext.Compilation.AssemblyName, out _)) return;
						analysisContext.RegisterSyntaxNodeAction(DiagnosticAction, DeclarationKind);
					}
				);

		}
		protected abstract void DiagnosticAction(SyntaxNodeAnalysisContext context);


		/// <summary>
		/// 尝试获取诊断配置组
		/// </summary>
		public bool TryGetDiagnosticConfigGroup(string AssemblyName, out List<DiagnosticConfigGroup> DiagnosticGroups)
		{
			if (!Configs.TryGetValue(AssemblyName, out DiagnosticGroups)) return false;
			return true;
		}


		/// <summary>
		/// 获取诊断描述
		/// </summary>
		public virtual DiagnosticDescriptor[] GetDiagnosticDescriptors(SyntaxKind declarationKind)
		{
			List<DiagnosticDescriptor> descriptors = new();
			HashSet<Type> types = new();
			foreach (List<DiagnosticConfigGroup> objectDiagnosticList in Configs.Values)
			{
				foreach (DiagnosticConfigGroup diagnosticConfig in objectDiagnosticList)
				{
					if (types.Contains(diagnosticConfig.GetType())) continue;
					types.Add(diagnosticConfig.GetType());
					foreach (DiagnosticConfig codeDiagnosticConfig in diagnosticConfig.Diagnostics.Values)
					{
						if (codeDiagnosticConfig.DeclarationKind != declarationKind) continue;
						descriptors.Add(codeDiagnosticConfig.Diagnostic);
					}
				}
			}
			return descriptors.ToArray();
		}
	}

	/// <summary>
	/// 命名规范代码修复基类
	/// </summary>
	public abstract class NamingCodeFixProviderBase<T> : CodeFixProvider
		where T : SyntaxNode
	{

		public virtual ProjectDiagnostics Configs => null;

		public abstract SyntaxKind DeclarationKind { get; }
		public override sealed ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(GetFixableDiagnosticIds(DeclarationKind));

		/// <summary>
		/// 获取诊断描述Id
		/// </summary>
		public virtual string[] GetFixableDiagnosticIds(SyntaxKind declarationKind)
		{
			List<string> descriptors = new();
			HashSet<Type> types = new();
			foreach (List<DiagnosticConfigGroup> objectDiagnosticList in Configs.Values)
			{
				foreach (DiagnosticConfigGroup diagnosticConfig in objectDiagnosticList)
				{
					if (types.Contains(diagnosticConfig.GetType())) continue;
					types.Add(diagnosticConfig.GetType());

					foreach (DiagnosticConfig codeDiagnosticConfig in diagnosticConfig.Diagnostics.Values)
					{
						if (codeDiagnosticConfig.DeclarationKind != declarationKind)
							descriptors.Add(codeDiagnosticConfig.Diagnostic.Id);
					}
				}
			}
			return descriptors.ToArray();
		}

		/// <summary>
		/// 尝试查找诊断配置
		/// </summary>
		public bool TryFindDiagnosticDescriptor(string id, out DiagnosticConfig codeDiagnostic)
		{
			foreach (List<DiagnosticConfigGroup> objectDiagnosticList in Configs.Values)
			{
				foreach (DiagnosticConfigGroup diagnosticConfig in objectDiagnosticList)
				{
					foreach (DiagnosticConfig codeDiagnosticConfig in diagnosticConfig.Diagnostics.Values)
					{
						if (codeDiagnosticConfig.Diagnostic.Id != id) continue;
						codeDiagnostic = codeDiagnosticConfig;
						return true;
					}
				}
			}
			codeDiagnostic = default;
			return false;
		}

		/// <summary>
		/// 尝试获取诊断配置组
		/// </summary>
		public bool TryGetDiagnosticConfigGroup(string AssemblyName, out List<DiagnosticConfigGroup> DiagnosticGroups)
		{
			if (!Configs.TryGetValue(AssemblyName, out DiagnosticGroups)) return false;
			return true;
		}


		public override sealed FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

		public virtual bool CheckCodeFix(DiagnosticConfig codeDiagnostic, Document document) => true;

		public override sealed async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var projectName = context.Document.Project.AssemblyName;
			if (!TryGetDiagnosticConfigGroup(projectName, out _)) return;

			if (context.Diagnostics.Length == 0) return;
			Diagnostic diagnostic = context.Diagnostics[0];
			var diagnosticSpan = diagnostic.Location.SourceSpan;

			var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

			// 找到需要修复的委托声明
			T declaration = null;
			IEnumerable<SyntaxNode> SyntaxNodes = root?.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf()?.OfType<T>();
			foreach (SyntaxNode node in SyntaxNodes)
			{
				declaration = node as T;
				break;
			}
			if (declaration == null) return;

			// 根据不同的诊断类型注册不同的代码修复
			if (TryFindDiagnosticDescriptor(diagnostic.Id, out DiagnosticConfig codeDiagnostic))
			{
				if (!CheckCodeFix(codeDiagnostic, context.Document)) return;

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