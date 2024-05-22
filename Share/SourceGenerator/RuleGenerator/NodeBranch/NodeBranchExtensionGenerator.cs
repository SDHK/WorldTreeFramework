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
	internal class NodeBranchExtensionGenerator
	{
		public static void Execute(GeneratorExecutionContext context)
		{
			int argumentCount = RuleGeneratorSetting.argumentCount;
			StringBuilder Code = new StringBuilder();
			Code.AppendLine(
@$"/****************************************
* 节点添加到分支扩展方法
*/
"
);
			Code.AppendLine("namespace WorldTree");
			Code.AppendLine("{");
			Code.AppendLine("	public static class NodeBranchExtension");
			Code.Append("	{");

			for (int i = 0; i <= argumentCount; i++)
			{
				string generics = RuleGeneratorHelper.GetGenerics(i);
				string genericsAngle = RuleGeneratorHelper.GetGenericsAngle(i);
				string genericParameter = RuleGeneratorHelper.GetGenericParameter(i);
				string genericTypeParameter = RuleGeneratorHelper.GetGenericTypeParameter(i);
				Code.Append
($@"

		/// <summary>
		/// 节点加入树结构
		/// </summary>
		public static INode AddSelfToTree<B, K{generics}>(this INode self, K key, INode parent{genericTypeParameter})
			where B : class, IBranch<K>
		{{
			if (self.TryAddSelfToTree<B, K>(key, parent))
			{{
				NodeRuleHelper.TrySendRule(self, TypeInfo<Awake{genericsAngle}>.Default{genericParameter});
				self.OnAddSelfToTree();
			}}
			return self;
		}}

		/// <summary>
		/// 添加节点
		/// </summary>
		public static INode AddNode<B, K{generics}>(this INode self, K key, long type, out INode node{genericTypeParameter}, bool isPool = true)
			where B : class, IBranch<K>
		=> node = NodeBranchHelper.GetBranch<B>(self)?.GetNode(key) ?? self.GetOrNewNode(type, isPool).AddSelfToTree<B, K{generics}>(key, self{genericParameter});

		/// <summary>
		/// 添加泛型节点
		/// </summary>
		public static T AddNode<N, B, K, T{generics}>(this N self, K key, out T node{genericTypeParameter}, bool isPool = true)
			where N : class, INode, AsBranch<B>
			where B : class, IBranch<K>
			where T : class, INode, NodeOf<N, B>, AsRule<Awake{genericsAngle}>
		=> node = (T)(NodeBranchHelper.GetBranch<B>(self)?.GetNode(key) ?? self.GetOrNewNode<T>(isPool).AddSelfToTree<B, K{generics}>(key, self{genericParameter}));
");
			}
			Code.AppendLine("	}");
			Code.Append("}");

			context.AddSource("NodeBranchExtension.cs", SourceText.From(Code.ToString(), Encoding.UTF8));
		}
	}
}