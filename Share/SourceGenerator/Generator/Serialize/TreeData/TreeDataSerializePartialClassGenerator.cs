/****************************************

* 作者：闪电黑客
* 日期：2024/10/16 20:04

* 描述：

*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace WorldTree.SourceGenerator
{
	internal static class TreeDataSerializePartialClassGenerator
	{
		/// <summary>
		/// 快速获取32位的哈希码
		/// </summary>
		public static int GetFNV1aHash32(this string str)
		{
			const int fnvPrime = 0x01000193;
			const int fnvOffsetBasis = unchecked((int)0x811C9DC5);
			int hash = fnvOffsetBasis;
			foreach (char c in str)
			{
				hash ^= c;
				hash *= fnvPrime;
			}
			return hash;
		}

		public static void Execute(GeneratorExecutionContext context, StringBuilder Code, TypeDeclarationSyntax typeDeclaration)
		{
			// 将 ClassDeclarationSyntax 转换为 INamedTypeSymbol
			INamedTypeSymbol? classSymbol = context.Compilation.ToINamedTypeSymbol(typeDeclaration);
			// 获取类的完整名称，包括泛型参数
			ClassGenerator(Code, classSymbol, out bool isBase);

			List<ISymbol>? fieldSymbols = null;
			if (!isBase) fieldSymbols = FindField(classSymbol);
			Code.AppendLine("	{");
			GeneratorSerialize(Code, classSymbol, fieldSymbols);
			GeneratorDeserialize(Code, classSymbol, fieldSymbols);
			Code.AppendLine("	}");
		}
		private static void GeneratorSerialize(StringBuilder Code, INamedTypeSymbol classSymbol, List<ISymbol>? fieldSymbols)
		{
			string className = classSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			Code.AppendLine($"		class TreeDataSerialize : TreeDataSerializeRule<TreeDataByteSequence, {className}>");
			Code.AppendLine("		{");
			Code.AppendLine($"			protected override void Execute(TreeDataByteSequence self, ref object value)");
			Code.AppendLine("			{");

			Code.AppendLine($"				{className} data = ({className})value;");
			Code.AppendLine($"				self.WriteType(typeof({className}));");
			if (classSymbol.TypeKind != TypeKind.Struct)
			{
				Code.AppendLine("				if (data == null)");
				Code.AppendLine("				{");
				Code.AppendLine("					self.WriteUnmanaged((long)ValueMarkCode.NULL_OBJECT);");
				Code.AppendLine("					return;");
				Code.AppendLine("				}");
			}
			if (fieldSymbols != null)
			{
				Code.AppendLine($"				self.WriteUnmanaged({fieldSymbols.Count});");

				foreach (ISymbol symbol in fieldSymbols)
				{
					int hash = symbol.Name.GetFNV1aHash32();
					Code.AppendLine($"				if (!self.WriteCheckNameCode({hash})) self.AddNameCode({hash}, nameof(data.{symbol.Name}));");
					Code.AppendLine($"				self.WriteValue(data.{symbol.Name});");
				}
			}

			Code.AppendLine("			}");
			Code.AppendLine("		}");
		}

		private static void GeneratorDeserialize(StringBuilder Code, INamedTypeSymbol classSymbol, List<ISymbol>? fieldSymbols)
		{
			string className = classSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

			Code.AppendLine($"		class TreeDataDeserialize : TreeDataDeserializeRule<TreeDataByteSequence, {className}>");
			Code.AppendLine("		{");
			Code.AppendLine("			protected override void Execute(TreeDataByteSequence self, ref object value)");
			Code.AppendLine("			{");
			Code.AppendLine("				self.TryReadType(out Type type);");
			Code.AppendLine("				self.ReadUnmanaged(out int count);");
			if (classSymbol.TypeKind != TypeKind.Struct)
			{
				Code.AppendLine("				if (count == ValueMarkCode.NULL_OBJECT)");
				Code.AppendLine("				{");
				Code.AppendLine("					value = null;");
				Code.AppendLine("					return;");
				Code.AppendLine("				}");
			}
			Code.AppendLine("				if (count < 0)");
			Code.AppendLine("				{");
			Code.AppendLine("					self.ReadBack(4);");
			Code.AppendLine("					self.SkipData(type);");
			Code.AppendLine("					return;");
			Code.AppendLine("				}");
			Code.AppendLine($"				if (typeof({className}) == type)");
			Code.AppendLine("				{");
			if (classSymbol.TypeKind != TypeKind.Struct)
			{
				Code.AppendLine($"					if (!(value is {className} obj)) obj = new {className}();");
			}
			else
			{
				Code.AppendLine($"					var obj = ({className})value;");
			}
			Code.AppendLine("					for (int i = 0; i < count; i++)");
			Code.AppendLine("					{");
			Code.AppendLine("						self.ReadUnmanaged(out int nameCode);");
			if (fieldSymbols.Count != 0)
			{
				Code.AppendLine("						switch (nameCode)");
				Code.AppendLine("						{");
				foreach (ISymbol symbol in fieldSymbols)
				{
					int hash = symbol.Name.GetFNV1aHash32();
					Code.AppendLine($"							case {hash}: self.ReadValue(ref obj.{symbol.Name}); break;");
				}
				Code.AppendLine($"							default: self.SkipData(); break;");
				Code.AppendLine("						}");
			}
			else
			{
				Code.AppendLine("						self.SkipData();");
			}
			Code.AppendLine("					}");
			Code.AppendLine("					value = obj;");
			Code.AppendLine("				}");

			if (classSymbol.TypeKind != TypeKind.Struct)
			{
				Code.AppendLine("				else");
				Code.AppendLine("				{");
				Code.AppendLine($"					self.SubTypeReadValue(type, typeof({className}), ref value);");
				Code.AppendLine("				}");
			}
			Code.AppendLine("			}");
			Code.AppendLine("		}");

		}

		private static List<ISymbol>? FindField(INamedTypeSymbol classSymbol)
		{
			// 获取字段和属性，过滤掉 TreeDataIgnore 标记的字段
			return NamedSymbolHelper.GetAllMembers(classSymbol)
					.Where(f =>
					{
						if (f is IFieldSymbol fieldSymbol && !fieldSymbol.IsStatic && !fieldSymbol.IsReadOnly && !fieldSymbol.IsConst)
						{
							if (fieldSymbol.AssociatedSymbol is IPropertySymbol) return false;
							if (NamedSymbolHelper.CheckAttribute(f, GeneratorHelper.TreeDataIgnoreAttribute)) return false;
							return true;
						}
						else if (f is IPropertySymbol propertySymbol && !propertySymbol.IsStatic && !propertySymbol.IsReadOnly && propertySymbol.GetMethod != null && propertySymbol.SetMethod != null)
						{
							if (NamedSymbolHelper.CheckAttribute(f, GeneratorHelper.TreeDataIgnoreAttribute)) return false;
							return true;
						}
						return false;
					}
					)
					.ToList();
		}

		private static void ClassGenerator(StringBuilder Code, INamedTypeSymbol typeNamedTypeSymbol, out bool isBase)
		{
			isBase = false;
			string className = typeNamedTypeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			if (typeNamedTypeSymbol.TypeKind == TypeKind.Interface)
			{
				isBase = true;
				Code.AppendLine($"	public partial interface {className}");
			}
			else if (typeNamedTypeSymbol.TypeKind == TypeKind.Struct)
			{
				Code.AppendLine($"    public partial struct {className}");
			}
			else if (typeNamedTypeSymbol.TypeKind == TypeKind.Class)
			{
				if (typeNamedTypeSymbol.IsAbstract)
				{
					isBase = true;
					Code.AppendLine($"	public abstract partial class {className}");
				}
				else
				{
					Code.AppendLine($"	public partial class {className}");
				}
			}

		}
	}
}
