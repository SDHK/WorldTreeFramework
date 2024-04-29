/****************************************

* 作者：闪电黑客
* 日期：2024/4/28 21:06

* 描述：

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WorldTree.SourceGenerator
{
	[Generator]
	internal class CopyNodeClassGenerator2 : ISourceGenerator
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

				ClassDeclarationSyntax? NodeClassDeclaration = null;

				// 获取当前编译器实例
				CSharpCompilation compilation = (CSharpCompilation)context.Compilation;

				INamedTypeSymbol? namedType = compilation.GetTypeByMetadataName("WorldTree.Node");
				string? s = null;

				if (namedType != null) s = GetTypeSourceCode(namedType);

				if (s == null) s = "";
				foreach (var classDeclarationSyntax in receiver.ClassDeclarations)
				{
					INamedTypeSymbol? interfaceName = compilation.GetTypeByMetadataName("WorldTree.INode");
					if (interfaceName == null) return;
					if (!compilation.ToINamedTypeSymbol(classDeclarationSyntax).CheckSelfInterface(interfaceName)) continue;

					if (compilation.ToINamedTypeSymbol(classDeclarationSyntax).CheckBaseExtendInterface(interfaceName, false)) continue;

					//string fileName = Path.GetFileNameWithoutExtension(classDeclarationSyntax.SyntaxTree.FilePath);

					string className = classDeclarationSyntax.Identifier.ValueText;

					//剔除名称里的泛型和符号
					className = className.Split('<')[0].Split('[')[0];

					//if (classDeclarationSyntax.TypeParameterList != null)
					//{
					//	// 处理泛型类
					//	string typeParameters = string.Join(", ", classDeclarationSyntax.TypeParameterList.Parameters.Select(p => p.Identifier.ValueText));
					//	className = $"{classDeclarationSyntax.Identifier.ValueText}<{typeParameters}>";
					//}
					//else
					//{
					//	// 非泛型类
					//	className = classDeclarationSyntax.Identifier.ValueText;
					//}

					context.AddSource($"{className}TestCopyNode111.cs", SourceText.From($"//测试{className}:{receiver.ClassDeclarations.Count} //{namedType.Name} + \n/*\n{s}\n*/", Encoding.UTF8));//生成代码
					return;
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
		public static string GetClassMembers(ClassDeclarationSyntax classDeclaration)
		{
			var fullText = classDeclaration.ToFullString();

			//判断字符串INode裁剪
			var startIndex = fullText.IndexOf(':') + 1;
			var endIndex = fullText.LastIndexOf('}') + 1;
			var membersText = fullText.Substring(startIndex, endIndex - startIndex);

			return membersText;
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

		/// <summary>
		/// 源码获取
		/// </summary>
		/// <param name="typeSymbol">命名符号</param>
		public static string? GetTypeSourceCode(INamedTypeSymbol typeSymbol)
		{
			// 找到声明该类型的语法树
			var syntaxTree = typeSymbol.DeclaringSyntaxReferences.FirstOrDefault()?.SyntaxTree;
			if (syntaxTree != null)
			{
				// 获取类型声明的语法节点
				var typeNode = typeSymbol.DeclaringSyntaxReferences.First().GetSyntax(CancellationToken.None);

				// 获取源代码字符串
				return typeNode.ToFullString();
			}
			return null;
		}
	}
}