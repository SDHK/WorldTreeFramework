/****************************************

* 作者：闪电黑客
* 日期：2024/4/28 21:06

* 描述：Node复制兄弟类 生成器

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WorldTree.SourceGenerator
{
	[Generator]
	internal class CopyNodeClassGenerator : ISourceGenerator
	{
		public void Initialize(GeneratorInitializationContext context)
		{
			context.RegisterForSyntaxNotifications(() => new FindSubClassSyntaxReceiver());
		}

		public void Execute(GeneratorExecutionContext context)
		{
			try
			{
				if (!(context.SyntaxContextReceiver is FindSubClassSyntaxReceiver receiver)) return;
				if (receiver.ClassDeclarations.Count == 0) return;

				// 获取当前编译器实例
				CSharpCompilation compilation = (CSharpCompilation)context.Compilation;

				// 获取Node类
				INamedTypeSymbol? namedType = compilation.GetTypeByMetadataName("WorldTree.Node");
				if (namedType == null) return;

				// 获取Node类的源代码
				string SourceCode = namedType.GetTypeSourceCode();

				// 如果没有源代码则返回，如果目标项目，没有引用Node的项目，也会返回null
				if (SourceCode == null) return;

				// 获取Node类的成员和方法字符串
				string NodeMembers = GetClassMembers(SourceCode);

				// 获取INode接口
				INamedTypeSymbol? interfaceName = compilation.GetTypeByMetadataName("WorldTree.INode");
				if (interfaceName == null) return;

				// 遍历所有的类声明
				foreach (var classDeclarationSyntax in receiver.ClassDeclarations)
				{
					if (!compilation.ToINamedTypeSymbol(classDeclarationSyntax).CheckSelfInterface(interfaceName)) continue;
					if (compilation.ToINamedTypeSymbol(classDeclarationSyntax).CheckBaseExtendInterface(interfaceName, false)) continue;

					// 获取文件名
					string fileName = Path.GetFileNameWithoutExtension(classDeclarationSyntax.SyntaxTree.FilePath);
					string className;
					if (classDeclarationSyntax.TypeParameterList != null)
					{
						// 处理泛型类
						string typeParameters = string.Join(", ", classDeclarationSyntax.TypeParameterList.Parameters.Select(p => p.Identifier.ValueText));
						className = $"{classDeclarationSyntax.Identifier.ValueText}<{typeParameters}>";
					}
					else
					{
						// 非泛型类
						className = classDeclarationSyntax.Identifier.ValueText;
					}

					if (className == "Node") continue;

					StringBuilder Code = new StringBuilder();

					Code.AppendLine(
	@$"/****************************************
* {className}对Node基类的实现内容拷贝
*/
"
	);

					if (className == "TestNode1")
					{
						className += "T";
					}
					Code.AppendLine("namespace WorldTree");
					Code.AppendLine("{");
					Code.AppendLine($"	public partial class {className} :{NodeMembers}");
					Code.Append("}");

					context.AddSource($"{fileName}CopyNode.cs", SourceText.From(Code.ToString(), Encoding.UTF8));//生成代码
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		/// <summary>
		/// 获取Node类的成员和方法字符串
		/// </summary>
		public static string GetClassMembers(string classDeclaration)
		{
			var startIndex = classDeclaration.IndexOf(':') + 1;
			var endIndex = classDeclaration.LastIndexOf('}') + 1;
			var membersText = classDeclaration.Substring(startIndex, endIndex - startIndex);
			return membersText;
		}
	}
}