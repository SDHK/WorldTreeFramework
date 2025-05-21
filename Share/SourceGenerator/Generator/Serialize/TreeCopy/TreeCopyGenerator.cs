using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WorldTree.SourceGenerator
{
	[Generator]
	internal class TreeCopyGenerator : ISourceGenerator
	{

		public void Initialize(GeneratorInitializationContext context)
		{
			context.RegisterForSyntaxNotifications(() => new FindTreeCopySyntaxReceiver());
		}


		public void Execute(GeneratorExecutionContext context)
		{
			if (!(context.SyntaxReceiver is FindTreeCopySyntaxReceiver receiver and not null)) return;
			StringBuilder Code = new StringBuilder();
			StringBuilder ClassCode = new StringBuilder();

			foreach (var TypeListItem in receiver.TypeDeclarationsDict)
			{
				Code.Clear();
				ClassCode.Clear();
				string Namespace = null;
				string Usings = null;
				string fileName = TypeListItem.Key;

				if (TypeListItem.Value.Count != 0)
				{
					var classDeclaration = TypeListItem.Value[0];
					Namespace ??= TreeSyntaxHelper.GetNamespace(classDeclaration);
					Usings ??= TreeSyntaxHelper.GetUsings(classDeclaration);
				}

				foreach (TypeDeclarationSyntax typeDeclaration in TypeListItem.Value)
				{
					TreeCopyPartialClassGenerator.Execute(context, ClassCode, typeDeclaration);
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

				context.AddSource($"{fileName}TreeCopy.cs", SourceText.From(Code.ToString(), Encoding.UTF8));
			}
		}
	}

	public static class TreeCopyPartialClassGenerator
	{
		public static void Execute(GeneratorExecutionContext context, StringBuilder Code, TypeDeclarationSyntax typeDeclaration)
		{
			INamedTypeSymbol classSymbol = context.Compilation.ToINamedTypeSymbol(typeDeclaration);

			string className = classSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

			bool isClass = true;
			//判断是结构体，并且不是非托管类型
			if (classSymbol.TypeKind == TypeKind.Struct && !classSymbol.IsUnmanagedType)
			{
				isClass = false;
				Code.AppendLine($"	public partial struct {className}");
			}
			//判断是类型，并且不是抽象类型
			else if (classSymbol.TypeKind == TypeKind.Class && !classSymbol.IsAbstract)
			{
				Code.AppendLine($"	public partial class {className}");
			}
			//其余情况不支持
			else
			{
				return;
			}
			List<ISymbol> fieldSymbols = FindField(classSymbol);
			//要考虑继承了字典的子类情况
			//要考虑引用还原的情况

			Code.AppendLine($"		class TreeCopy : TreeCopyRule<{className}>");
			Code.AppendLine("		{");
			Code.AppendLine($"			protected override void Execute(TreeCopyExecutor self, ref {className} source, ref {className} target)");
			Code.AppendLine("			{");
			Code.AppendLine("				if (source == null)");
			Code.AppendLine("				{");
			Code.AppendLine("					target = null;");
			Code.AppendLine("					return;");
			Code.AppendLine("				}");
			Code.AppendLine($"				if (source is not {className} sourceValue) return;");
			Code.AppendLine($"				if (target is not {className} targetValue)");
			Code.AppendLine("				{");
			Code.AppendLine("					if (target is IDisposable disposable) disposable.Dispose();");
			Code.AppendLine($"					target = targetValue = new {className}();");
			Code.AppendLine("				}");
			Code.AppendLine("				//拷贝成员");
			foreach (var fieldSymbol in fieldSymbols)
			{
				string fieldName = fieldSymbol.Name;
				string fieldType = fieldSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
				Code.AppendLine($"				self.CopyTo(sourceValue.{fieldName}, ref targetValue.{fieldName});");
			}

			Code.AppendLine("			}");
			Code.AppendLine("		}");


		}

		private static List<ISymbol> FindField(INamedTypeSymbol classSymbol)
		{
			// 获取字段和属性，过滤掉 TreeCopyIgnoreAttribute 标记的字段
			return NamedSymbolHelper.GetAllMembers(classSymbol)
					.Where(f =>
					{
						if (f is IFieldSymbol fieldSymbol && !fieldSymbol.IsStatic && !fieldSymbol.IsReadOnly && !fieldSymbol.IsConst)
						{
							if (fieldSymbol.AssociatedSymbol is IPropertySymbol) return false;
							if (NamedSymbolHelper.CheckAttribute(f, GeneratorHelper.TreeCopyIgnoreAttribute)) return false;
							return true;
						}
						else if (f is IPropertySymbol propertySymbol && !propertySymbol.IsStatic && !propertySymbol.IsReadOnly && propertySymbol.GetMethod != null && propertySymbol.SetMethod != null)
						{
							if (NamedSymbolHelper.CheckAttribute(f, GeneratorHelper.TreeCopyIgnoreAttribute)) return false;
							return true;
						}
						return false;
					}
					)
					.ToList();
		}

	}





	/// <summary>
	/// 查找拷贝标记类型
	/// </summary>
	public class FindTreeCopySyntaxReceiver : ISyntaxReceiver
	{
		public Dictionary<string, List<TypeDeclarationSyntax>> TypeDeclarationsDict = new();

		public void OnVisitSyntaxNode(SyntaxNode node)
		{
			// 判断是否是类或结构体或接口
			if (node is ClassDeclarationSyntax or StructDeclarationSyntax or InterfaceDeclarationSyntax)
			{
				var TypeDeclaration = node as TypeDeclarationSyntax;
				if (TreeSyntaxHelper.CheckAttribute(TypeDeclaration, GeneratorHelper.TreeCopyableAttribute))
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
