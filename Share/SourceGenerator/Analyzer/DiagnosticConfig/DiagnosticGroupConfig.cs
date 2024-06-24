/****************************************

* 作者：闪电黑客
* 日期：2024/6/24 19:56

* 描述：

*/
using Microsoft.CodeAnalysis;

namespace WorldTree.Analyzer
{
	/// <summary>
	/// 诊断集合配置
	/// </summary>
	public abstract class DiagnosticGroupConfig
	{
		/// <summary>
		/// 筛选
		/// </summary>
		public Func<ISymbol, bool> Screen;

		/// <summary>
		/// 命名规则
		/// </summary>
		public SortedDictionary<DiagnosticKey, DiagnosticConfig> CodeDiagnostics = new();

		/// <summary>
		/// 设置命名规则
		/// </summary>
		public void SetNamingRule(DiagnosticKey diagnosticKey, DiagnosticConfig namingRule)
		{
			namingRule.Init(GetType().Name.Replace("Diagnostic", ""));
			if (!CodeDiagnostics.ContainsKey(diagnosticKey))
			{
				CodeDiagnostics.Add(diagnosticKey, namingRule);
			}
			else
			{
				CodeDiagnostics[diagnosticKey] = namingRule;
			}
		}

		/// <summary>
		/// 初始化诊断配置集合
		/// </summary>
		public abstract DiagnosticGroupConfig Init();

	}
}