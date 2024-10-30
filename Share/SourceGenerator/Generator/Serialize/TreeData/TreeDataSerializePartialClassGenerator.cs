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

		private static INamedTypeSymbol? GetBaseClassName(TypeDeclarationSyntax typeDeclaration, INamedTypeSymbol? typeSymbol)
		{
			// 获取类型的符号信息
			if (typeSymbol == null) return null;

			// 获取特性
			var attribute = typeSymbol.GetAttributes()
				.FirstOrDefault(attr => attr.AttributeClass?.Name == GeneratorHelper.TreeDataSerializableAttribute);

			if (attribute == null) return null;

			// 获取 IsSub 参数值
			var isSub = attribute.ConstructorArguments.Length > 0
				? (bool?)attribute.ConstructorArguments[0].Value ?? false
				: false;

			// 如果 IsSub 为 true，返回基类名称
			if (isSub && typeSymbol.BaseType != null)
			{
				return typeSymbol.BaseType;
			}
			return null;
		}


		public static void Execute(GeneratorExecutionContext context, StringBuilder Code, TypeDeclarationSyntax typeDeclaration)
		{
			// 将 ClassDeclarationSyntax 转换为 INamedTypeSymbol
			INamedTypeSymbol? classSymbol = context.Compilation.ToINamedTypeSymbol(typeDeclaration);

			// 是否有父级转换
			INamedTypeSymbol? baseSymbol = GetBaseClassName(typeDeclaration, classSymbol);

			string? baseName = baseSymbol?.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);


			// 获取类的完整名称，包括泛型参数
			ClassGenerator(Code, classSymbol, out bool isBase);

			List<ISymbol>? fieldSymbols = null;
			if (!isBase) fieldSymbols = FindField(classSymbol, baseSymbol);
			Code.AppendLine("	{");
			GeneratorSerialize(Code, classSymbol, fieldSymbols, isBase, baseName);
			GeneratorDeserialize(Code, classSymbol, fieldSymbols, isBase, baseName);
			Code.AppendLine("	}");
		}
		private static void GeneratorSerialize(StringBuilder Code, INamedTypeSymbol classSymbol, List<ISymbol>? fieldSymbols, bool isBase, string baseName)
		{
			//获取类型上的特性

			string className = classSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			Code.AppendLine($"		class TreeDataSerialize : TreeDataSerializeRule<{className}>");
			Code.AppendLine("		{");
			Code.AppendLine($"			protected override void Execute(TreeDataByteSequence self, ref object value, ref int nameCode)");
			Code.AppendLine("			{");

			if (!isBase) Code.AppendLine($"				{className} obj = ({className})value;");
			Code.AppendLine("				if (nameCode == -1)");
			Code.AppendLine("				{");
			Code.AppendLine($"					self.WriteType(typeof({className}));");
			if (classSymbol.TypeKind != TypeKind.Struct)
			{
				if (isBase)
				{
					Code.AppendLine("					self.WriteUnmanaged((long)ValueMarkCode.NULL_OBJECT);");
				}
				else
				{
					Code.AppendLine("					if (obj == null)");
					Code.AppendLine("					{");
					Code.AppendLine("						self.WriteUnmanaged((long)ValueMarkCode.NULL_OBJECT);");
					Code.AppendLine("						return;");
					Code.AppendLine("					}");
				}
			}
			if (fieldSymbols != null)
			{
				if (baseName != null)
				{
					Code.AppendLine($"					self.WriteUnmanaged({fieldSymbols.Count + 1});");
				}
				else
				{
					Code.AppendLine($"					self.WriteUnmanaged({fieldSymbols.Count});");
				}
			}
			Code.AppendLine("				}");

			if (fieldSymbols != null)
			{
				foreach (ISymbol symbol in fieldSymbols)
				{
					int hash = symbol.Name.GetFNV1aHash32();
					Code.AppendLine($"				if (!self.WriteCheckNameCode({hash})) self.AddNameCode({hash}, nameof(obj.{symbol.Name}));");
					Code.AppendLine($"				self.WriteValue(obj.{symbol.Name});");
				}
				if (baseName != null)
				{
					//int hash = baseName.GetFNV1aHash32();
					//Code.AppendLine($"				if (!self.WriteCheckNameCode({hash})) self.AddNameCode({hash}, nameof({baseName}));");
					Code.AppendLine($"				self.WriteValue(typeof({baseName}), value, 0);");
				}
			}
			Code.AppendLine("			}");
			Code.AppendLine("		}");
		}

		private static void GeneratorDeserialize(StringBuilder Code, INamedTypeSymbol classSymbol, List<ISymbol>? fieldSymbols, bool isBase, string baseName)
		{
			string className = classSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

			Code.AppendLine($"		class TreeDataDeserialize : TreeDataDeserializeRule<{className}>");
			Code.AppendLine("		{");
			Code.AppendLine("			protected override void Execute(TreeDataByteSequence self, ref object value, ref int nameCode)");
			Code.AppendLine("			{");
			if (baseName != null)
			{
				Code.AppendLine("				if (nameCode != -1)");
				Code.AppendLine("				{");
				Code.AppendLine("					SwitchRead(self, ref value, nameCode);");
				Code.AppendLine("					return;");
				Code.AppendLine("				}");
			}
			Code.AppendLine($"				var targetType = typeof({className});");
			Code.AppendLine("				if (!(self.TryReadType(out var dataType) && dataType == targetType))");
			Code.AppendLine("				{");
			if (classSymbol.TypeKind != TypeKind.Struct)
			{
				Code.AppendLine("					self.SubTypeReadValue(dataType, targetType, ref value);");
			}
			else
			{
				Code.AppendLine("					self.SkipData(dataType);");
			}
			Code.AppendLine("					return;");
			Code.AppendLine("				}");
			Code.AppendLine("				self.ReadUnmanaged(out int count);");
			if (classSymbol.TypeKind != TypeKind.Struct)
			{
				Code.AppendLine("				if (count == ValueMarkCode.NULL_OBJECT)");
				Code.AppendLine("				{");
				Code.AppendLine("					value = null;");
				Code.AppendLine("					return;");
				Code.AppendLine("				}");
			}


			if (!isBase)
			{
				Code.AppendLine("				if (count < 0)");
				Code.AppendLine("				{");
				Code.AppendLine("					self.ReadBack(4);");
				Code.AppendLine("					self.SkipData(dataType);");
				Code.AppendLine("					return;");
				Code.AppendLine("				}");
				if (classSymbol.TypeKind == TypeKind.Class)
				{
					Code.AppendLine($"				if (value is not {className})value = new {className}();");
				}
				else
				{
					//Code.AppendLine($"				var obj = ({className})value;");
				}

			}
			else
			{
				Code.AppendLine("				self.ReadBack(4);");
				Code.AppendLine("				self.SkipData(dataType);");
			}

			if (fieldSymbols != null && fieldSymbols.Count != 0 || baseName != null)
			{
				Code.AppendLine("				for (int i = 0; i < count; i++)");
				Code.AppendLine("				{");

				Code.AppendLine("					self.ReadUnmanaged(out nameCode);");
				Code.AppendLine("					SwitchRead(self, ref value, nameCode);");
				Code.AppendLine("				}");
			}

			//if (!isBase && classSymbol.TypeKind == TypeKind.Struct) Code.AppendLine("				value = obj;");

			Code.AppendLine("			}");

			Code.AppendLine("			/// <summary>");
			Code.AppendLine("			/// 字段读取");
			Code.AppendLine("			/// </summary>");
			Code.AppendLine($"			private void SwitchRead(TreeDataByteSequence self, ref object value, int nameCode)");
			Code.AppendLine("			{");
			Code.AppendLine($"				if (value is not {className} obj) return;");
			Code.AppendLine("				switch (nameCode)");
			Code.AppendLine("				{");
			if (fieldSymbols != null)
			{
				foreach (ISymbol symbol in fieldSymbols)
				{
					int hash = symbol.Name.GetFNV1aHash32();
					if (symbol is IPropertySymbol propertySymbol)
					{
						string symbolName = propertySymbol.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
						Code.AppendLine($"					case {hash}: obj.{symbol.Name} = self.ReadValue<{symbolName}>(); break;");
					}
					else
					{
						Code.AppendLine($"					case {hash}: self.ReadValue(ref obj.{symbol.Name}); break;");
					}
				}
			}

			if (baseName != null)
			{
				Code.AppendLine($"					default: self.ReadValue(typeof({baseName}), ref value, nameCode); break;");
			}
			else
			{
				Code.AppendLine($"					default: self.SkipData(); break;");
			}
			Code.AppendLine("				}");
			Code.AppendLine("			}");


			Code.AppendLine("		}");

		}

		private static List<ISymbol>? FindField(INamedTypeSymbol classSymbol, INamedTypeSymbol baseTypeName)
		{
			Func<ISymbol, bool> filter = f =>
			{
				if (f.Name.Contains('.')) return false;
				if (f is IFieldSymbol fieldSymbol && !fieldSymbol.IsStatic && !fieldSymbol.IsReadOnly && !fieldSymbol.IsConst)
				{
					if (fieldSymbol.AssociatedSymbol is IPropertySymbol) return false;
					if (NamedSymbolHelper.CheckAttribute(f, GeneratorHelper.TreeDataIgnoreAttribute)) return false;
					return true;
				}
				else if (f is IPropertySymbol propertySymbol && !propertySymbol.IsStatic && !propertySymbol.IsReadOnly && propertySymbol.GetMethod != null && propertySymbol.SetMethod != null)
				{
					if (NamedSymbolHelper.CheckAttribute(f, GeneratorHelper.TreeDataIgnoreAttribute)) return false;
					// 过滤掉索引器
					if (propertySymbol.IsIndexer) return false;
					return true;
				}
				return false;
			};


			IEnumerable<ISymbol> members;
			if (baseTypeName != null)
			{
				// 只收集 classSymbol 自身声明的字段和属性
				members = classSymbol.GetMembers().Where(filter);
			}
			else
			{
				// 获取所有成员，包括继承的成员
				members = NamedSymbolHelper.GetAllMembers(classSymbol).Where(filter);
			}


			//IEnumerable<ISymbol> members = NamedSymbolHelper.GetAllMembers(classSymbol).Where(filter);

			//if (baseTypeName != null)
			//{
			//	// 获取字段和属性，过滤掉 TreeDataIgnore 标记的字段,并且过滤掉基类的字段
			//	return members.Where(f => !baseTypeName.GetMembers().Any(m => SymbolEqualityComparer.Default.Equals(m, f))).ToList();
			//}

			// 获取字段和属性，过滤掉 TreeDataIgnore 标记的字段
			return members.ToList();

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
