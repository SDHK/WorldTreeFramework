/****************************************

* 作者：闪电黑客
* 日期：2025/6/13 14:42

* 描述：

*/
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace WorldTree.SourceGenerator
{
	/// <summary>
	/// 生成器设置
	/// </summary>
	public static class ProjectSetting
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

	public class ProjectConfig : ProjectGeneratorsConfig
	{
		public ProjectConfig()
		{
			argumentCount = 5;
			Add("Core", ProjectSetting.CoreConfigs);
			Add("Node", ProjectSetting.NodeConfigs);
			Add("Rule", ProjectSetting.RuleConfigs);
			Add("Config", ProjectSetting.CoreConfigs);

			Add("WorldTree.Core", ProjectSetting.CoreConfigs);
			Add("WorldTree.UnityCore", ProjectSetting.CoreConfigs);

			Add("WorldTree.Node", ProjectSetting.NodeConfigs);
			Add("WorldTree.Rule", ProjectSetting.RuleConfigs);
			Add("WorldTree.UnityNode", ProjectSetting.NodeConfigs);
			Add("WorldTree.UnityRule", ProjectSetting.RuleConfigs);
		}
	}

	#region 生成器

	[Generator]
	public class BranchSupplementGeneratorRun : BranchSupplementGenerator<ProjectConfig> { }
	[Generator]
	public class CopyNodeClassGeneratorRun : CopyNodeClassGenerator<ProjectConfig> { }
	[Generator]
	public class NodeExtensionMethodGeneratorRun : NodeExtensionMethodGenerator<ProjectConfig> { }
	[Generator]
	public class NodeBranchHelperGeneratorRun : NodeBranchHelperGenerator<ProjectConfig> { }
	[Generator]
	public class RuleClassGeneratorRun : RuleClassGenerator<ProjectConfig> { }
	[Generator]
	public class RuleSupplementGeneratorRun : RuleSupplementGenerator<ProjectConfig> { }
	[Generator]
	public class RuleMethodGeneratorRun : RuleMethodGenerator<ProjectConfig> { }
	[Generator]
	public class RuleExtensionMethodGeneratorRun : RuleExtensionMethodGenerator<ProjectConfig> { }
	[Generator]
	public class TreeCopyGeneratorRun : TreeCopyGenerator<ProjectConfig> { }
	[Generator]
	public class TreeDataSerializeGeneratorRun : TreeDataSerializeGenerator<ProjectConfig> { }
	[Generator]
	public class TreePackSerializeGeneratorRun : TreePackSerializeGenerator<ProjectConfig> { }

	#endregion
}
