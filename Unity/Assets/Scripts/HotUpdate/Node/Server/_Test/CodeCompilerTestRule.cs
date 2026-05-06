
namespace WorldTree.Server
{
	/// <summary>
	/// 测试代码编译器规则
	/// </summary>
	public static class CodeCompilerTestRule
	{
		class Add : AddRule<CodeCompilerTest>
		{
			protected override void Execute(CodeCompilerTest self)
			{
				self.Log("代码编译器测试规则入口");
				//	self.AddChild(out MathParser parser);
				//	//self.Log($"得数为 ：{parser.Parse("3 + 5 * (2 - 8)")}");
				//	self.Log($"复杂算式结果：{parser.Parse("((15 + 3) * 4 - 8) / 2 + 10 * (6 - 2) - 5")}");
			}
		}
	}
}