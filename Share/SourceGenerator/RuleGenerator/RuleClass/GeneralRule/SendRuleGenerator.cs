/****************************************

* 作者：闪电黑客
* 日期：2024/4/7 19:55

* 描述：通用通知法则 生成器帮助类

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WorldTree.SourceGenerator
{
	internal class SendRuleGenerator
	{
		public static void Execute(GeneratorExecutionContext context)
		{
			int argumentCount = RuleGeneratorSetting.argumentCount;
			StringBuilder Code = new StringBuilder();
			Code.AppendLine(
@$"/****************************************
* 通用通知法则
*/
"
);
			Code.AppendLine("namespace WorldTree");
			Code.Append("{");

			for (int i = 0; i <= argumentCount; i++)
			{
				string generics = RuleGeneratorHelper.GetGenerics(i);
				string genericsAngle = RuleGeneratorHelper.GetGenericsAngle(i);
				Code.Append
($@"

	/// <summary>
	/// 节点：通用通知法则限制
	/// </summary>
	/// <remarks>节点拥有的法则，和Where约束搭配形成法则调用限制</remarks>
    public interface AsSendRule{genericsAngle} : AsRule<ISendRule{genericsAngle}> {{}}

    /// <summary>
    /// 通用通知法则接口
    /// </summary>
    public interface ISendRule{genericsAngle} : ISendRuleBase{genericsAngle} {{}}

    /// <summary>
    /// 通用通知法则
    /// </summary>
    public abstract class SendRule<N{generics}> : SendRuleBase<N, ISendRule{genericsAngle}{generics}> where N : class, INode, AsRule<ISendRule{genericsAngle}> {{}}
");
			}
			Code.Append("}");

			context.AddSource("SendRule.cs", SourceText.From(Code.ToString(), System.Text.Encoding.UTF8));
		}
	}
}