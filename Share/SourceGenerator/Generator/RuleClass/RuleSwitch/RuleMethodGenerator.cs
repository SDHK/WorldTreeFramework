/****************************************

* 作者：闪电黑客
* 日期：2025/4/24 14:30

* 描述：法则分流类型补充生成器

*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WorldTree.SourceGenerator
{


	public class RuleFileData
	{
		public string FileName;
		public string Usings;
		public string Namespace;
		public string StaticRuleName;

		/// <summary>
		/// 类名
		/// </summary>
		public Dictionary<string, RuleClassData> Class = new();
	}

	public class RuleClassData
	{
		public string ClassName;

		/// <summary>
		/// 模式
		/// </summary>
		public int mode;

		public List<RuleMethodData> Methods = new();

		public INamedTypeSymbol ruleTypeSymbol;
		public INamedTypeSymbol baseTypeSymbol;
		public RuleBaseEnum ruleBaseEnum;
	}

	public struct RuleMethodData
	{
		public AttributeSyntax attributeSyntax;
		public MethodDeclarationSyntax Method;
	}

	[Generator]
	internal class RuleMethodGenerator : ISourceGenerator
	{
		public Dictionary<string, RuleFileData> fileDatas = new();


		public void Initialize(GeneratorInitializationContext context)
		{
		}

		public void Execute(GeneratorExecutionContext context)
		{
			// 遍历收集标记了 NodeRule 特性的所有方法
			foreach (var syntaxTree in context.Compilation.SyntaxTrees)
			{
				SemanticModel semanticModel = context.Compilation.GetSemanticModel(syntaxTree);

				// 查找所有方法声明
				var methodDeclarations = syntaxTree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>();
				foreach (MethodDeclarationSyntax methodDeclaration in methodDeclarations)
				{
					if (!TreeSyntaxHelper.TryGetAttribute(methodDeclaration, GeneratorHelper.NodeRuleAttribute, out AttributeSyntax attributeSyntax)) continue;
					int count = attributeSyntax.ArgumentList.Arguments.Count;
					if (count == 0) continue;

					//一个参数则是简写方法
					if (count == 1)
					{
						if (TryGetRuleType(semanticModel, attributeSyntax.ArgumentList.Arguments[0], out INamedTypeSymbol ruleTypeSymbol, out INamedTypeSymbol baseTypeSymbol, out RuleBaseEnum ruleBaseEnum))
						{
							//因为是简写方法，所以直接使用方法所在的文件名
							string fileName = Path.GetFileNameWithoutExtension(methodDeclaration.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First().SyntaxTree.FilePath);

							if (!fileDatas.TryGetValue(fileName, out RuleFileData ruleFileData))
							{
								ruleFileData = new();
								var classSyntax = methodDeclaration.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First();
								ruleFileData.FileName = fileName;
								ruleFileData.StaticRuleName = classSyntax.Identifier.Text;
								ruleFileData.Usings = TreeSyntaxHelper.GetUsings(classSyntax);
								ruleFileData.Namespace = TreeSyntaxHelper.GetNamespace(classSyntax);
								fileDatas.Add(fileName, ruleFileData);
							}
							//获取方法名称
							string methodName = methodDeclaration.Identifier.Text;

							//使用正则表达式替换所有非字母、数字和下划线的字符为下划线
							//string ruleName = ruleTypeSymbol.ToDisplayString();
							//ruleName = System.Text.RegularExpressions.Regex.Replace(ruleName, @"[^a-zA-Z0-9_]", "_");
							string className = $"{methodName}_RuleMethod";
							if (!ruleFileData.Class.TryGetValue(className, out RuleClassData ruleClassData))
							{
								ruleClassData = new();
								ruleClassData.ClassName = className;
								ruleClassData.mode = count;
								ruleClassData.ruleTypeSymbol = ruleTypeSymbol;
								ruleClassData.baseTypeSymbol = baseTypeSymbol;
								ruleClassData.ruleBaseEnum = ruleBaseEnum;
								ruleFileData.Class.Add(className, ruleClassData);
							}
							ruleClassData.Methods.Add(new RuleMethodData()
							{
								attributeSyntax = attributeSyntax,
								Method = methodDeclaration
							});
						}
					}
					else
					{
						AttributeArgumentSyntax argumentSyntax = attributeSyntax.ArgumentList.Arguments[count == 3 ? 0 : 1];
						if (TryGetRuleType(semanticModel, argumentSyntax, out INamedTypeSymbol ruleTypeSymbol, out INamedTypeSymbol baseTypeSymbol, out RuleBaseEnum ruleBaseEnum))
						{
							//因为分发方法会有多个，所以使用方法标记的法则类型组合作为文件名
							var classSyntax = methodDeclaration.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First();

							string ruleName = ruleTypeSymbol.ToDisplayString();
							ruleName = System.Text.RegularExpressions.Regex.Replace(ruleName, @"[^a-zA-Z0-9_]", "_");
							string fileName = $"{classSyntax.Identifier.Text}_{ruleName}_RuleMethod";
							if (!fileDatas.TryGetValue(fileName, out RuleFileData ruleFileData))
							{
								ruleFileData = new();
								ruleFileData.FileName = fileName;
								ruleFileData.StaticRuleName = classSyntax.Identifier.Text;
								ruleFileData.Usings = TreeSyntaxHelper.GetUsings(classSyntax);
								ruleFileData.Namespace = TreeSyntaxHelper.GetNamespace(classSyntax);
								fileDatas.Add(fileName, ruleFileData);
							}
							//获取方法名称
							string methodName = methodDeclaration.Identifier.Text;
							string className = $"{ruleName}_RuleMethod";
							if (!ruleFileData.Class.TryGetValue(className, out RuleClassData ruleClassData))
							{
								ruleClassData = new();
								ruleClassData.ClassName = className;
								ruleClassData.mode = count;
								ruleClassData.ruleTypeSymbol = ruleTypeSymbol;
								ruleClassData.baseTypeSymbol = baseTypeSymbol;
								ruleClassData.ruleBaseEnum = ruleBaseEnum;
								ruleFileData.Class.Add(className, ruleClassData);
							}
							ruleClassData.Methods.Add(new RuleMethodData()
							{
								attributeSyntax = attributeSyntax,
								Method = methodDeclaration
							});
						}
					}
				}
			}


			// 生成代码
			foreach (var fileData in fileDatas.Values)
			{
				StringBuilder fileCode = new();
				StringBuilder ClassCode = new();

				foreach (var classData in fileData.Class.Values)
				{
					if (classData.mode == 1)
					{

						AddClassMode1(ClassCode, classData);
					}
				}

				if (ClassCode.ToString() != "")
				{
					fileCode.AppendLine(@$"//对于法则方法简写的调用生成");
					fileCode.AppendLine(fileData.Usings);
					fileCode.AppendLine($"namespace {fileData.Namespace}");
					fileCode.AppendLine("{");
					fileCode.AppendLine($"	public static partial class {fileData.StaticRuleName}");
					fileCode.AppendLine("	{");
					fileCode.Append(ClassCode);
					fileCode.AppendLine("	}");
					fileCode.Append("}");

					context.AddSource($"{fileData.FileName}RuleMethod.cs", SourceText.From(fileCode.ToString(), Encoding.UTF8));
				}
			}
		}

		/// <summary>
		/// 尝试获取法则类型
		/// </summary>
		private bool TryGetRuleType(SemanticModel semanticModel, AttributeArgumentSyntax argumentSyntax, out INamedTypeSymbol ruleTypeSymbol, out INamedTypeSymbol baseTypeSymbol, out RuleBaseEnum ruleBaseEnum)
		{
			ruleBaseEnum = default;
			ruleTypeSymbol = null;
			baseTypeSymbol = null;

			// 检查是否是 nameof 表达式
			if (argumentSyntax.Expression is not InvocationExpressionSyntax invocation) return false;
			if (invocation.Expression.ToString() != "nameof") return false;
			// 获取 nameof 参数的表达式
			var nameofArgument = invocation.ArgumentList.Arguments.FirstOrDefault()?.Expression;
			if (nameofArgument == null) return false;

			// 获取符号信息
			var symbolInfo = semanticModel.GetSymbolInfo(nameofArgument);
			if (symbolInfo.Symbol is not INamedTypeSymbol namedTypeSymbol) return false;

			ITypeSymbol typeSymbol;
			// 检查类型是否符合规则
			if (NamedSymbolHelper.CheckBase(namedTypeSymbol, GeneratorHelper.SendRule, out typeSymbol)) ruleBaseEnum = RuleBaseEnum.SendRule;
			else if (NamedSymbolHelper.CheckBase(namedTypeSymbol, GeneratorHelper.CallRule, out typeSymbol)) ruleBaseEnum = RuleBaseEnum.CallRule;
			else if (NamedSymbolHelper.CheckBase(namedTypeSymbol, GeneratorHelper.SendRuleAsync, out typeSymbol)) ruleBaseEnum = RuleBaseEnum.SendRuleAsync;
			else if (NamedSymbolHelper.CheckBase(namedTypeSymbol, GeneratorHelper.CallRuleAsync, out typeSymbol)) ruleBaseEnum = RuleBaseEnum.CallRuleAsync;
			else return false;

			baseTypeSymbol = typeSymbol as INamedTypeSymbol;
			ruleTypeSymbol = namedTypeSymbol;
			return true;
		}



		private void AddClassMode1(StringBuilder classCode, RuleClassData classData)
		{
			//	classData.baseTypeSymbol泛型获取 转为 string[]
			var types = classData.baseTypeSymbol.TypeArguments;
			List<string> typeNames = new();
			for (int i = 0; i < types.Length; i++)
			{
				//基类第二个泛型参数是Rule类型，直接忽略
				if (i == 1) continue;
				typeNames.Add(types[i].ToDisplayString());
			}
			bool isCall = classData.ruleBaseEnum is RuleBaseEnum.CallRule or RuleBaseEnum.CallRuleAsync;
			string genericTypeParameter = GetRuleTypeParameter(typeNames.ToArray(), isCall, out string outType);
			string genericParameter = GetRuleParameter(typeNames.ToArray(), isCall, out _);
			string methodName = classData.Methods[0].Method.Identifier.Text;

			switch (classData.ruleBaseEnum)
			{
				case RuleBaseEnum.SendRule:
					classCode.AppendLine(
					$$"""
							class {{classData.ClassName}} : {{classData.baseTypeSymbol.ToDisplayString()}} { protected override void Execute({{genericTypeParameter}}) => {{methodName}}({{genericParameter}}); }
					"""
					);
					break;
				case RuleBaseEnum.SendRuleAsync:
					classCode.AppendLine(
					$$"""
							class {{classData.ClassName}} : {{classData.baseTypeSymbol.ToDisplayString()}} { protected override TreeTask Execute({{genericTypeParameter}}) => {{methodName}}({{genericParameter}}); }
					"""
					);
					break;
				case RuleBaseEnum.CallRule:
					classCode.AppendLine(
					$$"""
							class {{classData.ClassName}} : {{classData.baseTypeSymbol.ToDisplayString()}} { protected override {{outType}} Execute({{genericTypeParameter}}) => {{methodName}}({{genericParameter}}); }
					"""
					);
					break;
				case RuleBaseEnum.CallRuleAsync:
					classCode.AppendLine(
					$$"""
							class {{classData.ClassName}}RuleExecute : {{classData.baseTypeSymbol.ToDisplayString()}} { protected override TreeTask<{{outType}}> Execute({{genericTypeParameter}}) => {{methodName}}({{genericParameter}}); }
					"""
					);
					break;
			}
		}



		/// <summary>
		/// 转换 泛型参数 ：T0, T1, T2, T3 =&gt; T0 self T1 arg1, T2 arg2, T3 arg3
		/// </summary>
		private string GetRuleTypeParameter(string[] Types, bool isCall, out string outType)
		{
			var result = new StringBuilder();
			for (int i = 0; i < Types.Length; i++)
			{
				if (i == 0)
				{
					result.Append($"{Types[i]} self");
				}
				else if (!(isCall && i == Types.Length - 1))
				{
					result.Append($", {Types[i]} arg{i}");
				}
			}
			if (isCall)
			{
				outType = Types[Types.Length - 1];
				return result.ToString();
			}
			else
			{
				outType = "void";
				return result.ToString();
			}
		}

		/// <summary>
		/// 转换 泛型参数 ：T0, T1, T2, T3 =&gt; self, arg1, arg2, arg3
		/// </summary>
		private string GetRuleParameter(string[] Types, bool isCall, out string outType)
		{
			var result = new StringBuilder();
			for (int i = 0; i < Types.Length; i++)
			{
				if (i == 0)
				{
					result.Append($"self");
				}
				else if (!(isCall && i == Types.Length - 1))
				{
					result.Append($", arg{i}");
				}
			}
			if (isCall)
			{
				outType = Types[Types.Length - 1];
				return result.ToString();
			}
			else
			{
				outType = "void";
				return result.ToString();
			}
		}

	}
}
