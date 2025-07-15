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
			typeof(INodeProxyGeneratorRun),
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
		public static HashSet<Type> UnityCoreConfigs = new() {
			typeof(INodeProxyGeneratorRun),
			typeof(RuleSupplementGeneratorRun),
			typeof(RuleMethodGeneratorRun),
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
	/// Unity环境配置
	/// </summary>
	public class ProjectGeneratorSetting : ProjectGeneratorsConfig
	{
		public ProjectGeneratorSetting()
		{
			ArgumentCount = 5;

			Add("WorldTree.Core", ProjectConfigHelper.CoreConfigs);
			Add("WorldTree.ModuleNode", ProjectConfigHelper.NodeConfigs);
			Add("WorldTree.ModuleRule", ProjectConfigHelper.RuleConfigs);

			Add("WorldTree.UnityCore", ProjectConfigHelper.UnityCoreConfigs);


			Add("WorldTree.Node", ProjectConfigHelper.NodeConfigs);
			Add("WorldTree.Rule", ProjectConfigHelper.RuleConfigs);
			Add("WorldTree.UnityNode", ProjectConfigHelper.NodeConfigs);
			Add("WorldTree.UnityRule", ProjectConfigHelper.RuleConfigs);
		}
	}
}
