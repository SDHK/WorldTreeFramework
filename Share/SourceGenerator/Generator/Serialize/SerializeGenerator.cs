/****************************************

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

			string? Namespace = null;
			string? Usings = null;
			string? fileName = null;

			if (receiver.ClassDeclarations.Count != 0)
			{
				var classDeclaration = receiver.ClassDeclarations[0];
				fileName ??= Path.GetFileNameWithoutExtension(classDeclaration.SyntaxTree.FilePath);
				Namespace ??= TreeSyntaxHelper.GetNamespace(classDeclaration);
				Usings ??= TreeSyntaxHelper.GetUsings(classDeclaration);
			}
			foreach (var classDeclaration in receiver.ClassDeclarations)
			{
				SerializeClassGenerator.Execute(context, ClassCode, classDeclaration);
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
		public List<ClassDeclarationSyntax> ClassDeclarations = new();
		public List<StructDeclarationSyntax> StructDeclarations = new();
		public List<InterfaceDeclarationSyntax> interfaceDeclarations = new();


		public void OnVisitSyntaxNode(SyntaxNode node)
		{
			// 判断是否是类或结构体
			if (node is ClassDeclarationSyntax classDeclarationSyntax)
			{
				if (TreeSyntaxHelper.CheckAttribute(classDeclarationSyntax, GeneratorHelper.TreePackAttribute))
					ClassDeclarations.Add(classDeclarationSyntax);
			}
			else if (node is StructDeclarationSyntax structDeclarationSyntax)
			{
				if (TreeSyntaxHelper.CheckAttribute(structDeclarationSyntax, GeneratorHelper.TreePackAttribute))
					StructDeclarations.Add(structDeclarationSyntax);
			}
			else if (node is InterfaceDeclarationSyntax interfaceDeclarationSyntax)
			{
				if (TreeSyntaxHelper.CheckAttribute(interfaceDeclarationSyntax, GeneratorHelper.TreePackAttribute))
					interfaceDeclarations.Add(interfaceDeclarationSyntax);
			}
		}
	}
}
