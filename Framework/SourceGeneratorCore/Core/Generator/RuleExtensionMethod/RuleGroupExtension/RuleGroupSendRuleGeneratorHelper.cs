﻿/****************************************

* 作者：闪电黑客
* 日期：2024/4/8 18:07

* 描述：

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WorldTree.SourceGenerator
{

	public static class RuleGroupSendRuleGeneratorHelper
	{
		public static void Execute(GeneratorExecutionContext context)
		{
			int argumentCount = ProjectGeneratorSetting.ArgumentCount;
			StringBuilder Code = new StringBuilder();
			Code.AppendLine(
@$"/****************************************
* 通知法则集合执行
*/
"
);
			Code.AppendLine("namespace WorldTree");
			Code.AppendLine("{");
			Code.AppendLine("	public static class RuleGroupSendRule");
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
							/// 尝试通知法则集合执行
							/// </summary>
							public static bool TrySend<R{{genericsType}}>(this IRuleGroup<R> group, INode node{{genericTypeParameter}})
								where R : ISendRule{{genericsTypeAngle}}
							{
								if (!((RuleGroup)group).TryGetValue(node.Type, out RuleList ruleList)) return false;
								((IRuleList<R>)ruleList).Send(node{{genericParameter}});
								return true;
							}

							/// <summary>
							/// 通知法则集合执行
							/// </summary>
							public static void Send<R{{genericsType}}>(this IRuleGroup<R> group, INode node{{genericTypeParameter}})
								where R : ISendRule{{genericsTypeAngle}}
							{
								group.TrySend(node{{genericParameter}});
							}
					""");
			}

			Code.AppendLine("	}");
			Code.Append("}");

			context.AddSource("RuleGroupSendRule.cs", SourceText.From(Code.ToString(), Encoding.UTF8));
		}
	}
}