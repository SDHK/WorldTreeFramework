/****************************************

* 作者：闪电黑客
* 日期：2024/5/6 18:08

* 描述：查找子类

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WorldTree.SourceGenerator
{
	/// <summary>
	/// 查找子类
	/// </summary>
	public class FindSubClassSyntaxReceiver : ISyntaxReceiver
	{
		public List<ClassDeclarationSyntax> ClassDeclarations = new();

		public void OnVisitSyntaxNode(SyntaxNode node)
		{
			//判断是否是类
			if (!(node is ClassDeclarationSyntax classDeclarationSyntax)) return;

			//判断类型是否有继承
			if (classDeclarationSyntax.BaseList is null) return;
			if (classDeclarationSyntax.BaseList.Types.Count == 0) return;

			ClassDeclarations.Add(classDeclarationSyntax);
		}
	}
}