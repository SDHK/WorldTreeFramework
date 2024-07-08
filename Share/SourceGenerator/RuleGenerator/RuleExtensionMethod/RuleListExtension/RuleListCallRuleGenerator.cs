/****************************************

* 作者：闪电黑客
* 日期：2024/4/8 12:10

* 描述：

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WorldTree.SourceGenerator
{
	internal class RuleListCallRuleGenerator
	{
		public static void Execute(GeneratorExecutionContext context)
		{
			int argumentCount = RuleGeneratorSetting.argumentCount;
			StringBuilder Code = new StringBuilder();
			Code.AppendLine(
@$"/****************************************
* 通知法则列表执行
*/
"
);
			Code.AppendLine("namespace WorldTree");
			Code.AppendLine("{");
			Code.AppendLine("	public static class RuleListCallRule");
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
							/// 法则列表调用执行
							/// </summary>
							public static OutT Call<R{{genericsType}}, OutT>(this IRuleList<R> ruleList, INode node{{genericTypeParameter}}, out OutT outT)
								where R : ICallRule<{{genericsTypeAfter}}OutT>
							{
								outT = TypeInfo<OutT>.Default;
								foreach (ICallRule<{{genericsTypeAfter}}OutT> rule in (RuleList)ruleList)
								{
									outT = rule.Invoke(node{{genericParameter}});
								}
								return outT;
							}

							/// <summary>
							/// 法则列表调用执行
							/// </summary>
							public static TreeList<OutT> Calls<R{{genericsType}}, OutT>(this IRuleList<R> ruleList, INode node{{genericTypeParameter}}, TreeList<OutT> outT)
								where R : ICallRule<{{genericsTypeAfter}}OutT>
							{
								foreach (ICallRule<{{genericsTypeAfter}}OutT> rule in  (RuleList)ruleList)
								{
									outT.Add(rule.Invoke(node{{genericParameter}}));
								}
								return outT;
							}
					""");
			}
			Code.AppendLine("	}");
			Code.Append("}");

			context.AddSource("RuleListCallRule.cs", SourceText.From(Code.ToString(), Encoding.UTF8));
		}
	}
}