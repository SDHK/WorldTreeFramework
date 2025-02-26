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
			INamedTypeSymbol? classSymbol = context.Compilation.ToINamedTypeSymbol(typeDeclaration);

			INamedTypeSymbol? baseSymbol = null;
			int membersCount = 0;

			// 获取类的完整名称，包括泛型参数
			ClassGenerator(Code, classSymbol, out bool isAbstract, out bool isClass);

			List<ISymbol>? fieldSymbols = null;
			if (!isAbstract) fieldSymbols = GetAllMembers(classSymbol, TreeDataSerializeGenerator.TypeFieldsCountDict, out baseSymbol, out membersCount);
			string? baseName = baseSymbol?.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);


			Code.AppendLine("	{");
			GeneratorSerialize(Code, classSymbol, fieldSymbols, isAbstract, baseName, membersCount, isClass);
			GeneratorDeserialize(Code, classSymbol, fieldSymbols, isAbstract, baseName, membersCount);
			Code.AppendLine("	}");
		}

		private static bool GetIsConstantFromAttributes(INamedTypeSymbol classSymbol)
		{
			var attribute = classSymbol.GetAttributes()
				.FirstOrDefault(attr => attr.AttributeClass?.Name == GeneratorHelper.TreeDataSerializableAttribute);

			if (attribute != null)
			{
				// 检查命名参数
				var namedArgument = attribute.NamedArguments
					.FirstOrDefault(arg => arg.Key == "IsConstant" && arg.Value.Value is bool);

				if (namedArgument.Value.Value is bool isConstant)
				{
					return isConstant;
				}

				// 检查位置参数
				if (attribute.ConstructorArguments.Length > 0 && attribute.ConstructorArguments[0].Value is bool positionalArgument)
				{
					return positionalArgument;
				}
			}

			return false;
		}


		private static string GetEnumUnderlyingType(ITypeSymbol enumType)
		{
			if (enumType is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.EnumUnderlyingType != null)
			{
				return namedTypeSymbol.EnumUnderlyingType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			}
			return "int"; // 默认返回 int 类型
		}

		private static void GeneratorSerialize(StringBuilder Code, INamedTypeSymbol classSymbol, List<ISymbol>? fieldSymbols, bool isAbstract, string baseName, int membersCount, bool isClass)
		{

			bool isConstant = GetIsConstantFromAttributes(classSymbol);

			string className = classSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			Code.AppendLine($"		class TreeDataSerialize : TreeDataSerializeRule<{className}>");
			Code.AppendLine("		{");
			Code.AppendLine($"			protected override void Execute(TreeDataByteSequence self, ref object value, ref SerializedTypeMode typeMode)");
			Code.AppendLine("			{");

			Code.AppendLine($"				if (self.TryWriteDataHead(value, typeMode, {membersCount}, out {className} obj, {(isConstant ? "true" : "false")} ,{(isClass ? "true" : "false")})) return;");

			if (NamedSymbolHelper.CheckInterface(classSymbol, GeneratorHelper.ISerializable, out _))
				Code.AppendLine($"				obj?.OnSerialize();");

			if (fieldSymbols != null)
			{
				foreach (ISymbol symbol in fieldSymbols)
				{
					int hash = symbol.Name.GetFNV1aHash32();
					Code.AppendLine($"				self.WriteUnmanaged({hash});");
					// 判断是否是枚举
					if (symbol is IPropertySymbol propertySymbol && propertySymbol.Type.TypeKind == TypeKind.Enum)
					{
						// 获取枚举的基础类型
						string enumUnderlyingType = GetEnumUnderlyingType(propertySymbol.Type);
						Code.AppendLine($"                self.WriteValue(({enumUnderlyingType})obj.{symbol.Name});");
					}
					else if (symbol is IFieldSymbol fieldSymbol && fieldSymbol.Type.TypeKind == TypeKind.Enum)
					{
						string enumUnderlyingType = GetEnumUnderlyingType(fieldSymbol.Type);
						Code.AppendLine($"                self.WriteValue(({enumUnderlyingType})obj.{symbol.Name});");
					}
					else
					{
						Code.AppendLine($"                self.WriteValue(obj.{symbol.Name});");
					}
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
			Code.AppendLine("			protected override void Execute(TreeDataByteSequence self, ref object value, ref int fieldNameCode)");
			Code.AppendLine("			{");
			if (baseName != null)
			{
				Code.AppendLine("				if (fieldNameCode != TreeDataCode.DESERIALIZE_SELF_MODE)");
				Code.AppendLine("				{");
				Code.AppendLine("					SwitchRead(self, ref value, fieldNameCode);");
				Code.AppendLine("					return;");
				Code.AppendLine("				}");
			}

			if (isAbstract)
			{
				Code.AppendLine("				int typePoint = self.ReadPoint;");
			}


			Code.AppendLine($"				if (self.TryReadClassHead(typeof({className}), ref value, out int count, out int objId, out int jumpReadPoint)) return;");
			if (!isAbstract)
			{
				if (classSymbol.TypeKind == TypeKind.Class)//类型新建
				{
					if (NamedSymbolHelper.CheckInterface(classSymbol, GeneratorHelper.INode, out _))
					{
						Code.AppendLine(@$"				if (value is not {className} obj)");
						Code.AppendLine("				{");
						Code.AppendLine($"					value = obj = self.Core.PoolGetNode<{className}>(true);");
						Code.AppendLine("				}");
					}
					else if (NamedSymbolHelper.CheckInterface(classSymbol, GeneratorHelper.IUnit, out _))
					{
						Code.AppendLine($"				if (value is not {className} obj)value = obj = self.Core.PoolGetUnit<{className}>();");
					}
					else
					{
						Code.AppendLine($"				if (value is not {className} obj)value = obj = new {className}();");
					}
					Code.AppendLine(@$"				if (objId != TreeDataCode.NULL_OBJECT) self.IdToObjectDict.Add(objId, value);");
				}
			}
			else //是抽象直接跳过
			{
				Code.AppendLine("				self.ReadJump(typePoint);");
				Code.AppendLine("				self.SkipData();");
			}

			if (fieldSymbols != null && fieldSymbols.Count != 0 || baseName != null)
			{
				Code.AppendLine("				for (int i = 0; i < count; i++)");
				Code.AppendLine("				{");

				Code.AppendLine("					self.ReadUnmanaged(out fieldNameCode);");
				Code.AppendLine("					SwitchRead(self, ref value, fieldNameCode);");
				Code.AppendLine("				}");
			}
			Code.AppendLine("				if (jumpReadPoint != TreeDataCode.NULL_OBJECT) self.ReadJump(jumpReadPoint);");
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
				Code.AppendLine($"			private static void SwitchRead(TreeDataByteSequence self, ref object value, int fieldNameCode)");
				Code.AppendLine("			{");
				Code.AppendLine($"				if (value is not {className} obj) return;");
				Code.AppendLine("				switch (fieldNameCode)");
				Code.AppendLine("				{");

				foreach (ISymbol symbol in fieldSymbols)
				{
					int hash = symbol.Name.GetFNV1aHash32();
					if (symbol is IPropertySymbol propertySymbol)
					{
						string symbolName = propertySymbol.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
						if (propertySymbol.Type.TypeKind == TypeKind.Enum)
						{
							string enumUnderlyingType = GetEnumUnderlyingType(propertySymbol.Type);
							Code.AppendLine($"					case {hash}: obj.{symbol.Name} = ({symbolName})self.ReadValue<{enumUnderlyingType}>(); break;");
						}
						else
						{
							Code.AppendLine($"					case {hash}: obj.{symbol.Name} = self.ReadValue<{symbolName}>(); break;");
						}
					}
					else if (symbol is IFieldSymbol fieldSymbol)
					{
						if (fieldSymbol.Type.TypeKind == TypeKind.Enum)
						{
							string symbolName = fieldSymbol.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
							string enumUnderlyingType = GetEnumUnderlyingType(fieldSymbol.Type);
							Code.AppendLine($"					case {hash}: obj.{symbol.Name} = ({symbolName})self.ReadValue<{enumUnderlyingType}>(); break;");
						}
						else
						{
							Code.AppendLine($"					case {hash}: self.ReadValue(ref obj.{symbol.Name}); break;");
						}
					}
				}

				if (baseName != null)
				{
					Code.AppendLine($"					default: self.ReadValue(typeof({baseName}), ref value, fieldNameCode); break;");
				}
				else
				{
					Code.AppendLine($"					default: self.SkipData(); break;");
				}
				Code.AppendLine("				}");
				if (classSymbol.TypeKind == TypeKind.Struct)
				{
					Code.AppendLine("				value = obj;");
				}
				Code.AppendLine("			}");
			}
			Code.AppendLine("		}");

		}


		private static void ClassGenerator(StringBuilder Code, INamedTypeSymbol typeNamedTypeSymbol, out bool isAbstract, out bool isClass)
		{
			isAbstract = false;
			isClass = false;
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
					isClass = true;
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
						if (NamedSymbolHelper.IsTypeSymbolEqual(type, ignoreType.Key, TypeCompareOptions.CompareToGenericTypeDefinition))
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

	}
}
