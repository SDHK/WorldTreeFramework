/****************************************

* 作者：闪电黑客
* 日期：2026/10/31

* 描述：async TreeTask 的诊断配置，可在 ProjectAnalyzerSetting 里按程序集启用。

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace WorldTree.Analyzer
{
	/// <summary>
	/// async TreeTask 诊断配置
	/// </summary>
	public class TreeTaskDiagnosticConfig : DiagnosticConfigGroup
	{
		public TreeTaskDiagnosticConfig()
		{
			Screen = (compilation, symbol) => symbol is IMethodSymbol;

			SetConfig(DiagnosticKey.TreeTaskAwaitMissing, new DiagnosticConfig
			{
				Title = "async TreeTask 缺少 await",
				MessageFormat = "async TreeTask 方法 {0} 没有 await；每条执行路径至少要 await 一次",
				Description = "TreeTask 构建器在第一次挂起时创建任务；没有 await 会在 SetResult/SetException 空引用。",
				CodeFixTitle = "插入 await TreeTaskCompleted()",
				DeclarationKind = SyntaxKind.MethodDeclaration,
				Check = TreeTaskDiagnosticHelper.HasNoAwait,
			});
			OverrideDiagnostic(DiagnosticKey.TreeTaskAwaitMissing, "WTREE001");

			SetConfig(DiagnosticKey.TreeTaskEarlyExit, new DiagnosticConfig
			{
				Title = "async TreeTask 在 await 前退出",
				MessageFormat = "async TreeTask 方法 {0} 在第一个 await 之前可能 return/throw；先 await TreeTaskCompleted()",
				Description = "第一个 await 之前 return/throw 会返回 null 任务，调用方 await 会空引用。",
				CodeFixTitle = "插入 await TreeTaskCompleted()",
				DeclarationKind = SyntaxKind.MethodDeclaration,
				Check = TreeTaskDiagnosticHelper.HasEarlyExit,
			});
			OverrideDiagnostic(DiagnosticKey.TreeTaskEarlyExit, "WTREE002");
		}

		/// <summary>
		/// SetConfig 自动生成的 id 会撞名，这里换成固定 id。
		/// </summary>
		private void OverrideDiagnostic(DiagnosticKey key, string id)
		{
			DiagnosticConfig config = Diagnostics[key];
			config.Diagnostic = new DiagnosticDescriptor(
				id,
				config.Title,
				config.MessageFormat,
				"WorldTree",
				DiagnosticSeverity.Error,
				true,
				config.Description);
			Diagnostics[key] = config;
		}
	}
}
