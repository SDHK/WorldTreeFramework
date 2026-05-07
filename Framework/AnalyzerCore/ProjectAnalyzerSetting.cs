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
			new ObjectDiagnosticConfig()
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
			new ObjectDiagnosticConfig()
		};
	}

	public class ProjectAnalyzerSetting : ProjectDiagnosticConfig
	{
		public ProjectAnalyzerSetting()
		{
			Authors = new HashSet<string>() { "闪电黑客" };
			Add("Core", ProjectConfigHelper.CoreConfigs);
			Add("Module", ProjectConfigHelper.ModuleConfigs);
		}
	}
}
