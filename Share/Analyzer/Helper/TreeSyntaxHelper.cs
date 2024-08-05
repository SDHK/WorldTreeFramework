/****************************************

* 作者：闪电黑客
* 日期：2024/6/25 14:36

* 描述：

*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace WorldTree
{
	/// <summary>
	/// 语法树帮助类
	/// </summary>
	internal static class TreeSyntaxHelper
	{
		/// <summary>
		/// 检查是否有指定的特性
		/// </summary>
		public static bool CheckAttribute(TypeDeclarationSyntax node, string attributeName)
		{
			if (node.AttributeLists.Count == 0) return false;

			foreach (var attributeList in node.AttributeLists)
			{
				foreach (var attribute in attributeList.Attributes)
				{
					var attributeNameText = attribute.Name.ToString();
					if (attributeNameText == attributeName || attributeNameText == attributeName.Replace("Attribute", ""))
					{
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// 检查是否有指定的特性
		/// </summary>
		public static bool CheckAttribute(FieldDeclarationSyntax node, string attributeName)
		{
			if (node.AttributeLists.Count == 0) return false;

			foreach (var attributeList in node.AttributeLists)
			{
				foreach (var attribute in attributeList.Attributes)
				{
					var attributeNameText = attribute.Name.ToString();
					if (attributeNameText == attributeName || attributeNameText == attributeName.Replace("Attribute", ""))
					{
						return true;
					}
				}
			}
			return false;
		}



		/// <summary>
		/// 检查类是否直接实现了指定接口
		/// </summary>
		/// <param name="classDeclaration">类声明</param>
		/// <param name="interfaceName">接口名称</param>
		/// <returns>如果类直接实现了指定接口，则返回true；否则返回false。</returns>
		public static bool CheckExtendDirectlyInterface(ClassDeclarationSyntax classDeclaration, string interfaceName)
		{
			if (classDeclaration.BaseList == null) return false;

			return classDeclaration.BaseList.Types
				.Any(nodeType =>
					nodeType is SimpleBaseTypeSyntax simpleBaseTypeSyntax &&
					simpleBaseTypeSyntax.Type is IdentifierNameSyntax identifierNameSyntax &&
					identifierNameSyntax.Identifier.Text == interfaceName);
		}


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
		/// 获取类的完整名称包括泛型
		/// </summary>
		public static string GetFullTypeName(TypeDeclarationSyntax typeDeclaration)
		{
			// 获取类的名称
			string className = typeDeclaration.Identifier.Text;

			// 获取泛型参数列表
			if (typeDeclaration.TypeParameterList != null)
			{
				className += typeDeclaration.TypeParameterList.ToFullString();
			}
			return className.Trim();
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
		public static string GetNamespace(TypeDeclarationSyntax typeDeclaration)
		{
			var root = typeDeclaration.SyntaxTree.GetRoot();
			var ns = root.DescendantNodes().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
			if (ns == null) return "";
			return ns.Name.ToString();
		}



		/// <summary>
		/// 检查节点是否有注释
		/// </summary>
		public static bool CheckComment(SyntaxNode node)
		{
			// 获取节点之前的Trivia集合
			IEnumerable<SyntaxTrivia> leadingTrivia = node.GetLeadingTrivia()
				.Where(trivia => trivia.IsKind(SyntaxKind.SingleLineCommentTrivia) ||
					trivia.IsKind(SyntaxKind.MultiLineCommentTrivia) ||
					trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia) ||
					trivia.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia));

			string commentText = "";
			foreach (var item in leadingTrivia) commentText += item.ToFullString();

			if (commentText.Contains('/'))
			{
				// 使用正则表达式去除空白字符、斜杠和星号
				commentText = Regex.Replace(commentText, @"[\s/*]+", "");
				// 使用正则表达式去除所有标签
				commentText = Regex.Replace(commentText, "<[^>]*>", "");
				// 检查注释内容是否非空
				if (!string.IsNullOrWhiteSpace(commentText)) return true;
			}
			
			return false; // 没有找到注释，返回false
		}


		/// <summary>
		/// 检查节点是否有 summary 注释
		/// </summary>
		public static bool CheckSummaryComment(SyntaxNode node)
		{
			// 获取节点之前的Trivia集合
			IEnumerable<SyntaxTrivia> leadingTrivia = node.GetLeadingTrivia()
				.Where(trivia => trivia.IsKind(SyntaxKind.SingleLineCommentTrivia) ||
					trivia.IsKind(SyntaxKind.MultiLineCommentTrivia) ||
					trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia) ||
					trivia.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia));

			string commentText = "";
			foreach (var item in leadingTrivia) commentText += item.ToFullString();
			// 检查注释内容是否包含 < > 标签
			if (commentText.Contains('/') && commentText.Contains('<') && commentText.Contains('>'))
			{
				// 使用正则表达式去除空白字符、斜杠和星号
				commentText = Regex.Replace(commentText, @"[\s/*]+", "");
				// 使用正则表达式去除所有标签
				commentText = Regex.Replace(commentText, "<[^>]*>", "");
				// 检查注释内容是否非空
				if (!string.IsNullOrWhiteSpace(commentText)) return true;
			}
			return false; // 没有找到 summary 注释，返回false
		}

		/// <summary>
		/// 获取类型原注释，添加或插入remarks节点备注
		/// </summary>
		public static string GetCommentAddOrInsertRemarks(TypeDeclarationSyntax typeDeclarationSyntax, string remarksToAdd, string tab)
		{
			var triviaList = typeDeclarationSyntax.GetLeadingTrivia().Where(i => i.Kind() == SyntaxKind.SingleLineDocumentationCommentTrivia);
			bool remarksExists = false;
			StringBuilder allComments = new StringBuilder();
			if (triviaList.Any())
			{
				StringBuilder CommentNode = new StringBuilder();
				int CommentNodeIndex = 0;
				foreach (var trivia in triviaList)
				{
					CommentNode.Clear();

					//根据换行符分割注释
					string[] triviaStrings = trivia.ToFullString().Split('\n');

					//遍历出一行注释
					foreach (string triviaStringLine in triviaStrings)
					{
						if (triviaStringLine == string.Empty) continue;

						//去掉前后空格和制表符
						string newTriviaStringLine = triviaStringLine.TrimStart('\t', ' ').TrimEnd('\n', ' ');

						//如果有remarks节点，插入到remarks节点
						if (newTriviaStringLine.Contains("</remarks>"))
						{
							remarksExists = true;
							var index = newTriviaStringLine.IndexOf("</remarks>");
							CommentNode.Append(tab + newTriviaStringLine.Insert(index, $"\n{remarksToAdd.Trim('\n')}\n{tab}/// ") + "\n");
						}
						else
						{
							CommentNode.Append(tab + newTriviaStringLine + "\n");
						}
					}
					CommentNodeIndex++;

					if (CommentNodeIndex != triviaList.Count())
					{
						allComments.AppendLine(CommentNode.ToString());
					}
					else
					{
						allComments.Append(CommentNode.ToString());
					}
				}
			}

			// If there is no remarks node, add one
			if (!remarksExists)
			{
				allComments.AppendLine($"{tab}/// <remarks>");
				allComments.Append($"{remarksToAdd}");
				allComments.AppendLine($"{tab}/// </remarks>");
			}

			return allComments.ToString();
		}

		/// <summary>
		/// 检测是否有私有访问修饰符,包括没写访问修饰符的情况
		/// </summary>
		public static bool ModifiersCheckPrivateKeyword(SyntaxTokenList modifiers)
		{
			var privateKeyword = modifiers.Any(SyntaxKind.PrivateKeyword);
			if (!privateKeyword)
			{
				privateKeyword = true;
				foreach (var modifier in modifiers)
				{
					if (modifier.IsKind(SyntaxKind.PrivateKeyword))
					{
						privateKeyword = true;
						break;
					}
					else if (modifier.IsKind(SyntaxKind.PublicKeyword))
					{
						privateKeyword = false;
					}
					else if (modifier.IsKind(SyntaxKind.ProtectedKeyword))
					{
						privateKeyword = false;
					}
				}
			}
			return privateKeyword;
		}

		/// <summary>
		/// 判断否包含指定的修饰符
		/// </summary>
		public static bool SyntaxKindContains(List<SyntaxKind> keys, List<SyntaxKind> values, bool valuesZero = true)
		{
			foreach (var value in values)
			{
				if (!keys.Contains(value))
				{
					return false;
				}
			}
			if (values.Count == 0)
			{
				return valuesZero;
			}
			else
			{
				return true;
			}
		}

		/// <summary>
		/// 判断是否包含指定的修饰符
		/// </summary>
		public static bool SyntaxKindContains(SyntaxTokenList keys, List<SyntaxKind> values, bool valuesZero = true)
		{
			if (values.Count == 0)
			{
				return valuesZero;
			}
			foreach (var value in values)
			{
				bool flag = false;
				foreach (var key in keys)
				{
					if (key.IsKind(value))
					{
						flag = true;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// 判断是否包含指定的其中一个修饰符
		/// </summary>
		public static bool SyntaxKindContainsAny(SyntaxTokenList keys, List<SyntaxKind> values, bool valuesZero = true)
		{
			if (values.Count == 0)
			{
				return valuesZero;
			}

			foreach (var value in values)
			{
				foreach (var key in keys)
				{
					if (key.IsKind(value))
					{
						return true; // 如果找到匹配的修饰符，立即返回true
					}
				}
			}

			return false; // 如果没有找到任何匹配
		}

		/// <summary>
		/// 获取节点所在的类型
		/// </summary>
		public static BaseTypeDeclarationSyntax GetParentType(SyntaxNode syntaxNode)
		{
			while (syntaxNode != null)
			{
				if (syntaxNode is BaseTypeDeclarationSyntax typeDeclaration)
				{
					return typeDeclaration;
				}
				syntaxNode = syntaxNode.Parent;
			}
			return null;
		}

		/// <summary>
		/// 获取节点所在的方法
		/// </summary>
		public static MethodDeclarationSyntax GetParentMethod(SyntaxNode syntaxNode)
		{
			while (syntaxNode != null)
			{
				if (syntaxNode is MethodDeclarationSyntax methodDeclaration)
				{
					return methodDeclaration;
				}
				syntaxNode = syntaxNode.Parent;
			}
			return null;
		}

		/// <summary>
		/// 获取节点所在的匿名委托
		/// </summary>
		/// <param name="syntaxNode">要检查的语法节点</param>
		/// <returns>匿名委托的语法节点，如果不存在则返回null</returns>
		public static SyntaxNode GetParentAnonymousDelegate(SyntaxNode syntaxNode)
		{
			while (syntaxNode != null)
			{
				if (syntaxNode is AnonymousMethodExpressionSyntax ||
					syntaxNode is ParenthesizedLambdaExpressionSyntax ||
					syntaxNode is SimpleLambdaExpressionSyntax)
				{
					return syntaxNode;
				}
				syntaxNode = syntaxNode.Parent;
			}
			return null;
		}
	}
}