/****************************************

* 作者：闪电黑客
* 日期：2024/5/6 18:07

* 描述：查找子接口

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WorldTree.SourceGenerator
{
	/// <summary>
	/// 查找子接口
	/// </summary>
	public class FindSubInterfaceSyntaxReceiver : ISyntaxReceiver
	{
		public List<InterfaceDeclarationSyntax> InterfaceDeclarations = new();

		public void OnVisitSyntaxNode(SyntaxNode node)
		{
			//判断是否是类
			if (!(node is InterfaceDeclarationSyntax interfaceDeclarationSyntax)) return;

			//判断类型是否有继承
			if (interfaceDeclarationSyntax.BaseList is null) return;
			if (interfaceDeclarationSyntax.BaseList.Types.Count == 0) return;

			InterfaceDeclarations.Add(interfaceDeclarationSyntax);
		}
	}
}