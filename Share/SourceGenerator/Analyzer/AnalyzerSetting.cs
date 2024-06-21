/****************************************

* 作者：闪电黑客
* 日期：2024/6/18 16:42

* 描述：

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace WorldTree.Analyzer
{
	/// <summary>
	/// 分析器设置
	/// </summary>
	public static partial class AnalyzerSetting
	{
		/// <summary>
		/// 项目诊断配置
		/// </summary>
		public static Dictionary<string, List<ObjectDiagnostic>> ProjectDiagnostics = new()
		{
			{ "App",new(){
					new ListDiagnostic().Init(),
					new ObjectDiagnostic().Init()
				}
			},
			{ "DotNet.Core",new(){
					new ListDiagnostic().Init(),
					new ObjectDiagnostic().Init()
				}
			},
		};


		/// <summary>
		/// 获取诊断描述
		/// </summary>
		public static DiagnosticDescriptor[] GetDiagnosticDescriptors(SyntaxKind declarationKind)
		{
			List<DiagnosticDescriptor> descriptors = new();
			HashSet<Type> types = new();
			foreach (List<ObjectDiagnostic> objectDiagnosticList in ProjectDiagnostics.Values)
			{
				foreach (ObjectDiagnostic objectDiagnostic in objectDiagnosticList)
				{
					if (types.Contains(objectDiagnostic.GetType())) continue;
					types.Add(objectDiagnostic.GetType());

					foreach (CodeDiagnosticConfig codeDiagnosticConfig in objectDiagnostic.CodeDiagnostics.Values)
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
			foreach (List<ObjectDiagnostic> objectDiagnosticList in ProjectDiagnostics.Values)
			{
				foreach (ObjectDiagnostic objectDiagnostic in objectDiagnosticList)
				{
					if (types.Contains(objectDiagnostic.GetType())) continue;
					types.Add(objectDiagnostic.GetType());

					foreach (CodeDiagnosticConfig codeDiagnosticConfig in objectDiagnostic.CodeDiagnostics.Values)
					{
						if (codeDiagnosticConfig.DeclarationKind==declarationKind)
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
		public static bool TryFindDiagnosticDescriptor(string id, out CodeDiagnosticConfig codeDiagnostic)
		{
			foreach (List<ObjectDiagnostic> objectDiagnosticList in ProjectDiagnostics.Values)
			{
				foreach (ObjectDiagnostic objectDiagnostic in objectDiagnosticList)
				{
					foreach (CodeDiagnosticConfig codeDiagnosticConfig in objectDiagnostic.CodeDiagnostics.Values)
					{
						if (codeDiagnosticConfig.Diagnostic.Id == id)
						{
							codeDiagnostic = codeDiagnosticConfig;
							return true;
						}
					}
				}
			}
			codeDiagnostic = null;
			return false;
		}
	}
}