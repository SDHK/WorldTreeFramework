/****************************************

* 作者：闪电黑客
* 日期：2024/4/16 17:05

* 描述：Node复制兄弟类 生成器

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WorldTree.SourceGenerator
{
	//[Generator]
	internal class CopyNodeClassGenerator2

	//: ISourceGenerator
	{
		public void Initialize(GeneratorInitializationContext context)
		{
			context.RegisterForSyntaxNotifications(() => new FindINodeSubSyntaxReceiver());
		}

		public void Execute(GeneratorExecutionContext context)
		{
			try
			{
				if (!(context.SyntaxReceiver is FindINodeSubSyntaxReceiver receiver)) return;
				if (receiver.ClassDeclarations.Count == 0) return;
				if (receiver.ClassDeclarations.Count != 0) return;

				ClassDeclarationSyntax? NodeClassDeclaration = null;

				// 获取当前编译器实例
				CSharpCompilation compilation = (CSharpCompilation)context.Compilation;

				// 遍历所有的语法树
				foreach (var tree in compilation.SyntaxTrees)
				{
					// 获取根节点
					var root = (CompilationUnitSyntax)tree.GetRoot();

					// 遍历所有的类声明
					foreach (var classDeclaration in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
					{
						// 如果类的名称匹配
						if (classDeclaration.Identifier.Text == "Node")
						{
							if (!IsValidClassDeclaration(classDeclaration)) continue;

							//判断命名空间，如果不是WorldTree则跳过
							if (classDeclaration.Parent is NamespaceDeclarationSyntax namespaceDeclaration)
							{
								if (namespaceDeclaration.Name.ToString() != "WorldTree") continue;
							}

							//判断classDeclaration不是一个类种类
							if (classDeclaration.Modifiers.Any(SyntaxKind.AbstractKeyword) == false) continue;

							NodeClassDeclaration = classDeclaration;
							break;
						}
					}
					if (NodeClassDeclaration != null) break;
				}

				// 如果没找到类型，直接返回
				if (NodeClassDeclaration == null) return;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		/// <summary>
		/// 检查类声明是否有效
		/// </summary>
		private static bool IsValidClassDeclaration(ClassDeclarationSyntax classDeclaration)
		{
			// 检查父节点类型
			var parentNode = classDeclaration.Parent;
			if (parentNode is not (NamespaceDeclarationSyntax or CompilationUnitSyntax))
			{
				return false;
			}

			// 检查是否为泛型类型
			if (classDeclaration.TypeParameterList != null)
			{
				return false;
			}

			return true;
		}
	}
}