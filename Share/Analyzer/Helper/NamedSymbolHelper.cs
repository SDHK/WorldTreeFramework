﻿/****************************************

* 作者：闪电黑客
* 日期：2024/6/25 14:36

* 描述：

*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;

namespace WorldTree
{
	/// <summary>
	/// 命名符号帮助类
	/// </summary>
	internal static class NamedSymbolHelper
	{

		[Flags]
		public enum TypeCompareOptions
		{
			None = 0,
			/// <summary>
			/// 忽略类型参数名称
			/// </summary>
			IgnoreTypeParameterNames = 1,
			/// <summary>
			/// 忽略类型参数序号
			/// </summary>
			IgnoreTypeParameterOrdinal = 2,
			/// <summary>
			/// 严格比较类型参数
			/// </summary>
			StrictTypeParameterComparison = 4
		}


		/// <summary>
		/// 获取继承了指定基类或接口的所有子类
		/// </summary>
		/// <param name="compilation">编译类</param>
		/// <param name="baseTypeName">基类或接口的全名</param>
		/// <returns>继承了指定基类或接口的所有子类</returns>
		public static IEnumerable<INamedTypeSymbol> GetDerivedTypes(this Compilation compilation, string baseTypeName)
		{
			INamedTypeSymbol? baseTypeSymbol = compilation.GetTypeByMetadataName(baseTypeName);
			return compilation.GetDerivedTypes(baseTypeSymbol);
		}

		/// <summary>
		/// 获取继承了指定基类或接口的所有子类
		/// </summary>
		/// <param name="compilation">编译类</param>
		/// <param name="baseTypeDeclaration">基类或接口</param>
		/// <returns>继承了指定基类或接口的所有子类</returns>
		public static IEnumerable<INamedTypeSymbol> GetDerivedTypes(this Compilation compilation, TypeDeclarationSyntax baseTypeDeclaration)
		{
			var semanticModel = compilation.GetSemanticModel(baseTypeDeclaration.SyntaxTree);
			return compilation.GetDerivedTypes(semanticModel.GetDeclaredSymbol(baseTypeDeclaration));
		}

		/// <summary>
		/// 获取继承了指定基类或接口的所有子类
		/// </summary>
		/// <param name="compilation">编译类</param>
		/// <param name="baseTypeSymbol">基类或接口</param>
		/// <returns>继承了指定基类或接口的所有子类</returns>
		public static IEnumerable<INamedTypeSymbol> GetDerivedTypes(this Compilation compilation, INamedTypeSymbol? baseTypeSymbol)
		{
			if (baseTypeSymbol == null)
			{
				return Enumerable.Empty<INamedTypeSymbol>();
			}

			return compilation.GetSymbolsWithName(
				name => true, // 获取所有命名类型符号
				SymbolFilter.Type
			).OfType<INamedTypeSymbol>()
			 .Where(type => IsDerivedFrom(type, baseTypeSymbol));
		}


		/// <summary>
		/// 获取同程序集下的派生类型
		/// </summary>
		/// <param name="baseTypeSymbol">基类或接口</param>
		/// <param name="compilation">编译类</param>
		/// <returns>同程序集下的派生类型</returns>
		public static List<TypeDeclarationSyntax> GetDerivedTypes(INamedTypeSymbol baseTypeSymbol, Compilation compilation)
		{
			var derivedTypes = new List<TypeDeclarationSyntax>();

			if (baseTypeSymbol == null)
			{
				return derivedTypes;
			}

			foreach (var syntaxTree in compilation.SyntaxTrees)
			{
				SemanticModel model = compilation.GetSemanticModel(syntaxTree);
				SyntaxNode root = syntaxTree.GetRoot();

				var typeDeclarations = root.DescendantNodes().OfType<TypeDeclarationSyntax>();
				foreach (var typeDeclaration in typeDeclarations)
				{
					var typeSymbol = model.GetDeclaredSymbol(typeDeclaration) as INamedTypeSymbol;
					if (typeSymbol != null && IsDerivedFrom(typeSymbol, baseTypeSymbol))
					{
						derivedTypes.Add(typeDeclaration);
					}
				}
			}
			return derivedTypes;
		}


		/// <summary>
		/// 检查类型是否相等
		/// </summary>
		public static bool IsTypeSymbolEqual(INamedTypeSymbol type1, INamedTypeSymbol type2, TypeCompareOptions options = TypeCompareOptions.None)
		{
			if (SymbolEqualityComparer.Default.Equals(type1.OriginalDefinition, type2.OriginalDefinition))
			{
				if (type1.TypeArguments.Length == type2.TypeArguments.Length)
				{
					for (int i = 0; i < type1.TypeArguments.Length; i++)
					{
						var arg1 = type1.TypeArguments[i];
						var arg2 = type2.TypeArguments[i];

						if (arg1 is ITypeParameterSymbol param1 && arg2 is ITypeParameterSymbol param2)
						{
							// 严格比较
							if (options.HasFlag(TypeCompareOptions.StrictTypeParameterComparison))
							{
								if (!SymbolEqualityComparer.Default.Equals(param1, param2))
									return false;
							}
							// 忽略名称
							else if (!options.HasFlag(TypeCompareOptions.IgnoreTypeParameterNames))
							{
								if (param1.Name != param2.Name)
									return false;
							}
							// 检查序号
							else if (!options.HasFlag(TypeCompareOptions.IgnoreTypeParameterOrdinal))
							{
								if (param1.Ordinal != param2.Ordinal)
									return false;
							}
						}
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


		/// <summary>
		/// 检查类型是否派生自指定基类或接口
		/// </summary>
		public static bool IsDerivedFrom(INamedTypeSymbol type, INamedTypeSymbol baseType)
		{
			// 排除基类本身
			if (SymbolEqualityComparer.Default.Equals(type, baseType)) return false;

			// 接口判断
			if (baseType.TypeKind == TypeKind.Interface)
				return type.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, baseType));

			// 遍历基类
			var currentType = type;
			while (currentType != null)
			{
				if (SymbolEqualityComparer.Default.Equals(currentType, baseType))
				{
					return true;
				}
				currentType = currentType.BaseType;
			}
			return false;
		}

		/// <summary>
		/// 获取所有成员
		/// </summary>
		public static List<ISymbol> GetAllMembers(INamedTypeSymbol classSymbol)
		{
			var members = new List<ISymbol>(classSymbol.GetMembers());

			// 获取父类的成员
			var baseType = classSymbol.BaseType;
			while (baseType != null)
			{
				foreach (var member in baseType.GetMembers())
				{
					// 检查是否已经存在同名成员
					if (!members.Any(m => m.Name == member.Name && m.Kind == member.Kind))
					{
						members.Add(member);
					}
				}
				baseType = baseType.BaseType;
			}
			return members;
		}


		/// <summary>
		/// 获取字段类型
		/// </summary>
		public static ITypeSymbol? ToITypeSymbol(this Compilation compilation, FieldDeclarationSyntax fieldDeclaration)
		{
			return compilation.GetSemanticModel(fieldDeclaration.SyntaxTree).GetTypeInfo(fieldDeclaration.Declaration.Type).Type;
		}



		/// <summary>
		/// 将类声明语法转换为命名类型符号
		/// </summary>
		/// <param name="typeDecl">类声明语法</param>
		/// <param name="compilation">编译类</param>
		public static INamedTypeSymbol? ToINamedTypeSymbol(this Compilation compilation, TypeDeclarationSyntax typeDecl)
		{
			return compilation.GetSemanticModel(typeDecl.SyntaxTree).GetDeclaredSymbol(typeDecl) as INamedTypeSymbol;
		}

		/// <summary>
		/// 判断字段是否为未托管类型
		/// </summary>
		public static bool IsFieldUnmanaged(ITypeSymbol typeInfo)
		{
			// 检查类型是否是未托管类型
			if (typeInfo is ITypeParameterSymbol typeParameterSymbol)
			{
				return typeParameterSymbol.ConstraintTypes.Any(constraint => constraint.SpecialType == SpecialType.System_ValueType);
			}
			return false;
		}

		/// <summary>
		/// 检测是否有指定特性
		/// </summary>
		public static bool CheckAttribute(ISymbol fieldSymbol, string attributeName)
		{
			return fieldSymbol.GetAttributes().Any(attr => attr.AttributeClass?.ToDisplayString().Contains(attributeName) == true);
		}



		/// <summary>
		/// 类型名称转换为命名类型符号
		/// </summary>
		/// <param name="compilation">编译类</param>
		/// <param name="typeFullName">带命名空间全名</param>
		public static INamedTypeSymbol? ToINamedTypeSymbol(this Compilation compilation, string typeFullName)
		{
			return compilation.GetTypeByMetadataName(typeFullName);
		}

		/// <summary>
		/// 检查类是否继承了指定接口
		/// </summary>
		/// <param name="namedTypeSymbol">命名符号</param>
		/// <param name="interfaceSymbol">接口</param>
		public static bool CheckAllInterface(this INamedTypeSymbol namedTypeSymbol, INamedTypeSymbol interfaceSymbol)
		{
			return namedTypeSymbol.AllInterfaces.Contains(interfaceSymbol);
		}

		/// <summary>
		/// 检查类 声明时自身写的，是否含有指定接口
		/// </summary>
		/// <param name="namedTypeSymbol">命名符号</param>
		/// <param name="interfaceSymbol">接口</param>
		public static bool CheckSelfInterface(this INamedTypeSymbol namedTypeSymbol, INamedTypeSymbol interfaceSymbol)
		{
			return namedTypeSymbol.Interfaces.Contains(interfaceSymbol);
		}

		/// <summary>
		/// 迭代检查类及其基类(包括抽象类)是否继承了指定接口,如果是泛型，名称需一一对应
		/// </summary>
		/// <param name="typeSymbol">类声明语法</param>
		/// <param name="interfaceSymbol">接口名称(包括命名空间)</param>
		/// <param name="CheckSelf">是否检查类型自身</param>
		public static bool CheckBaseExtendInterface(this INamedTypeSymbol typeSymbol, INamedTypeSymbol interfaceSymbol, bool CheckSelf = true)
		{
			// 使用队列存储需要检查的类型符号
			Queue<INamedTypeSymbol> typeSymbolQueue = new Queue<INamedTypeSymbol>();
			typeSymbolQueue.Enqueue(typeSymbol);

			while (typeSymbolQueue.Count != 0)
			{
				INamedTypeSymbol currentTypeSymbol = typeSymbolQueue.Dequeue();
				if (!CheckSelf && (currentTypeSymbol == typeSymbol)) continue;

				// 检查当前类型符号是否实现了接口
				if (currentTypeSymbol.AllInterfaces.Contains(interfaceSymbol)) return true;

				// 将基类符号加入队列
				INamedTypeSymbol? baseTypeSymbol = currentTypeSymbol.BaseType;
				if (baseTypeSymbol != null) typeSymbolQueue.Enqueue(baseTypeSymbol);
			}

			return false;
		}

		/// <summary>
		/// 检测是否接口实现 的 属性或方法节点
		/// </summary>
		public static bool CheckInterfaceImplements(ISymbol methodSymbol)
		{
			return methodSymbol.ContainingType.AllInterfaces.Any(@interface =>
			{
				var interfaceMembers = @interface.GetMembers();
				return interfaceMembers.Any(member =>
				{
					var methodImplementingInterfaceMember = methodSymbol.ContainingType.FindImplementationForInterfaceMember(member);
					return methodImplementingInterfaceMember?.Equals(methodSymbol, SymbolEqualityComparer.Default) ?? false;
				});
			});
		}

		/// <summary>
		/// 检测是否继承接口
		/// </summary>
		public static bool CheckInterface(ITypeSymbol typeSymbol, ITypeSymbol interfaceSymbol)
		{
			foreach (var implementedInterface in typeSymbol.AllInterfaces)
			{
				// 检查当前接口是否与目标接口相同，或其原始定义是否与目标接口的原始定义相同
				if (SymbolEqualityComparer.Default.Equals(implementedInterface, interfaceSymbol) ||
					SymbolEqualityComparer.Default.Equals(implementedInterface.OriginalDefinition, interfaceSymbol.OriginalDefinition))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 检测是否继承接口（只对比接口带命名空间名称，不包括泛型）
		/// </summary>
		/// <param name="typeSymbol">子接口</param>
		/// <param name="InterfaceName">接口名称</param>
		/// <param name="Interface">基类接口符号</param>
		/// <returns></returns>
		public static bool CheckInterface(ITypeSymbol typeSymbol, string InterfaceName, out INamedTypeSymbol? Interface)
		{
			Interface = null;
			foreach (var Interfaces in typeSymbol.AllInterfaces)
			{
				//剔除掉泛型参数
				string name = Interfaces.ToDisplayString();
				int index = name.IndexOf('<');
				if (index != -1) name = name.Remove(index);
				if (name != InterfaceName)
				{
					continue;
				}
				Interface = Interfaces;
				return true;
			}
			return false;
		}

		/// <summary>
		/// 检测是否继承接口（只对比接口名称，不包括泛型和命名空间）
		/// </summary>
		/// <param name="typeSymbol">子接口</param>
		/// <param name="InterfaceName">接口名称</param>
		/// <param name="Interface">基类接口符号</param>
		/// <returns></returns>
		public static bool CheckInterfaceName(ITypeSymbol typeSymbol, string InterfaceName, out INamedTypeSymbol? Interface)
		{
			Interface = null;
			foreach (var Interfaces in typeSymbol.AllInterfaces)
			{
				if (Interfaces.Name != InterfaceName)
				{
					continue;
				}
				Interface = Interfaces;
				return true;
			}
			return false;
		}


		///// <summary>
		///// 检测是否继承类型
		///// </summary>
		//public static bool CheckBase(ITypeSymbol typeSymbol, ITypeSymbol baseSymbol)
		//{
		//	var currentBaseType = typeSymbol.BaseType;
		//	while (currentBaseType != null)
		//	{
		//		if (SymbolEqualityComparer.Default.Equals(currentBaseType, baseSymbol) ||
		//			SymbolEqualityComparer.Default.Equals(currentBaseType.OriginalDefinition, baseSymbol.OriginalDefinition))
		//		{
		//			return true;
		//		}
		//		currentBaseType = currentBaseType.BaseType;
		//	}
		//	return false;
		//}

		/// <summary>
		/// 检测是否继承类型，支持泛型检测
		/// </summary>
		public static bool CheckBase(ITypeSymbol typeSymbol, ITypeSymbol baseSymbol)
		{
			// 1. 先检查类型本身
			if (SymbolEqualityComparer.Default.Equals(typeSymbol, baseSymbol) ||
				SymbolEqualityComparer.Default.Equals(typeSymbol.OriginalDefinition, baseSymbol.OriginalDefinition))
			{
				return true;
			}

			// 2. 检查接口实现
			if (baseSymbol.TypeKind == TypeKind.Interface)
			{
				foreach (var @interface in typeSymbol.AllInterfaces)
				{
					if (SymbolEqualityComparer.Default.Equals(@interface, baseSymbol) ||
						SymbolEqualityComparer.Default.Equals(@interface.OriginalDefinition, baseSymbol.OriginalDefinition))
					{
						return true;
					}
				}
			}

			// 3. 检查基类继承
			var currentBaseType = typeSymbol.BaseType;
			while (currentBaseType != null)
			{
				// 直接比较类型
				if (SymbolEqualityComparer.Default.Equals(currentBaseType, baseSymbol))
				{
					return true;
				}

				// 比较原始定义（处理泛型情况）
				if (SymbolEqualityComparer.Default.Equals(currentBaseType.OriginalDefinition, baseSymbol.OriginalDefinition))
				{
					// 如果是泛型类型，可能还需要检查类型参数是否匹配
					if (baseSymbol is INamedTypeSymbol baseNamedType &&
						currentBaseType is INamedTypeSymbol currentNamedType)
					{
						// 如果不需要严格匹配泛型参数，直接返回true
						return true;

						// 如果需要严格匹配泛型参数，可以添加以下检查：
						/*
						if (baseNamedType.TypeArguments.Length == currentNamedType.TypeArguments.Length)
						{
							bool allTypeArgumentsMatch = true;
							for (int i = 0; i < baseNamedType.TypeArguments.Length; i++)
							{
								if (!SymbolEqualityComparer.Default.Equals(
									baseNamedType.TypeArguments[i], 
									currentNamedType.TypeArguments[i]))
								{
									allTypeArgumentsMatch = false;
									break;
								}
							}
							if (allTypeArgumentsMatch)
							{
								return true;
							}
						}
						*/
					}
					else
					{
						return true;
					}
				}

				currentBaseType = currentBaseType.BaseType;
			}

			return false;
		}

		/// <summary>
		/// 检测是否继承类型 （只对比名称，不包括泛型）
		/// </summary>
		/// <param name="typeSymbol">子类</param>
		/// <param name="BaseName">父类全名称</param>
		/// <param name="baseSymbol">基类符号</param>
		public static bool CheckBase(ITypeSymbol typeSymbol, string BaseName, out ITypeSymbol? baseSymbol)
		{
			baseSymbol = null;
			var currentBaseType = typeSymbol;
			while (currentBaseType != null)
			{
				if (currentBaseType.Name == BaseName)
				{
					baseSymbol = currentBaseType;
					return true;
				}
				currentBaseType = currentBaseType.BaseType;
			}
			return false;
		}



		/// <summary>
		/// 获取法则参数泛型类型注释，例：
		/// <para>T1 : <see cref="float"/>, OutT : <see cref="int"/></para>
		/// </summary>
		public static string GetRuleParametersTypeCommentPara(INamedTypeSymbol typeSymbol, string tab)
		{
			if (!typeSymbol.IsGenericType) return "";
			StringBuilder sb = new StringBuilder();
			StringBuilder sbType = new StringBuilder();

			sb.AppendLine($"{tab}/// <para>");
			sb.Append($"{tab}/// {typeSymbol.Name}");
			int index = 1;
			for (int i = 0; i < typeSymbol.TypeArguments.Length; i++)
			{
				string name = typeSymbol.TypeArguments[i].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

				//不是泛型
				if (typeSymbol.TypeArguments[i].TypeKind != TypeKind.TypeParameter)
				{
					sbType.Append($", <see cref=\"{name}\"/>");
				}
				else //是泛型参数
				{
					sbType.Append($", <typeparamref name= \"{name}\"/>");
				}
				index++;
			}
			sb.AppendLine($"&lt;{sbType.ToString().TrimStart(',', ' ')}&gt;");
			sb.AppendLine($"{tab}/// </para>");
			return sb.ToString();
		}




		/// <summary>
		/// 获取所有接口，包括引用的程序集中的接口
		/// </summary>
		/// <param name="compilation"></param>
		/// <returns></returns>
		public static List<INamedTypeSymbol> CollectAllInterfaces(Compilation compilation)
		{
			List<INamedTypeSymbol> allInterfaces = new List<INamedTypeSymbol>();

			// 遍历当前编译上下文中的所有类型
			foreach (var syntaxTree in compilation.SyntaxTrees)
			{
				var semanticModel = compilation.GetSemanticModel(syntaxTree);
				var root = syntaxTree.GetRoot();
				var nodes = new Stack<SyntaxNode>(root.ChildNodes());

				while (nodes.Count > 0)
				{
					var node = nodes.Pop();

					if (node is InterfaceDeclarationSyntax interfaceDeclaration)
					{
						var interfaceSymbol = semanticModel.GetDeclaredSymbol(interfaceDeclaration);
						if (interfaceSymbol != null)
						{
							allInterfaces.Add(interfaceSymbol);
						}
					}

					foreach (var child in node.ChildNodes())
					{
						nodes.Push(child);
					}
				}
			}

			// 遍历引用的程序集中的所有类型
			foreach (var referencedAssembly in compilation.References)
			{
				var assemblySymbol = compilation.GetAssemblyOrModuleSymbol(referencedAssembly) as IAssemblySymbol;
				if (assemblySymbol != null)
				{
					var typesInAssembly = GetAllTypes(assemblySymbol.GlobalNamespace);
					var interfacesInAssembly = typesInAssembly.Where(t => t.TypeKind == TypeKind.Interface);
					allInterfaces.AddRange(interfacesInAssembly);
				}
			}

			return allInterfaces;
		}

		/// <summary>
		/// 获取命名空间下的所有类型
		/// </summary>
		public static IEnumerable<INamedTypeSymbol> GetAllTypes(INamespaceSymbol namespaceSymbol)
		{
			foreach (var memberSymbol in namespaceSymbol.GetMembers())
			{
				if (memberSymbol is INamedTypeSymbol typeSymbol)
				{
					yield return typeSymbol;
				}
				else if (memberSymbol is INamespaceSymbol nestedNamespaceSymbol)
				{
					foreach (var nestedTypeSymbol in GetAllTypes(nestedNamespaceSymbol))
					{
						yield return nestedTypeSymbol;
					}
				}
			}
		}



		/// <summary>
		/// 源码获取(能获取到但是会导致生成的代码文件无法被项目收集编译)
		/// </summary>
		/// <param name="typeSymbol">命名符号</param>
		public static string GetTypeSourceCode(this INamedTypeSymbol typeSymbol)
		{
			// 检查 typeSymbol 是否为 null
			if (typeSymbol == null)
			{
				return null;
			}

			// 获取类型声明的语法参考
			var declaringSyntaxReferences = typeSymbol.DeclaringSyntaxReferences;

			// 检查 declaringSyntaxReferences 是否为空
			if (declaringSyntaxReferences.Length == 0)
			{
				return null;
			}

			// 获取第一个语法参考
			var syntaxReference = declaringSyntaxReferences.First();

			// 获取语法树
			SyntaxTree? syntaxTree = syntaxReference.SyntaxTree;

			// 检查语法树是否为 null
			if (syntaxTree == null)
			{
				return null;
			}

			// 获取类型声明的语法节点
			SyntaxNode? typeNode = syntaxReference.GetSyntax();

			// 检查语法节点是否为 null
			if (typeNode == null)
			{
				return null;
			}

			// 获取源代码字符串
			return typeNode.ToFullString();
		}
	}
}