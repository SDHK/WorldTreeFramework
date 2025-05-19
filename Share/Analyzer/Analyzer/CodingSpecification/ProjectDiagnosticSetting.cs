/****************************************

* 作者：闪电黑客
* 日期：2024/6/18 16:42

* 描述：

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;

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
		public static Dictionary<string, List<DiagnosticConfigGroup>> ProjectDiagnostics = new()
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


		/// <summary>
		/// 尝试获取诊断配置组
		/// </summary>
		public static bool TryGetDiagnosticConfigGroup(string AssemblyName, out List<DiagnosticConfigGroup> DiagnosticGroups)
		{
			DiagnosticGroups = null;
			//if (!options.AnalyzerConfigOptionsProvider.GlobalOptions.TryGetValue("build_property.AssemblyName", out var AnalysisName)) return false;
			//if (string.IsNullOrEmpty(AnalysisName)) return false;
			if (!ProjectDiagnostics.TryGetValue(AssemblyName, out DiagnosticGroups)) return false;
			return true;
		}


		/// <summary>
		/// 获取诊断描述
		/// </summary>
		public static DiagnosticDescriptor[] GetDiagnosticDescriptors(SyntaxKind declarationKind)
		{
			List<DiagnosticDescriptor> descriptors = new();
			HashSet<Type> types = new();
			foreach (List<DiagnosticConfigGroup> objectDiagnosticList in ProjectDiagnostics.Values)
			{
				foreach (DiagnosticConfigGroup diagnosticConfig in objectDiagnosticList)
				{
					if (types.Contains(diagnosticConfig.GetType())) continue;
					types.Add(diagnosticConfig.GetType());

					foreach (DiagnosticConfig codeDiagnosticConfig in diagnosticConfig.Diagnostics.Values)
					{
						if (codeDiagnosticConfig.DeclarationKind == declarationKind)
						{
							descriptors.Add(codeDiagnosticConfig.Diagnostic);
						}
					}
				}
			}
			return descriptors.ToArray();
		}

		/// <summary>
		/// 获取诊断描述Id
		/// </summary>
		public static string[] GetDiagnosticDescriptorsId(SyntaxKind declarationKind)
		{
			List<string> descriptors = new();
			HashSet<Type> types = new();
			foreach (List<DiagnosticConfigGroup> objectDiagnosticList in ProjectDiagnostics.Values)
			{
				foreach (DiagnosticConfigGroup diagnosticConfig in objectDiagnosticList)
				{
					if (types.Contains(diagnosticConfig.GetType())) continue;
					types.Add(diagnosticConfig.GetType());

					foreach (DiagnosticConfig codeDiagnosticConfig in diagnosticConfig.Diagnostics.Values)
					{
						if (codeDiagnosticConfig.DeclarationKind == declarationKind)
						{
							descriptors.Add(codeDiagnosticConfig.Diagnostic.Id);
						}
					}
				}
			}
			return descriptors.ToArray();
		}

		/// <summary>
		/// 尝试查找诊断配置
		/// </summary>
		public static bool TryFindDiagnosticDescriptor(string id, out DiagnosticConfig codeDiagnostic)
		{
			foreach (List<DiagnosticConfigGroup> objectDiagnosticList in ProjectDiagnostics.Values)
			{
				foreach (DiagnosticConfigGroup diagnosticConfig in objectDiagnosticList)
				{
					foreach (DiagnosticConfig codeDiagnosticConfig in diagnosticConfig.Diagnostics.Values)
					{
						if (codeDiagnosticConfig.Diagnostic.Id == id)
						{
							codeDiagnostic = codeDiagnosticConfig;
							return true;
						}
					}
				}
			}
			codeDiagnostic = default;
			return false;
		}
	}
}