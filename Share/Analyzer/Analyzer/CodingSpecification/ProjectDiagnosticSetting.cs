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
			new StaticRuleDiagnosticConfig(),
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

			new RuleDiagnosticConfig(),
			new ProjectBanModelDiagnosticConfig(),
			new StaticRuleDiagnosticConfig(),
			new ObjectDiagnosticConfig()
		};

		/// <summary>
		/// 项目诊断配置
		/// </summary>
		public static Dictionary<string, List<DiagnosticConfigGroup>> ProjectDiagnostics = new()
		{
			//后端配置
			{ "App",CoreConfigs},
			{ "DotNet.Core",CoreConfigs},

			//前端限制
			{ "WorldTree.Core",CoreConfigs},
			{ "WorldTree.CoreUnity",CoreConfigs},

			{ "WorldTree.Node",NodeConfigs},
			{ "WorldTree.NodeUnity",NodeConfigs},
			{ "WorldTree.Rule",RuleConfigs},
			{ "WorldTree.RuleUnity",RuleConfigs},
		};


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