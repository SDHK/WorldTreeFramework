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
	internal static class SendRuleBaseGeneratorHelper
	{
		public static void Execute(GeneratorExecutionContext context)
		{
			int argumentCount = RuleGeneratorSetting.argumentCount;
			StringBuilder Code = new StringBuilder();
			Code.AppendLine(
@$"/****************************************
* 可以理解为Node的无返回值扩展方法
*
* ISendRuleBase 继承 IRule
* 主要作用：统一 调用方法 Invoke(INode self,T1 arg1, ...);
*
* SendRuleBase 则继承 RuleBase
* 同时还继承了 ISendRuleBase 可以转换为 ISendRuleBase 进行统一调用。
*
* 主要作用：确定Node的类型并转换，并统一 Invoke 中转调用 Execute 的过程。
* 其中 Invoke 设定为虚方法方便子类写特殊的中转调用。
*/
"
);

			Code.AppendLine("namespace WorldTree");
			Code.AppendLine("{");

			for (int i = 0; i <= argumentCount; i++)
			{
				string genericsAngle = RuleGeneratorHelper.GetGenericsAngle(i);
				string GenericTypeParameter = RuleGeneratorHelper.GetGenericTypeParameter(i);

				Code.AppendLine("    /// <summary>");
				Code.AppendLine("    /// 通知法则基类接口");
				Code.AppendLine("    /// </summary>");
				Code.AppendLine($"    public interface ISendRuleBase{genericsAngle} : IRule");
				Code.AppendLine("    {");
				Code.AppendLine($"        void Invoke(INode self{GenericTypeParameter});");
				Code.AppendLine("    }");
			}

			for (int i = 0; i <= argumentCount; i++)
			{
				string generics = RuleGeneratorHelper.GetGenerics(i);
				string genericsAngle = RuleGeneratorHelper.GetGenericsAngle(i);
				string GenericTypeParameter = RuleGeneratorHelper.GetGenericTypeParameter(i);
				string GenericParameter = RuleGeneratorHelper.GetGenericParameter(i);

				Code.AppendLine("    /// <summary>");
				Code.AppendLine("    /// 通知法则基类");
				Code.AppendLine("    /// </summary>");
				Code.AppendLine($"    public abstract class SendRuleBase<N, R{generics}> : RuleBase<N, R>, ISendRuleBase{genericsAngle}");
				Code.AppendLine("        where N : class, INode, AsRule<R>");
				Code.AppendLine($"        where R : ISendRuleBase{genericsAngle}");
				Code.AppendLine("    {");

				Code.AppendLine($"        public virtual void Invoke(INode self{GenericTypeParameter}) => Execute(self as N{GenericParameter});");
				Code.AppendLine($"        protected abstract void Execute(N self{GenericTypeParameter});");
				Code.AppendLine("    }");
			}

			Code.AppendLine("}");

			context.AddSource("SendRuleBase.cs", SourceText.From(Code.ToString(), System.Text.Encoding.UTF8));//生成代码
		}
	}
}