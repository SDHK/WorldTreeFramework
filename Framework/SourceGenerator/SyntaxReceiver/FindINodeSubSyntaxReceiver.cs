/****************************************

* 作者：闪电黑客
* 日期：2024/4/16 19:57

* 描述：查找继承了INode的类

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace WorldTree.SourceGenerator
{
	/// <summary>
	/// 查找直接继承了INode的类
	/// </summary>
	public class FindINodeSubSyntaxReceiver : ISyntaxReceiver
	{
		public List<ClassDeclarationSyntax> ClassDeclarations = new();

		public void OnVisitSyntaxNode(SyntaxNode node)
		{
			//判断是否是类
			if (!(node is ClassDeclarationSyntax classDeclarationSyntax)) return;

			//判断类型是否有继承
			if (classDeclarationSyntax.BaseList is null) return;
			if (classDeclarationSyntax.BaseList.Types.Count == 0) return;

			// 跳过抽象类
			if (classDeclarationSyntax.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.AbstractKeyword))) return;

			// 仅包含部分类
			if (!classDeclarationSyntax.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PartialKeyword))) return;


			//判断是否继承了INode,如果没有继承则不生成
			if (!TreeSyntaxHelper.CheckExtendDirectlyInterface(classDeclarationSyntax, "INode")) return;

			ClassDeclarations.Add(classDeclarationSyntax);
		}
	}
}