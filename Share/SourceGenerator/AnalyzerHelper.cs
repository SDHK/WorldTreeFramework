/****************************************

* 作者：闪电黑客
* 日期：2024/4/16 17:36

* 描述：

*/

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorldTree.SourceGenerator
{
	internal static class AnalyzerHelper
	{
		/// <summary>
		/// INamedTypeSymbol 是否含有指定接口
		/// </summary>
		public static bool HasInterface(this INamedTypeSymbol namedTypeSymbol, string InterfaceName)
		{
			foreach (INamedTypeSymbol? iInterface in namedTypeSymbol.AllInterfaces)
			{
				if (iInterface.ToString() == InterfaceName) return true;
			}
			return false;
		}

		/// <summary>
		/// 检查类是否继承了指定接口
		/// </summary>
		/// <param name="classDecl">类声明语法</param>
		/// <param name="interfaceName">接口名称</param>
		public static bool CheckExtendInterface(ClassDeclarationSyntax classDecl, string interfaceName)
		{
			// 获取类型的基类列表
			SeparatedSyntaxList<BaseTypeSyntax>? baseTypes = classDecl.BaseList?.Types;
			if (baseTypes is not null)
			{
				// 检查基类列表中是否包含指定接口
				bool inheritsDirectly = false;
				foreach (BaseTypeSyntax baseTypeItem in baseTypes)
				{
					// 判断基类型名称是否为指定接口
					inheritsDirectly = baseTypeItem.Type.ToString() == interfaceName;
					if (inheritsDirectly) return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 迭代检查类及其基类(包括抽象类)是否继承了指定接口
		/// </summary>
		/// <param name="classDecl">类声明语法</param>
		/// <param name="interfaceName">接口名称</param>
		public static bool CheckBaseExtendInterface(ClassDeclarationSyntax classDecl, string interfaceName)
		{
			// 使用队列存储需要检查的类型
			Queue<ClassDeclarationSyntax> classQueue = new Queue<ClassDeclarationSyntax>();
			classQueue.Enqueue(classDecl);

			while (classQueue.Count != 0)
			{
				ClassDeclarationSyntax currentClass = classQueue.Dequeue();

				// 获取类型的基类列表
				SeparatedSyntaxList<BaseTypeSyntax>? baseTypes = currentClass.BaseList?.Types;

				if (baseTypes is not null)
				{
					// 检查基类列表中是否包含指定接口
					bool inheritsDirectly = false;
					foreach (BaseTypeSyntax baseTypeItem in baseTypes)
					{
						// 判断基类型名称是否为指定接口
						inheritsDirectly = baseTypeItem.Type.ToString() == interfaceName;
						if (inheritsDirectly) return true;
					}

					// 将基类加入队列
					foreach (BaseTypeSyntax baseType in baseTypes)
					{
						// 获取基类的语法节点
						TypeSyntax baseTypeSyntax = baseType.Type;

						// 如果基类是另一个类,则加入队列
						if (baseTypeSyntax is IdentifierNameSyntax identifierName)
						{
							// 查找类定义语法节点
							ClassDeclarationSyntax baseClassDecl = currentClass.SyntaxTree.GetRoot()
								.DescendantNodes()
								.OfType<ClassDeclarationSyntax>()
								.FirstOrDefault(c => c.Identifier.ValueText == identifierName.Identifier.ValueText);

							if (baseClassDecl != null)
							{
								classQueue.Enqueue(baseClassDecl);
							}
						}
					}
				}
			}
			return false;
		}
	}
}