/****************************************

* 作者： 闪电黑客
* 日期： 2023/7/17 11:37

* 描述：

*/

namespace WorldTree
{
	public static partial class TreeValueBaseRule
	{
		private class TreeValueStringAwakeRule : AwakeRule<TreeValue<string>>
		{
			protected override void OnEvent(TreeValue<string> self)
			{
				self.Value = "";
			}
		}

		private class TreeValueStringValueAwakeRule : AwakeRule<TreeValue<string>, string>
		{
			protected override void OnEvent(TreeValue<string> self, string value)
			{
				self.Value = value;
			}
		}
	}
}