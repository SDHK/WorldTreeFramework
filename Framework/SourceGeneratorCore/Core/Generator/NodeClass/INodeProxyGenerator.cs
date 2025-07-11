/****************************************

* 作者：闪电黑客
* 日期：2024/4/16 17:05

* 描述：Node复制兄弟类 生成器

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WorldTree.SourceGenerator
{
	/// <summary>
	/// INode接口代理生成器
	/// </summary>
	public abstract class INodeProxyGenerator<C> : SourceGeneratorBase<C>
		where C : ProjectGeneratorsConfig, new()
	{
		public override void Initialize(GeneratorInitializationContext context)
		{
			context.RegisterForSyntaxNotifications(() => new FindPartialINodeSubSyntaxReceiver());
		}

		public override void ExecuteCore(GeneratorExecutionContext context)
		{
			if (!(context.SyntaxReceiver is FindPartialINodeSubSyntaxReceiver receiver)) return;
			if (receiver.ClassDeclarations.Count == 0) return;

			foreach (ClassDeclarationSyntax CopyClassDeclarations in receiver.ClassDeclarations)
			{
				// 检查是否实现了特性INodePorxy
				if (!TreeSyntaxHelper.CheckAttribute(CopyClassDeclarations, GeneratorHelper.INodeProxyAttribute)) continue;
				string fileName = Path.GetFileNameWithoutExtension(CopyClassDeclarations.SyntaxTree.FilePath);
				// 获取修饰符（如 public、partial 等）
				string modifiers = CopyClassDeclarations.Modifiers.ToFullString().Trim();
				// 获取类名
				string className = CopyClassDeclarations.Identifier.Text;
				// 获取泛型参数（如 <T>），如果有
				string typeParameters = CopyClassDeclarations.TypeParameterList?.ToFullString() ?? "";
				// 拼接
				string classDeclaration = $"{modifiers} class {className}{typeParameters}";

				INamedTypeSymbol INodeSymbol = NamedSymbolHelper.ToINamedTypeSymbol(context.Compilation, GeneratorHelper.INode);

				// 获取CopyClassDeclarations中已实现的方法名称集合
				var implementedMethods = GetImplementedMethodNames(CopyClassDeclarations, context.Compilation);
				// 获取INode及其所有继承接口的方法
				GetAllInterfaceMembers(INodeSymbol, out IEnumerable<IMethodSymbol> allMethods, out IEnumerable<IPropertySymbol> properties);
				// 获取CopyClassDeclarations中已实现的属性名称集合
				var implementedProperties = GetImplementedPropertyNames(CopyClassDeclarations, context.Compilation);

				// 声明 StringBuilder 用于生成代码
				var sb = new StringBuilder();

				foreach (var propertie in properties)
				{
					// 检查当前属性是否已在CopyClassDeclarations中实现
					if (IsPropertyImplemented(propertie, implementedProperties))
					{
						continue; // 跳过已实现的属性
					}

					var propertyType = propertie.Type.ToDisplayString();
					var propertyName = propertie.Name;

					// 获取属性的特性标记
					var attributes = new List<string>();
					foreach (var attr in propertie.GetAttributes())
					{
						var attrName = attr.AttributeClass?.Name;
						if (attrName != null)
						{
							if (attrName.EndsWith("Attribute"))
								attrName = attrName.Substring(0, attrName.Length - 9);

							// 处理构造函数参数
							var constructorArguments = attr.ConstructorArguments
								.Select(arg => arg.ToCSharpString())
								.ToArray();

							if (constructorArguments.Length > 0)
							{
								attributes.Add($"[{attrName}({string.Join(", ", constructorArguments)})]");
							}
							else
							{
								attributes.Add($"[{attrName}]");
							}
						}
					}

					// 生成特性标记
					foreach (var attr in attributes)
					{
						sb.AppendLine($"		{attr}");
					}

					// 构建访问器
					var accessors = new List<string>();
					if (propertie.GetMethod != null)
					{
						accessors.Add("get;");
					}
					if (propertie.SetMethod != null)
					{
						accessors.Add("set;");
					}

					string accessorString = accessors.Count > 0 ? $" {{ {string.Join(" ", accessors)} }}" : " { get; set; }";

					// 直接复制属性定义
					sb.AppendLine($"		public {propertyType} {propertyName}{accessorString}\n");
				}


				foreach (var method in allMethods)
				{
					// 检查当前方法是否已在CopyClassDeclarations中实现
					if (IsMethodImplemented(method, implementedMethods))
					{
						continue; // 跳过已实现的方法
					}
					var returnType = method.ReturnType.ToDisplayString();
					var methodName = method.Name;
					var parameters = string.Join(", ", method.Parameters.Select(p => $"{p.Type.ToDisplayString()} {p.Name}"));
					var parameterNames = string.Join(", ", method.Parameters.Select(p => p.Name));
					// 获取泛型参数
					string methodTypeParameters = "";
					if (method.TypeParameters.Length > 0)
					{
						methodTypeParameters = "<" + string.Join(", ", method.TypeParameters.Select(tp => tp.Name)) + ">";
					}

					// 获取where约束
					string whereConstraints = "";
					if (method.TypeParameters.Length > 0)
					{
						var constraints = new List<string>();
						foreach (var typeParam in method.TypeParameters)
						{
							var constraintList = new List<string>();

							// 处理class/struct约束
							if (typeParam.HasReferenceTypeConstraint)
								constraintList.Add("class");
							if (typeParam.HasValueTypeConstraint)
								constraintList.Add("struct");
							if (typeParam.HasUnmanagedTypeConstraint)
								constraintList.Add("unmanaged");
							if (typeParam.HasNotNullConstraint)
								constraintList.Add("notnull");

							// 处理类型约束
							foreach (var constraintType in typeParam.ConstraintTypes)
							{
								constraintList.Add(constraintType.ToDisplayString());
							}

							// 处理new()约束
							if (typeParam.HasConstructorConstraint)
								constraintList.Add("new()");

							if (constraintList.Count > 0)
							{
								constraints.Add($"where {typeParam.Name} : {string.Join(", ", constraintList)}");
							}
						}

						if (constraints.Count > 0)
						{
							whereConstraints = " " + string.Join(" ", constraints);
						}
					}

					sb.AppendLine($"		public {returnType} {methodName}{methodTypeParameters}({parameters}){whereConstraints} => INodeProxyRule.{methodName}{methodTypeParameters}(this{(parameterNames.Length > 0 ? ", " + parameterNames : "")});\n");
				}


				StringBuilder Code = new StringBuilder();
				string Namespace = TreeSyntaxHelper.GetNamespace(CopyClassDeclarations);
				string Usings = TreeSyntaxHelper.GetUsings(CopyClassDeclarations);
				Code.AppendLine(
@$"/****************************************
* {className}对INode接口的代理实现
*/
"
);
				Code.AppendLine(Usings);
				Code.AppendLine($"namespace {Namespace}");
				Code.AppendLine("{");
				Code.AppendLine($"	{classDeclaration}");
				Code.AppendLine("	{");
				Code.Append(sb);
				Code.AppendLine("	}");
				Code.Append("}");

				context.AddSource($"{fileName}_INodeProxy.cs", SourceText.From(Code.ToString(), Encoding.UTF8));//生成代码

			}
		}

		/// <summary>
		/// 获取类中已实现的方法符号信息
		/// </summary>
		private HashSet<IMethodSymbol> GetImplementedMethodNames(ClassDeclarationSyntax classDeclaration, Compilation compilation)
		{
			var implementedMethods = new HashSet<IMethodSymbol>(SymbolEqualityComparer.Default);

			var semanticModel = compilation.GetSemanticModel(classDeclaration.SyntaxTree);
			var classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration);

			if (classSymbol != null)
			{
				foreach (var member in classSymbol.GetMembers())
				{
					if (member is IMethodSymbol method && method.MethodKind == MethodKind.Ordinary)
					{
						implementedMethods.Add(method);
					}
				}
			}

			return implementedMethods;
		}

		/// <summary>
		/// 检查方法是否已在类中实现
		/// </summary>
		private bool IsMethodImplemented(IMethodSymbol method, HashSet<IMethodSymbol> implementedMethods)
		{
			// 使用Symbol比较，检查方法名、参数类型和泛型参数
			return implementedMethods.Any(impl =>
				impl.Name == method.Name &&
				impl.TypeParameters.Length == method.TypeParameters.Length &&
				impl.Parameters.Length == method.Parameters.Length &&
				impl.Parameters.Zip(method.Parameters, (implParam, methodParam) =>
					SymbolEqualityComparer.Default.Equals(implParam.Type, methodParam.Type)).All(x => x));
		}
		/// <summary>
		/// 构建IMethodSymbol的方法签名用于比较
		/// </summary>
		private string BuildMethodSignature(IMethodSymbol method)
		{
			var methodName = method.Name;
			var typeParameterCount = method.TypeParameters.Length;
			var parameterTypes = method.Parameters
				.Select(p => p.Type.ToDisplayString())
				.ToList();

			return $"{methodName}|{typeParameterCount}|{string.Join(",", parameterTypes)}";
		}

		/// <summary>
		/// 构建属性签名用于比较
		/// </summary>
		private string BuildPropertySignature(IPropertySymbol property)
		{
			var propertyName = property.Name;
			var propertyType = property.Type.ToDisplayString();
			return $"{propertyName}|{propertyType}";
		}

		/// <summary>
		/// 获取类中已实现的属性名称和符号信息
		/// </summary>
		private HashSet<IPropertySymbol> GetImplementedPropertyNames(ClassDeclarationSyntax classDeclaration, Compilation compilation)
		{
			var implementedProperties = new HashSet<IPropertySymbol>(SymbolEqualityComparer.Default);

			var semanticModel = compilation.GetSemanticModel(classDeclaration.SyntaxTree);
			var classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration);

			if (classSymbol != null)
			{
				foreach (var member in classSymbol.GetMembers())
				{
					if (member is IPropertySymbol property)
					{
						implementedProperties.Add(property);
					}
				}
			}

			return implementedProperties;
		}

		/// <summary>
		/// 检查属性是否已在类中实现
		/// </summary>
		private bool IsPropertyImplemented(IPropertySymbol property, HashSet<IPropertySymbol> implementedProperties)
		{
			// 使用Symbol比较，这样可以准确识别相同的属性，即使命名空间写法不同
			return implementedProperties.Any(impl =>
				SymbolEqualityComparer.Default.Equals(impl.Type, property.Type) &&
				impl.Name == property.Name);
		}


		/// <summary>
		/// 获取接口及其所有继承接口的成员（方法和属性）
		/// </summary>
		private void GetAllInterfaceMembers(INamedTypeSymbol interfaceSymbol, out IEnumerable<IMethodSymbol> methods, out IEnumerable<IPropertySymbol> properties)
		{
			var allMethods = new List<IMethodSymbol>();
			var allProperties = new List<IPropertySymbol>();
			var visitedInterfaces = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);

			GetMembersRecursively(interfaceSymbol, allMethods, allProperties, visitedInterfaces);

			// 去重，按方法签名
			var uniqueMethods = new Dictionary<string, IMethodSymbol>();
			foreach (var method in allMethods)
			{
				var signature = BuildMethodSignature(method);
				if (!uniqueMethods.ContainsKey(signature))
				{
					uniqueMethods[signature] = method;
				}
			}

			// 去重，按属性签名
			var uniqueProperties = new Dictionary<string, IPropertySymbol>();
			foreach (var property in allProperties)
			{
				var signature = BuildPropertySignature(property);
				if (!uniqueProperties.ContainsKey(signature))
				{
					uniqueProperties[signature] = property;
				}
			}

			methods = uniqueMethods.Values;
			properties = uniqueProperties.Values;
		}

		/// <summary>
		/// 递归获取接口及其继承接口的所有成员（方法和属性）
		/// </summary>
		private void GetMembersRecursively(INamedTypeSymbol interfaceSymbol, List<IMethodSymbol> allMethods, List<IPropertySymbol> allProperties, HashSet<INamedTypeSymbol> visitedInterfaces)
		{
			if (visitedInterfaces.Contains(interfaceSymbol))
				return;

			visitedInterfaces.Add(interfaceSymbol);

			// 添加当前接口的成员
			foreach (var member in interfaceSymbol.GetMembers())
			{
				if (member is IMethodSymbol method && method.MethodKind == MethodKind.Ordinary)
				{
					allMethods.Add(method);
				}
				else if (member is IPropertySymbol property)
				{
					allProperties.Add(property);
				}
			}

			// 递归处理继承的接口
			foreach (var baseInterface in interfaceSymbol.Interfaces)
			{
				GetMembersRecursively(baseInterface, allMethods, allProperties, visitedInterfaces);
			}
		}

	}
}