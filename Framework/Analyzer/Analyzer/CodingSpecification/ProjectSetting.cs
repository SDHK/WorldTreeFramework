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

		private static List<DiagnosticConfigGroup> NodeConfigs = new()
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

		private static List<DiagnosticConfigGroup> RuleConfigs = new()
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

		/// <summary>
		/// 项目诊断配置
		/// </summary>
		public static ProjectDiagnostics Configs = new()
		{
			//后端配置
			{ "App",CoreConfigs},
			{ "Core",CoreConfigs},
			{ "Node",NodeConfigs},
			{ "Rule",RuleConfigs},
			{ "Config",RuleConfigs},

			//前端限制
			{ "WorldTree.Core",CoreConfigs},
			{ "WorldTree.UnityCore",CoreConfigs},

			{ "WorldTree.Node",NodeConfigs},
			{ "WorldTree.Rule",RuleConfigs},
			{ "WorldTree.UnityNode",NodeConfigs},
			{ "WorldTree.UnityRule",RuleConfigs},
		};
	}

	#region 基本诊断

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class ClassNamingDiagnosticRun : ClassNamingDiagnostic { public override ProjectDiagnostics Configs => ProjectSetting.Configs; }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ClassNamingProviderRun)), Shared]
	public class ClassNamingProviderRun : ClassNamingProvider { public override ProjectDiagnostics Configs => ProjectSetting.Configs; }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class DelegateNamingDiagnosticRun : DelegateNamingDiagnostic { public override ProjectDiagnostics Configs => ProjectSetting.Configs; }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DelegateNamingProviderRun)), Shared]
	public class DelegateNamingProviderRun : DelegateNamingProvider { public override ProjectDiagnostics Configs => ProjectSetting.Configs; }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class EnumMemberNamingDiagnosticRun : EnumMemberNamingDiagnostic { public override ProjectDiagnostics Configs => ProjectSetting.Configs; }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EnumMemberNamingProviderRun)), Shared]
	public class EnumMemberNamingProviderRun : EnumMemberNamingProvider { public override ProjectDiagnostics Configs => ProjectSetting.Configs; }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class EnumNamingDiagnosticRun : EnumNamingDiagnostic { public override ProjectDiagnostics Configs => ProjectSetting.Configs; }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EnumNamingProviderRun)), Shared]
	public class EnumNamingProviderRun : EnumNamingProvider { public override ProjectDiagnostics Configs => ProjectSetting.Configs; }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class FieldNamingDiagnosticRun : FieldNamingDiagnostic { public override ProjectDiagnostics Configs => ProjectSetting.Configs; }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FieldNamingProviderRun)), Shared]
	public class FieldNamingProviderRun : FieldNamingProvider { public override ProjectDiagnostics Configs => ProjectSetting.Configs; }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class InterfaceNamingDiagnosticRun : InterfaceNamingDiagnostic { public override ProjectDiagnostics Configs => ProjectSetting.Configs; }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(InterfaceNamingProviderRun)), Shared]
	public class InterfaceNamingProviderRun : InterfaceNamingProvider { public override ProjectDiagnostics Configs => ProjectSetting.Configs; }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class LocalMethodNamingDiagnosticRun : LocalMethodNamingDiagnostic { public override ProjectDiagnostics Configs => ProjectSetting.Configs; }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(LocalMethodNamingProviderRun)), Shared]
	public class LocalMethodNamingProviderRun : LocalMethodNamingProvider { public override ProjectDiagnostics Configs => ProjectSetting.Configs; }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class LocalVariableNamingDiagnosticRun : LocalVariableNamingDiagnostic { public override ProjectDiagnostics Configs => ProjectSetting.Configs; }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(LocalVariableNamingProviderRun)), Shared]
	public class LocalVariableNamingProviderRun : LocalVariableNamingProvider { public override ProjectDiagnostics Configs => ProjectSetting.Configs; }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class MethodNamingDiagnosticRun : MethodNamingDiagnostic { public override ProjectDiagnostics Configs => ProjectSetting.Configs; }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MethodNamingProviderRun)), Shared]
	public class MethodNamingProviderRun : MethodNamingProvider { public override ProjectDiagnostics Configs => ProjectSetting.Configs; }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class ParameterNamingDiagnosticRun : ParameterNamingDiagnostic { public override ProjectDiagnostics Configs => ProjectSetting.Configs; }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ParameterNamingProviderRun)), Shared]
	public class ParameterNamingProviderRun : ParameterNamingProvider { public override ProjectDiagnostics Configs => ProjectSetting.Configs; }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class PropertyNamingDiagnosticRun : PropertyNamingDiagnostic { public override ProjectDiagnostics Configs => ProjectSetting.Configs; }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PropertyNamingProviderRun)), Shared]
	public class PropertyNamingProviderRun : PropertyNamingProvider { public override ProjectDiagnostics Configs => ProjectSetting.Configs; }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class StructNamingDiagnosticRun : StructNamingDiagnostic { public override ProjectDiagnostics Configs => ProjectSetting.Configs; }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StructNamingProviderRun)), Shared]
	public class StructNamingProviderRun : StructNamingProvider { public override ProjectDiagnostics Configs => ProjectSetting.Configs; }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class TypeParameterNamingDiagnosticRun : TypeParameterNamingDiagnostic { public override ProjectDiagnostics Configs => ProjectSetting.Configs; }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(TypeParameterNamingProviderRun)), Shared]
	public class TypeParameterNamingProviderRun : TypeParameterNamingProvider { public override ProjectDiagnostics Configs => ProjectSetting.Configs; }

	#endregion

	#region 框架诊断

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class NodeRuleAttributeDiagnosticRun : NodeRuleAttributeDiagnostic { public override ProjectDiagnostics Configs => ProjectSetting.Configs; }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NodeRuleAttributeCodeFixProviderRun)), Shared]
	public class NodeRuleAttributeCodeFixProviderRun : NodeRuleAttributeCodeFixProvider { public override ProjectDiagnostics Configs => ProjectSetting.Configs; }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class NodeRuleClassDiagnosticRun : NodeRuleClassDiagnostic { public override ProjectDiagnostics Configs => ProjectSetting.Configs; }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NodeRuleClassCodeFixProviderRun)), Shared]
	public class NodeRuleClassCodeFixProviderRun : NodeRuleClassCodeFixProvider { public override ProjectDiagnostics Configs => ProjectSetting.Configs; }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class NodeRuleMethodDiagnosticRun : NodeRuleMethodDiagnostic { public override ProjectDiagnostics Configs => ProjectSetting.Configs; }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NodeRuleMethodCodeFixProviderRun)), Shared]
	public class NodeRuleMethodCodeFixProviderRun : NodeRuleMethodCodeFixProvider { public override ProjectDiagnostics Configs => ProjectSetting.Configs; }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class NodeRuleSwitchAttributeDiagnosticRun : NodeRuleSwitchAttributeDiagnostic { public override ProjectDiagnostics Configs => ProjectSetting.Configs; }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NodeRuleSwitchAttributeCodeFixProviderRun)), Shared]
	public class NodeRuleSwitchAttributeCodeFixProviderRun : NodeRuleSwitchAttributeCodeFixProvider { public override ProjectDiagnostics Configs => ProjectSetting.Configs; }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class SimpleMemberAccessDiagnosticRun : SimpleMemberAccessDiagnostic { public override ProjectDiagnostics Configs => ProjectSetting.Configs; }

	#endregion
}