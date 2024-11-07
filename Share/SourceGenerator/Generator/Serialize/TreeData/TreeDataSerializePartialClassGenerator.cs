/****************************************

* 作者：闪电黑客
* 日期：2024/10/16 20:04

* 描述：

*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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
			INamedTypeSymbol? classSymbol = context.Compilation.ToINamedTypeSymbol(typeDeclaration);

			INamedTypeSymbol? baseSymbol = null;
			int membersCount = 0;

			// 获取类的完整名称，包括泛型参数
			ClassGenerator(Code, classSymbol, out bool isAbstract);

			List<ISymbol>? fieldSymbols = null;
			if (!isAbstract) fieldSymbols = GetAllMembers(classSymbol, TreeDataSerializeGenerator.TypeFieldsCountDict, out baseSymbol, out membersCount);
			string? baseName = baseSymbol?.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);


			Code.AppendLine("	{");
			GeneratorSerialize(Code, classSymbol, fieldSymbols, isAbstract, baseName, membersCount);
			GeneratorDeserialize(Code, classSymbol, fieldSymbols, isAbstract, baseName, membersCount);
			Code.AppendLine("	}");
		}

		//private static ISymbol? GetMethodWithAttribute(INamedTypeSymbol classSymbol, string attributeName)
		//{
		//	return classSymbol.GetMembers().FirstOrDefault(m => m.GetAttributes().Any(a => a.AttributeClass?.Name == attributeName));
		//}

		private static ISymbol? GetMethodWithAttribute(INamedTypeSymbol classSymbol, string attributeName)
		{
			while (classSymbol != null)
			{
				var method = classSymbol.GetMembers().FirstOrDefault(m => m.GetAttributes().Any(a => a.AttributeClass?.Name == attributeName));
				if (method != null)
				{
					return method;
				}
				classSymbol = classSymbol.BaseType;
			}
			return null;
		}



		private static void GeneratorSerialize(StringBuilder Code, INamedTypeSymbol classSymbol, List<ISymbol>? fieldSymbols, bool isAbstract, string baseName, int membersCount)
		{
			string className = classSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			Code.AppendLine($"		class TreeDataSerialize : TreeDataSerializeRule<{className}>");
			Code.AppendLine("		{");
			Code.AppendLine($"			protected override void Execute(TreeDataByteSequence self, ref object value, ref int nameCode)");
			Code.AppendLine("			{");

			if (!isAbstract)
			{
				Code.AppendLine($"				{className} obj = ({className})value;");
				if (NamedSymbolHelper.CheckInterface(classSymbol, GeneratorHelper.ISerializable, out _))
				 Code.AppendLine($"				obj.OnSerialize();");
			}

			Code.AppendLine("				if (nameCode == -1)");
			Code.AppendLine("				{");
			Code.AppendLine($"					self.WriteType(typeof({className}));");
			if (classSymbol.TypeKind != TypeKind.Struct)
			{
				if (isAbstract)
				{
					Code.AppendLine("					self.WriteUnmanaged((int)ValueMarkCode.NULL_OBJECT);");
				}
				else
				{
					Code.AppendLine("					if (obj == null)");
					Code.AppendLine("					{");
					Code.AppendLine("						self.WriteUnmanaged((int)ValueMarkCode.NULL_OBJECT);");
					Code.AppendLine("						return;");
					Code.AppendLine("					}");
				}
			}
			if (fieldSymbols != null)
			{
				Code.AppendLine($"					self.WriteUnmanaged({membersCount});");
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
					Code.AppendLine($"				self.WriteValue(typeof({baseName}), value, 0);");
				}
			}
			Code.AppendLine("			}");
			Code.AppendLine("		}");
		}

		private static void GeneratorDeserialize(StringBuilder Code, INamedTypeSymbol classSymbol, List<ISymbol>? fieldSymbols, bool isAbstract, string baseName, int membersCount)
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


			if (!isAbstract)
			{
				Code.AppendLine("				if (count < 0)");
				Code.AppendLine("				{");
				Code.AppendLine("					self.ReadBack(4);");
				Code.AppendLine("					self.SkipData(dataType);");
				Code.AppendLine("					return;");
				Code.AppendLine("				}");
				if (classSymbol.TypeKind == TypeKind.Class)
				{
					if (NamedSymbolHelper.CheckInterface(classSymbol, GeneratorHelper.INode, out _))
					{
						Code.AppendLine($"				if (value is not {className} obj)value = obj = self.Core.PoolGetNode<{className}>();");
					}
					else if (NamedSymbolHelper.CheckInterface(classSymbol, GeneratorHelper.IUnit, out _))
					{
						Code.AppendLine($"				if (value is not {className} obj)value = obj = self.Core.PoolGetUnit<{className}>();");
					}
					else
					{
						Code.AppendLine($"				if (value is not {className} obj)value = obj = new {className}();");
					}
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

			if (!isAbstract)
			{
				if (NamedSymbolHelper.CheckInterface(classSymbol, GeneratorHelper.ISerializable, out _))
					Code.AppendLine($"				obj.OnDeserialize();");
			}

			Code.AppendLine("			}");

			if (fieldSymbols != null)
			{
				Code.AppendLine("			/// <summary>");
				Code.AppendLine("			/// 字段读取");
				Code.AppendLine("			/// </summary>");
				Code.AppendLine($"			private static void SwitchRead(TreeDataByteSequence self, ref object value, int nameCode)");
				Code.AppendLine("			{");
				Code.AppendLine($"				if (value is not {className} obj) return;");
				Code.AppendLine("				switch (nameCode)");
				Code.AppendLine("				{");

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
			}
			Code.AppendLine("		}");

		}


		private static void ClassGenerator(StringBuilder Code, INamedTypeSymbol typeNamedTypeSymbol, out bool isAbstract)
		{
			isAbstract = false;
			string className = typeNamedTypeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			if (typeNamedTypeSymbol.TypeKind == TypeKind.Interface)
			{
				isAbstract = true;
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
					isAbstract = true;
					Code.AppendLine($"	public abstract partial class {className}");
				}
				else
				{
					Code.AppendLine($"	public partial class {className}");
				}
			}
		}



		/// <summary>
		/// 获取类型所有符合条件的字段和属性成员，遇到指定基类停止搜索
		/// </summary>
		public static List<ISymbol> GetAllMembers(INamedTypeSymbol classSymbol, Dictionary<INamedTypeSymbol, int> ignoreBaseSymbols, out INamedTypeSymbol? baseSymbol, out int membersCount)
		{
			int baseMembersCount = 0;
			INamedTypeSymbol? baseTypeSymbol = null;

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
				}

				return false;
			}

			var members = new List<ISymbol>();
			var processedTypes = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);

			bool CollectMembers(INamedTypeSymbol type)
			{
				// 避免重复处理
				if (!processedTypes.Add(type))
					return false;

				// 检查是否达到了需要忽略的基类
				if (ignoreBaseSymbols != null)
				{
					foreach (var ignoreType in ignoreBaseSymbols)
					{
						if (IsTypeSymbolEqual(type, ignoreType.Key))
						{
							baseMembersCount = ignoreType.Value;
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

			membersCount = members.Count + baseMembersCount;
			baseSymbol = baseTypeSymbol;
			return members;
		}

		/// <summary>
		/// 比较两个类型是否相等（包括泛型参数）
		/// </summary>
		private static bool IsTypeSymbolEqual(INamedTypeSymbol type1, INamedTypeSymbol type2)
		{
			if (SymbolEqualityComparer.Default.Equals(type1.OriginalDefinition, type2.OriginalDefinition))
			{
				// 检查泛型参数数量
				if (type1.TypeArguments.Length == type2.TypeArguments.Length)
				{
					// 比较每个泛型参数
					for (int i = 0; i < type1.TypeArguments.Length; i++)
					{
						var arg1 = type1.TypeArguments[i];
						var arg2 = type2.TypeArguments[i];

						// 如果是类型参数（如 T, T1 等）
						if (arg1 is ITypeParameterSymbol param1 && arg2 is ITypeParameterSymbol param2)
						{
							// 这里可以选择合适的比较方式，这里使用最宽松的比较
							continue;
						}
						// 如果是具体类型，直接比较
						else if (!SymbolEqualityComparer.Default.Equals(arg1, arg2))
						{
							return false;
						}
					}
					return true;
				}
			}
			return false;
		}


	}
}
