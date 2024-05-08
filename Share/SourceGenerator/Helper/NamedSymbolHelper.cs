/****************************************

* 作者：闪电黑客
* 日期：2024/4/29 11:52

* 描述：

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WorldTree.SourceGenerator
{
	/// <summary>
	/// 命名符号帮助类
	/// </summary>
	internal static class NamedSymbolHelper
	{
		/// <summary>
		/// 将类声明语法转换为命名类型符号
		/// </summary>
		/// <param name="typeDecl">类声明语法</param>
		/// <param name="compilation">编译类</param>
		public static INamedTypeSymbol? ToINamedTypeSymbol(this Compilation compilation, TypeDeclarationSyntax typeDecl)
		{
			return compilation.GetSemanticModel(typeDecl.SyntaxTree).GetDeclaredSymbol(typeDecl) as INamedTypeSymbol;
		}

		public static INamedTypeSymbol? ToINamedTypeSymbol(this Compilation compilation, string typeFullName)
		{
			return compilation.GetTypeByMetadataName(typeFullName);
		}

		/// <summary>
		/// 检查类是否继承了指定接口
		/// </summary>
		/// <param name="namedTypeSymbol"></param>
		/// <param name="interfaceSymbol"></param>
		/// <returns></returns>
		public static bool CheckAllInterface(this INamedTypeSymbol namedTypeSymbol, INamedTypeSymbol interfaceSymbol)
		{
			return namedTypeSymbol.AllInterfaces.Contains(interfaceSymbol);
		}

		/// <summary>
		/// 检查类 声明时自身写的，是否含有指定接口
		/// </summary>
		/// <param name="namedTypeSymbol">命名符号</param>
		/// <param name="interfaceSymbol">接口</param>
		public static bool CheckSelfInterface(this INamedTypeSymbol namedTypeSymbol, INamedTypeSymbol interfaceSymbol)
		{
			return namedTypeSymbol.Interfaces.Contains(interfaceSymbol);
		}

		/// <summary>
		/// 迭代检查类及其基类(包括抽象类)是否继承了指定接口
		/// </summary>
		/// <param name="typeSymbol">类声明语法</param>
		/// <param name="interfaceSymbol">接口名称(包括命名空间)</param>
		/// <param name="CheckSelf">是否检查类型自身</param>
		public static bool CheckBaseExtendInterface(this INamedTypeSymbol typeSymbol, INamedTypeSymbol interfaceSymbol, bool CheckSelf = true)
		{
			// 使用队列存储需要检查的类型符号
			Queue<INamedTypeSymbol> typeSymbolQueue = new Queue<INamedTypeSymbol>();
			typeSymbolQueue.Enqueue(typeSymbol);

			while (typeSymbolQueue.Count != 0)
			{
				INamedTypeSymbol currentTypeSymbol = typeSymbolQueue.Dequeue();
				if (!CheckSelf && (currentTypeSymbol == typeSymbol)) continue;

				// 检查当前类型符号是否实现了接口
				if (currentTypeSymbol.AllInterfaces.Contains(interfaceSymbol)) return true;

				// 将基类符号加入队列
				INamedTypeSymbol? baseTypeSymbol = currentTypeSymbol.BaseType;
				if (baseTypeSymbol != null) typeSymbolQueue.Enqueue(baseTypeSymbol);
			}

			return false;
		}

		/// <summary>
		/// 源码获取
		/// </summary>
		/// <param name="typeSymbol">命名符号</param>
		public static string GetTypeSourceCode1(this INamedTypeSymbol typeSymbol)
		{
			// 找到声明该类型的语法树
			SyntaxTree? syntaxTree = typeSymbol?.DeclaringSyntaxReferences.FirstOrDefault()?.SyntaxTree;

			if (syntaxTree != null)
			{
				// 获取类型声明的语法节点
				SyntaxNode? typeNode = typeSymbol?.DeclaringSyntaxReferences.First()?.GetSyntax();

				// 获取源代码字符串
				return typeNode?.ToFullString();
			}
			return null;
		}

		/// <summary>
		/// 源码获取
		/// </summary>
		/// <param name="typeSymbol">命名符号</param>
		public static string GetTypeSourceCode(this INamedTypeSymbol typeSymbol)
		{
			// 检查 typeSymbol 是否为 null
			if (typeSymbol == null)
			{
				return null;
			}

			// 获取类型声明的语法参考
			var declaringSyntaxReferences = typeSymbol.DeclaringSyntaxReferences;

			// 检查 declaringSyntaxReferences 是否为空
			if (declaringSyntaxReferences.Length == 0)
			{
				return null;
			}

			// 获取第一个语法参考
			var syntaxReference = declaringSyntaxReferences.First();

			// 获取语法树
			SyntaxTree? syntaxTree = syntaxReference.SyntaxTree;

			// 检查语法树是否为 null
			if (syntaxTree == null)
			{
				return null;
			}

			// 获取类型声明的语法节点
			SyntaxNode? typeNode = syntaxReference.GetSyntax();

			// 检查语法节点是否为 null
			if (typeNode == null)
			{
				return null;
			}

			// 获取源代码字符串
			return typeNode.ToFullString();
		}
	}
}