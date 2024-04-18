﻿/****************************************

* 作者：闪电黑客
* 日期：2024/4/16 19:57

* 描述：查找继承了INode的类

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WorldTree.SourceGenerator
{
	/// <summary>
	/// 查找继承了INode的类
	/// </summary>
	public class FindINodeSubSyntaxReceiver : ISyntaxReceiver
	{
		public List<ClassDeclarationSyntax> ClassDeclarations = new();

		public void OnVisitSyntaxNode(SyntaxNode node)
		{
			//判断是否是类
			if (!(node is ClassDeclarationSyntax classDeclarationSyntax)) return;

			//判断类型是否继承了INode，并且父类没有实现INode
			if (classDeclarationSyntax.BaseList is null) return;
			if (classDeclarationSyntax.BaseList.Types.Count == 0) return;

			// 跳过Node类
			if (classDeclarationSyntax.Identifier.Text == "Node") return;

			//判断是否继承了INode,如果没有继承则不生成
			if (!AnalyzerHelper.CheckExtendInterface(classDeclarationSyntax, "INode")) return;

			//判断父类是否继承了INode,如果继承了则不生成
			if (AnalyzerHelper.CheckBaseExtendInterface(classDeclarationSyntax, "INode", false)) return;

			ClassDeclarations.Add(classDeclarationSyntax);
		}
	}
}