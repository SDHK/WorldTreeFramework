/****************************************

* 作者：闪电黑客
* 日期：2024/4/16 19:57

* 描述：查找继承了INode的类

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
			//SyntaxNode node = context.Node;

			//判断是否是类
			if (!(node is ClassDeclarationSyntax classDeclarationSyntax)) return;

			//判断类型是否有继承
			if (classDeclarationSyntax.BaseList is null) return;
			if (classDeclarationSyntax.BaseList.Types.Count == 0) return;

			// 跳过Node类
			if (classDeclarationSyntax.Identifier.Text == "Node") return;

			//判断是否继承了INode,如果没有继承则不生成

			if (!TreeSyntaxHelper.CheckExtendInterface(classDeclarationSyntax, "INode")) return;

			//判断父类是否继承了INode,如果继承了则不生成

			if (TreeSyntaxHelper.CheckBaseExtendInterface(classDeclarationSyntax, "INode", false)) return;

			ClassDeclarations.Add(classDeclarationSyntax);
		}
	}

	/// <summary>
	/// 查找继承了IRule的类
	/// </summary>
	public class FindIRuleSubSyntaxReceiver : ISyntaxContextReceiver
	{
		public List<ClassDeclarationSyntax> ClassDeclarations = new();

		public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
		{
			SyntaxNode node = context.Node;

			//判断是否是接口
			if (!(node is InterfaceDeclarationSyntax interfaceDeclarationSyntax)) return;

			//判断类型是否继承了IRule
			if (!TreeSyntaxHelper.CheckExtendInterface(interfaceDeclarationSyntax, "IRule")) return;
		}

		//public void OnVisitSyntaxNode(SyntaxNode node)
		//{
		//	//判断是否是接口
		//	if (!(node is InterfaceDeclarationSyntax interfaceDeclarationSyntax)) return;

		//	//判断类型是否继承了IRule
		//	if (!AnalyzerHelper.CheckExtendInterface(interfaceDeclarationSyntax, "IRule")) return;
		//}
	}

	/// <summary>
	/// 查找子类
	/// </summary>
	public class FindSubClassSyntaxReceiver : ISyntaxContextReceiver
	{
		public List<ClassDeclarationSyntax> ClassDeclarations = new();

		public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
		{
			SyntaxNode node = context.Node;

			//判断是否是类
			if (!(node is ClassDeclarationSyntax classDeclarationSyntax)) return;

			//判断类型是否有继承
			if (classDeclarationSyntax.BaseList is null) return;
			if (classDeclarationSyntax.BaseList.Types.Count == 0) return;

			ClassDeclarations.Add(classDeclarationSyntax);
		}
	}
}