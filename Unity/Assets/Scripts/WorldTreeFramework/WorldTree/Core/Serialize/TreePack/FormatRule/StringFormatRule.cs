/****************************************

* 作者：闪电黑客
* 日期：2024/8/15 17:10

* 描述：

*/
namespace WorldTree.TreePackFormats
{
	public static class StringFormatRule
	{
		class Serialize : TreePackSerializeRule<TreePackByteSequence, string>
		{
			protected override void Execute(TreePackByteSequence self, ref string value)
			{
				self.WriteString(value);
			}
		}
		class Deserialize : TreePackDeserializeRule<TreePackByteSequence, string>
		{
			protected override void Execute(TreePackByteSequence self, ref string value)
			{
				value = self.ReadString();
			}
		}
	}
}
