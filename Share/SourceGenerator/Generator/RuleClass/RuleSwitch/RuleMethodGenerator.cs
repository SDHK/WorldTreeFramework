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
using System.Reflection;
using System.Text;

namespace WorldTree.SourceGenerator
{
	public class RuleFileData
	{
		public string FileName;
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
		public string switchValueType;
		public string switchValue;
	}

	public struct RuleMethodData
	{
		public AttributeSyntax attributeSyntax;
		public MethodDeclarationSyntax Method;

		public string caseValue;
	}

	[Generator]
	internal class RuleMethodGenerator : ISourceGenerator
	{
		public Dictionary<string, RuleFileData> fileDatas = new();


		private List<string> typeNames = new();
		private List<string> typeTNames = new();

		// 创建一个 SymbolDisplayFormat 对象，用于自定义显示格式
		//因为委托简写可以省略写类型，导致原文件不需要写参数的命名空间，
		//所以这里要获取到泛型参数的类型全名，包括命名空间。
		SymbolDisplayFormat customFormat = new SymbolDisplayFormat(
		globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
		typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
		genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters);

		public void Initialize(GeneratorInitializationContext context)
		{
		}

		public void Execute(GeneratorExecutionContext context)
		{
			fileDatas.Clear();

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
						if (!TryGetRuleType(semanticModel, attributeSyntax.ArgumentList.Arguments[0], out INamedTypeSymbol ruleTypeSymbol, out INamedTypeSymbol baseTypeSymbol, out RuleBaseEnum ruleBaseEnum))
							continue;
						//因为是简写方法，所以直接使用方法所在的文件名
						string fileName = Path.GetFileNameWithoutExtension(methodDeclaration.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First().SyntaxTree.FilePath);

						if (!fileDatas.TryGetValue(fileName, out RuleFileData ruleFileData))
						{
							ruleFileData = new();
							var classSyntax = methodDeclaration.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First();
							ruleFileData.FileName = fileName;
							ruleFileData.StaticRuleName = classSyntax.Identifier.Text;
							//ruleFileData.Usings = TreeSyntaxHelper.GetUsings(classSyntax);
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
					else
					{
						AttributeArgumentSyntax argumentSyntax = attributeSyntax.ArgumentList.Arguments[count == 3 ? 0 : 1];

						if (!TryGetRuleType(semanticModel, argumentSyntax, out INamedTypeSymbol ruleTypeSymbol, out INamedTypeSymbol baseTypeSymbol, out RuleBaseEnum ruleBaseEnum))
							continue;
						AttributeArgumentSyntax argumentSwitch = attributeSyntax.ArgumentList.Arguments[count == 3 ? 1 : 2];
						AttributeArgumentSyntax argumentCase = attributeSyntax.ArgumentList.Arguments[count == 3 ? 2 : 3];

						//如果是分发方法，则需要获取Switch值
						if (!TryGetSwitchValue(context, semanticModel, methodDeclaration, argumentSwitch, argumentCase, out string switchValueType, out string switchValue, out string caseValue))
							continue;

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
							//ruleFileData.Usings = TreeSyntaxHelper.GetUsings(classSyntax);
							ruleFileData.Namespace = TreeSyntaxHelper.GetNamespace(classSyntax);
							fileDatas.Add(fileName, ruleFileData);
						}

						//将switchValue 中的特殊字符替换为下划线
						string switchValueName = System.Text.RegularExpressions.Regex.Replace(switchValue, @"[^a-zA-Z0-9_]", "_");
						string className = $"{ruleName}_{switchValueName}_RuleMethod";
						if (!ruleFileData.Class.TryGetValue(className, out RuleClassData ruleClassData))
						{
							ruleClassData = new();
							ruleClassData.ClassName = className;
							ruleClassData.mode = count;
							ruleClassData.ruleTypeSymbol = ruleTypeSymbol;
							ruleClassData.baseTypeSymbol = baseTypeSymbol;
							ruleClassData.ruleBaseEnum = ruleBaseEnum;
							ruleClassData.switchValueType = switchValueType;
							ruleClassData.switchValue = switchValue;
							ruleFileData.Class.Add(className, ruleClassData);
						}
						ruleClassData.Methods.Add(new RuleMethodData()
						{
							attributeSyntax = attributeSyntax,
							Method = methodDeclaration,

							caseValue = caseValue
						});
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
					if (classData.mode != 1)
					{
						AddClassMode3(ClassCode, classData);
					}
					else
					{
						AddClassMode1(ClassCode, classData);
					}
				}

				if (ClassCode.ToString() != "")
				{
					fileCode.AppendLine(@$"//对于法则方法简写的调用生成");
					//fileCode.AppendLine(fileData.Usings);
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



		private void AddClassMode1(StringBuilder classCode, RuleClassData classData)
		{
			if (classData.Methods.Count == 0) return;

			typeNames.Clear();
			typeTNames.Clear();
			var types = classData.baseTypeSymbol.TypeArguments;
			for (int i = 0; i < types.Length; i++)
			{
				//基类第二个泛型参数是Rule类型，直接忽略
				if (i == 1) continue;
				typeNames.Add(types[i].ToDisplayString());
				ParseTypeSymbol(types[i], typeTNames);
			}
			string typeTName = typeTNames.Count == 0 ? "" : $"<{string.Join(", ", typeTNames)}>";

			bool isCall = classData.ruleBaseEnum is RuleBaseEnum.CallRule or RuleBaseEnum.CallRuleAsync;
			string genericTypeParameter = GetRuleTypeParameter(typeNames, isCall, out string outType);
			string genericParameter = GetRuleParameter(typeNames, isCall, out _);
			string methodName = classData.Methods[0].Method.Identifier.Text;

			switch (classData.ruleBaseEnum)
			{
				case RuleBaseEnum.SendRule:
					classCode.AppendLine(
					$$"""
							class {{classData.ClassName}}{{typeTName}} : {{classData.ruleTypeSymbol.ToDisplayString()}} { protected override void Execute({{genericTypeParameter}}) => {{methodName}}({{genericParameter}}); }
					"""
					);
					break;
				case RuleBaseEnum.SendRuleAsync:
					classCode.AppendLine(
					$$"""
							class {{classData.ClassName}}{{typeTName}} : {{classData.ruleTypeSymbol.ToDisplayString()}} { protected override TreeTask Execute({{genericTypeParameter}}) => {{methodName}}({{genericParameter}}); }
					"""
					);
					break;
				case RuleBaseEnum.CallRule:
					classCode.AppendLine(
					$$"""
							class {{classData.ClassName}}{{typeTName}} : {{classData.ruleTypeSymbol.ToDisplayString()}} { protected override {{outType}} Execute({{genericTypeParameter}}) => {{methodName}}({{genericParameter}}); }
					"""
					);
					break;
				case RuleBaseEnum.CallRuleAsync:
					classCode.AppendLine(
					$$"""
							class {{classData.ClassName}}{{typeTName}} : {{classData.ruleTypeSymbol.ToDisplayString()}} { protected override TreeTask<{{outType}}> Execute({{genericTypeParameter}}) => {{methodName}}({{genericParameter}}); }
					"""
					);
					break;
			}
		}

		private void AddClassMode3(StringBuilder classCode, RuleClassData classData)
		{
			if (classData.Methods.Count == 0) return;
			typeNames.Clear();
			typeTNames.Clear();
			var types = classData.baseTypeSymbol.TypeArguments;
			for (int i = 0; i < types.Length; i++)
			{
				//基类第二个泛型参数是Rule类型，直接忽略
				if (i == 1) continue;
				typeNames.Add(types[i].ToDisplayString());
				ParseTypeSymbol(types[i], typeTNames);
			}
			string typeTName = typeTNames.Count == 0 ? "" : $"<{string.Join(", ", typeTNames)}>";

			bool isCall = classData.ruleBaseEnum is RuleBaseEnum.CallRule or RuleBaseEnum.CallRuleAsync;
			string genericTypeParameter = GetRuleTypeParameter(typeNames, isCall, out string outType);
			string genericParameter = GetRuleParameter(typeNames, isCall, out _);
			string genericType = GetRuleType(typeNames, isCall, out _);

			classCode.AppendLine($"		sealed class {classData.ClassName}{typeTName} : {classData.ruleTypeSymbol.ToDisplayString()}");
			classCode.AppendLine($"		{{");
			classCode.AppendLine($"			/// <summary>");
			classCode.AppendLine($"			/// 法则分流字典");
			classCode.AppendLine($"			/// </summary>");

			switch (classData.ruleBaseEnum)
			{
				case RuleBaseEnum.SendRule:

					classCode.AppendLine($"			private Dictionary<{classData.switchValueType},Action<{genericType}>> methodDict = new()");
					break;
				case RuleBaseEnum.SendRuleAsync:
					classCode.AppendLine($"			private Dictionary<{classData.switchValueType},Func<{genericType}, TreeTask>> methodDict = new()");
					break;
				case RuleBaseEnum.CallRule:
					classCode.AppendLine($"			private Dictionary<{classData.switchValueType},Func<{genericType}, {outType}>> methodDict = new()");
					break;
				case RuleBaseEnum.CallRuleAsync:
					classCode.AppendLine($"			private Dictionary<{classData.switchValueType},Func<{genericType}, TreeTask<{outType}>>> methodDict = new()");
					break;
			}
			classCode.AppendLine($"			{{");
			foreach (var MethodData in classData.Methods)
			{
				string methodName = MethodData.Method.Identifier.Text;
				classCode.AppendLine($"				{{{MethodData.caseValue}, {methodName}}},");
			}
			classCode.AppendLine($"			}};");


			switch (classData.ruleBaseEnum)
			{
				case RuleBaseEnum.SendRule:
					classCode.AppendLine(
					$$"""
								protected override void Execute({{genericTypeParameter}}) 
								{
									if (methodDict.TryGetValue({{classData.switchValue}}, out var method))
									method({{genericParameter}});
								}
					"""
					);
					break;
				case RuleBaseEnum.SendRuleAsync:
					classCode.AppendLine(
					$$"""
								protected override TreeTask Execute({{genericTypeParameter}})
								{
									if (methodDict.TryGetValue({{classData.switchValue}}, out var method))
									{
										await method({{genericParameter}});
									}
									else
									{
										await self.TreeTaskCompleted();
									}
								}
					"""
					);
					break;
				case RuleBaseEnum.CallRule:
					classCode.AppendLine(
					$$"""
								protected override {{outType}} Execute({{genericTypeParameter}}) 
								{
									if (methodDict.TryGetValue({{classData.switchValue}}, out var method))
									{
										return method({{genericParameter}}); 
									}
									else
									{
										return default({{outType}});
									}				
								}
					"""
					);
					break;
				case RuleBaseEnum.CallRuleAsync:
					classCode.AppendLine(
					$$"""
								protected override TreeTask<{{outType}}> Execute({{genericTypeParameter}})
								{
									if (methodDict.TryGetValue({{classData.switchValue}}, out var method))
									{
										return await method({{genericParameter}});
									}
									else
									{
										await self.TreeTaskCompleted();
										return default({{outType}});
									}
								}
					"""
					);
					break;
			}

			classCode.AppendLine($"		}}");
		}

		/// <summary>
		/// 尝试获取Switch值
		/// </summary>
		private bool TryGetSwitchValue(GeneratorExecutionContext context, SemanticModel semanticModel, MethodDeclarationSyntax method, AttributeArgumentSyntax argumentSwitch, AttributeArgumentSyntax argumentCase, out string switchValueType, out string switchValue, out string caseValue)
		{
			//argumentSwitch 是 nameof 表达式 
			//argumentCase 是 object 常量

			switchValueType = null;
			switchValue = null;
			caseValue = null;

			//先检查方法参数数量
			if (method.ParameterList.Parameters.Count == 0) return false;


			// 检查是否是 nameof 表达式
			if (argumentSwitch.Expression is not InvocationExpressionSyntax invocation) return false;
			if (invocation.Expression.ToString() != "nameof") return false;


			// 获取 nameof 参数的表达式
			var nameofArgument = invocation.ArgumentList.Arguments.FirstOrDefault()?.Expression;
			if (nameofArgument == null) return false;


			// 如果是成员访问表达式，保留完整的表达式文本
			if (nameofArgument is MemberAccessExpressionSyntax memberAccess)
			{
				switchValue = memberAccess.ToString(); // 这将保留完整的 "self.Id"
													   // 获取表达式的类型信息
				var typeInfo = semanticModel.GetTypeInfo(memberAccess);
				if (typeInfo.Type != null)
				{
					// 获取完整类型名称，包括命名空间
					switchValueType = typeInfo.Type.ToDisplayString(customFormat);
				}
			}
			else
			{
				// 原来的逻辑处理简单属性
				var symbolInfo = semanticModel.GetSymbolInfo(nameofArgument);
				if (symbolInfo.Symbol is not IParameterSymbol parameterSymbol) return false;
				switchValue = parameterSymbol.Name;

				// 获取完整类型名称，包括命名空间
				switchValueType = parameterSymbol.Type.ToDisplayString(customFormat);
			}

			bool isSwitch = false;
			//收集方法参数名称
			for (int i = 0; i < method.ParameterList.Parameters.Count; i++)
			{
				string argName = method.ParameterList.Parameters[i].Identifier.Text;
				//判断switchValue是否在参数列表中
				if (switchValue.Contains('.'))
				{
					if (switchValue.Split('.')[0] != argName)
					{
						continue;
					}
				}
				else if (switchValue != argName)
				{
					continue;
				}

				//如果在参数列表中，则替换为需要生成的参数名称
				if (i == 0)
				{
					switchValue = switchValue.Replace(argName, "self");
				}
				else
				{
					switchValue = switchValue.Replace(argName, $"arg{i}");
				}
				isSwitch = true;
			}
			//如果没有找到参数，则返回false
			if (!isSwitch) return false;


			// 直接拿 argumentCase 填写的文本
			if (argumentCase.Expression is LiteralExpressionSyntax literalExpression)
			{
				// 使用 ToString() 获取完整的文本表示，包括引号
				caseValue = literalExpression.ToString();
				// 或者使用 Token.Text
				// caseValue = literalExpression.Token.Text;
				return true;
			}
			else if (argumentCase.Expression is MemberAccessExpressionSyntax memberAccess1)
			{
				// 处理成员访问表达式，如 TestEnum.Test1
				caseValue = memberAccess1.ToString();
				return true;
			}

			return false;
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

		/// <summary>
		/// 转换 泛型参数 ：T0, T1, T2, T3 =&gt; T0, T1, T2, T3 
		/// </summary>
		private string GetRuleType(List<string> Types, bool isCall, out string outType)
		{
			var result = new StringBuilder();
			for (int i = 0; i < Types.Count; i++)
			{
				if (i == 0)
				{
					result.Append($"{Types[i]}");
				}
				else if (!(isCall && i == Types.Count - 1))
				{
					result.Append($", {Types[i]}");
				}
			}
			if (isCall)
			{
				outType = Types[Types.Count - 1];
				return result.ToString();
			}
			else
			{
				outType = "void";
				return result.ToString();
			}
		}

		/// <summary>
		/// 转换 泛型参数 ：T0, T1, T2, T3 =&gt; T0 self, T1 arg1, T2 arg2, T3 arg3
		/// </summary>
		private string GetRuleTypeParameter(List<string> Types, bool isCall, out string outType)
		{
			var result = new StringBuilder();
			for (int i = 0; i < Types.Count; i++)
			{
				if (i == 0)
				{
					result.Append($"{Types[i]} self");
				}
				else if (!(isCall && i == Types.Count - 1))
				{
					result.Append($", {Types[i]} arg{i}");
				}
			}
			if (isCall)
			{
				outType = Types[Types.Count - 1];
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
		private string GetRuleParameter(List<string> Types, bool isCall, out string outType)
		{
			var result = new StringBuilder();
			for (int i = 0; i < Types.Count; i++)
			{
				if (i == 0)
				{
					result.Append($"self");
				}
				else if (!(isCall && i == Types.Count - 1))
				{
					result.Append($", arg{i}");
				}
			}
			if (isCall)
			{
				outType = Types[Types.Count - 1];
				return result.ToString();
			}
			else
			{
				outType = "void";
				return result.ToString();
			}
		}

		private void ParseTypeSymbol(ITypeSymbol typeSymbol, List<string> typeTNames)
		{
			// 如果是命名类型（如 List<int> 或 Dictionary<string, List<T>>）
			if (typeSymbol is INamedTypeSymbol namedTypeSymbol)
			{
				if (namedTypeSymbol.TypeKind == TypeKind.Error)
				{
					typeTNames.Add(typeSymbol.ToDisplayString());
					return;
				}
				// 递归解析其泛型参数
				foreach (var typeArgument in namedTypeSymbol.TypeArguments)
				{
					ParseTypeSymbol(typeArgument, typeTNames);
				}
			}
			// 如果是泛型符号（如 T、U）
			else
			{
				typeTNames.Add(typeSymbol.ToDisplayString());
			}
		}
	}
}
