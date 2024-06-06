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
				string genericsType = GeneratorTemplate.GenericsTypes[i];
				string genericsTypeAngle = GeneratorTemplate.GenericsTypesAngle[i];

				Code.Append(
					$$"""

						/// <summary>
						/// 法则约束：初始化法则
						/// </summary>
						public interface AsAwake{{genericsTypeAngle}} : AsRule<Awake{{genericsTypeAngle}}>, INode {}

						/// <summary>
						/// 初始化法则
						/// </summary>
						public interface Awake{{genericsTypeAngle}} : ISendRule{{genericsTypeAngle}} {}

						/// <summary>
						/// 初始化法则
						/// </summary>
						public abstract class AwakeRule<N{{genericsType}}> : SendRule<N, Awake{{genericsTypeAngle}}{{genericsType}}> where N : class, INode, AsRule<Awake{{genericsTypeAngle}}> {}
					""");
			}
			Code.Append("}");

			context.AddSource("AwakeRule.cs", SourceText.From(Code.ToString(), System.Text.Encoding.UTF8));
		}
	}
}