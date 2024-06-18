/****************************************

* 作者：闪电黑客
* 日期：2024/6/6 10:28

* 描述：对于法则委托简写的调用生成

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WorldTree.SourceGenerator
{
	[Generator]
	public class RuleExecuteGenerator : ISourceGenerator
	{
		private List<FieldDeclarationSyntax> delegateInstances = new();

		/// <summary>
		/// 接口名称,泛型数量,法则类型
		/// </summary>
		private Dictionary<string, Dictionary<int, INamedTypeSymbol>> RuleTypes = new();

		/// <summary>
		/// 文件名-字段名-委托类型集合-委托类型语法树节点
		/// </summary>
		public Dictionary<string, List<(string, INamedTypeSymbol, FieldDeclarationSyntax)>> fileClassDict = new();

		/// <summary>
		/// 文件名-引用
		/// </summary>
		public Dictionary<string, string> fileUsings = new();

		/// <summary>
		/// 文件名-命名空间
		/// </summary>
		public Dictionary<string, string> fileNamespace = new();

		public void Initialize(GeneratorInitializationContext context)
		{ }

		public void Execute(GeneratorExecutionContext context)
		{
			delegateInstances.Clear();
			RuleTypes.Clear();
			fileClassDict.Clear();
			fileUsings.Clear();
			fileNamespace.Clear();

			//classInterfaceSyntax.Clear();

			CollectAllInterfaces(context.Compilation);

			foreach (var syntaxTree in context.Compilation.SyntaxTrees)
			{
				var semanticModel = context.Compilation.GetSemanticModel(syntaxTree);
				CollectPrivateStaticField(syntaxTree, semanticModel);

				//那到所有的委托实例的类型
				foreach (FieldDeclarationSyntax delegateInstance in delegateInstances)
				{
					var typeSymbol = semanticModel.GetTypeInfo(delegateInstance.Declaration.Type).Type;
					INamedTypeSymbol? delegateType = (INamedTypeSymbol)typeSymbol;

					//委托泛型数量,如果是-1则表示不是委托，因为生成的委托是有泛型的
					var TypeCount = delegateType?.TypeArguments.Count() ?? 0;
					if (TypeCount == 0) continue;

					//判断是否和接口名称一致
					if (!RuleTypes.TryGetValue(delegateType.Name, out var Count_Type)) continue;

					//判断是否有对应的泛型数量
					if (!Count_Type.TryGetValue(TypeCount - 1, out _)) continue;

					//那到所在的文件名
					string fileName = Path.GetFileNameWithoutExtension(delegateInstance.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First().SyntaxTree.FilePath);

					//收集委托实例
					if (!fileClassDict.TryGetValue(fileName, out List<(string, INamedTypeSymbol, FieldDeclarationSyntax)> set))
					{
						set = new List<(string, INamedTypeSymbol, FieldDeclarationSyntax)>();
						fileClassDict.Add(fileName, set);
						var classSyntax = delegateInstance.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First();
						fileUsings.Add(fileName, TreeSyntaxHelper.GetUsings(classSyntax));
						fileNamespace.Add(fileName, TreeSyntaxHelper.GetNamespace(classSyntax));
					}
					string? delegateName = delegateInstance?.Declaration?.Variables.First().Identifier.Text;
					if (delegateName == null) continue;
					set.Add((delegateName, delegateType, delegateInstance));
				}
			}

			foreach (var fileClassList in fileClassDict)
			{
				StringBuilder fileCode = new();
				StringBuilder ClassCode = new();
				string className = null;

				foreach ((string, INamedTypeSymbol, FieldDeclarationSyntax) delegateType in fileClassList.Value)
				{
					//委托类型全名包括泛型
					var delegateClassName = delegateType.Item2?.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
					FieldDeclarationSyntax delegateInstance = delegateType.Item3;
					var TypeCount = delegateType.Item2?.TypeArguments.Count() ?? 0;
					if (TypeCount == 0) continue;
					if (!RuleTypes.TryGetValue(delegateType.Item2.Name, out var Count_Type)) continue;
					if (!Count_Type.TryGetValue(TypeCount - 1, out var ruleType)) continue;

					//获取委托所在的静态兄弟类类名,只获取第一个，默认委托都在一个类中，并且一个文件只有一个静态类
					className ??= delegateInstance.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First().Identifier.Text;

					//获取委托字段的名称
					var delegateName = delegateType.Item1;

					//组装法则类型名称
					string RuleName = delegateClassName.Replace("On", "");
					int index = RuleName.IndexOf('<');
					string RuleFullName = RuleName.Insert(index, "Rule");
					string NodeName = RuleName.Split('<')[1].Split(',')[0].TrimEnd('>');

					INamedTypeSymbol? baseInterface = null;

					int ruleTypeCode = 0;
					string[]? types = null;
					if (NamedSymbolHelper.CheckInterface(ruleType, GeneratorHelper.ISendRule, out baseInterface)) ruleTypeCode = 1;
					else if (NamedSymbolHelper.CheckInterface(ruleType, GeneratorHelper.ISendRuleAsync, out baseInterface)) ruleTypeCode = 2;
					else if (NamedSymbolHelper.CheckInterface(ruleType, GeneratorHelper.ICallRule, out baseInterface)) ruleTypeCode = 3;
					else if (NamedSymbolHelper.CheckInterface(ruleType, GeneratorHelper.ICallRuleAsync, out baseInterface)) ruleTypeCode = 4;
					types = GetReflectionRuleType(delegateType.Item2, ruleType, baseInterface);
					string genericTypeParameter = GetRuleTypeParameter(types, ruleTypeCode > 2, out string outType);
					string genericParameter = GetRuleParameter(types, ruleTypeCode > 2, out _);

					switch (ruleTypeCode)
					{
						case 1:

							ClassCode.AppendLine(
							$$"""
									class {{delegateName}}RuleExecute : {{RuleFullName}} { protected override void Execute({{genericTypeParameter}}) => {{delegateName}}({{genericParameter}}); }
							"""
							);
							break;

						case 2:

							ClassCode.AppendLine(
							$$"""
									class {{delegateName}}RuleExecute : {{RuleFullName}} { protected override TreeTask Execute({{genericTypeParameter}}) => {{delegateName}}({{genericParameter}}); }
							"""
							);
							break;

						case 3:
							ClassCode.AppendLine(
							$$"""
									class {{delegateName}}RuleExecute : {{RuleFullName}} { protected override {{outType}} Execute({{genericTypeParameter}}) => {{delegateName}}({{genericParameter}}); }
							"""
							);
							break;

						case 4:

							ClassCode.AppendLine(
							$$"""
									class {{delegateName}}RuleExecute : {{RuleFullName}} { protected override TreeTask<{{outType}}> Execute({{genericTypeParameter}}) => {{delegateName}}({{genericParameter}}); }
							"""
							);
							break;
					}
				}
				if (ClassCode.ToString() != "")
				{
					fileCode.AppendLine(@$"//对于法则委托简写的调用生成");

					fileCode.AppendLine(fileUsings[fileClassList.Key]);
					fileCode.AppendLine($"namespace {fileNamespace[fileClassList.Key]}");
					fileCode.AppendLine("{");

					fileCode.AppendLine($"	public static partial class {className}");
					fileCode.AppendLine("	{");

					fileCode.Append(ClassCode);

					fileCode.AppendLine("	}");
					fileCode.Append("}");

					context.AddSource($"{fileClassList.Key}Execute.cs", SourceText.From(fileCode.ToString(), Encoding.UTF8));
				}
			}
		}

		/// <summary>
		/// 收集所有静态兄弟类中的 静态字段
		/// </summary>
		/// <remarks>因为委托本身也是代码生成，所以无法检测字段是否为委托</remarks>
		private void CollectPrivateStaticField(SyntaxTree syntaxTree, SemanticModel semanticModel)
		{
			delegateInstances = new List<FieldDeclarationSyntax>();
			var nodes = syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>()
				.Where(c => c.Modifiers.Any(SyntaxKind.StaticKeyword) && c.Modifiers.Any(SyntaxKind.PartialKeyword));

			foreach (var node in nodes)
			{
				var fieldDeclarations = node.DescendantNodes().OfType<FieldDeclarationSyntax>();
				foreach (var fieldDeclaration in fieldDeclarations)
				{
					bool hasStaticKeyword = false;
					bool hasPrivateKeyword = true;

					foreach (var modifier in fieldDeclaration.Modifiers)
					{
						if (modifier.Kind() == SyntaxKind.StaticKeyword)
						{
							hasStaticKeyword = true;
						}
						else if (modifier.Kind() == SyntaxKind.PrivateKeyword)
						{
							hasPrivateKeyword = true;
						}
						else if (modifier.Kind() == SyntaxKind.PublicKeyword)
						{
							hasPrivateKeyword = false;
						}
						else if (modifier.Kind() == SyntaxKind.ProtectedKeyword)
						{
							hasPrivateKeyword = false;
						}
					}

					if (hasStaticKeyword && hasPrivateKeyword)
					{
						delegateInstances.Add(fieldDeclaration);
					}
				}
			}
		}

		/// <summary>
		/// 收集全局4大法则接口，包括引用项目的
		/// </summary>
		/// <param name="Compilation"></param>
		private void CollectAllInterfaces(Compilation Compilation)
		{
			foreach (var item in NamedSymbolHelper.CollectAllInterfaces(Compilation))
			{
				//检测是否继承忽略接口
				//if (NamedSymbolHelper.CheckInterface(item, GeneratorHelper.ISourceGeneratorIgnore, out _)) continue;

				//检测是否继承4大法则接口
				if (!(NamedSymbolHelper.CheckInterface(item, GeneratorHelper.ISendRule, out _) ||
					NamedSymbolHelper.CheckInterface(item, GeneratorHelper.ICallRule, out _) ||
					NamedSymbolHelper.CheckInterface(item, GeneratorHelper.ISendRuleAsync, out _) ||
					NamedSymbolHelper.CheckInterface(item, GeneratorHelper.ICallRuleAsync, out _))) continue;

				string DelegateName = $"On{item.Name}";
				string RuleType = "";
				if (NamedSymbolHelper.CheckInterface(item, GeneratorHelper.ISendRule, out _))
				{
					RuleType = GeneratorHelper.ISendRule;
				}
				else if (NamedSymbolHelper.CheckInterface(item, GeneratorHelper.ICallRule, out _))
				{
					RuleType = GeneratorHelper.ICallRule;
				}
				else if (NamedSymbolHelper.CheckInterface(item, GeneratorHelper.ISendRuleAsync, out _))
				{
					RuleType = GeneratorHelper.ISendRuleAsync;
				}
				else if (NamedSymbolHelper.CheckInterface(item, GeneratorHelper.ICallRuleAsync, out _))
				{
					RuleType = GeneratorHelper.ICallRuleAsync;
				}
				else
				{
					continue;
				}

				if (!RuleTypes.TryGetValue(DelegateName, out Dictionary<int, INamedTypeSymbol> Length_Type))
				{
					Length_Type = new Dictionary<int, INamedTypeSymbol>();
					RuleTypes.Add(DelegateName, Length_Type);
				}
				if (!Length_Type.ContainsKey(item.TypeArguments.Length))
				{
					Length_Type.Add(item.TypeArguments.Length, item);
				}
				else
				{
					Length_Type[item.TypeArguments.Length] = item;
				}
			}
		}

		/// <summary>
		/// 泛型参数映射
		/// </summary>
		/// <param name="delegateType">法则委托简写类型</param>
		/// <param name="ruleClass">法则类型</param>
		/// <param name="ruleBase">法则基类</param>
		/// <remarks>delegateType 类型 映射到 ruleClass 类型，然后再映射到 ruleBase 类型</remarks>
		private string[] GetReflectionRuleType(INamedTypeSymbol delegateType, INamedTypeSymbol ruleClass, INamedTypeSymbol ruleBase)
		{
			//SymbolDisplayFormat.MinimallyQualifiedFormat
			//因为委托简写可以省略写类型，导致原文件不需要写参数的命名空间，
			//所以这里要获取到泛型参数的类型全名，包括命名空间。

			// 获取 ruleType 的泛型参数
			var ruleTypeArgs = GetGenericArguments(delegateType.ToDisplayString()).ToArray();

			var types = new List<string>();
			types.Add(ruleTypeArgs[0]);

			// 获取 ruleBaseSymbol 的泛型参数
			string[]? ruleBaseArgs = GetGenericArguments(ruleBase.ToDisplayString());

			if (ruleBaseArgs == null)
			{
				return types.ToArray();
			}

			// 获取 ruleClass 的泛型参数
			var ruleClassArgs = ruleClass.TypeArguments.Select(t => t.Name.Trim()).ToArray();

			// 创建一个字典，将 ruleClass 的泛型参数映射到 ruleType 的泛型参数
			Dictionary<string, string> ruleTypeToRuleClass = new();
			for (int i = 0; i < ruleClassArgs.Length; i++) ruleTypeToRuleClass.Add(ruleClassArgs[i], ruleTypeArgs[i + 1]);

			// 创建一个新的列表，将 ruleBaseSymbol 的每个泛型参数都映射到一个新的 ruleType 的泛型参数
			for (int i = 0; i < ruleBaseArgs.Length; i++)
			{
				if (ruleTypeToRuleClass.TryGetValue(ruleBaseArgs[i], out string value))
				{
					types.Add(value);
				}
				else
				{
					types.Add(ruleBaseArgs[i]);
				}
			}

			// 返回一个字符串，该字符串包含了这些参数类型
			return types.ToArray();
		}

		/// <summary>
		/// 获取泛型参数数组
		/// </summary>
		private string[]? GetGenericArguments(string ruleType)
		{
			int startIndex = ruleType.IndexOf('<') + 1;
			if (startIndex == -1) return null;
			int endIndex = ruleType.LastIndexOf('>');
			if (endIndex == -1) return null;

			string content = ruleType.Substring(startIndex, endIndex - startIndex);
			List<string> splitContent = new List<string>();
			int bracketCount = 0;
			int lastSplit = 0;
			for (int i = 0; i < content.Length; i++)
			{
				if (content[i] == '<')
				{
					bracketCount++;
				}
				else if (content[i] == '>')
				{
					bracketCount--;
				}
				else if (content[i] == ',' && bracketCount == 0)
				{
					splitContent.Add(content.Substring(lastSplit, i - lastSplit).Trim());
					lastSplit = i + 1;
				}
			}
			splitContent.Add(content.Substring(lastSplit).Trim());

			return splitContent.ToArray();
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