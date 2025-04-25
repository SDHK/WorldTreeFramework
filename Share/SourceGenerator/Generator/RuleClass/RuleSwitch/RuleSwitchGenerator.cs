/****************************************

* 作者：闪电黑客
* 日期：2025/4/24 14:30

* 描述：法则分流类型补充生成器

*/
using Microsoft.CodeAnalysis;

namespace WorldTree.SourceGenerator
{
	[Generator]
	internal class RuleSwitchGenerator : ISourceGenerator
	{



		public void Initialize(GeneratorInitializationContext context)
		{
			context.RegisterForSyntaxNotifications(() => new FindSubInterfaceSyntaxReceiver());
		}

		public void Execute(GeneratorExecutionContext context)
		{
			if (!(context.SyntaxReceiver is FindSubInterfaceSyntaxReceiver receiver)) return;
			if (receiver.InterfaceDeclarations.Count == 0) return;

		}
	}
}
