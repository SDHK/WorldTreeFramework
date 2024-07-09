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
			int argumentCount = RuleGeneratorSetting.argumentCount;
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
							public static INode AddSelfToTree<B, K{{genericsType}}>(INode self, K key, INode parent{{genericTypeParameter}})
								where B : class, IBranch<K>
							{
								if (self.TryAddSelfToTree<B, K>(key, parent))
								{
									NodeRuleHelper.TrySendRule(self, TypeInfo<Awake{{genericsTypeAngle}}>.Default{{genericParameter}});
									self.OnAddSelfToTree();
								}
								return self;
							}

							/// <summary>
							/// 添加节点，无约束
							/// </summary>
							public static INode AddNode<B, K{{genericsType}}>(INode self, K key, long type, out INode node{{genericTypeParameter}})
								where B : class, IBranch<K>
							=> node = self.GetBranch<B>()?.GetNode(key) ?? NodeBranchHelper.AddSelfToTree<B, K{{genericsType}}>(self.Core.PoolGetNode(type),key, self{{genericParameter}});

							/// <summary>
							/// 添加泛型节点
							/// </summary>
							public static T AddNode<N, B, K, T{{genericsType}}>(N self, K key, out T node{{genericTypeParameter}})
								where N : class, INode, AsBranch<B>
								where B : class, IBranch<K>
								where T : class, INode, NodeOf<N, B>, AsRule<Awake{{genericsTypeAngle}}>
							=> node = (T)(self.GetBranch<B>()?.GetNode(key) ?? NodeBranchHelper.AddSelfToTree<B, K{{genericsType}}>(self.Core.PoolGetNode<T>(),key, self{{genericParameter}}));
					""");
			}
			Code.AppendLine("	}");
			Code.Append("}");

			context.AddSource("NodeBranchHelper.cs", SourceText.From(Code.ToString(), Encoding.UTF8));
		}
	}
}