/****************************************

* 作者：闪电黑客
* 日期：2024/4/9 18:01

* 描述：

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WorldTree.SourceGenerator
{
	internal class RuleGroupCallRuleGenerator
	{
		public static void Execute(GeneratorExecutionContext context)
		{
			int argumentCount = RuleGeneratorSetting.argumentCount;
			StringBuilder Code = new StringBuilder();
			Code.AppendLine(
@$"/****************************************
* 尝试调用法则集合执行
*/
"
);
			Code.AppendLine("namespace WorldTree");
			Code.AppendLine("{");
			Code.AppendLine("	public static class RuleGroupCallRule");
			Code.Append("	{");

			for (int i = 0; i <= argumentCount; i++)
			{

				string genericsType = GeneratorTemplate.GenericsTypes[i];
				string genericsTypeAfter = GeneratorTemplate.GenericsTypesAfter[i];
				string genericParameter = GeneratorTemplate.GenericsParameter[i];
				string genericTypeParameter = GeneratorTemplate.GenericsTypeParameter[i];
			
				Code.AppendLine(
					$$"""

							/// <summary>
							/// 尝试调用法则集合执行
							/// </summary>
							public static bool TryCall<R{{genericsType}}, OutT>(this IRuleGroup<R> group, INode node{{genericTypeParameter}}, out OutT outT)
								where R : ICallRule<{{genericsTypeAfter}}OutT>
							{
								if (((RuleGroup)group).TryGetValue(node.Type, out RuleList ruleList))
								{
									((IRuleList<R>)ruleList).Call(node{{genericParameter}}, out outT);
									return true;
								}
								outT = TypeInfo<OutT>.Default;
								return false;
							}

							/// <summary>
							/// 调用法则集合执行
							/// </summary>
							public static OutT Call<R{{genericsType}}, OutT>(this IRuleGroup<R> group, INode node{{genericTypeParameter}}, out OutT outT)
								where R : ICallRule<{{genericsTypeAfter}}OutT>
							{
								group.TryCall(node{{genericParameter}}, out outT);
								return outT;
							}

							/// <summary>
							/// 尝试调用法则集合执行
							/// </summary>
							public static bool TryCalls<R{{genericsType}}, OutT>(this IRuleGroup<R> group, INode node{{genericTypeParameter}}, TreeList<OutT> outTList)
								where R : ICallRule<{{genericsTypeAfter}}OutT>
							{
								if ((group as RuleGroup).TryGetValue(node.Type, out RuleList ruleList))
								{
									((IRuleList<R>)ruleList).Calls(node{{genericParameter}}, outTList);
									return true;
								}
								return true;
							}

							/// <summary>
							/// 调用法则集合执行
							/// </summary>
							public static TreeList<OutT> Calls<R{{genericsType}}, OutT>(this IRuleGroup<R> group, INode node{{genericTypeParameter}}, TreeList<OutT> outTList)
								where R : ICallRule<{{genericsTypeAfter}}OutT>
							{
								group.TryCalls(node{{genericParameter}}, outTList);
								return outTList;
							}
					""");
			}
			Code.AppendLine("	}");
			Code.Append("}");

			context.AddSource("RuleGroupCallRule.cs", SourceText.From(Code.ToString(), Encoding.UTF8));
		}
	}
}