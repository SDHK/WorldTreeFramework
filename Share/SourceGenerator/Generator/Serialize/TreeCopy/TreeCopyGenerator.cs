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

		public static HashSet<INamedTypeSymbol> TypeFieldsDict = new HashSet<INamedTypeSymbol>();

		public void Initialize(GeneratorInitializationContext context)
		{
			context.RegisterForSyntaxNotifications(() => new FindTreeCopySyntaxReceiver());
		}


		public void Execute(GeneratorExecutionContext context)
		{
			TypeFieldsDict.Clear();
			foreach (INamedTypeSymbol classSymbol in NamedSymbolHelper.CollectAllClass(context.Compilation))
			{
				FindTreeDataSpecialAttribute(classSymbol);
			}
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

		private static void FindTreeDataSpecialAttribute(INamedTypeSymbol classSymbol)
		{
			// 1. 直接查找类上的特性
			foreach (AttributeData attribute in classSymbol.GetAttributes())
			{
				if (attribute.AttributeClass?.Name == GeneratorHelper.TreeDataSpecialAttribute &&
					attribute.ConstructorArguments.Length == 1 &&
					attribute.ConstructorArguments[0].Value is int intValue)
				{
					var baseType = classSymbol.BaseType;
					if (baseType != null && baseType.TypeArguments.Length > 1)
					{
						var genericType = baseType.TypeArguments[0] as INamedTypeSymbol;
						if (genericType != null)
						{
							// 处理泛型类型和特性参数
							if (!TypeFieldsDict.Contains(genericType)) TypeFieldsDict.Add(genericType);
						}
					}
				}
			}
		}
	}

	public static class TreeCopyPartialClassGenerator
	{
		public static void Execute(GeneratorExecutionContext context, StringBuilder Code, TypeDeclarationSyntax typeDeclaration)
		{
			INamedTypeSymbol classSymbol = context.Compilation.ToINamedTypeSymbol(typeDeclaration);

			string className = classSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

			//判断是结构体，并且不是非托管类型
			if (classSymbol.TypeKind == TypeKind.Struct && !classSymbol.IsUnmanagedType)
			{
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

			//List<ISymbol> fieldSymbols = FindField(classSymbol);

			INamedTypeSymbol baseSymbol = null;
			List<ISymbol> fieldSymbols = GetAllMembers(classSymbol, TreeCopyGenerator.TypeFieldsDict, out baseSymbol);
			string baseName = baseSymbol?.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

			Code.AppendLine("	{");

			if (classSymbol.TypeKind == TypeKind.Struct)
			{
				Code.AppendLine($"		class TreeCopy : TreeCopyStructRule<{className}>");
				Code.AppendLine("		{");
				Code.AppendLine($"			protected override void Execute(TreeCopyExecutor self, ref {className} source, ref {className} target)");
				Code.AppendLine("			{");
			}
			else
			{
				Code.AppendLine($"		class TreeCopy : TreeCopyRule<{className}>");
				Code.AppendLine("		{");
				Code.AppendLine($"			protected override void Execute(TreeCopyExecutor self, ref object sourceObj, ref object targetObj)");
				Code.AppendLine("			{");
				Code.AppendLine("				if (sourceObj == null)");
				Code.AppendLine("				{");
				Code.AppendLine("					targetObj = null;");
				Code.AppendLine("					return;");
				Code.AppendLine("				}");
				Code.AppendLine($"				if (sourceObj is not {className} source) return;");
				Code.AppendLine($"				if (targetObj is not {className} target)");
				Code.AppendLine("				{");
				Code.AppendLine("					if (targetObj is System.IDisposable disposable) disposable.Dispose();");
				Code.AppendLine($"					targetObj = target = new {className}();");
				Code.AppendLine("				}");
			}

			foreach (var symbol in fieldSymbols)
			{
				string fieldName = symbol.Name;
				if (symbol is IFieldSymbol fieldSymbol)
				{
					//判断是否是属性的后备字段
					if (fieldSymbol.AssociatedSymbol is IPropertySymbol _) continue;

					//值类型或者字符串直接赋值
					if (fieldSymbol.Type.IsUnmanagedType || fieldSymbol.Type.SpecialType == SpecialType.System_String)
						Code.AppendLine($"				target.{fieldName} = source.{fieldName};");
					else
						Code.AppendLine($"				self.CloneObject(source.{fieldName}, ref target.{fieldName});");
				}
				else if (symbol is IPropertySymbol propertySymbol)
				{
					//值类型或者字符串直接赋值
					if (propertySymbol.Type.IsUnmanagedType || propertySymbol.Type.SpecialType == SpecialType.System_String)
					{
						Code.AppendLine($"				target.{fieldName} = source.{fieldName};");
					}
					else
					{
						Code.AppendLine($"				var m{fieldName} = target.{fieldName};");
						Code.AppendLine($"				target.{fieldName} = self.CloneObject(source.{fieldName}, ref m{fieldName});");
					}
				}
			}

			if (classSymbol.TypeKind != TypeKind.Struct && baseName != null)
			{
				Code.AppendLine($"				self.TypeCloneObject(typeof({baseName}), sourceObj, ref targetObj);");
			}

			//要考虑继承了字典的子类情况
			//要考虑引用还原的情况
			Code.AppendLine("			}");
			Code.AppendLine("		}");
			Code.AppendLine("	}");
		}

		private static List<ISymbol> FindField(INamedTypeSymbol classSymbol)
		{
			// 获取字段和属性，过滤掉 TreeCopyIgnoreAttribute 标记的字段
			return NamedSymbolHelper.GetAllMembers(classSymbol)
					.Where(FilterMember)
					.ToList();
		}

		// 成员过滤条件
		static bool FilterMember(ISymbol symbol)
		{
			// 排除包含点的名称
			if (symbol.Name.Contains('.')) return false;

			// 检查是否有忽略特性
			if (NamedSymbolHelper.CheckAttribute(symbol, GeneratorHelper.TreeDataIgnoreAttribute))
				return false;

			// 处理字段
			if (symbol is IFieldSymbol fieldSymbol)
			{
				// 只接受非静态、非只读、非常量的字段
				if (!fieldSymbol.IsStatic && !fieldSymbol.IsReadOnly && !fieldSymbol.IsConst)
				{
					// 排除自动实现的属性的后备字段
					return fieldSymbol.AssociatedSymbol is not IPropertySymbol;
				}
			}
			// 处理属性
			else if (symbol is IPropertySymbol propertySymbol)
			{
				// 只接受非静态、非只读、有get/set方法且非索引器的属性
				return !propertySymbol.IsStatic &&
					   !propertySymbol.IsReadOnly &&
					   propertySymbol.GetMethod != null &&
					   propertySymbol.SetMethod != null &&
					   !propertySymbol.IsIndexer;

				// 检查 Get 和 Set 方法是否是隐式声明的
				// propertySymbol.GetMethod.IsImplicitlyDeclared && propertySymbol.SetMethod.IsImplicitlyDeclared;
			}

			return false;
		}

		/// <summary>
		/// 获取类型所有符合条件的字段和属性成员，遇到指定基类停止搜索
		/// </summary>
		public static List<ISymbol> GetAllMembers(INamedTypeSymbol classSymbol, HashSet<INamedTypeSymbol> ignoreBaseSymbols, out INamedTypeSymbol baseSymbol)
		{
			INamedTypeSymbol baseTypeSymbol = null;

			// 成员过滤条件
			static bool FilterMember(ISymbol symbol)
			{
				// 排除包含点的名称
				if (symbol.Name.Contains('.')) return false;

				// 检查是否有忽略特性
				if (NamedSymbolHelper.CheckAttribute(symbol, GeneratorHelper.TreeCopyIgnoreAttribute))
					return false;

				// 处理字段
				if (symbol is IFieldSymbol fieldSymbol)
				{
					// 只接受非静态、非只读、非常量的字段
					if (!fieldSymbol.IsStatic && !fieldSymbol.IsReadOnly && !fieldSymbol.IsConst)
					{
						// 排除自动实现的属性的后备字段
						return fieldSymbol.AssociatedSymbol is not IPropertySymbol;
					}
				}
				// 处理属性
				else if (symbol is IPropertySymbol propertySymbol)
				{
					// 只接受非静态、非只读、有get/set方法且非索引器的属性
					return !propertySymbol.IsStatic &&
						   !propertySymbol.IsReadOnly &&
						   propertySymbol.GetMethod != null &&
						   propertySymbol.SetMethod != null &&
						   !propertySymbol.IsIndexer;
				}

				return false;
			}

			var members = new List<ISymbol>();
			var processedTypes = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);

			bool CollectMembers(INamedTypeSymbol type)
			{
				// 避免重复处理
				if (!processedTypes.Add(type)) return false;

				// 检查是否达到了需要忽略的基类
				if (ignoreBaseSymbols != null)
				{
					foreach (var ignoreType in ignoreBaseSymbols)
					{
						if (NamedSymbolHelper.IsTypeSymbolEqual(type, ignoreType, TypeCompareOptions.CompareToGenericTypeDefinition))
						{
							baseTypeSymbol = type;
							return true;
						}
					}
				}

				// 获取并过滤成员
				foreach (var member in type.GetMembers().Where(FilterMember))
				{
					// 检查是否已存在同名成员
					if (!members.Any(m => m.Name == member.Name))
					{
						members.Add(member);
					}
				}

				// 处理基类
				if (type.BaseType != null)
				{
					return CollectMembers(type.BaseType);
				}

				return false;
			}

			// 开始收集成员
			CollectMembers(classSymbol);
			baseSymbol = baseTypeSymbol;
			return members;
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
