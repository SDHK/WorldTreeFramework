/****************************************

* 作者：闪电黑客
* 日期：2024/6/6 10:28

* 描述：

*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection.Metadata;

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
		/// 文件名-接口集合
		/// </summary>
		public Dictionary<string, List<INamedTypeSymbol>> fileClassDict = new();


		/// <summary>
		/// 文件名-引用
		/// </summary>
		public Dictionary<string, string> fileUsings = new();
		/// <summary>
		/// 文件名-命名空间
		/// </summary>
		public Dictionary<string, string> fileNamespace = new();

		/// <summary>
		/// 接口名-语法树
		/// </summary>
		public Dictionary<string, FieldDeclarationSyntax> classInterfaceSyntax = new();

		public void Initialize(GeneratorInitializationContext context)
		{
			delegateInstances.Clear();
			RuleTypes.Clear();
			fileClassDict.Clear();
			fileUsings.Clear();
			fileNamespace.Clear();
			classInterfaceSyntax.Clear();
		}
		public void Execute(GeneratorExecutionContext context)
		{
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
					if (!fileClassDict.TryGetValue(fileName, out List<INamedTypeSymbol> set))
					{
						set = new List<INamedTypeSymbol>();
						fileClassDict.Add(fileName, set);
						var classSyntax = delegateInstance.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First();
						fileUsings.Add(fileName, TreeSyntaxHelper.GetUsings(classSyntax));
						fileNamespace.Add(fileName, TreeSyntaxHelper.GetNamespace(classSyntax));
					}
					set.Add(delegateType);

					//委托类型全名包括泛型
					var delegateClassName = delegateType?.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
					classInterfaceSyntax.Add(delegateClassName, delegateInstance);

				}
			}

			foreach (var fileClassList in fileClassDict)
			{
				StringBuilder fileCode = new();
				StringBuilder ClassCode = new();
				string className = null;


				foreach (INamedTypeSymbol delegateType in fileClassList.Value)
				{

					//委托类型全名包括泛型
					var delegateClassName = delegateType?.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

					if (!classInterfaceSyntax.TryGetValue(delegateClassName, out FieldDeclarationSyntax delegateInstance)) continue;
					var TypeCount = delegateType?.TypeArguments.Count() ?? 0;
					if (TypeCount == 0) continue;
					if (!RuleTypes.TryGetValue(delegateType.Name, out var Count_Type)) continue;
					if (!Count_Type.TryGetValue(TypeCount - 1, out var ruleType)) continue;

					//获取委托所在的静态兄弟类类名,只获取第一个，默认委托都在一个类中，并且一个文件只有一个静态类
					className ??= delegateInstance.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First().Identifier.Text;

					//获取委托字段的名称
					var delegateName = delegateInstance?.Declaration?.Variables.First().Identifier.Text;

					//组装法则类型名称
					string RuleName = delegateClassName.Replace("On", "");
					int index = RuleName.IndexOf('<');
					string RuleFullName = RuleName.Insert(index, "Rule");
					string NodeName = RuleName.Split('<')[1].Split(',')[0].TrimEnd('>');

					INamedTypeSymbol? baseInterface = null;


					if (NamedSymbolHelper.CheckInterface(ruleType, GeneratorHelper.ISendRule, out baseInterface))
					{
						string genericTypeParameter = GetGenericTypeParameter(baseInterface);
						string genericParameter = GetGenericParameter(baseInterface);

						ClassCode.AppendLine(
						$$"""
									class {{delegateName}}RuleExecute : {{RuleFullName}}
									{
										protected override void Execute({{NodeName}} self{{genericTypeParameter}})
										=> {{delegateName}}(self{{genericParameter}});
									}
							"""
						);
					}
					else if (NamedSymbolHelper.CheckInterface(ruleType, GeneratorHelper.ISendRuleAsync, out baseInterface))
					{
						string genericTypeParameter = GetGenericTypeParameter(baseInterface);
						string genericParameter = GetGenericParameter(baseInterface);
						ClassCode.AppendLine(
						$$"""
									class {{delegateName}}RuleExecute : {{RuleFullName}}
									{
										protected override TreeTask Execute({{NodeName}} self{{genericTypeParameter}})
										=> {{delegateName}}(self{{genericParameter}});
									}
							"""
						);
					}
					else if (NamedSymbolHelper.CheckInterface(ruleType, GeneratorHelper.ICallRule, out baseInterface))
					{
						string genericTypeParameter = GetCallRuleGenericTypesParameters(baseInterface);
						string genericParameter = GetCallRuleGenericParameters(baseInterface);

						string outType = baseInterface.TypeArguments.Last().ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

						ClassCode.AppendLine(
						$$"""
									class {{delegateName}}RuleExecute : {{RuleFullName}}
									{
										protected override {{outType}} Execute({{NodeName}} self{{genericTypeParameter}})
										=> {{delegateName}}(self{{genericParameter}});
									}
							"""
						);
					}
					else if (NamedSymbolHelper.CheckInterface(ruleType, GeneratorHelper.ICallRuleAsync, out baseInterface))
					{
						string genericTypeParameter = GetCallRuleGenericTypesParameters(baseInterface);
						string genericParameter = GetCallRuleGenericParameters(baseInterface);

						string outType = baseInterface.TypeArguments.Last().ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

						ClassCode.AppendLine(
						$$"""
									class {{delegateName}}RuleExecute : {{RuleFullName}}
									{
										protected override TreeTask<{{outType}}> Execute({{NodeName}} self{{genericTypeParameter}})
										=> {{delegateName}}(self{{genericParameter}});
									}
							"""
						);
					}

				}
				if (ClassCode.ToString() != "")
				{
					fileCode.AppendLine(fileUsings[fileClassList.Key]);
					fileCode.AppendLine($"namespace {fileNamespace[fileClassList.Key]}");
					fileCode.AppendLine("{");

					fileCode.AppendLine($"	public static partial class {className}");
					fileCode.AppendLine("	{");

					fileCode.Append(ClassCode);

					fileCode.AppendLine("	}");
					fileCode.AppendLine("}");

					context.AddSource($"{fileClassList.Key}Execute.cs", SourceText.From(fileCode.ToString(), Encoding.UTF8));
				}
			}

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

		/// <summary>
		/// 获取泛型参数 , T1 arg1, T2 arg2
		/// </summary>
		public static string GetCallRuleGenericTypesParameters(INamedTypeSymbol typeSymbol)
		{
			if (!typeSymbol.IsGenericType) return "";

			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < typeSymbol.TypeArguments.Length - 1; i++)
			{
				sb.Append($", {typeSymbol.TypeArguments[i].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)} arg{i + 1}");
			}
			return sb.ToString();
		}

		/// <summary>
		/// 获取泛型参数 , T1 arg1, T2 arg2
		/// </summary>
		public static string GetGenericParameter(INamedTypeSymbol typeSymbol)
		{
			if (!typeSymbol.IsGenericType) return "";

			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < typeSymbol.TypeArguments.Length; i++)
			{
				sb.Append($", arg{i + 1}");
			}
			return sb.ToString();
		}

		/// <summary>
		/// 获取泛型参数 , T1 arg1, T2 arg2
		/// </summary>
		public static string GetCallRuleGenericParameters(INamedTypeSymbol typeSymbol)
		{
			if (!typeSymbol.IsGenericType) return "";

			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < typeSymbol.TypeArguments.Length - 1; i++)
			{
				sb.Append($", arg{i + 1}");
			}
			return sb.ToString();
		}


		/// <summary>
		/// 转换 泛型参数 ： <T1, T2, T3> => T1 arg1, T2 arg2, T3 arg3
		/// </summary>
		private string GetRuleTypeParameter(string ruleType, bool isCall, out string outType)
		{
			var match = Regex.Match(ruleType, @"<(.*)>");
			if (match.Success)
			{
				var parameters = match.Groups[1].Value.Split(',');
				var result = new StringBuilder();
				for (int i = 0; i < parameters.Length; i++)
				{
					if (i == 0)
					{
						result.Append($"{parameters[i]} self");
					}
					else if (!(isCall && i == parameters.Length - 1))
					{
						result.Append($", {parameters[i]} arg{i}");
					}
				}
				if (isCall)
				{
					outType = parameters[parameters.Length - 1];
					return result.ToString();
				}
				else
				{
					outType = "void";
					return result.ToString();
				}
			}
			outType = "void";
			return string.Empty;
		}

		/// <summary>
		/// 转换 泛型参数 ： <T1, T2, T3> => T1, T2, T3
		/// </summary>
		private string GetRuleParameter(string ruleType, bool isCall, out string outType)
		{
			var match = Regex.Match(ruleType, @"<(.*)>");
			if (match.Success)
			{
				var parameters = match.Groups[1].Value.Split(',');
				var result = new StringBuilder();
				for (int i = 0; i < parameters.Length; i++)
				{
					if (i == 0)
					{
						result.Append($"self");
					}
					else if (!(isCall && i == parameters.Length - 1))
					{
						result.Append($", arg{i}");
					}
				}
				if (isCall)
				{
					outType = parameters[parameters.Length - 1];
					return result.ToString();
				}
				else
				{
					outType = "void";
					return result.ToString();
				}
			}
			outType = "void";
			return string.Empty;
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



		private void CollectAllInterfaces(Compilation Compilation)
		{

			foreach (var item in NamedSymbolHelper.CollectAllInterfaces(Compilation))
			{
				//检测是否继承忽略接口
				if (NamedSymbolHelper.CheckInterface(item, GeneratorHelper.ISourceGeneratorIgnore, out _)) continue;

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






	}
}
