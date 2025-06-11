/****************************************

* 作者：闪电黑客
* 日期：2024/8/28 11:53

* 描述：因为Rule全是私有类中类，暂时无用

*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Text;

namespace WorldTree.SourceGenerator
{
	[Generator]
	public class RuleTypesGenerator : ISourceGenerator
	{
		public void Initialize(GeneratorInitializationContext context)
		{
			context.RegisterForSyntaxNotifications(() => new FindRuleTypesSyntaxReceiver());
		}

		public void Execute(GeneratorExecutionContext context)
		{
			if (!(context.SyntaxReceiver is FindRuleTypesSyntaxReceiver receiver and not null)) return;

			var ClassCode = new StringBuilder();

			if (receiver.classDeclarationSyntax == null) return;
			var classDeclaration = receiver.classDeclarationSyntax;

			ClassCode.AppendLine($"using System;");
			ClassCode.AppendLine($"using System.Collections.Generic;");
			ClassCode.AppendLine($"namespace {TreeSyntaxHelper.GetNamespace(classDeclaration)}");
			ClassCode.AppendLine("{");
			ClassCode.AppendLine($"	/// <summary>");
			ClassCode.AppendLine($"	/// 法则类型列表静态类");
			ClassCode.AppendLine($"	/// <summary>");
			ClassCode.AppendLine($"	public static partial class {classDeclaration.Identifier.Text}");
			ClassCode.AppendLine("	{");
			ClassCode.AppendLine("		/// <summary>");
			ClassCode.AppendLine("		/// 法则类型列表");
			ClassCode.AppendLine("		/// <summary>");
			ClassCode.AppendLine("		public static Type[] Types = new Type[]");
			ClassCode.AppendLine("		{");
			// 查找所有语法树
			IEnumerable<INamedTypeSymbol> TypeSymbols = context.Compilation.GetDerivedTypes("WorldTree.IRule");
			foreach (INamedTypeSymbol typeSymbol in TypeSymbols)
			{
				// 判断是否是抽象类
				if (typeSymbol.IsAbstract) continue;
				// 判断是否是接口
				if (typeSymbol.TypeKind == TypeKind.Interface) continue;
				ClassCode.AppendLine($"			typeof({typeSymbol}),");
			}
			ClassCode.AppendLine("		};");
			ClassCode.AppendLine("	}");
			ClassCode.AppendLine("}");
			// 将生成的代码添加到编译过程中
			context.AddSource("GeneratedRuleTypes.cs", ClassCode.ToString());
		}
	}

	/// <summary>
	/// 查找序列化标记类型
	/// </summary>
	public class FindRuleTypesSyntaxReceiver : ISyntaxReceiver
	{
		public ClassDeclarationSyntax? classDeclarationSyntax = null;

		public void OnVisitSyntaxNode(SyntaxNode node)
		{
			if (classDeclarationSyntax != null) return;
			if (node is not ClassDeclarationSyntax classSyntax) return; // 确保正确的类型转换
			if (!classSyntax.Modifiers.Any(SyntaxKind.StaticKeyword)) return;
			// 检查类型是否是部分类
			if (!classSyntax.Modifiers.Any(SyntaxKind.PartialKeyword)) return;
			// 检查方法是否有特性
			if (TreeSyntaxHelper.CheckAttribute(classSyntax, GeneratorHelper.RuleTypesGeneratorAttribute))
			{
				classDeclarationSyntax = classSyntax;
			}
		}
	}
}
