/****************************************

* 作者：闪电黑客
* 日期：2024/4/16 17:05

* 描述：Node复制兄弟类 生成器
* 
* 由 INodeProxyGenerator 代替功能，暂时无用

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WorldTree.SourceGenerator
{


	public abstract class CopyNodeClassGenerator<C> : SourceGeneratorBase<C>
		where C : ProjectGeneratorsConfig, new()
	{
		public override void Initialize(GeneratorInitializationContext context)
		{
			context.RegisterForSyntaxNotifications(() => new FindPartialINodeSubSyntaxReceiver());
		}

		public override void ExecuteCore(GeneratorExecutionContext context)
		{
			try
			{
				if (!(context.SyntaxReceiver is FindPartialINodeSubSyntaxReceiver receiver)) return;
				if (receiver.ClassDeclarations.Count == 0) return;

				ClassDeclarationSyntax NodeClassDeclaration = null;

				// 获取当前编译器实例
				CSharpCompilation compilation = (CSharpCompilation)context.Compilation;

				// 遍历所有的语法树
				foreach (SyntaxTree tree in compilation.SyntaxTrees)
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
							NodeClassDeclaration = classDeclaration;
							break;
						}
					}
					if (NodeClassDeclaration != null) break;
				}

				// 如果没找到类型，直接返回
				if (NodeClassDeclaration == null) return;

				foreach (ClassDeclarationSyntax CopyClassDeclarations in receiver.ClassDeclarations)
				{
					string fileName = Path.GetFileNameWithoutExtension(CopyClassDeclarations.SyntaxTree.FilePath);

					string className;
					if (CopyClassDeclarations.TypeParameterList != null)
					{
						// 处理泛型类
						string typeParameters = string.Join(", ", CopyClassDeclarations.TypeParameterList.Parameters.Select(p => p.Identifier.ValueText));
						className = $"{CopyClassDeclarations.Identifier.ValueText}<{typeParameters}>";
					}
					else
					{
						// 非泛型类
						className = CopyClassDeclarations.Identifier.ValueText;
					}

					SyntaxTree syntaxTree = CopyClassDeclarations.SyntaxTree;
					SyntaxNode syntaxRoot = syntaxTree.GetRoot();
					StringBuilder Code = new StringBuilder();
					string Namespace = TreeSyntaxHelper.GetNamespace(NodeClassDeclaration);
					string Usings = TreeSyntaxHelper.GetUsings(NodeClassDeclaration);
					Code.AppendLine(
	@$"/****************************************
* {className}对Node基类的实现内容拷贝
*/
"
	);
					Code.AppendLine(Usings);
					Code.AppendLine($"namespace {Namespace}");
					Code.AppendLine("{");

					Code.AppendLine($"	public partial class {className} ");
					Code.AppendLine("	{");
					Code.AppendLine($" {GetClassMembers(CopyClassDeclarations, NodeClassDeclaration)}");
					Code.AppendLine("	}");
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
		/// 获取Node类的成员和方法字符串
		/// </summary>
		public static string GetClassMembers(ClassDeclarationSyntax mainClass, ClassDeclarationSyntax copyClass)
		{
			// 获取主类成员的字符串表示并存储在HashSet中
			var mainMembersText = new HashSet<string>(mainClass.Members.Select(m => m.ToString()));

			// 选择拷贝类中不与主类重复的成员
			List<MemberDeclarationSyntax> uniqueMembers = GetUniqueMembers(mainClass, copyClass);

			// 使用SyntaxFactory创建一个新的ClassDeclarationSyntax
			ClassDeclarationSyntax uniqueClassDeclaration = SyntaxFactory.ClassDeclaration(copyClass.Identifier)
				.WithModifiers(copyClass.Modifiers)
				.WithMembers(SyntaxFactory.List<MemberDeclarationSyntax>(uniqueMembers));



			var fullText = uniqueClassDeclaration.ToFullString();

			//判断字符串INode裁剪
			var startIndex = fullText.IndexOf('{') + 1;
			var endIndex = fullText.LastIndexOf('}') - 1;
			var membersText = fullText.Substring(startIndex, endIndex - startIndex);

			return membersText;
		}


		private static List<MemberDeclarationSyntax> GetUniqueMembers(ClassDeclarationSyntax mainClass, ClassDeclarationSyntax copyClass)
		{
			// 获取主类成员的名称并存储在HashSet中
			var mainMemberNames = new HashSet<string?>(mainClass.Members.Select(m =>
			{
				switch (m)
				{
					case MethodDeclarationSyntax method:
						return method.Identifier.ValueText;
					case PropertyDeclarationSyntax property:
						return property.Identifier.ValueText;
					case FieldDeclarationSyntax field:
						return field.Declaration.Variables.First().Identifier.ValueText;
					default:
						return null;
				}
			}).Where(name => name != null));

			var uniqueMembers = new List<MemberDeclarationSyntax>();
			var pendingTrivia = new List<SyntaxTrivia>(); // 用于暂存不被拷贝成员的Trivia

			foreach (var m in copyClass.Members)
			{
				string name = null;
				switch (m)
				{
					case MethodDeclarationSyntax method:
						name = method.Identifier.ValueText;
						break;
					case PropertyDeclarationSyntax property:
						name = property.Identifier.ValueText;
						break;
					case FieldDeclarationSyntax field:
						name = field.Declaration.Variables.First().Identifier.ValueText;
						break;
				}

				if (name != null && !mainMemberNames.Contains(name))
				{
					// 如果成员将被拷贝，检查是否有待处理的Trivia
					var leadingTrivia = m.GetLeadingTrivia();
					if (pendingTrivia.Any())
					{
						// 将待处理的Trivia添加到当前成员的前导Trivia中
						leadingTrivia = leadingTrivia.InsertRange(0, pendingTrivia);
						pendingTrivia.Clear(); // 清空待处理的Trivia
					}

					// 如果包含#region或#endregion，则将Trivia连同成员一起添加
					uniqueMembers.Add(m.WithLeadingTrivia(leadingTrivia));


				}
				else
				{
					// 收集可能包含预处理指令的Trivia
					pendingTrivia.AddRange(m.GetLeadingTrivia().Where(trivia => trivia.ToString().TrimStart().StartsWith("#")));
				}
			}

			return uniqueMembers;
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