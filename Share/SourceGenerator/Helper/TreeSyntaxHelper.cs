/****************************************

* 作者：闪电黑客
* 日期：2024/4/16 17:36

* 描述：

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace WorldTree.SourceGenerator
{
	/// <summary>
	/// 语法树帮助类
	/// </summary>
	internal static class TreeSyntaxHelper
	{
		/// <summary>
		/// 检查类是否继承了指定接口
		/// </summary>
		/// <param name="classDecl">类声明语法</param>
		/// <param name="interfaceName">接口名称</param>
		/// <remarks>只检测自身声明时是否有接口，不包括判断基类</remarks>
		public static bool CheckExtendInterface(TypeDeclarationSyntax classDecl, string interfaceName)
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
		/// <param name="CheckSelf">是否检查类型自身</param>
		public static bool CheckBaseExtendInterface(TypeDeclarationSyntax classDecl, string interfaceName, bool CheckSelf = true)
		{
			// 使用队列存储需要检查的类型
			Queue<TypeDeclarationSyntax> classQueue = new Queue<TypeDeclarationSyntax>();
			classQueue.Enqueue(classDecl);

			while (classQueue.Count != 0)
			{
				TypeDeclarationSyntax currentClass = classQueue.Dequeue();

				// 获取类型的基类列表
				SeparatedSyntaxList<BaseTypeSyntax>? baseTypes = currentClass.BaseList?.Types;

				if (baseTypes is not null)
				{
					foreach (BaseTypeSyntax baseTypeItem in baseTypes)
					{
						// 判断基类型名称是否为指定接口
						if (!CheckSelf && classDecl == currentClass) break;
						if (baseTypeItem.Type.ToString() == interfaceName) return true;
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

		/// <summary> 获取泛型参数的Where约束字符串 where T1 : IEquatable<T1> where T2 : IEquatable<T2> </summary>
		public static string GetWhereTypeArguments(TypeDeclarationSyntax typeDeclaration)
		{
			var typeParameters = typeDeclaration.TypeParameterList?.Parameters;
			if (typeParameters == null || !typeParameters.Value.Any()) return "";

			StringBuilder sb = new StringBuilder();
			foreach (var typeParameter in typeParameters)
			{
				var constraints = typeDeclaration.ConstraintClauses.FirstOrDefault(c => c.Name.Identifier.Text == typeParameter.Identifier.Text);
				if (constraints != null)
				{
					sb.Append($" where {typeParameter.Identifier.Text} : ");
					var interfaces = constraints.Constraints.Select(c => c.ToString());
					sb.Append(string.Join(", ", interfaces));
				}
			}
			return sb.ToString();
		}

		/// <summary>
		/// 复制类型所在源码文件的命名空间
		/// </summary>
		public static string GetUsings(TypeDeclarationSyntax typeDeclaration)
		{
			var root = typeDeclaration.SyntaxTree.GetRoot();
			var usings = root.DescendantNodes().OfType<UsingDirectiveSyntax>();
			return string.Join(Environment.NewLine, usings.Select(u => u.ToString()));
		}

		/// <summary>
		/// 获取所在命名空间
		/// </summary>
		public static string GetUsing(TypeDeclarationSyntax typeDeclaration)
		{
			var root = typeDeclaration.SyntaxTree.GetRoot();
			var ns = root.DescendantNodes().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
			if (ns == null) return "";
			return ns.Name.ToString();
		}
	}
}