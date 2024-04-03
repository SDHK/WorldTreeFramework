/****************************************

* 作者：闪电黑客
* 日期：2024/4/3 11:58

* 描述：通知法则基类 生成器

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Reflection.Metadata;
using System.Text;

namespace WorldTree.SourceGenerator
{
	[Generator]
	internal class SendRuleBaseGenerator : ISourceGenerator
	{
		public void Initialize(GeneratorInitializationContext context)
		{
			context.RegisterForSyntaxNotifications(() => SyntaxContextReceiver.Create());
		}

		public void Execute(GeneratorExecutionContext context)
		{
			try
			{
				if (!(context.SyntaxReceiver is SyntaxContextReceiver receiver and not null)) return;
				if (receiver.isGenerator == false) return;
				StringBuilder Code = new StringBuilder();

				StringBuilder genericsBuilder = new StringBuilder();
				for (int i = 0; i < RuleGeneratorSetting.argumentCount; i++)
				{
					if (i == 0) genericsBuilder.Append("<");
					genericsBuilder.Append($"T{i}");

					if (i == RuleGeneratorSetting.argumentCount - 1)
					{
						genericsBuilder.Append(">");
					}
					else
					{
						genericsBuilder.Append(",");
					}
				}

				StringBuilder GenericParameter = new StringBuilder();
				for (int i = 0; i < RuleGeneratorSetting.argumentCount; i++)
				{
					GenericParameter.Append($", T{i} arg{i}");
				}

				Code.AppendLine("using System;");
				Code.AppendLine("namespace WorldTree");
				Code.AppendLine("{");

				for (int i = 0; i < RuleGeneratorSetting.argumentCount; i++)
				{
					Code.AppendLine("    /// <summary>");
					Code.AppendLine("    /// 通知法则基类接口");
					Code.AppendLine("    /// </summary>");
					Code.AppendLine($"    public interface ISendRuleBase{genericsBuilder} : IRule");
					Code.AppendLine("    {");
					Code.AppendLine($"        void Invoke(INode self{GenericParameter});");
					Code.AppendLine("    }");
				}

				for (int i = 0; i < RuleGeneratorSetting.argumentCount; i++)
				{
					Code.AppendLine("    /// <summary>");
					Code.AppendLine("    /// 通知法则基类");
					Code.AppendLine("    /// </summary>");
					Code.AppendLine($"    public abstract class SendRuleBase<N, R{RuleGeneratorSetting.argumentCount}> : RuleBase<N, R>, ISendRuleBase{genericsBuilder}");
					Code.AppendLine("    where N : class, INode, AsRule<R>");
					Code.AppendLine($"    where R : ISendRuleBase{genericsBuilder}");
					Code.AppendLine("    {");
					Code.AppendLine($"        public virtual void Invoke(INode self{GenericParameter}) => Execute");

					Code.AppendLine("    }");
				}

				Code.AppendLine("}");

				context.AddSource("SendRuleBase.cs", SourceText.From(Code.ToString(), System.Text.Encoding.UTF8));//生成代码
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		private class SyntaxContextReceiver : ISyntaxReceiver
		{
			internal static ISyntaxReceiver Create() => new SyntaxContextReceiver();

			public bool isGenerator = false;

			public void OnVisitSyntaxNode(SyntaxNode node)
			{
				//判断是否是类
				if (node is not InterfaceDeclarationSyntax interfaceDeclarationSyntax) return;

				//判断类型是否是INode,因为INode是基类在核心程序集中
				if (interfaceDeclarationSyntax.Identifier.ValueText == "INode")
				{
					isGenerator = true;
				}
			}
		}
	}
}