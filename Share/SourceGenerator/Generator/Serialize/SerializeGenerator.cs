﻿/****************************************

* 作者：闪电黑客
* 日期：2024/7/29 11:51

* 描述：

*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Text;

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
					if (typeDeclaration is ClassDeclarationSyntax classDeclaration)
					{
						SerializeClassGenerator.Execute(context, ClassCode, classDeclaration);
					}
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


				context.AddSource($"{fileName}Serialize.cs", SourceText.From(Code.ToString(), Encoding.UTF8));
			}
		}
	}

	internal static class SerializeClassGenerator
	{
		public static void Execute(GeneratorExecutionContext context, StringBuilder Code, ClassDeclarationSyntax classDeclaration)
		{
			// 将 ClassDeclarationSyntax 转换为 INamedTypeSymbol
			INamedTypeSymbol? classSymbol = context.Compilation.ToINamedTypeSymbol(classDeclaration);

			// 获取字段，过滤掉 TreePackIgnore 标记的字段
			List<IFieldSymbol> fieldSymbols = classSymbol.GetMembers()
				.OfType<IFieldSymbol>()
				.Where(f => !NamedSymbolHelper.CheckAttribute(f, GeneratorHelper.TreePackIgnoreAttribute))
				.ToList();

			// 获取类的完整名称，包括泛型参数
			string className = TreeSyntaxHelper.GetFullTypeName(classDeclaration);
			Code.AppendLine($"	public partial class {className}");
			Code.AppendLine("	{");

			Code.AppendLine($"		class Serialize : SerializeRule<ByteSequence, {className}>");
			Code.AppendLine("		{");
			Code.AppendLine($"			protected override void Execute(ByteSequence self, ref {className} value)");
			Code.AppendLine("			{");
			foreach (IFieldSymbol fieldSymbol in fieldSymbols)
			{
				if (fieldSymbol.Type.IsUnmanagedType)
				{
					Code.AppendLine($"				self.Write(value.{fieldSymbol.Name});");
				}
				else
				{
					Code.AppendLine($"				self.Serialize(ref value.{fieldSymbol.Name});");
				}
			}
			Code.AppendLine("			}");
			Code.AppendLine("		}");


			Code.AppendLine($"		class Deserialize : DeserializeRule<ByteSequence, {className}>");
			Code.AppendLine("		{");
			Code.AppendLine($"			protected override void Execute(ByteSequence self, ref {className} value)");
			Code.AppendLine("			{");
			Code.AppendLine($"				if (value == null) value = new();");
			foreach (IFieldSymbol fieldSymbol in fieldSymbols)
			{
				if (fieldSymbol.Type.IsUnmanagedType)
				{
					Code.AppendLine($"				self.Read(out value.{fieldSymbol.Name});");
				}
				else
				{
					Code.AppendLine($"				self.Deserialize(ref value.{fieldSymbol.Name});");
				}
			}
			Code.AppendLine("			}");
			Code.AppendLine("		}");

			Code.AppendLine("	}");
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
				if (TreeSyntaxHelper.CheckAttribute(TypeDeclaration, GeneratorHelper.TreePackAttribute))
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