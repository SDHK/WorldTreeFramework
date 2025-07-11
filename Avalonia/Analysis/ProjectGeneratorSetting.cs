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
			typeof(INodeProxyGeneratorRun),
			typeof(BranchSupplementGeneratorRun) ,
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
			typeof(INodeProxyGeneratorRun),
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

	/// <summary>
	/// 控制台环境生成器配置
	/// </summary>
	public class ProjectGeneratorSetting : ProjectGeneratorsConfig
	{
		public ProjectGeneratorSetting()
		{
			ArgumentCount = 5;
			Add("Avalonia.Core", ProjectConfigHelper.CoreConfigs);
			Add("Node", ProjectConfigHelper.NodeConfigs);
			Add("Rule", ProjectConfigHelper.RuleConfigs);
			Add("Config", ProjectConfigHelper.CoreConfigs);
		}
	}
}
