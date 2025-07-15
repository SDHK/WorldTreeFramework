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

		public static List<DiagnosticConfigGroup> NodeConfigs = new()
		{
			new ListDiagnosticConfig(),
			new ArrayDiagnosticConfig(),
			new DictionaryDiagnosticConfig(),
			new HashSetDiagnosticConfig(),
			new QueueDiagnosticConfig(),
			new StackDiagnosticConfig(),


			new NodeDiagnosticConfig(),
			new CodeNodeDiagnosticConfig(),
			new RuleDiagnosticConfig(),
			new StaticRuleDiagnosticConfig(),
			//new ProjectBanUnitDiagnosticConfig(), //由于Rule也继承了Unit，所以这里不再需要
			new ObjectDiagnosticConfig()
		};

		public static List<DiagnosticConfigGroup> RuleConfigs = new()
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
			//new ProjectBanDiagnosticConfig(),
			new ObjectDiagnosticConfig()
		};
	}

	/// <summary>
	/// Unity环境分析器配置
	/// </summary>
	public class ProjectAnalyzerSetting : ProjectDiagnosticConfig
	{
		public ProjectAnalyzerSetting()
		{
			Add("WorldTree.Core", ProjectConfigHelper.CoreConfigs);
			Add("WorldTree.ModuleNode", ProjectConfigHelper.NodeConfigs);
			Add("WorldTree.ModuleRule", ProjectConfigHelper.RuleConfigs);

			Add("WorldTree.UnityCore", ProjectConfigHelper.CoreConfigs);

			Add("WorldTree.Node", ProjectConfigHelper.NodeConfigs);
			Add("WorldTree.Rule", ProjectConfigHelper.RuleConfigs);
			Add("WorldTree.UnityNode", ProjectConfigHelper.NodeConfigs);
			Add("WorldTree.UnityRule", ProjectConfigHelper.RuleConfigs);
		}
	}
}