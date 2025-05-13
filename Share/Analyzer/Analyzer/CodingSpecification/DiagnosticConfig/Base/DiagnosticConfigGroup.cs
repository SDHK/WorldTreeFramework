/****************************************

* 作者：闪电黑客
* 日期：2024/6/24 19:56

* 描述：

*/
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;


namespace WorldTree.Analyzer
{
	/// <summary>
	/// 诊断配置集合
	/// </summary>
	public abstract class DiagnosticConfigGroup
	{
		/// <summary>
		/// 筛选
		/// </summary>
		public Func<Compilation, ISymbol, bool> Screen = null;

		/// <summary>
		/// 诊断配置集合
		/// </summary>
		public SortedDictionary<DiagnosticKey, DiagnosticConfig> Diagnostics = new();

		/// <summary>
		/// 设置诊断配置
		/// </summary>
		public void SetConfig(DiagnosticKey diagnosticKey, DiagnosticConfig namingRule)
		{
			namingRule.Init(GetType().Name.Replace("DiagnosticConfig", ""));
			namingRule.DiagnosticKey = diagnosticKey;
			if (!Diagnostics.ContainsKey(diagnosticKey))
			{
				Diagnostics.Add(diagnosticKey, namingRule);
			}
			else
			{
				Diagnostics[diagnosticKey] = namingRule;
			}
		}
	}
}