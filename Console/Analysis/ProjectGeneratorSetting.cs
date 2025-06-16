/****************************************

* 作者：闪电黑客
* 日期：2025/6/13 14:42

* 描述：

*/
using System;
using System.Collections.Generic;

namespace WorldTree.SourceGenerator
{
	/// <summary>
	/// 生成器设置
	/// </summary>
	public static class ProjectConfigHelper
	{
		public static HashSet<Type> CoreConfigs = new() {
			typeof(BranchSupplementGeneratorRun) ,
			typeof(CopyNodeClassGeneratorRun),
			typeof(NodeExtensionMethodGeneratorRun),
			typeof(NodeBranchHelperGeneratorRun),
			typeof(RuleClassGeneratorRun),

			typeof(RuleSupplementGeneratorRun),
			typeof(RuleMethodGeneratorRun),
			typeof(RuleExtensionMethodGeneratorRun),
			typeof(TreeCopyGeneratorRun),
			typeof(TreeDataSerializeGeneratorRun),
			typeof(TreePackSerializeGeneratorRun),
		};
		public static HashSet<Type> NodeConfigs = new()
		{
			typeof(RuleSupplementGeneratorRun),
			typeof(TreeCopyGeneratorRun),
			typeof(TreeDataSerializeGeneratorRun),
			typeof(TreePackSerializeGeneratorRun),
		};
		public static HashSet<Type> RuleConfigs = new()
		{
			typeof(RuleMethodGeneratorRun),
		};
	}

	public class ProjectGeneratorSetting : ProjectGeneratorsConfig
	{
		public ProjectGeneratorSetting()
		{
			argumentCount = 5;
			Add("Core", ProjectConfigHelper.CoreConfigs);
			Add("Node", ProjectConfigHelper.NodeConfigs);
			Add("Rule", ProjectConfigHelper.RuleConfigs);
			Add("Config", ProjectConfigHelper.CoreConfigs);
		}
	}
}
