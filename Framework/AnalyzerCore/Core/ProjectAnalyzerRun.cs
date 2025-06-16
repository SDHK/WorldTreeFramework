using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Composition;

namespace WorldTree.Analyzer
{
	#region 基本诊断

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class ClassNamingDiagnosticRun : ClassNamingDiagnostic<ProjectAnalyzerSetting> { }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ClassNamingProviderRun)), Shared]
	public class ClassNamingProviderRun : ClassNamingProvider<ProjectAnalyzerSetting> { }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class DelegateNamingDiagnosticRun : DelegateNamingDiagnostic<ProjectAnalyzerSetting> { }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DelegateNamingProviderRun)), Shared]
	public class DelegateNamingProviderRun : DelegateNamingProvider<ProjectAnalyzerSetting> { }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class EnumMemberNamingDiagnosticRun : EnumMemberNamingDiagnostic<ProjectAnalyzerSetting> { }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EnumMemberNamingProviderRun)), Shared]
	public class EnumMemberNamingProviderRun : EnumMemberNamingProvider<ProjectAnalyzerSetting> { }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class EnumNamingDiagnosticRun : EnumNamingDiagnostic<ProjectAnalyzerSetting> { }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EnumNamingProviderRun)), Shared]
	public class EnumNamingProviderRun : EnumNamingProvider<ProjectAnalyzerSetting> { }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class FieldNamingDiagnosticRun : FieldNamingDiagnostic<ProjectAnalyzerSetting> { }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FieldNamingProviderRun)), Shared]
	public class FieldNamingProviderRun : FieldNamingProvider<ProjectAnalyzerSetting> { }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class InterfaceNamingDiagnosticRun : InterfaceNamingDiagnostic<ProjectAnalyzerSetting> { }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(InterfaceNamingProviderRun)), Shared]
	public class InterfaceNamingProviderRun : InterfaceNamingProvider<ProjectAnalyzerSetting> { }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class LocalMethodNamingDiagnosticRun : LocalMethodNamingDiagnostic<ProjectAnalyzerSetting> { }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(LocalMethodNamingProviderRun)), Shared]
	public class LocalMethodNamingProviderRun : LocalMethodNamingProvider<ProjectAnalyzerSetting> { }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class LocalVariableNamingDiagnosticRun : LocalVariableNamingDiagnostic<ProjectAnalyzerSetting> { }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(LocalVariableNamingProviderRun)), Shared]
	public class LocalVariableNamingProviderRun : LocalVariableNamingProvider<ProjectAnalyzerSetting> { }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class MethodNamingDiagnosticRun : MethodNamingDiagnostic<ProjectAnalyzerSetting> { }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MethodNamingProviderRun)), Shared]
	public class MethodNamingProviderRun : MethodNamingProvider<ProjectAnalyzerSetting> { }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class ParameterNamingDiagnosticRun : ParameterNamingDiagnostic<ProjectAnalyzerSetting> { }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ParameterNamingProviderRun)), Shared]
	public class ParameterNamingProviderRun : ParameterNamingProvider<ProjectAnalyzerSetting> { }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class PropertyNamingDiagnosticRun : PropertyNamingDiagnostic<ProjectAnalyzerSetting> { }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PropertyNamingProviderRun)), Shared]
	public class PropertyNamingProviderRun : PropertyNamingProvider<ProjectAnalyzerSetting> { }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class StructNamingDiagnosticRun : StructNamingDiagnostic<ProjectAnalyzerSetting> { }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StructNamingProviderRun)), Shared]
	public class StructNamingProviderRun : StructNamingProvider<ProjectAnalyzerSetting> { }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class TypeParameterNamingDiagnosticRun : TypeParameterNamingDiagnostic<ProjectAnalyzerSetting> { }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(TypeParameterNamingProviderRun)), Shared]
	public class TypeParameterNamingProviderRun : TypeParameterNamingProvider<ProjectAnalyzerSetting> { }

	#endregion

	#region 框架诊断

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class NodeRuleAttributeDiagnosticRun : NodeRuleAttributeDiagnostic<ProjectAnalyzerSetting> { }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NodeRuleAttributeCodeFixProviderRun)), Shared]
	public class NodeRuleAttributeCodeFixProviderRun : NodeRuleAttributeCodeFixProvider<ProjectAnalyzerSetting> { }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class NodeRuleClassDiagnosticRun : NodeRuleClassDiagnostic<ProjectAnalyzerSetting> { }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NodeRuleClassCodeFixProviderRun)), Shared]
	public class NodeRuleClassCodeFixProviderRun : NodeRuleClassCodeFixProvider<ProjectAnalyzerSetting> { }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class NodeRuleMethodDiagnosticRun : NodeRuleMethodDiagnostic<ProjectAnalyzerSetting> { }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NodeRuleMethodCodeFixProviderRun)), Shared]
	public class NodeRuleMethodCodeFixProviderRun : NodeRuleMethodCodeFixProvider<ProjectAnalyzerSetting> { }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class NodeRuleSwitchAttributeDiagnosticRun : NodeRuleSwitchAttributeDiagnostic<ProjectAnalyzerSetting> { }
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NodeRuleSwitchAttributeCodeFixProviderRun)), Shared]
	public class NodeRuleSwitchAttributeCodeFixProviderRun : NodeRuleSwitchAttributeCodeFixProvider<ProjectAnalyzerSetting> { }

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class SimpleMemberAccessDiagnosticRun : SimpleMemberAccessDiagnostic<ProjectAnalyzerSetting> { }

	#endregion

}
