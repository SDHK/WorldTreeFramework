/****************************************

* 作者：闪电黑客
* 日期：2024/4/15 14:26

* 描述：查找核心程序集

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WorldTree.SourceGenerator
{
	/// <summary>
	/// 查找核心程序集
	/// </summary>
	public class FindCoreSyntaxReceiver : ISyntaxReceiver
	{
		public bool isGenerator = false;

		public void OnVisitSyntaxNode(SyntaxNode node)
		{
			//判断是否是类
			if (node is not InterfaceDeclarationSyntax interfaceDeclarationSyntax) return;

			//判断类型是否是INode,因为INode是基类在核心程序集中
			if (interfaceDeclarationSyntax.Identifier.ValueText != "INode") return;

			isGenerator = true;
		}
	}
}