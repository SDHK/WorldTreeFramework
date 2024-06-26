/****************************************

* 作者：闪电黑客
* 日期：2024/6/26 14:41

* 描述：

*/
using Microsoft.CodeAnalysis.CSharp;

namespace WorldTree.Analyzer
{
	/// <summary>
	/// Node类型诊断
	/// </summary>
	public class NodeDiagnosticConfig : CodeNodeDiagnosticConfig
	{
		public NodeDiagnosticConfig() : base()
		{
			SetConfig(DiagnosticKey.MethodNaming, new DiagnosticConfig()
			{
				Title = "Node方法禁止",
				MessageFormat = "Node中不可以写方法",
				DeclarationKind = SyntaxKind.MethodDeclaration,
				Check = s => false,
			});
		}
	}


}