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

			foreach (var classDeclaration in receiver.ClassDeclarations)
			{
				SerializeClassGenerator.Execute(context, classDeclaration);
			}

			//context.AddSource($"TestSerialize.cs", SourceText.From($"{receiver.ClassDeclarations.Count}", Encoding.UTF8));

		}
	}

	internal static class SerializeClassGenerator
	{
		/// <summary>
		/// 获取未托管类型参数
		/// </summary>
		private static HashSet<string> GetUnmanagedTypeParameters(ClassDeclarationSyntax classDeclaration)
		{
			var unmanagedTypeParameters = new HashSet<string>();

			var typeParameters = classDeclaration.TypeParameterList?.Parameters;
			foreach (var typeParameter in typeParameters)
			{
				var constraints = classDeclaration.ConstraintClauses
					.FirstOrDefault(c => c.Name.Identifier.Text == typeParameter.Identifier.Text);

				if (constraints != null && constraints.Constraints.Any(c => c is TypeConstraintSyntax typeConstraint && typeConstraint.Type.ToString() == "unmanaged"))
				{
					unmanagedTypeParameters.Add(typeParameter.Identifier.Text);
				}
			}

			return unmanagedTypeParameters;
		}


		public static void Execute(GeneratorExecutionContext context, ClassDeclarationSyntax classDeclaration)
		{
		

			// 将 ClassDeclarationSyntax 转换为 INamedTypeSymbol
			INamedTypeSymbol? classSymbol = context.Compilation.ToINamedTypeSymbol(classDeclaration);

			// 获取字段，过滤掉 TreePackIgnore 标记的字段
			List<IFieldSymbol> fieldSymbols = classSymbol.GetMembers()
				.OfType<IFieldSymbol>()
				.Where(f => !NamedSymbolHelper.CheckAttribute(f, GeneratorHelper.TreePackIgnoreAttribute))
				.ToList();



			//生成序列化代码
			StringBuilder Code = new StringBuilder();
			Code.AppendLine(
@$"/****************************************
* 序列化兄弟类
*/
"
);
			//原类型的命名空间
			var Usings = TreeSyntaxHelper.GetUsings(classDeclaration);
			string Namespace = TreeSyntaxHelper.GetNamespace(classDeclaration);
			// 获取类的完整名称，包括泛型参数
			string className = TreeSyntaxHelper.GetFullTypeName(classDeclaration);
			Code.AppendLine(Usings);
			Code.AppendLine($"namespace {Namespace}");
			Code.AppendLine("{");
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
			}
			Code.AppendLine("			}");
			Code.AppendLine("		}");


			Code.AppendLine("	}");
			Code.Append("}");
			//string fileName = Path.GetFileNameWithoutExtension(classDeclaration.SyntaxTree.FilePath);

			context.AddSource($"{classDeclaration.Identifier}Serialize.cs", SourceText.From(Code.ToString(), Encoding.UTF8));
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
