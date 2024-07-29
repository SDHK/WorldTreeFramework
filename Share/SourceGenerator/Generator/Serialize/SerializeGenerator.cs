/****************************************

* 作者：闪电黑客
* 日期：2024/7/29 11:51

* 描述：

*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WorldTree.SourceGenerator
{
	[Generator]
	internal class SerializeGenerator : ISourceGenerator
	{
		public void Initialize(GeneratorInitializationContext context)
		{
			context.RegisterForSyntaxNotifications(() => new FindTreePackSyntaxReceiver());
		}

		public void Execute(GeneratorExecutionContext context)
		{
			if (!(context.SyntaxReceiver is FindTreePackSyntaxReceiver receiver and not null)) return;
			
		}
	}



	/// <summary>
	/// 查找序列化标记类型
	/// </summary>
	public class FindTreePackSyntaxReceiver : ISyntaxReceiver
	{
		public List<ClassDeclarationSyntax> ClassDeclarations = new();
		public List<StructDeclarationSyntax> StructDeclarations = new();
		public List<InterfaceDeclarationSyntax> interfaceDeclarations = new();


		public void OnVisitSyntaxNode(SyntaxNode node)
		{
			// 判断是否是类或结构体
			if (node is ClassDeclarationSyntax classDeclarationSyntax)
			{
				if (TreeSyntaxHelper.CheckAttribute(classDeclarationSyntax, GeneratorHelper.TreePackAttribute))
					ClassDeclarations.Add(classDeclarationSyntax);
			}
			else if (node is StructDeclarationSyntax structDeclarationSyntax)
			{
				if (TreeSyntaxHelper.CheckAttribute(structDeclarationSyntax, GeneratorHelper.TreePackAttribute))
					StructDeclarations.Add(structDeclarationSyntax);
			}
			else if (node is InterfaceDeclarationSyntax interfaceDeclarationSyntax)
			{
				if (TreeSyntaxHelper.CheckAttribute(interfaceDeclarationSyntax, GeneratorHelper.TreePackAttribute))
					interfaceDeclarations.Add(interfaceDeclarationSyntax);
			}
		}
	}
}
