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
using System.Linq;
using System.Security;
using System.Text;

namespace WorldTree.SourceGenerator
{
	public abstract class NodeLinkRemarksGenerator<C> : SourceGeneratorBase<C>
		where C : ProjectGeneratorsConfig, new()
	{
		public Dictionary<string, HashSet<ClassDeclarationSyntax>> ClassDeclarations = new();
		public Dictionary<string, string> Usings = new();

		/// <summary>
		/// 类型，分支，子级
		/// </summary>
		private Dictionary<string, Dictionary<INamedTypeSymbol, HashSet<INamedTypeSymbol>>> nodes = new();

		/// <summary>
		/// 父级类型映射,子级，分支，父级
		/// </summary>
		private Dictionary<string, Dictionary<INamedTypeSymbol, HashSet<string>>> Parents = new();
		public override void Initialize(GeneratorInitializationContext context)
		{
		}

		public void GetClassDeclarations(GeneratorExecutionContext context)
		{
			// 清理旧数据
			ClassDeclarations.Clear();
			Usings.Clear();

			// 遍历所有语法树，收集继承了INode的部分类
			foreach (var tree in context.Compilation.SyntaxTrees)
			{
				var semanticModel = context.Compilation.GetSemanticModel(tree);
				var classDeclarations = tree.GetRoot()
					.DescendantNodes()
					.OfType<ClassDeclarationSyntax>();
				foreach (var classDecl in classDeclarations)
				{
					// 跳过抽象类
					if (classDecl.Modifiers.Any(m => m.IsKind(SyntaxKind.AbstractKeyword))) continue;
					// 仅收集部分类
					if (!classDecl.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword))) continue;

					var symbol = semanticModel.GetDeclaredSymbol(classDecl) as INamedTypeSymbol;
					if (symbol == null) continue;

					// 检查是否实现了INode接口
					if (!symbol.AllInterfaces.Any(i => i.Name == "INode")) continue;

					// 获取文件名（不含扩展名）
					var fileName = System.IO.Path.GetFileNameWithoutExtension(tree.FilePath);
					if (!ClassDeclarations.TryGetValue(fileName, out var classSet))
					{
						classSet = new HashSet<ClassDeclarationSyntax>();
						ClassDeclarations[fileName] = classSet;
					}
					classSet.Add(classDecl);
					var usings = TreeSyntaxHelper.GetUsings(classDecl);
					Usings[fileName] = usings;
				}
			}
		}


		public override void ExecuteCore(GeneratorExecutionContext context)
		{
			//收集所有继承了INode的类型
			GetClassDeclarations(context);

			if (ClassDeclarations.Count == 0) return;
			Parents.Clear();
			nodes.Clear();

			INamedTypeSymbol INodeSymbol = NamedSymbolHelper.ToINamedTypeSymbol(context.Compilation, GeneratorHelper.INode);
			INamedTypeSymbol NodeOfSymbol = NamedSymbolHelper.ToINamedTypeSymbol(context.Compilation, GeneratorHelper.NodeOf);

			foreach (INamedTypeSymbol item in NamedSymbolHelper.CollectAllClass(context.Compilation))
			{
				// 检查类是否是抽象类
				if (item.IsAbstract) continue;
				// 检查类是否继承自INode
				if (!NamedSymbolHelper.TryGetInterfaceName(item, "INode", out _)) continue;
				// 检查类是否继承自NodeOf的子类
				if (!NamedSymbolHelper.TryGetInterfacesName(item, "NodeOf", out List<INamedTypeSymbol> NodeOfs)) continue;

				foreach (INamedTypeSymbol NodeOf in NodeOfs)
				{
					// 获取NodeOf的类型参数
					if (NodeOf.TypeArguments.Length != 2) continue;
					INamedTypeSymbol nodeType = NodeOf.TypeArguments[0] as INamedTypeSymbol;
					if (nodeType == null) continue;
					INamedTypeSymbol branchType = NodeOf.TypeArguments[1] as INamedTypeSymbol;
					if (branchType == null) continue;
					string branchTypeName = branchType.ToDisplayString();
					string nodeTypeName = nodeType.ToDisplayString();
					string itemName = item.ToDisplayString();

					//检查是否已经存在该节点类型
					if (!nodes.TryGetValue(nodeTypeName, out var branchDict))
					{
						nodes[nodeTypeName] = branchDict = [];
					}
					// 检查是否已经存在该分支类型
					if (!branchDict.TryGetValue(branchType, out var nodeSet))
					{
						branchDict[branchType] = nodeSet = [];
					}
					//添加节点类型到分支类型中
					if (!nodeSet.Contains(item)) nodeSet.Add(item);

					// 记录父级类型
					if (!Parents.TryGetValue(itemName, out var parentSet))
					{
						Parents[itemName] = parentSet = new();
					}
					// 添加父级类型
					if (!parentSet.TryGetValue(branchType, out var parentHashSet))
					{
						parentSet[branchType] = parentHashSet = new();
					}
					// 添加父级类型到父级集合中
					if (!parentHashSet.Contains(nodeTypeName)) parentHashSet.Add(nodeTypeName);
				}
			}

			StringBuilder Code = new StringBuilder();
			if (nodes.Count == 0) return;

			foreach (KeyValuePair<string, HashSet<ClassDeclarationSyntax>> classDeclarationsGroup in ClassDeclarations)
			{
				Code.Clear();
				if (classDeclarationsGroup.Value.Count == 0) continue;

				Code.AppendLine(
@$"/****************************************
* 对Node关系注释追加生成
*/
"
);
				string fileName = classDeclarationsGroup.Key;

				Usings.TryGetValue(classDeclarationsGroup.Key, out string usings);
				string Namespace = TreeSyntaxHelper.GetNamespace(classDeclarationsGroup.Value.FirstOrDefault());
				Code.AppendLine(usings);
				Code.AppendLine($"namespace {Namespace}");
				Code.AppendLine("{");

				foreach (ClassDeclarationSyntax NodeClassDeclaration in classDeclarationsGroup.Value)
				{
					// 检查类是否是部分类
					if (!NodeClassDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword)) continue;
					// NodeClassDeclaration 转为 INamedTypeSymbol
					INamedTypeSymbol nodeType = context.Compilation.GetSemanticModel(NodeClassDeclaration.SyntaxTree).GetDeclaredSymbol(NodeClassDeclaration) as INamedTypeSymbol;

					Parents.TryGetValue(nodeType.ToDisplayString(), out var ParentBranch);
					nodes.TryGetValue(nodeType.ToDisplayString(), out var NodeBranch);
					if ((ParentBranch == null || ParentBranch.Count == 0) && (NodeBranch == null || NodeBranch.Count == 0)) continue;
					Code.AppendLine($"	/// <remarks>");
					if (ParentBranch != null && ParentBranch.Count != 0)
					{
						//Code.AppendLine($"	/// <para>父级</para>");
						foreach (var parentItem in ParentBranch)
						{
							string ParentBranchType = parentItem.Key.ToDisplayString();
							string branchKeyType = GetBranchKeyType(context.Compilation, parentItem.Key);
							var parentNodes = parentItem.Value;
							if (parentNodes.Count == 0) continue;
							Code.AppendLine($"	/// <para>父级： <see cref=\"{SecurityElement.Escape(ParentBranchType)}\"/> <see cref=\"{SecurityElement.Escape(branchKeyType)}\"/></para>");
							foreach (var parentNode in parentNodes)
							{
								Code.AppendLine($"	/// <para><see cref=\"{SecurityElement.Escape(parentNode)}\"/> </para>");
							}
						}
					}

					if (NodeBranch != null && NodeBranch.Count != 0)
					{
						//Code.AppendLine($"	/// <para>子级</para>");
						// 检查是否存在该节点类型
						foreach (var item in NodeBranch)
						{
							string branchType = item.Key.ToDisplayString();
							string branchKeyType = GetBranchKeyType(context.Compilation, item.Key);
							Code.AppendLine($"	/// <para>子级： <see cref=\"{SecurityElement.Escape(branchType)}\"/> <see cref=\"{SecurityElement.Escape(branchKeyType)}\"/></para>");
							foreach (var node in item.Value)
							{
								Code.AppendLine($"	/// <para><see cref=\"{SecurityElement.Escape(node.ToDisplayString())}\"/> </para>");
							}
						}
					}
					Code.AppendLine($"	/// </remarks>");
					Code.AppendLine($"	public partial class {nodeType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}; //结构注释");
				}
				Code.AppendLine("}");
				context.AddSource($"NodeLinkRemark_{fileName}.cs", SourceText.From(Code.ToString(), Encoding.UTF8));//生成代码
			}
		}

		/// <summary>
		/// 获取分支类型的键类型
		/// </summary>
		private static string GetBranchKeyType(Compilation compilation, INamedTypeSymbol branchType)
		{
			INamedTypeSymbol IBranchTypeSymbol = NamedSymbolHelper.ToINamedTypeSymbol(compilation, GeneratorHelper.IBranch_1);
			if (!NamedSymbolHelper.IsDerivedFrom(branchType, IBranchTypeSymbol, out INamedTypeSymbol baseTypeSymbol, TypeCompareOptions.CompareToGenericTypeDefinition)) return string.Empty;

			// 获取分支类型的名称
			if (baseTypeSymbol.TypeArguments.Length > 0)
			{
				return baseTypeSymbol.TypeArguments[0].ToDisplayString(); // 返回第一个类型参数的名称
			}
			return string.Empty; // 如果没有类型参数，返回分支类型的名称
		}
	}
}