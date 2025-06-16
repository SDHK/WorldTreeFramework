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
	public static class RuleListCallRuleGeneratorHelper
	{
		public static void Execute(GeneratorExecutionContext context)
		{
			int argumentCount = ProjectGeneratorSetting.ArgumentCount;
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
							public static OutT Call<R{{genericsType}}, OutT>(this IRuleList<R> iRuleList, INode node{{genericTypeParameter}}, out OutT outT)
								where R : ICallRule<{{genericsTypeAfter}}OutT>
							{
								RuleList ruleList = (RuleList)iRuleList;
								outT = default(OutT);
								for(int i = 0; i < ruleList.Count; i++)
								{
									outT = ((ICallRule<{{genericsTypeAfter}}OutT>)ruleList[i]).Invoke(node{{genericParameter}});
								}
								return outT;
							}

							/// <summary>
							/// 法则列表调用执行
							/// </summary>
							public static TreeList<OutT> Calls<R{{genericsType}}, OutT>(this IRuleList<R> iRuleList, INode node{{genericTypeParameter}}, TreeList<OutT> outT)
								where R : ICallRule<{{genericsTypeAfter}}OutT>
							{
								RuleList ruleList = (RuleList)iRuleList;
								for(int i = 0; i < ruleList.Count; i++)
								{
									outT.Add(((ICallRule<{{genericsTypeAfter}}OutT>)ruleList[i]).Invoke(node{{genericParameter}}));
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