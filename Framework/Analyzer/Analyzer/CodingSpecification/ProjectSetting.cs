/****************************************

* 作者：闪电黑客
* 日期：2024/6/18 16:42

* 描述：

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Composition;

namespace WorldTree.Analyzer
{
	/// <summary>
	/// 分析器设置
	/// </summary>
	public static class ProjectSetting
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

	public class ProjectConfig : ProjectDiagnosticsConfig
	{
		public ProjectConfig()
		{
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


	#region 基本诊断

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class ClassNamingDiagnosticRun : ClassNamingDiagnostic<ProjectConfig> { }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ClassNamingProviderRun)), Shared]
	public class ClassNamingProviderRun : ClassNamingProvider<ProjectConfig> { }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class DelegateNamingDiagnosticRun : DelegateNamingDiagnostic<ProjectConfig> { }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DelegateNamingProviderRun)), Shared]
	public class DelegateNamingProviderRun : DelegateNamingProvider<ProjectConfig> { }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class EnumMemberNamingDiagnosticRun : EnumMemberNamingDiagnostic<ProjectConfig> { }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EnumMemberNamingProviderRun)), Shared]
	public class EnumMemberNamingProviderRun : EnumMemberNamingProvider<ProjectConfig> { }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class EnumNamingDiagnosticRun : EnumNamingDiagnostic<ProjectConfig> { }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EnumNamingProviderRun)), Shared]
	public class EnumNamingProviderRun : EnumNamingProvider<ProjectConfig> { }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class FieldNamingDiagnosticRun : FieldNamingDiagnostic<ProjectConfig> { }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FieldNamingProviderRun)), Shared]
	public class FieldNamingProviderRun : FieldNamingProvider<ProjectConfig> { }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class InterfaceNamingDiagnosticRun : InterfaceNamingDiagnostic<ProjectConfig> { }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(InterfaceNamingProviderRun)), Shared]
	public class InterfaceNamingProviderRun : InterfaceNamingProvider<ProjectConfig> { }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class LocalMethodNamingDiagnosticRun : LocalMethodNamingDiagnostic<ProjectConfig> { }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(LocalMethodNamingProviderRun)), Shared]
	public class LocalMethodNamingProviderRun : LocalMethodNamingProvider<ProjectConfig> { }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class LocalVariableNamingDiagnosticRun : LocalVariableNamingDiagnostic<ProjectConfig> { }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(LocalVariableNamingProviderRun)), Shared]
	public class LocalVariableNamingProviderRun : LocalVariableNamingProvider<ProjectConfig> { }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class MethodNamingDiagnosticRun : MethodNamingDiagnostic<ProjectConfig> { }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MethodNamingProviderRun)), Shared]
	public class MethodNamingProviderRun : MethodNamingProvider<ProjectConfig> { }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class ParameterNamingDiagnosticRun : ParameterNamingDiagnostic<ProjectConfig> { }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ParameterNamingProviderRun)), Shared]
	public class ParameterNamingProviderRun : ParameterNamingProvider<ProjectConfig> { }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class PropertyNamingDiagnosticRun : PropertyNamingDiagnostic<ProjectConfig> { }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PropertyNamingProviderRun)), Shared]
	public class PropertyNamingProviderRun : PropertyNamingProvider<ProjectConfig> { }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class StructNamingDiagnosticRun : StructNamingDiagnostic<ProjectConfig> { }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StructNamingProviderRun)), Shared]
	public class StructNamingProviderRun : StructNamingProvider<ProjectConfig> { }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class TypeParameterNamingDiagnosticRun : TypeParameterNamingDiagnostic<ProjectConfig> { }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(TypeParameterNamingProviderRun)), Shared]
	public class TypeParameterNamingProviderRun : TypeParameterNamingProvider<ProjectConfig> { }

	#endregion

	#region 框架诊断

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class NodeRuleAttributeDiagnosticRun : NodeRuleAttributeDiagnostic<ProjectConfig> { }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NodeRuleAttributeCodeFixProviderRun)), Shared]
	public class NodeRuleAttributeCodeFixProviderRun : NodeRuleAttributeCodeFixProvider<ProjectConfig> { }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class NodeRuleClassDiagnosticRun : NodeRuleClassDiagnostic<ProjectConfig> { }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NodeRuleClassCodeFixProviderRun)), Shared]
	public class NodeRuleClassCodeFixProviderRun : NodeRuleClassCodeFixProvider<ProjectConfig> { }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class NodeRuleMethodDiagnosticRun : NodeRuleMethodDiagnostic<ProjectConfig> { }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NodeRuleMethodCodeFixProviderRun)), Shared]
	public class NodeRuleMethodCodeFixProviderRun : NodeRuleMethodCodeFixProvider<ProjectConfig> { }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class NodeRuleSwitchAttributeDiagnosticRun : NodeRuleSwitchAttributeDiagnostic<ProjectConfig> { }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NodeRuleSwitchAttributeCodeFixProviderRun)), Shared]
	public class NodeRuleSwitchAttributeCodeFixProviderRun : NodeRuleSwitchAttributeCodeFixProvider<ProjectConfig> { }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class SimpleMemberAccessDiagnosticRun : SimpleMemberAccessDiagnostic<ProjectConfig> { }

	#endregion
}