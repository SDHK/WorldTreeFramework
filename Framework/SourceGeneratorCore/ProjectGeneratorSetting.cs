using System;
using System.Collections.Generic;

namespace WorldTree.SourceGenerator
{
	/// <summary>
	/// 核心生成器设置
	/// </summary>
	public static class ProjectConfigHelper
	{
		public static HashSet<Type> CoreConfigs = new()
		{
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
		public static HashSet<Type> ModuleConfigs = new()
		{
			typeof(BranchSupplementGeneratorRun),
			typeof(INodeProxyGeneratorRun),
			typeof(RuleSupplementGeneratorRun),
			typeof(RuleMethodGeneratorRun),

			typeof(TreeCopyGeneratorRun),
			typeof(TreeDataSerializeGeneratorRun),
			typeof(TreePackSerializeGeneratorRun),
		};

	}

	public class ProjectGeneratorSetting : ProjectGeneratorsConfig
	{
		public ProjectGeneratorSetting()
		{
			ArgumentCount = 5;
			Add("WorldTree.Core", ProjectConfigHelper.CoreConfigs);
			Add("WorldTree.Module", ProjectConfigHelper.ModuleConfigs);
		}
	}
}
