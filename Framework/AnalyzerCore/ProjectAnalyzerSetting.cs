using System.Collections.Generic;

namespace WorldTree.Analyzer
{
	/// <summary>
	/// 核心分析器设置
	/// </summary>
	public static class ProjectConfigHelper
	{
		public static List<DiagnosticConfigGroup> CoreConfigs = new()
		{
			new BranchDiagnosticConfig(),
			new ListDiagnosticConfig(),
			new ArrayDiagnosticConfig(),
			new DictionaryDiagnosticConfig(),
			new HashSetDiagnosticConfig(),
			new QueueDiagnosticConfig(),
			new StackDiagnosticConfig(),

			new CodeNodeDiagnosticConfig(),
			new RuleDiagnosticConfig(),
			new StaticRuleDiagnosticConfig(),
			new ObjectDiagnosticConfig(),
			new TreeTaskDiagnosticConfig()
		};

		public static List<DiagnosticConfigGroup> ModuleConfigs = new()
		{
			new BranchDiagnosticConfig(),
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
			new ObjectDiagnosticConfig(),
			new TreeTaskDiagnosticConfig()
		};
	}

	public class ProjectAnalyzerSetting : ProjectDiagnosticConfig
	{
		public ProjectAnalyzerSetting()
		{
			Add("WorldTree.Core", ProjectConfigHelper.CoreConfigs);
			Add("WorldTree.Module", ProjectConfigHelper.ModuleConfigs);
		}
	}
}
