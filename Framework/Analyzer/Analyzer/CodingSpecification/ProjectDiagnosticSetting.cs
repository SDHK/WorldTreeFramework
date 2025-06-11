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
	public static partial class ProjectDiagnosticSetting
	{
		private static List<DiagnosticConfigGroup> CoreConfigs = new()
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

			//编辑限制
			//{ "DotNet.NodeRuleEditor",NodeConfigs},
			//{ "Unity.NodeRuleEditor",NodeConfigs}

		};



		#region 基本

		[DiagnosticAnalyzer(LanguageNames.CSharp)]
		public class ClassNamingDiagnosticConfig : ClassNamingDiagnostic { public override ProjectDiagnostics Configs => Configs; }
		[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ClassNamingProviderConfig)), Shared]
		public class ClassNamingProviderConfig : ClassNamingProvider { public override ProjectDiagnostics Configs => Configs; }

		[DiagnosticAnalyzer(LanguageNames.CSharp)]
		public class NodeRuleClassDiagnosticConfig : NodeRuleClassDiagnostic { public override ProjectDiagnostics Configs => Configs; }
		[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NodeRuleClassCodeFixProviderConfig)), Shared]
		public class NodeRuleClassCodeFixProviderConfig : NodeRuleClassCodeFixProvider { public override ProjectDiagnostics Configs => Configs; }

		[DiagnosticAnalyzer(LanguageNames.CSharp)]
		public class DelegateNamingDiagnosticConfig : DelegateNamingDiagnostic { public override ProjectDiagnostics Configs => Configs; }
		[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DelegateNamingProviderConfig)), Shared]
		public class DelegateNamingProviderConfig : DelegateNamingProvider { public override ProjectDiagnostics Configs => Configs; }

		[DiagnosticAnalyzer(LanguageNames.CSharp)]
		public class EnumMemberNamingDiagnosticConfig : EnumMemberNamingDiagnostic { public override ProjectDiagnostics Configs => Configs; }
		[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EnumMemberNamingProviderConfig)), Shared]
		public class EnumMemberNamingProviderConfig : EnumMemberNamingProvider { public override ProjectDiagnostics Configs => Configs; }

		[DiagnosticAnalyzer(LanguageNames.CSharp)]
		public class EnumNamingDiagnosticConfig : EnumNamingDiagnostic { public override ProjectDiagnostics Configs => Configs; }
		[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EnumNamingProviderConfig)), Shared]
		public class EnumNamingProviderConfig : EnumNamingProvider { public override ProjectDiagnostics Configs => Configs; }

		[DiagnosticAnalyzer(LanguageNames.CSharp)]
		public class FieldNamingDiagnosticConfig : FieldNamingDiagnostic { public override ProjectDiagnostics Configs => Configs; }
		[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FieldNamingProviderConfig)), Shared]
		public class FieldNamingProviderConfig : FieldNamingProvider { public override ProjectDiagnostics Configs => Configs; }

		[DiagnosticAnalyzer(LanguageNames.CSharp)]
		public class InterfaceNamingDiagnosticConfig : InterfaceNamingDiagnostic { public override ProjectDiagnostics Configs => Configs; }
		[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(InterfaceNamingProviderConfig)), Shared]
		public class InterfaceNamingProviderConfig : InterfaceNamingProvider { public override ProjectDiagnostics Configs => Configs; }

		[DiagnosticAnalyzer(LanguageNames.CSharp)]
		public class LocalMethodNamingDiagnosticConfig : LocalMethodNamingDiagnostic { public override ProjectDiagnostics Configs => Configs; }
		[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(LocalMethodNamingProviderConfig)), Shared]
		public class LocalMethodNamingProviderConfig : LocalMethodNamingProvider { public override ProjectDiagnostics Configs => Configs; }

		[DiagnosticAnalyzer(LanguageNames.CSharp)]
		public class LocalVariableNamingDiagnosticConfig : LocalVariableNamingDiagnostic { public override ProjectDiagnostics Configs => Configs; }
		[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(LocalVariableNamingProviderConfig)), Shared]
		public class LocalVariableNamingProviderConfig : LocalVariableNamingProvider { public override ProjectDiagnostics Configs => Configs; }

		[DiagnosticAnalyzer(LanguageNames.CSharp)]
		public class MethodNamingDiagnosticConfig : MethodNamingDiagnostic { public override ProjectDiagnostics Configs => Configs; }
		[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MethodNamingProviderConfig)), Shared]
		public class MethodNamingProviderConfig : MethodNamingProvider { public override ProjectDiagnostics Configs => Configs; }

		[DiagnosticAnalyzer(LanguageNames.CSharp)]
		public class ParameterNamingDiagnosticConfig : ParameterNamingDiagnostic { public override ProjectDiagnostics Configs => Configs; }
		[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ParameterNamingProviderConfig)), Shared]
		public class ParameterNamingProviderConfig : ParameterNamingProvider { public override ProjectDiagnostics Configs => Configs; }

		[DiagnosticAnalyzer(LanguageNames.CSharp)]
		public class PropertyNamingDiagnosticConfig : PropertyNamingDiagnostic { public override ProjectDiagnostics Configs => Configs; }
		[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PropertyNamingProviderConfig)), Shared]
		public class PropertyNamingProviderConfig : PropertyNamingProvider { public override ProjectDiagnostics Configs => Configs; }

		[DiagnosticAnalyzer(LanguageNames.CSharp)]
		public class StructNamingDiagnosticConfig : StructNamingDiagnostic { public override ProjectDiagnostics Configs => Configs; }
		[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StructNamingProviderConfig)), Shared]
		public class StructNamingProviderConfig : StructNamingProvider { public override ProjectDiagnostics Configs => Configs; }

		[DiagnosticAnalyzer(LanguageNames.CSharp)]
		public class TypeParameterNamingDiagnosticConfig : TypeParameterNamingDiagnostic { public override ProjectDiagnostics Configs => Configs; }
		[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(TypeParameterNamingProviderConfig)), Shared]
		public class TypeParameterNamingProviderConfig : TypeParameterNamingProvider { public override ProjectDiagnostics Configs => Configs; }

		#endregion


	}
}