/****************************************

* 作者：闪电黑客
* 日期：2024/4/15 17:09

* 描述：

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WorldTree.SourceGenerator
{
	internal class NodeBranchExtensionGeneratorHelper
	{
		public static void Execute(GeneratorExecutionContext context)
		{
			int argumentCount = GeneratorSetting.argumentCount;
			StringBuilder Code = new StringBuilder();
			Code.AppendLine(
@$"/****************************************
* 节点添加到分支帮助方法
*/
"
);
			Code.AppendLine("namespace WorldTree");
			Code.AppendLine("{");
			Code.AppendLine("	public static partial class NodeBranchHelper");
			Code.Append("	{");

			for (int i = 0; i <= argumentCount; i++)
			{
				string genericsType = GeneratorTemplate.GenericsTypes[i];
				string genericsTypeAngle = GeneratorTemplate.GenericsTypesAngle[i];
				string genericParameter = GeneratorTemplate.GenericsParameter[i];
				string genericTypeParameter = GeneratorTemplate.GenericsTypeParameter[i];

				Code.AppendLine(
					$$"""

							/// <summary>
							/// 节点加入树结构，框架内部使用
							/// </summary>
							public static INode AddNodeToTree<B, K{{genericsType}}>(INode self, B nullBranch, K key, INode node{{genericTypeParameter}})
								where B : class, IBranch<K>
							{
								if (node.TryAddSelfToTree<B, K>(key, self))
								{
									NodeRuleHelper.TrySendRule(node, default(Awake{{genericsTypeAngle}}){{genericParameter}});
									node.OnAddSelfToTree();
								}
								return node;
							}

							/// <summary>
							/// 添加节点，无约束
							/// </summary>
							public static INode AddNode<B, K{{genericsType}}>(INode self, B nullBranch, K key, long type, out INode node{{genericTypeParameter}})
								where B : class, IBranch<K>
							=> node = GetBranch<B>(self)?.GetNode(key) ?? AddNodeToTree(self, nullBranch, key, self.Core.PoolGetNode(type){{genericParameter}});

							/// <summary>
							/// 添加泛型节点
							/// </summary>
							public static T AddNode<N, B, K, T{{genericsType}}>(N self, B nullBranch, K key, out T node{{genericTypeParameter}})
								where N : class, INode, AsBranch<B>
								where B : class, IBranch<K>
								where T : class, INode, NodeOf<N, B>, AsRule<Awake{{genericsTypeAngle}}>
							=> node = (T)(GetBranch<B>(self)?.GetNode(key) ?? AddNodeToTree(self, nullBranch, key, self.Core.PoolGetNode<T>(){{genericParameter}}));
					""");
			}
			Code.AppendLine("	}");
			Code.Append("}");

			context.AddSource("NodeBranchHelper.cs", SourceText.From(Code.ToString(), Encoding.UTF8));
		}
	}
}