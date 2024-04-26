/****************************************

* 作者：闪电黑客
* 日期：2024/4/20 15:05

* 描述：

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WorldTree.SourceGenerator
{
	internal class AwakeRuleGenerator
	{
		public static void Execute(GeneratorExecutionContext context)
		{
			int argumentCount = RuleGeneratorSetting.argumentCount;
			StringBuilder Code = new StringBuilder();
			Code.AppendLine(
@$"/****************************************
* 初始化法则
* 用于节点Add添加到世界树上时的构造参数传递
* 在OnGet与OnAdd之间执行，在全局之前广播前执行。
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
	/// 节点：初始化法则限制
	/// </summary>
	/// <remarks>节点拥有的法则，和Where约束搭配形成法则调用限制</remarks>
    public interface AsAwakeRule{genericsAngle} : AsRule<IAwakeRule{genericsAngle}>, INode {{}}

    /// <summary>
    /// 初始化法则接口
    /// </summary>
    public interface IAwakeRule{genericsAngle} : ISendRuleBase{genericsAngle} {{}}

    /// <summary>
    /// 初始化法则
    /// </summary>
    public abstract class AwakeRule<N{generics}> : SendRuleBase<N, IAwakeRule{genericsAngle}{generics}> where N : class, INode, AsRule<IAwakeRule{genericsAngle}> {{}}
");
			}
			Code.Append("}");

			context.AddSource("AwakeRule.cs", SourceText.From(Code.ToString(), System.Text.Encoding.UTF8));
		}
	}
}