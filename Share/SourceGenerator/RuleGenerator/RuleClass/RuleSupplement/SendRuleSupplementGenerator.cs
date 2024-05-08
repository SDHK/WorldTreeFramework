/****************************************

* 作者：闪电黑客
* 日期：2024/4/28 14:22

* 描述：补充法则类型生成器

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Xml.Linq;

namespace WorldTree.SourceGenerator
{
	/// <summary>
	/// 补充法则类型生成器
	/// </summary>
	[Generator]
	internal class SendRuleSupplementGenerator : ISourceGenerator
	{
		public void Initialize(GeneratorInitializationContext context)
		{
			context.RegisterForSyntaxNotifications(() => new FindSubInterfaceSyntaxReceiver());
		}

		public void Execute(GeneratorExecutionContext context)
		{
			if (!(context.SyntaxReceiver is FindSubInterfaceSyntaxReceiver receiver)) return;
			if (receiver.InterfaceDeclarations.Count == 0) return;

			SendRuleClass.Init(context.Compilation);

			foreach (InterfaceDeclarationSyntax interfaceDeclaration in receiver.InterfaceDeclarations)
			{
				//string fileName = Path.GetFileNameWithoutExtension(interfaceDeclaration.SyntaxTree.FilePath);
				//var a = context.Compilation.GetTypeByMetadataName("WorldTree.ISendRuleBase");

				//context.AddSource($"{fileName}Supplement.cs", $"//TEST{SendRuleClass.GetNameWithGenericArguments(context.Compilation.ToINamedTypeSymbol(interfaceDeclaration))}");//生成代码
				//return;

				SendRuleClass.Add(interfaceDeclaration, context.Compilation);
			}

			SendRuleClass.Execute(context);
		}
	}

	public static class SendRuleClass
	{
		//文件名-类集合
		public static Dictionary<string, List<INamedTypeSymbol>> fileClassDict = new();

		public static Dictionary<string, string> fileUsings = new();
		public static Dictionary<string, string> classWheres = new();

		public static string ISendRuleBase = "ISendRuleBase";
		public static string ICallRuleBase = "ICallRuleBase";
		public static string ISendRuleAsyncBase = "ISendRuleAsyncBase";
		public static string ICallRuleAsyncBase = "ICallRuleAsyncBase";

		public static void Init(Compilation compilation)
		{
			fileClassDict.Clear();
			fileUsings.Clear();
			classWheres.Clear();
		}

		public static void Add(InterfaceDeclarationSyntax interfaceDeclaration, Compilation compilation)
		{
			INamedTypeSymbol? namedType = compilation.ToINamedTypeSymbol(interfaceDeclaration);
			if (namedType == null) return;

			//检测是否继承4大法则接口
			if (!(CheckRuleBase(namedType, ISendRuleBase, out _) ||
				CheckRuleBase(namedType, ICallRuleBase, out _) ||
				CheckRuleBase(namedType, ISendRuleAsyncBase, out _) ||
				CheckRuleBase(namedType, ICallRuleAsyncBase, out _))) return;

			string fileName = Path.GetFileNameWithoutExtension(interfaceDeclaration.SyntaxTree.FilePath);
			if (!fileClassDict.TryGetValue(fileName, out List<INamedTypeSymbol> set))
			{
				set = new List<INamedTypeSymbol>();
				fileClassDict.Add(fileName, set);
				fileUsings.Add(fileName, GetUsings(interfaceDeclaration));
			}

			if (set.Any(a => a.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) == namedType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat))) return;
			set.Add(namedType);

			classWheres.Add(namedType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat), GetWhereTypeArguments(interfaceDeclaration));
		}

		public static void Execute(GeneratorExecutionContext context)
		{
			foreach (var fileClassList in fileClassDict)
			{
				StringBuilder ClassCode = new();
				StringBuilder MethodCode = new();

				ClassCode.AppendLine(fileUsings[fileClassList.Key]);
				ClassCode.AppendLine("namespace WorldTree");
				ClassCode.AppendLine("{");

				foreach (INamedTypeSymbol fileClass in fileClassList.Value)
				{
					if (CheckRuleBase(fileClass, ISendRuleBase, out INamedTypeSymbol? baseInterface))
					{
						if (baseInterface == null) continue;
						SendRule(ClassCode, fileClass, baseInterface);
						SendRuleMethod(MethodCode, fileClass, baseInterface);
					}
				}

				ClassCode.AppendLine($"	public static class {fileClassList.Key}Supplement");
				ClassCode.AppendLine("	{");

				ClassCode.AppendLine($"{MethodCode}");

				ClassCode.AppendLine("	}");
				ClassCode.Append("}");

				context.AddSource($"{fileClassList.Key}Supplement.cs", SourceText.From(ClassCode.ToString(), Encoding.UTF8));
			}

			//context.AddSource($"TestSupplement.cs", $"//{fileClassDict.Count} : {SendRuleBaseList.Count}");
		}

		private static void SendRule(StringBuilder Code, INamedTypeSymbol typeSymbol, INamedTypeSymbol baseInterface)
		{
			//获取类名
			string IClassName = typeSymbol.Name;
			string ClassName = IClassName.TrimStart('I');

			//获取类全名
			string TypeArguments = GetTypeArguments(typeSymbol);
			string BaseTypeArguments = GetTypeArguments(baseInterface);
			string IClassFullName = GetNameWithGenericArguments(typeSymbol);
			string ClassFullName = IClassFullName.TrimStart('I');
			string WhereTypeArguments = classWheres[IClassFullName];

			//生成As代码
			Code.AppendLine(@$"	public interface As{ClassFullName} : AsRule<{IClassFullName}>, INode {WhereTypeArguments}{{}}");

			//生成基类代码
			Code.AppendLine(@$"	public abstract class {ClassName}<N{TypeArguments}> : SendRuleBase<N, {IClassFullName}{BaseTypeArguments}> where N : class, INode, AsRule<{IClassFullName}> {WhereTypeArguments}{{}}");
		}

		private static void SendRuleMethod(StringBuilder Code, INamedTypeSymbol typeSymbol, INamedTypeSymbol baseInterface)
		{
			//获取类名
			string IClassName = typeSymbol.Name;
			string ClassName = IClassName.TrimStart('I');
			int baseTypeCount = baseInterface.TypeArguments.Count();

			//获取类全名
			string TypeArgumentsAngle = GetTypeArgumentsAngle(typeSymbol);

			string IClassFullName = GetNameWithGenericArguments(typeSymbol);

			string ClassFullName = IClassFullName.TrimStart('I');

			string genericParameter = RuleGeneratorHelper.GetGenericParameter(baseTypeCount);

			string genericTypeParameter = GetGenericTypeParameter(baseInterface);

			string WhereTypeArguments = classWheres[IClassFullName];

			//生成调用方法
			Code.AppendLine(@$"		public static void {ClassName}{TypeArgumentsAngle}(this As{ClassFullName} self{genericTypeParameter}){WhereTypeArguments} => NodeSendRule.SendRule(self, TypeInfo<{IClassFullName}>.Default{genericParameter});");
		}

		/// <summary>
		/// 获取类型名称包括泛型
		/// </summary>
		public static string GetNameWithGenericArguments(INamedTypeSymbol typeSymbol)
		{
			if (!typeSymbol.IsGenericType) return typeSymbol.Name;
			return typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
		}

		/// <summary>
		/// 获取泛型参数
		/// </summary>
		public static string GetTypeArgumentsAngle(INamedTypeSymbol typeSymbol)
		{
			if (!typeSymbol.IsGenericType) return "";
			return typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat).Replace(typeSymbol.Name, "");
		}

		/// <summary>
		/// 获取泛型参数 , T1, T2
		/// </summary>
		public static string GetTypeArguments(INamedTypeSymbol typeSymbol)
		{
			if (!typeSymbol.IsGenericType) return "";
			string typeArguments = typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat).Replace(typeSymbol.Name, "");
			return ", " + typeArguments.Trim('<', '>');
		}

		/// <summary>
		/// 获取泛型参数 , T1 arg1, T2 arg2
		/// </summary>
		public static string GetGenericTypeParameter(INamedTypeSymbol typeSymbol)
		{
			if (!typeSymbol.IsGenericType) return "";

			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < typeSymbol.TypeArguments.Length; i++)
			{
				sb.Append($", {typeSymbol.TypeArguments[i].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)} arg{i + 1}");
			}
			return sb.ToString();
		}

		/// <summary> 获取泛型参数的Where约束字符串 where T1 : IEquatable<T1> where T2 : IEquatable<T2> </summary>
		public static string GetWhereTypeArguments(InterfaceDeclarationSyntax interfaceDeclaration)
		{
			var typeParameters = interfaceDeclaration.TypeParameterList?.Parameters;
			if (typeParameters == null || !typeParameters.Value.Any()) return "";

			StringBuilder sb = new StringBuilder();
			foreach (var typeParameter in typeParameters)
			{
				var constraints = interfaceDeclaration.ConstraintClauses.FirstOrDefault(c => c.Name.Identifier.Text == typeParameter.Identifier.Text);
				if (constraints != null)
				{
					sb.Append($" where {typeParameter.Identifier.Text} : ");
					var interfaces = constraints.Constraints.Select(c => c.ToString());
					sb.Append(string.Join(", ", interfaces));
				}
			}
			return sb.ToString();
		}

		/// <summary>
		/// 检测是否继承了对应的基类
		/// </summary>
		/// <param name="typeSymbol">子接口</param>
		/// <param name="RuleBases">基类集合</param>
		/// <param name="baseClass">继承的基类</param>
		private static bool CheckRuleBase(INamedTypeSymbol typeSymbol, Dictionary<string, int> RuleBases, out string? baseClass, out int TypeCount)
		{
			baseClass = null;
			TypeCount = 0;
			foreach (var RuleBase in RuleBases)
			{
				foreach (var Interfaces in typeSymbol.AllInterfaces)
				{
					if (Interfaces.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) == RuleBase.Key)
					{
						baseClass = RuleBase.Key;
						TypeCount = RuleBase.Value; ;
						return true;
					}
				}
			}
			return baseClass != null;
		}

		/// <summary>
		/// 检测是否继承接口（只对比接口名称，不包括泛型）
		/// </summary>
		/// <param name="typeSymbol">子接口</param>
		/// <param name="RuleBases">接口名称</param>
		/// <param name="Interface">基类接口符号</param>
		/// <returns></returns>
		private static bool CheckRuleBase(INamedTypeSymbol typeSymbol, string RuleBases, out INamedTypeSymbol? Interface)
		{
			Interface = null;
			foreach (var Interfaces in typeSymbol.AllInterfaces)
			{
				if (Interfaces.Name != RuleBases) continue;
				Interface = Interfaces;
				return true;
			}
			return false;
		}

		/// <summary>
		/// 复制类型所在源码文件的命名空间
		/// </summary>
		/// <param name="typeSymbol"></param>
		/// <returns></returns>
		private static string GetUsings(InterfaceDeclarationSyntax typeSymbol)
		{
			var root = typeSymbol.SyntaxTree.GetRoot();
			var usings = root.DescendantNodes().OfType<UsingDirectiveSyntax>();
			return string.Join(Environment.NewLine, usings.Select(u => u.ToString()));
		}
	}
}