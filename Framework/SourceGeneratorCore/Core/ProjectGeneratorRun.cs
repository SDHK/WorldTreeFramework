using Microsoft.CodeAnalysis;

namespace WorldTree.SourceGenerator
{
	[Generator]
	public class BranchSupplementGeneratorRun : BranchSupplementGenerator<ProjectGeneratorSetting> { }
	[Generator]
	public class CopyNodeClassGeneratorRun : CopyNodeClassGenerator<ProjectGeneratorSetting> { }
	[Generator]
	public class NodeLinkRemarksGeneratorRun : NodeLinkRemarksGenerator<ProjectGeneratorSetting> { }
	[Generator]
	public class NodeExtensionMethodGeneratorRun : NodeExtensionMethodGenerator<ProjectGeneratorSetting> { }
	[Generator]
	public class NodeBranchHelperGeneratorRun : NodeBranchHelperGenerator<ProjectGeneratorSetting> { }
	[Generator]
	public class RuleClassGeneratorRun : RuleClassGenerator<ProjectGeneratorSetting> { }
	[Generator]
	public class RuleSupplementGeneratorRun : RuleSupplementGenerator<ProjectGeneratorSetting> { }
	[Generator]
	public class RuleMethodGeneratorRun : RuleMethodGenerator<ProjectGeneratorSetting> { }
	[Generator]
	public class RuleExtensionMethodGeneratorRun : RuleExtensionMethodGenerator<ProjectGeneratorSetting> { }
	[Generator]
	public class TreeCopyGeneratorRun : TreeCopyGenerator<ProjectGeneratorSetting> { }
	[Generator]
	public class TreeDataSerializeGeneratorRun : TreeDataSerializeGenerator<ProjectGeneratorSetting> { }
	[Generator]
	public class TreePackSerializeGeneratorRun : TreePackSerializeGenerator<ProjectGeneratorSetting> { }
}
