/****************************************

* 作者：闪电黑客
* 日期：2024/7/29 11:51

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
	internal class TreePackSerializeGenerator : ISourceGenerator
	{
		public Dictionary<INamedTypeSymbol, List<INamedTypeSymbol>> TypeSubDict = new();


		public void Initialize(GeneratorInitializationContext context)
		{
			context.RegisterForSyntaxNotifications(() => new FindTreePackSyntaxReceiver());
		}



		public void Execute(GeneratorExecutionContext context)
		{
			if (!(context.SyntaxReceiver is FindTreePackSyntaxReceiver receiver and not null)) return;

			StringBuilder Code = new StringBuilder();
			StringBuilder ClassCode = new StringBuilder();

			foreach (var TypeListItem in receiver.TypeDeclarationsDict)
			{
				string? Namespace = null;
				string? Usings = null;
				string fileName = TypeListItem.Key;

				if (TypeListItem.Value.Count != 0)
				{
					var classDeclaration = TypeListItem.Value[0];
					Namespace ??= TreeSyntaxHelper.GetNamespace(classDeclaration);
					Usings ??= TreeSyntaxHelper.GetUsings(classDeclaration);
				}

				foreach (TypeDeclarationSyntax typeDeclaration in TypeListItem.Value)
				{
					INamedTypeSymbol? baseNamedType = context.Compilation.ToINamedTypeSymbol(typeDeclaration);
					if (!TypeSubDict.TryGetValue(baseNamedType, out var SubList))
					{
						SubList = new();
						TypeSubDict.Add(baseNamedType, SubList);
					}
					// 获取所有标记在类型上的特性
					var attributes = baseNamedType.GetAttributes();
					// 查找特定的 TreePackSubAttribute 特性
					attributes.Where(attr => attr.AttributeClass?.Name == GeneratorHelper.TreePackSubAttribute).ToList()
					.ForEach(attr =>
					{
						// 获取特性参数
						var typeArgument = attr.ConstructorArguments.FirstOrDefault();
						if (typeArgument.Kind == TypedConstantKind.Type && typeArgument.Value is INamedTypeSymbol namedTypeSymbol)
						{
							SubList.Add(namedTypeSymbol);
							var typeName = namedTypeSymbol.ToDisplayString();
						}
					});

					TreePackSerializePartialClassGenerator.Execute(context, ClassCode, typeDeclaration, SubList);
				}


				if (ClassCode.Length == 0) return;

				Code.AppendLine(
@$"/****************************************
* 生成序列化部分
*/
"
	);
				Code.AppendLine(Usings);
				Code.AppendLine($"namespace {Namespace}");
				Code.AppendLine("{");
				Code.Append(ClassCode.ToString());
				Code.Append("}");


				context.AddSource($"{fileName}TreePackSerialize.cs", SourceText.From(Code.ToString(), Encoding.UTF8));
			}
		}


		/// <summary>
		/// 格式化代码
		/// </summary>
		public static string FormatCode(string code)
		{
			//解析代码字符串为语法树
			var tree = CSharpSyntaxTree.ParseText(code);
			//获取根节点
			var root = tree.GetRoot();
			//格式化代码
			var formattedCode = root.NormalizeWhitespace();
			//返回格式化后的代码
			return formattedCode.ToFullString();
		}
	}


	/// <summary>
	/// 查找序列化标记类型
	/// </summary>
	public class FindTreePackSyntaxReceiver : ISyntaxReceiver
	{
		public Dictionary<string, List<TypeDeclarationSyntax>> TypeDeclarationsDict = new();

		public void OnVisitSyntaxNode(SyntaxNode node)
		{
			// 判断是否是类或结构体或接口
			if (node is ClassDeclarationSyntax or StructDeclarationSyntax or InterfaceDeclarationSyntax)
			{
				var TypeDeclaration = node as TypeDeclarationSyntax;
				if (TreeSyntaxHelper.CheckAttribute(TypeDeclaration, GeneratorHelper.TreePackSerializableAttribute))
				{
					string fileName = Path.GetFileNameWithoutExtension(TypeDeclaration.SyntaxTree.FilePath);
					if (!TypeDeclarationsDict.TryGetValue(fileName, out var list))
					{
						list = new();
						TypeDeclarationsDict.Add(fileName, list);
					}
					list.Add(TypeDeclaration);
				}
			}
		}
	}
}
