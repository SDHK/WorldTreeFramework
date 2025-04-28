/****************************************

* 作者：闪电黑客
* 日期：2024/6/6 10:28

* 描述：对于法则委托简写的调用生成

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
	[Generator]
	public class RuleExecuteGenerator : ISourceGenerator
	{
		private List<FieldDeclarationSyntax> delegateInstances = new();

		/// <summary>
		/// 接口名称,泛型数量,法则类型, 法则基类类型，Switch类型
		/// </summary>
		private Dictionary<string, Dictionary<int, (INamedTypeSymbol, INamedTypeSymbol, RuleBaseEnum)>> RuleTypes = new();

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

			CollectAllInterfaces(context.Compilation);

			foreach (var syntaxTree in context.Compilation.SyntaxTrees)
			{
				var semanticModel = context.Compilation.GetSemanticModel(syntaxTree);
				CollectPrivateStaticField(syntaxTree, semanticModel);

				//拿到所有的委托实例的类型
				foreach (FieldDeclarationSyntax delegateInstance in delegateInstances)
				{
					var typeSymbol = semanticModel.GetTypeInfo(delegateInstance.Declaration.Type).Type;
					INamedTypeSymbol delegateType = (INamedTypeSymbol)typeSymbol;

					//委托泛型数量,如果是-1则表示不是委托，因为生成的委托是有泛型的
					var TypeCount = delegateType?.TypeArguments.Count() ?? 0;
					if (TypeCount == 0) continue;

					//判断是否和接口名称一致
					if (!RuleTypes.TryGetValue(delegateType.Name, out var Count_Type)) continue;

					//判断是否有对应的泛型数量
					if (!Count_Type.TryGetValue(TypeCount - 1, out _)) continue;

					//拿到所在的文件名
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
					string delegateFieldName = delegateInstance?.Declaration?.Variables.First().Identifier.Text;
					if (delegateFieldName == null) continue;
					set.Add((delegateFieldName, delegateType, delegateInstance));
				}
			}

			foreach (var fileClassList in fileClassDict)
			{
				StringBuilder fileCode = new();
				StringBuilder ClassCode = new();
				string className = null;

				//委托字段名称，委托类型包括泛型，委托实例
				foreach ((string, INamedTypeSymbol, FieldDeclarationSyntax) delegateType in fileClassList.Value)
				{
					//委托类型全名包括泛型
					var delegateClassName = delegateType.Item2?.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
					FieldDeclarationSyntax delegateInstance = delegateType.Item3;
					var TypeCount = delegateType.Item2?.TypeArguments.Count() ?? 0;
					if (TypeCount == 0) continue;
					if (!RuleTypes.TryGetValue(delegateType.Item2.Name, out var Count_Type)) continue;
					if (!Count_Type.TryGetValue(TypeCount - 1, out var ruleTypeData)) continue;

					//获取委托所在的静态兄弟类类名,只获取第一个，默认委托都在一个类中，并且一个文件只有一个静态类
					className ??= delegateInstance.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First().Identifier.Text;

					//获取委托字段的名称
					var delegateFieldName = delegateType.Item1;

					//组装法则类型名称
					string RuleName = delegateClassName.Replace("On", "");
					int index = RuleName.IndexOf('<');
					string RuleFullName = RuleName.Insert(index, "Rule");
					string NodeName = RuleName.Split('<')[1].Split(',')[0].TrimEnd('>');

					INamedTypeSymbol ruleType = ruleTypeData.Item1;
					INamedTypeSymbol baseInterface = ruleTypeData.Item2;
					RuleBaseEnum ruleTypeCode = ruleTypeData.Item3;
					string[] types = null;
					types = GetReflectionRuleType(delegateType.Item2, ruleType, baseInterface);
					bool isCall = ruleTypeCode is RuleBaseEnum.CallRule or RuleBaseEnum.CallRuleAsync;
					string genericTypeParameter = GetRuleTypeParameter(types, isCall, out string outType);
					string genericParameter = GetRuleParameter(types, isCall, out _);

					switch (ruleTypeCode)
					{
						case RuleBaseEnum.SendRule:

							ClassCode.AppendLine(
							$$"""
									class {{delegateFieldName}}RuleExecute : {{RuleFullName}} { protected override void Execute({{genericTypeParameter}}) => {{delegateFieldName}}({{genericParameter}}); }
							"""
							);
							break;

						case RuleBaseEnum.SendRuleAsync:

							ClassCode.AppendLine(
							$$"""
									class {{delegateFieldName}}RuleExecute : {{RuleFullName}} { protected override TreeTask Execute({{genericTypeParameter}}) => {{delegateFieldName}}({{genericParameter}}); }
							"""
							);
							break;

						case RuleBaseEnum.CallRule:
							ClassCode.AppendLine(
							$$"""
									class {{delegateFieldName}}RuleExecute : {{RuleFullName}} { protected override {{outType}} Execute({{genericTypeParameter}}) => {{delegateFieldName}}({{genericParameter}}); }
							"""
							);
							break;

						case RuleBaseEnum.CallRuleAsync:

							ClassCode.AppendLine(
							$$"""
									class {{delegateFieldName}}RuleExecute : {{RuleFullName}} { protected override TreeTask<{{outType}}> Execute({{genericTypeParameter}}) => {{delegateFieldName}}({{genericParameter}}); }
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

					//忽略掉法则分流的委托
					if (TreeSyntaxHelper.TryGetAttribute(fieldDeclaration, GeneratorHelper.RuleSwitchAttribute, out AttributeSyntax attributeSyntax))
					{
						//continue;
					}
					foreach (var modifier in fieldDeclaration.Modifiers)
					{
						if (modifier.IsKind(SyntaxKind.StaticKeyword))
						{
							hasStaticKeyword = true;
						}
						else if (modifier.IsKind(SyntaxKind.PrivateKeyword))
						{
							hasPrivateKeyword = true;
						}
						else if (modifier.IsKind(SyntaxKind.PublicKeyword))
						{
							hasPrivateKeyword = false;
						}
						else if (modifier.IsKind(SyntaxKind.ProtectedKeyword))
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
		private void CollectAllInterfaces(Compilation Compilation)
		{
			foreach (var item in NamedSymbolHelper.CollectAllInterfaces(Compilation))
			{
				//检测是否继承4大法则接口
				INamedTypeSymbol baseInterface;
				RuleBaseEnum ruleBaseEnum;
				if (NamedSymbolHelper.CheckInterfaceName(item, GeneratorHelper.ISendRule, out baseInterface)) ruleBaseEnum = RuleBaseEnum.SendRule;
				else if (NamedSymbolHelper.CheckInterfaceName(item, GeneratorHelper.ICallRule, out baseInterface)) ruleBaseEnum = RuleBaseEnum.CallRule;
				else if (NamedSymbolHelper.CheckInterfaceName(item, GeneratorHelper.ISendRuleAsync, out baseInterface)) ruleBaseEnum = RuleBaseEnum.SendRuleAsync;
				else if (NamedSymbolHelper.CheckInterfaceName(item, GeneratorHelper.ICallRuleAsync, out baseInterface)) ruleBaseEnum = RuleBaseEnum.CallRuleAsync;
				else continue;

				string DelegateName = $"On{item.Name}";

				if (!RuleTypes.TryGetValue(DelegateName, out var Length_Type))
				{
					Length_Type = new();
					RuleTypes.Add(DelegateName, Length_Type);
				}
				if (!Length_Type.ContainsKey(item.TypeArguments.Length))
				{
					Length_Type.Add(item.TypeArguments.Length, (item, baseInterface, ruleBaseEnum));
				}
				else
				{
					Length_Type[item.TypeArguments.Length] = (item, baseInterface, ruleBaseEnum);
				}
			}
		}

		/// <summary>
		/// 泛型参数映射
		/// </summary>
		/// <param name="DelegateType">法则委托简写类型</param>
		/// <param name="IRuleClass">法则定义类型</param>
		/// <param name="IRuleBase">法则定义类型基类</param>
		/// <remarks> delegateType 类型 映射到 IRuleClass 类型，然后再映射到 IRuleBase 类型</remarks>
		private string[] GetReflectionRuleType(INamedTypeSymbol DelegateType, INamedTypeSymbol IRuleClass, INamedTypeSymbol IRuleBase)
		{
			//假设：

			// DelegateType 是实例	OnTestEvent<TestClass, int>	定义是：OnTestEvent<N, X> ，其中 TestClass 是目标不是参数
			// IRuleClass	是定义	TestNodeEvent<X>			比 OnTestEvent<N, X> 少个目标 N，其余一致
			// IRuleBase	是定义	ISendRule<TestEnum, X, List<int>>。而IRuleClass一定继承IRuleBase

			//功能是
			//OnTestEvent 剔除0号参数后，按照顺序将 int 映射到 TestNodeEvent<int>
			//然后通过泛型符号 X 映射到 ISendRule<TestEnum, int, List<int>> 
			//最后组合返回 {TestClass，TestEnum，int，List<int>} 的参数类型数组

			//===========
			// 创建一个 SymbolDisplayFormat 对象，用于自定义显示格式
			//因为委托简写可以省略写类型，导致原文件不需要写参数的命名空间，
			//所以这里要获取到泛型参数的类型全名，包括命名空间。
			var customFormat = new SymbolDisplayFormat(
			globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
			typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
			genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters);

			// 获取 delegateType 的实例 泛型类型
			string[] delegateTypeArgs = DelegateType.TypeArguments.Select(t => t.ToDisplayString(customFormat)).ToArray();

			var types = new List<string>
			{
				//0号是目标，不是法则参数，不需要映射，直接添加。
				delegateTypeArgs[0]
			};

			// 获取 IRuleBase 的泛型参数定义符号和类型。
			// 这里面同时存 泛型符号 和 类型，其中泛型符号能在IRuleClass中找到。
			string[] IRuleBaseArgs = IRuleBase.TypeArguments.Select(t => t.ToDisplayString(customFormat)).ToArray();
			if (IRuleBaseArgs == null && IRuleBaseArgs.Length == 0) return types.ToArray();

			// 获取 IRuleClass 的泛型符号。
			// 这里面只有泛型符号，例如：T,N,R 等等，绝对不存在类型
			string[] IRuleClassArgs = IRuleClass.TypeArguments.Select(t => t.Name.Trim()).ToArray();

			// 创建一个字典，将 IRuleClass 的泛型符号 映射到 delegateType 的泛型类型。
			// 下标映射，也就是用 泛型符号 对应 实例类型，因为接口和委托的参数顺序是一样的。
			Dictionary<string, string> IRuleClassToDelegateType = new();
			for (int i = 0; i < IRuleClassArgs.Length; i++) IRuleClassToDelegateType.Add(IRuleClassArgs[i], delegateTypeArgs[i + 1]);

			//按顺序查找对应 符号 的 类型，没有则直接使用 IRuleBaseArgs 的类型
			//因为IRuleBaseArgs中的符号能在IRuleClass中找到，没有就说明这是一个类型。
			for (int i = 0; i < IRuleBaseArgs.Length; i++)
			{
				var argType = IRuleBaseArgs[i];
				if (IRuleClassToDelegateType.TryGetValue(argType, out string value))
				{
					types.Add(value);
				}
				else
				{
					types.Add(argType);
				}
			}
			// 返回一个字符串，该字符串包含了这些参数类型
			return types.ToArray();
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

		/// <summary>
		/// 获取泛型参数数组
		/// </summary>
		private string[] GetGenericArguments(string ruleType)
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

	}
}