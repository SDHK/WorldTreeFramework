/****************************************

* 作者：闪电黑客
* 日期：2024/4/3 11:58

* 描述：通知法则基类 生成器

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WorldTree.SourceGenerator
{
	internal static class SendRuleBaseGenerator
	{
		public static void Execute(GeneratorExecutionContext context)
		{
			int argumentCount = RuleGeneratorSetting.argumentCount;
			StringBuilder Code = new StringBuilder();
			Code.AppendLine(
@$"/****************************************
* 可以理解为Node的无返回值扩展方法
*
* ISendRule 继承 IRule
* 主要作用：统一 调用方法 Invoke(INode self,T1 arg1, ...);
*
* SendRule 则继承 Rule
* 同时还继承了 ISendRule 可以转换为 ISendRule 进行统一调用。
*
* 主要作用：确定Node的类型并转换，并统一 Invoke 中转调用 Execute 的过程。
* 其中 Invoke 设定为虚方法方便子类写特殊的中转调用。
*/
"
);
			Code.AppendLine("namespace WorldTree");
			Code.Append("{");

			for (int i = 0; i <= argumentCount; i++)
			{
				string generics = RuleGeneratorHelper.GetGenerics(i);
				string genericsAngle = RuleGeneratorHelper.GetGenericsAngle(i);
				string GenericTypeParameter = RuleGeneratorHelper.GetGenericTypeParameter(i);
				string GenericParameter = RuleGeneratorHelper.GetGenericParameter(i);

				Code.Append
($@"

	/// <summary>
	/// 通知法则基类接口
	/// </summary>
	public interface ISendRule{genericsAngle} : IRule
	{{
		void Invoke(INode self{GenericTypeParameter});
	}}

	/// <summary>
	/// 通知法则基类
	/// </summary>
    public abstract class SendRule<N, R{generics}> : Rule<N, R>, ISendRule{genericsAngle}
		where N : class, INode, AsRule<R>
		where R : ISendRule{genericsAngle}
	{{
		public virtual void Invoke(INode self{GenericTypeParameter}) => Execute(self as N{GenericParameter});
		protected abstract void Execute(N self{GenericTypeParameter});
	}}
");
			}

			Code.Append("}");

			context.AddSource("SendRuleBase.cs", SourceText.From(Code.ToString(), System.Text.Encoding.UTF8));//生成代码
		}
	}
}