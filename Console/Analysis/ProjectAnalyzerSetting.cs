/****************************************

* 作者：闪电黑客
* 日期：2024/6/18 16:42

* 描述：

*/

using System.Collections.Generic;

namespace WorldTree.Analyzer
{
	/// <summary>
	/// 分析器设置
	/// </summary>
	public static class ProjectConfigHelper
	{
		public static List<DiagnosticConfigGroup> CoreConfigs = new()
		{
			new ListDiagnosticConfig(),
			new ArrayDiagnosticConfig(),
			new DictionaryDiagnosticConfig(),
			new HashSetDiagnosticConfig(),
			new QueueDiagnosticConfig(),
			new StackDiagnosticConfig(),

			new CodeNodeDiagnosticConfig(),
			new RuleDiagnosticConfig(),
			new StaticRuleDiagnosticConfig(),
			new ObjectDiagnosticConfig()
		};

		public static List<DiagnosticConfigGroup> HotFixConfigs = new()
		{
			new ListDiagnosticConfig(),
			new ArrayDiagnosticConfig(),
			new DictionaryDiagnosticConfig(),
			new HashSetDiagnosticConfig(),
			new QueueDiagnosticConfig(),
			new StackDiagnosticConfig(),

			new RuleSwitchAttributeDiagnosticConfig(),
			new NodeRuleAttributeDiagnosticConfig(),

			new CodeNodeDiagnosticConfig(),
			new RuleDiagnosticConfig(),
			new StaticRuleDiagnosticConfig(),
			new ObjectDiagnosticConfig()
		};
	}

	/// <summary>
	/// 控制台环境分析器配置
	/// </summary>
	public class ProjectAnalyzerSetting : ProjectDiagnosticConfig
	{
		public ProjectAnalyzerSetting()
		{
			Add("Config", ProjectConfigHelper.CoreConfigs);
			Add("HotFix", ProjectConfigHelper.HotFixConfigs);
		}
	}
}