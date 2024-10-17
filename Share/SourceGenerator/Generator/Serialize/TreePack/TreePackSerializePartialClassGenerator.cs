/****************************************

* 作者：闪电黑客
* 日期：2024/8/12 11:49

* 描述：

*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace WorldTree.SourceGenerator
{
	internal static class TreePackSerializePartialClassGenerator
	{
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

		private static List<ISymbol>? FindField(INamedTypeSymbol classSymbol)
		{
			// 获取字段和属性，过滤掉 TreePackIgnore 标记的字段
			return NamedSymbolHelper.GetAllMembers(classSymbol)
					.Where(f =>
					{
						if (f is IFieldSymbol fieldSymbol && !fieldSymbol.IsStatic && !fieldSymbol.IsReadOnly && !fieldSymbol.IsConst )
						{
							if (fieldSymbol.AssociatedSymbol is IPropertySymbol) return false;
							if (NamedSymbolHelper.CheckAttribute(f, GeneratorHelper.TreePackIgnoreAttribute)) return false;
							return true;
						}
						else if (f is IPropertySymbol propertySymbol && !propertySymbol.IsStatic && !propertySymbol.IsReadOnly && propertySymbol.GetMethod != null && propertySymbol.SetMethod != null)
						{
							if (NamedSymbolHelper.CheckAttribute(f, GeneratorHelper.TreePackIgnoreAttribute)) return false;
							return true;
						}
						return false;
					}
					)
					.ToList();
		}


		private static void GeneratorSerialize(StringBuilder Code, INamedTypeSymbol classSymbol, List<ISymbol>? fieldSymbols, List<INamedTypeSymbol> SubList)
		{
			string className = classSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

			Code.AppendLine($"		class TreePackSerialize : TreePackSerializeRule<TreePackByteSequence, {className}>");
			Code.AppendLine("		{");

			if (SubList != null && SubList.Count != 0)
			{
				Code.AppendLine($"			static readonly System.Collections.Generic.Dictionary<Type, short> m_typeToMarkDict = new({SubList.Count})");
				Code.AppendLine("			{");
				Code.AppendLine($"				{{typeof({className}), ValueMarkCode.THIS_OBJECT }},");
				for (int i = 0; i < SubList.Count; i++)
				{
					Code.AppendLine($"				{{typeof({SubList[i].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}), {i} }},");
				}
				Code.AppendLine("			};");
			}

			Code.AppendLine($"			protected override void Execute(TreePackByteSequence self, ref {className} value)");
			Code.AppendLine("			{");
			if (classSymbol.TypeKind != TypeKind.Struct)
			{
				Code.AppendLine("				if(value == null)");
				Code.AppendLine("				{");
				Code.AppendLine("					self.WriteUnmanaged(ValueMarkCode.NULL_OBJECT);");
				Code.AppendLine("					return;");
				Code.AppendLine("				}");
				Code.AppendLine("				else");
				Code.AppendLine("				{");
				Code.AppendLine($"					self.WriteUnmanaged<short>({fieldSymbols.Count});");
				Code.AppendLine("				}");
			}


			if (SubList != null && SubList.Count != 0)
			{
				Code.AppendLine("				if (m_typeToMarkDict.TryGetValue(value.GetType(), out short markCode))");
				Code.AppendLine("				{");
				Code.AppendLine("					self.WriteUnmanaged(markCode);");
				Code.AppendLine("					switch (markCode)");
				Code.AppendLine("					{");
				for (int i = 0; i < SubList.Count; i++)
				{
					Code.AppendLine($"						case {i}:");
					Code.AppendLine($"							self.WriteValue(System.Runtime.CompilerServices.Unsafe.As<{className}, {SubList[i].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}>(ref value));return;");
				}
				Code.AppendLine($"						default: break;");
				Code.AppendLine("					}");
				Code.AppendLine("				}");
				Code.AppendLine("				else");
				Code.AppendLine("				{");
				Code.AppendLine($"					self.LogError($\"{{value.GetType()}}:没有可转换类型\");");
				Code.AppendLine("					return;");
				Code.AppendLine("				}");
			}

			if (fieldSymbols != null)
				foreach (ISymbol symbol in fieldSymbols)
				{
					if (symbol is IFieldSymbol fieldSymbol)
					{
						//判断是否是属性的后备字段
						if (fieldSymbol.AssociatedSymbol is IPropertySymbol propertySymbol) continue;

						if (fieldSymbol.Type is IArrayTypeSymbol arrayTypeSymbol)
						{
							if (arrayTypeSymbol.ElementType.IsUnmanagedType)
								Code.AppendLine($"				self.WriteUnmanagedArray(value.{symbol.Name});");
							else
								Code.AppendLine($"				self.WriteArray(value.{symbol.Name});");
						}
						else
						{
							if (fieldSymbol.Type.IsUnmanagedType)
								Code.AppendLine($"				self.WriteUnmanaged(value.{symbol.Name});");
							else
								Code.AppendLine($"				self.WriteValue(value.{symbol.Name});");
						}
					}
					else if (symbol is IPropertySymbol propertySymbol)
					{
						if (propertySymbol.Type is IArrayTypeSymbol arrayTypeSymbol)
						{
							if (arrayTypeSymbol.ElementType.IsUnmanagedType)
								Code.AppendLine($"				self.WriteUnmanagedArray(value.{symbol.Name});");
							else
								Code.AppendLine($"				self.WriteArray(value.{symbol.Name});");
						}
						else
						{
							if (propertySymbol.Type.IsUnmanagedType)
								Code.AppendLine($"				self.WriteUnmanaged(value.{symbol.Name});");
							else
								Code.AppendLine($"				self.WriteValue(value.{symbol.Name});");
						}
					}
				}
			Code.AppendLine("			}");
			Code.AppendLine("		}");

		}


		private static void GeneratorDeserialize(StringBuilder Code, INamedTypeSymbol classSymbol, List<ISymbol>? fieldSymbols, List<INamedTypeSymbol> SubList)
		{
			string className = classSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			Code.AppendLine($"		class TreePackDeserialize : TreePackDeserializeRule<TreePackByteSequence, {className}>");
			Code.AppendLine("		{");
			Code.AppendLine($"			protected override void Execute(TreePackByteSequence self, ref {className} value)");
			Code.AppendLine("			{");


			if (classSymbol.TypeKind != TypeKind.Struct)
			{
				Code.AppendLine("				if (self.ReadUnmanaged(out short tagCount) == ValueMarkCode.NULL_OBJECT)");
				Code.AppendLine("				{");
				Code.AppendLine("					value = default;");
				Code.AppendLine("					return;");
				Code.AppendLine("				}");
			}

			if (SubList != null && SubList.Count != 0)
			{
				Code.AppendLine("				self.ReadUnmanaged(out short markCode);");
				Code.AppendLine("				switch (markCode)");
				Code.AppendLine("				{");
				Code.AppendLine($"					case ValueMarkCode.THIS_OBJECT: break;");
				for (int i = 0; i < SubList.Count; i++)
				{
					Code.AppendLine($"					case {i}:");
					Code.AppendLine($"						self.ReadValue(ref System.Runtime.CompilerServices.Unsafe.As<{className}, {SubList[i].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}>(ref value));return;");
				}
				Code.AppendLine($"					default:");
				Code.AppendLine($"						self.LogError($\"{{value.GetType()}}:没有可转换类型\");return;");
				Code.AppendLine("				}");
			}

			if (fieldSymbols != null)
			{
				if (classSymbol.TypeKind == TypeKind.Class)
					Code.AppendLine($"				if (value == null) value = new();");
				foreach (ISymbol symbol in fieldSymbols)
				{
					if (symbol is IFieldSymbol fieldSymbol)
					{
						//判断是否是属性的后备字段
						if (fieldSymbol.AssociatedSymbol is IPropertySymbol propertySymbol) continue;

						if (fieldSymbol.Type is IArrayTypeSymbol arrayTypeSymbol)
						{
							if (arrayTypeSymbol.ElementType.IsUnmanagedType)
								Code.AppendLine($"				self.ReadUnmanagedArray(ref value.{symbol.Name});");
							else
								Code.AppendLine($"				self.ReadArray(ref value.{symbol.Name});");
						}
						else
						{
							if (fieldSymbol.Type.IsUnmanagedType)
								Code.AppendLine($"				self.ReadUnmanaged(out value.{symbol.Name});");
							else
								Code.AppendLine($"				self.ReadValue(ref value.{symbol.Name});");
						}
					}
					else if (symbol is IPropertySymbol propertySymbol)
					{
						if (propertySymbol.Type is IArrayTypeSymbol arrayTypeSymbol)
						{
							Code.AppendLine($"				{arrayTypeSymbol.ElementType}[] m{symbol.Name} = default;");

							if (arrayTypeSymbol.ElementType.IsUnmanagedType)
								Code.AppendLine($"				self.ReadUnmanagedArray(ref m{symbol.Name});");
							else
								Code.AppendLine($"				self.ReadArray(ref m{symbol.Name});");
						}
						else
						{
							Code.AppendLine($"				{propertySymbol.Type.Name} m{symbol.Name} = default;");
							if (propertySymbol.Type.IsUnmanagedType)
								Code.AppendLine($"				self.ReadUnmanaged(out m{symbol.Name});");
							else
								Code.AppendLine($"				self.ReadValue(ref m{symbol.Name});");
						}
						Code.AppendLine($"				value.{propertySymbol.Name} = m{symbol.Name};");
					}
				}

			}

			Code.AppendLine("			}");
			Code.AppendLine("		}");
		}

		public static void Execute(GeneratorExecutionContext context, StringBuilder Code, TypeDeclarationSyntax typeDeclaration, List<INamedTypeSymbol> SubList)
		{
			// 将 ClassDeclarationSyntax 转换为 INamedTypeSymbol
			INamedTypeSymbol? classSymbol = context.Compilation.ToINamedTypeSymbol(typeDeclaration);

			// 获取类的完整名称，包括泛型参数
			ClassGenerator(Code, classSymbol, out bool isBase);

			List<ISymbol>? fieldSymbols = null;
			if (!isBase) fieldSymbols = FindField(classSymbol);

			Code.AppendLine("	{");
			GeneratorSerialize(Code, classSymbol, fieldSymbols, SubList);
			GeneratorDeserialize(Code, classSymbol, fieldSymbols, SubList);
			Code.AppendLine("	}");
		}

	}
}
